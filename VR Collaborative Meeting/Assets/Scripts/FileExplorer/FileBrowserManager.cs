using System.Collections;
using UnityEngine;
using System.IO;
using Assets.Scripts.Managers;
using UnityEngine.XR;

// Demo class to illustrate the usage of the FileBrowser script
// Able to save and load files containing serialized data (e.g. text)
public class FileBrowserManager : MonoBehaviour {

	// Use the file browser prefab
	public GameObject FileBrowserPrefab;

    // Open file button
    public GameObject FileOpenButton;

	// Define a file extension
	public string[] FileExtensions;

    // Loading slides 
    public GameObject loadingIndicator;
    public GameObject nextSlideButton;
    public GameObject previousSlideButton;

	// Input field to get text to save
	private GameObject _textToSaveInputField;
        
    //public Image slide_Image;
    private Texture2D current_Texture;

    bool gazed;
    bool explorerOpened;

    AndroidJavaClass pptxViewer;
    AndroidJavaClass pptxReeciver;

    public void OnGazeEnter()
    {
        gazed = true;
    }

    public void OnGazeExit()
    {
        gazed = false;
    }

    public void OnOpenFileBrowser()
    {
        if (PhotonNetwork.isMasterClient)
            StartCoroutine(OpenFileBrowserRoutine());
    }

