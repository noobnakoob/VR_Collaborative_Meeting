using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace GracesGames.SimpleFileBrowser.Scripts {
	// Demo class to illustrate the usage of the FileBrowser script
	// Able to save and load files containing serialized data (e.g. text)
	public class DemoCaller : MonoBehaviour {

		// Use the file browser prefab
		public GameObject FileBrowserPrefab;

        public GameObject FileOpenButton;

		// Define a file extension
		public string[] FileExtensions;

        public GameObject loadingIndicator;
        public GameObject nextSlideButton;
        public GameObject previousSlideButton;

		// Input field to get text to save
		private GameObject _textToSaveInputField;

		// Label to display loaded text
		private GameObject _loadedText;

		public bool PortraitMode;
        
        //public Image slide_Image;

        private Texture2D current_Texture;

        public static DemoCaller Instance;

        bool gazed;

        bool explorerOpened;

        private void Start()
        {
            Instance = this;
        }

        public void OnGazeEnter()
        {
            gazed = true;
        }

        public void OnGazeExit()
        {
            gazed = false;
        }

        IEnumerator CreateTextureFromFile(string path)
        {
            if (File.Exists(path))
            {
                WWW www = new WWW("file://" + path);
                yield return www;

                GetComponent<ImageManager>().GenerateTexture(www.bytes);
            }
            else
                Debug.Log("File doesnt exist on specified path!");
        }

        void CreateTextureFromByteArray(byte[] texture_Array)
        {
            if (texture_Array != null)
                GetComponent<ImageManager>().GenerateTexture(texture_Array);
            else
                Debug.Log("Byte array not valid!");
        }

        public void OnFileSelect(string path)
        {
            LoadFileUsingPath(path);
        }

        public void OnOpenFileBrowser()
        {
            if (PhotonNetwork.isMasterClient)
                StartCoroutine(OpenFileBrowserRoutine());
        }

        // Open a file browser to save and load files
        private IEnumerator OpenFileBrowserRoutine() {
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

        AndroidJavaClass pptxViewer;

        private void LoadPPTXFromFile(string path)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            pptxViewer = new AndroidJavaClass("com.petar.ppt.pptxViewer2");
            pptxViewer.CallStatic("load", currentActivity, path);
            LoadFirstSlide();
        }

        void LoadFirstSlide()
        {
            byte[] image_Array;
            bool slideReady;
            loadingIndicator.SetActive(true);
            nextSlideButton.SetActive(false);
            previousSlideButton.SetActive(false);
            image_Array = pptxViewer.CallStatic<byte[]>("getPrepareSlide");

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
            byte[] image_Array;
            bool slideReady;
            bool slideLoadingInitialized = false;            

            yield return new WaitForSeconds(2f);

            if (gazed && !slideLoadingInitialized)
            {
                nextSlideButton.SetActive(false);
                previousSlideButton.SetActive(false);

                slideLoadingInitialized = true;
                loadingIndicator.SetActive(true);
                pptxViewer.CallStatic("prepareNextSlide");
                image_Array = pptxViewer.CallStatic<byte[]>("getPrepareSlide");

                
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
        }

        IEnumerator GeneratePreviousSlide()
        {
            byte[] image_Array;
            bool slideReady;
            bool slideLoadingInitialized = false;
            yield return new WaitForSeconds(2f);

            if (gazed && !slideLoadingInitialized)
            {
                nextSlideButton.SetActive(false);
                previousSlideButton.SetActive(false);
                loadingIndicator.SetActive(true);

                pptxViewer.CallStatic("preparePreviosSlide");
                image_Array = pptxViewer.CallStatic<byte[]>("getPrepareSlide");
                                
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
}