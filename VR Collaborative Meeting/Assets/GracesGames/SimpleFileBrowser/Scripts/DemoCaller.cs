using System;
using UnityEngine;
using UnityEngine.UI;

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

		// Input field to get text to save
		private GameObject _textToSaveInputField;

		// Label to display loaded text
		private GameObject _loadedText;

		public bool PortraitMode;
        
        public Image slide_Image;

        private Texture2D current_Texture;

        public static DemoCaller Instance;

        private void Start()
        {
            Instance = this;
        }

        public void OnOpenFile()
        {
            slide_Image.gameObject.SetActive(false);
            OpenFileBrowser();
        }

        void CreateTextureFromFile(string path)
        {
            current_Texture = new Texture2D(0, 0);

            if (File.Exists(path))
            {
                WWW www = new WWW(path);

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
            FileOpenButton.SetActive(true);
            CreateTextureFromFile(path);
        }

        public void EnterDirectory()
        {
            
        }

		// Open a file browser to save and load files
		private void OpenFileBrowser() {
            // Create the file browser and name it
            FileOpenButton.SetActive(false);
			GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
			fileBrowserObject.name = "FileBrowser";
			// Set the mode to save or load
			FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
			fileBrowserScript.SetupFileBrowser(ViewMode.Portrait);
			fileBrowserScript.OpenFilePanel(FileExtensions);
			fileBrowserScript.OnFileSelect += LoadFileUsingPath;
		}

		// Loads a file using a path
		private void LoadFileUsingPath(string path) {
			if (path.Length != 0) {
				BinaryFormatter bFormatter = new BinaryFormatter();
				// Open the file using the path
				FileStream file = File.OpenRead(path);
				// Convert the file from a byte array into a string
				string fileData = bFormatter.Deserialize(file) as string;
				// We're done working with the file so we can close it
				file.Close();
				// Set the LoadedText with the value of the file
				_loadedText.GetComponent<Text>().text = "Loaded data: \n" + fileData;
			} else {
				Debug.Log("Invalid path given");
			}
		}
	}
}