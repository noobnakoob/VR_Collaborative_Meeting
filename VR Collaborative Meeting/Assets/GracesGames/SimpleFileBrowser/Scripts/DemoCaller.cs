using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

// Include these namespaces to use BinaryFormatter
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
        
        public Image slide_Image;

        private Texture2D current_Texture;

        public static DemoCaller Instance;

        bool gazed;

        bool explorerOpened;

        private void Start()
        {
            Instance = this;
            //String path = Application.persistentDataPath + "/test.pptx";
            //if (File.Exists(path))
            //{
            //    Debug.Log("pptxViewer: File: " + path  + "Exist" );
            //    LoadPPTXFromFile(path);
            //}
            //else
            //{
            //    Debug.Log("pptxViewer: File: " + path + "NotExist");
            //}
            //path = Application.persistentDataPath + "\test.pptx";
            //if (File.Exists(path))
            //{
            //    Debug.Log("pptxViewer: File: " + path + "Exist");
            //    LoadPPTXFromFile(path);
            //}
            //else
            //{
            //    Debug.Log("pptxViewer: File: " + path + "NotExist");
            //}
            //path = "/storage/emulated/0/test.pptx";
            //if (File.Exists(path))
            //{
            //    Debug.Log("pptxViewer: File: " + path + "Exist");
            //    LoadPPTXFromFile(path);
            //}
            //else
            //{
            //    Debug.Log("pptxViewer: File: " + path + "NotExist");
            //}
            /*path = "file:///storage/emulated/0/test.pptx";
            if (File.Exists(path))
            {
                Debug.Log("pptxViewer: File: " + path + "Exist");
                LoadPPTXFromFile(path);
            }
            else
            {
                Debug.Log("pptxViewer: File: " + path + "NotExist");
            }*/
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
            current_Texture = new Texture2D(0, 0);

            if (File.Exists(path))
            {
                WWW www = new WWW("file://" + path);
                yield return www;

                www.LoadImageIntoTexture(current_Texture);

                Sprite newSprite = Sprite.Create(current_Texture, new Rect(0, 0, current_Texture.width, current_Texture.height), new Vector2(0.5f, 0.5f));
                slide_Image.gameObject.SetActive(true);
                slide_Image.sprite = newSprite;
            }
            else
                Debug.Log("Not");
        }

        public void OnFileSelect(string path)
        {
            LoadFileUsingPath(path);
        }

        public void OnOpenFileBrowser()
        {
            StartCoroutine(OpenFileBrowserRoutine());
        }

        // Open a file browser to save and load files
        private IEnumerator OpenFileBrowserRoutine() {
            // Create the file browser and name it
            yield return new WaitForSeconds(2f);

            if (gazed && !explorerOpened)
            {
                explorerOpened = true;
                slide_Image.gameObject.SetActive(false);

                FileOpenButton.SetActive(false);
                GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
                fileBrowserObject.name = "FileBrowser";
                // Set the mode to save or load
                FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
                fileBrowserScript.SetupFileBrowser(ViewMode.Portrait);
                fileBrowserScript.OpenFilePanel(FileExtensions);
                fileBrowserScript.OnFileSelect += LoadFileUsingPath;
            }
		}

        public void CloseFileBrowser()
        {
            slide_Image.gameObject.SetActive(false);
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
				Debug.Log("Invalid path given");
			}
		}

        AndroidJavaClass pptxViewer;

        private void LoadPPTXFromFile(string path)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            pptxViewer = new AndroidJavaClass("com.petar.ppt.pptxViewer");
            pptxViewer.CallStatic("load", currentActivity, path);
            StartCoroutine(LoadFirstSlide());
        }

        IEnumerator LoadFirstSlide()
        {
            string pathValue = "";
            bool slideReady;
            loadingIndicator.SetActive(true);
            nextSlideButton.SetActive(false);
            previousSlideButton.SetActive(false);

            //do
            //{
                pathValue = pptxViewer.CallStatic<string>("getPrepareSlide");
                yield return pathValue;

                if (pathValue == "" || pathValue == null)
                    slideReady = false;
                else
                    slideReady = true;
            //}
            //while (!slideReady);

            if (slideReady)
            {
                loadingIndicator.SetActive(false);
                CreateTextureFromFile(pathValue);
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
            string pathValue = "";
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

                //do
                //{
                    pathValue = pptxViewer.CallStatic<string>("getPrepareSlide");
                    yield return pathValue;

                    if (pathValue == "" || pathValue == null)
                        slideReady = false;
                    else
                        slideReady = true;
                //}
                //while (!slideReady);

                if (slideReady)
                {
                    loadingIndicator.SetActive(false);
                    CreateTextureFromFile(pathValue);
                    nextSlideButton.SetActive(true);
                    previousSlideButton.SetActive(true);
                }
            }
        }

        IEnumerator GeneratePreviousSlide()
        {
            string pathValue = "";
            bool slideReady;
            bool slideLoadingInitialized = false;
            yield return new WaitForSeconds(2f);

            if (gazed && !slideLoadingInitialized)
            {
                nextSlideButton.SetActive(false);
                previousSlideButton.SetActive(false);
                loadingIndicator.SetActive(true);

                pptxViewer.CallStatic("preparePreviosSlide");

                //do
                //{
                    pathValue = pptxViewer.CallStatic<string>("getPrepareSlide");
                    yield return pathValue;

                    if (pathValue == "" || pathValue == null)
                        slideReady = false;
                    else
                        slideReady = true;
                //}
                //while (!slideReady);

                if (slideReady)
                {
                    loadingIndicator.SetActive(false);
                    CreateTextureFromFile(pathValue);
                    nextSlideButton.SetActive(true);
                    previousSlideButton.SetActive(true);
                }
            }
        }
	}
}