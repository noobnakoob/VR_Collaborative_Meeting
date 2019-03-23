using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

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
    }

    IEnumerator CreateTextureFromFile(string path)
    {
        if (File.Exists(path))
        {
            WWW www = new WWW("file://" + path);
            yield return www;
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);

            GetComponent<ImageManager>().GenerateTexture(texture);
        }
        else
            Debug.Log("File doesnt exist on specified path!");
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
        else {
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
        previousSlideButton.SetActive(false);
        StartCoroutine(GetPrepareSlide());
    }

    IEnumerator GetPrepareSlide()
    {
        byte[] image_Array = null;
        bool slideReady = false;

        while (!slideReady)
        {
            pptxViewer.CallStatic("getPrepareSlide");
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
                previousSlideButton.SetActive(true);
            }        
        }
        pptxReeciver.SetStatic<byte[]>("bytes", null);
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
            previousSlideButton.SetActive(false);

            slideLoadingInitialized = true;
            loadingIndicator.SetActive(true);
            pptxViewer.CallStatic("prepareNextSlide");

            StartCoroutine(GetPrepareSlide());
        }
    }

    IEnumerator GeneratePreviousSlide()
    {
        bool slideLoadingInitialized = false;
        yield return new WaitForSeconds(2f);

        if (gazed && !slideLoadingInitialized)
        {
            nextSlideButton.SetActive(false);
            previousSlideButton.SetActive(false);
            loadingIndicator.SetActive(true);

            pptxViewer.CallStatic("preparePreviosSlide");

            StartCoroutine(GetPrepareSlide());
        }
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
            PhotonNetwork.LeaveLobby();
            while (PhotonNetwork.insideLobby)
                yield return null;

            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.inRoom)
                yield return null;

            PhotonNetwork.Disconnect();
            while (PhotonNetwork.connected)
                yield return null;

            PhotonNetwork.LoadLevel(1);
        }
    }
}