    public void OpenFileBrowserVoice()
    {
        if (!explorerOpened)
        {
            explorerOpened = true;
            GetComponent<ImageManager>().RemoveTexture();
            nextSlideButton.SetActive(false);
            nextSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
            previousSlideButton.SetActive(false);
            previousSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);

            FileOpenButton.SetActive(false);
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";

            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser();
            fileBrowserScript.OpenFilePanel(FileExtensions);
            fileBrowserScript.OnFileSelect += LoadFileUsingPath;
        }
    }

    // Open a file browser to save and load files
    private IEnumerator OpenFileBrowserRoutine()
    {
        // Create the file browser and name it
        yield return new WaitForSeconds(2f);

        if (gazed && !explorerOpened)
        {
            explorerOpened = true;
            GetComponent<ImageManager>().RemoveTexture();
            nextSlideButton.SetActive(false);
            nextSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
            previousSlideButton.SetActive(false);
            previousSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);

            FileOpenButton.SetActive(false);
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";

            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser();
            fileBrowserScript.OpenFilePanel(FileExtensions);
            fileBrowserScript.OnFileSelect += LoadFileUsingPath;
        }
    }

    public void CloseFileBrowser()
    {
        GetComponent<ImageManager>().RemoveTexture();
        explorerOpened = false;
        FileOpenButton.SetActive(true);
        nextSlideButton.SetActive(false);
        nextSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
        previousSlideButton.SetActive(false);
        previousSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
    }

    IEnumerator CreateTextureFromFile(string path)
    {
        if (File.Exists(path))
        {
            string pathHeader = "";

#if UNITY_EDITOR
            pathHeader = "file://";
#elif UNITY_ANDROID
            pathHeader = "file:///";
#endif           

            WWW www = new WWW(pathHeader + path);
            yield return www;
            Texture2D texture = null;            
            texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            www.Dispose();
            int width = texture.width;
            int height = texture.height;

            float scale = 1f;
            if (width < height)
            {
                scale = height / 512f;
                height = 512;
                width = Mathf.RoundToInt(width / scale);

            }
            else
            {
                scale = width / 512f;
                width = 512;
                height = Mathf.RoundToInt(height / scale);
            }

            TextureScale.Bilinear(texture, width, height);
            GetComponent<ImageManager>().GenerateTexture(texture);
        }
        else
            Debug.Log("File doesnt exist on specified path!");
    }

    static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
    {
        //We need the source texture in VRAM because we render with it
        src.filterMode = fmode;
        src.Apply(true);

        //Using RTT for best quality and performance. Thanks, Unity 5
        RenderTexture rtt = new RenderTexture(width, height, 32);

        //Set the RTT in order to render to it
        Graphics.SetRenderTarget(rtt);

        //Setup 2D matrix in range 0..1, so nobody needs to care about sized
        GL.LoadPixelMatrix(0, 1, 1, 0);

        //Then clear & draw the texture to fill the entire RTT.
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
    }

    Texture2D RescaleTexture(Texture2D pSource, float pScale)
    {
        //*** Variables
        int i;

        //*** Get All the source pixels
        Color[] aSourceColor = pSource.GetPixels(0);
        Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

        //*** Calculate New Size
        float xWidth = Mathf.RoundToInt((float)pSource.width * pScale);
        float xHeight = Mathf.RoundToInt((float)pSource.height * pScale);

        //*** Make New
        Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

        //*** Make destination array
        int xLength = (int)xWidth * (int)xHeight;
        Color[] aColor = new Color[xLength];

        Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

        //*** Loop through destination pixels and process
        Vector2 vCenter = new Vector2();
        for (i = 0; i < xLength; i++)
        {

            //*** Figure out x&y
            float xX = (float)i % xWidth;
            float xY = Mathf.Floor((float)i / xWidth);

            //*** Calculate Center
            vCenter.x = (xX / xWidth) * vSourceSize.x;
            vCenter.y = (xY / xHeight) * vSourceSize.y;

            //*** Do Based on mode
            //*** Calculate grid around point
            int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
            int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
            int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
            int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

            //*** Loop and accumulate
            //Vector4 oColorTotal = new Vector4();
            Color oColorTemp = new Color();
            float xGridCount = 0;
            for (int iy = xYFrom; iy < xYTo; iy++)
            {
                for (int ix = xXFrom; ix < xXTo; ix++)
                {

                    //*** Get Color
                    oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                    //*** Sum
                    xGridCount++;
                }
            }

            //*** Average Color
            aColor[i] = oColorTemp / (float)xGridCount;
        }
            //*** Set Pixels
            oNewTex.SetPixels(aColor);
            oNewTex.Apply();

            //*** Return
            return oNewTex;
        }

    void CreateTextureFromByteArray(byte[] texture_Array)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(texture_Array);

        if (texture_Array != null)
            GetComponent<ImageManager>().GenerateTexture(texture);
        else
            Debug.Log("Byte array not valid!");
    }

    public void OnFileSelect(string path)
    {
        LoadFileUsingPath(path);
    }      

	// Loads a file using a path
	private void LoadFileUsingPath(string path)
    {
        explorerOpened = false;
		if (path.Length != 0)
        {
            string[] extension = path.Split('.');

            switch (extension[extension.Length - 1])
            {
                case "jpg":
                    StartCoroutine(CreateTextureFromFile(path));
                    break;
                case "png":
                    StartCoroutine(CreateTextureFromFile(path));
                    break;
                case "pptx":
                    LoadPPTXFromFile(path);
                    break;
            }
            FileOpenButton.SetActive(true);
        }
        else
        {
			Debug.Log("Invalid path given!");
		}
	}

    private void LoadPPTXFromFile(string path)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        pptxViewer = new AndroidJavaClass("com.petar.pptxplugins.PPTX");
        pptxViewer.CallStatic("load", currentActivity, path);
        pptxReeciver = new AndroidJavaClass("com.petar.pptxplugins.PPTXReceiver");
        pptxReeciver.CallStatic("createInstance", currentActivity);
        LoadFirstSlide();
    }

    void LoadFirstSlide()
    {
        loadingIndicator.SetActive(true);
        nextSlideButton.SetActive(false);
        nextSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
        previousSlideButton.SetActive(false);
        previousSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
        StartCoroutine(GetPrepareSlide());
    }

    IEnumerator GetPrepareSlide()
    {
        byte[] image_Array = null;
        bool slideReady = false;
        ImageManager.Instance.RemoveTexture();
       
        while (!slideReady)
        {
            yield return new WaitForSeconds(1f);
            image_Array = pptxReeciver.GetStatic<byte[]>("bytes");
            if (image_Array == null)
                slideReady = false;
            else
                slideReady = true;

            if (slideReady)
            {
                loadingIndicator.SetActive(false);
                CreateTextureFromByteArray(image_Array);
                nextSlideButton.SetActive(true);
                nextSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
                previousSlideButton.SetActive(true);
                previousSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
            }       
        }
        pptxReeciver.SetStatic<byte[]>("bytes", null);
        pptxReeciver.CallStatic("clear");
        slideReady = false;
        image_Array = null;
    }        

    public void OnNextSlide()
    {
        StartCoroutine(GenerateNextSlide());
    }

    public void OnPreviousSlide()
    {
        StartCoroutine(GeneratePreviousSlide());
    }

    IEnumerator GenerateNextSlide()
    {
        bool slideLoadingInitialized = false; 
        yield return new WaitForSeconds(2f);

        if (gazed && !slideLoadingInitialized)
        {
            nextSlideButton.SetActive(false);
            nextSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
            previousSlideButton.SetActive(false);
            previousSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);

            slideLoadingInitialized = true;
            loadingIndicator.SetActive(true);
            
            pptxViewer.CallStatic("prepareNextSlide");

            StartCoroutine(GetPrepareSlide());
        }
        slideLoadingInitialized = false;
    }

    IEnumerator GeneratePreviousSlide()
    {
        bool slideLoadingInitialized = false;
        yield return new WaitForSeconds(2f);

        if (gazed && !slideLoadingInitialized)
        {
            nextSlideButton.SetActive(false);
            nextSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
            previousSlideButton.SetActive(false);
            previousSlideButton.transform.GetChild(0).transform.gameObject.SetActive(false);
            loadingIndicator.SetActive(true);
            slideLoadingInitialized = true;
            
            pptxViewer.CallStatic("preparePreviousSlide");

            StartCoroutine(GetPrepareSlide());
        }
        slideLoadingInitialized = false;
    }

    public void OnExitRoom()
    {
        StartCoroutine(ExitRoomRoutine());
    }

    IEnumerator ExitRoomRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (gazed)
        {
            yield return new WaitForEndOfFrame();
            PhotonNetwork.LeaveLobby();
            while (PhotonNetwork.insideLobby)
                yield return null;

            yield return new WaitForEndOfFrame();
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.inRoom)
                yield return null;

            yield return new WaitForEndOfFrame();
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.connected)
                yield return null;

            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            Debug.Log("Else");
        }
    }
}