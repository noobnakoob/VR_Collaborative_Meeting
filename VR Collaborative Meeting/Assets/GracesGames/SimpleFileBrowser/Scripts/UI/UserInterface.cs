﻿using UnityEngine;
using UnityEngine.UI;

using System.IO;

using GracesGames.Common.Scripts;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

	// The UI used in the file browser. 

	public abstract class UserInterface : MonoBehaviour {

		// Dimension used to set the scale of the UI
		[Range(0.1f, 1.0f)] public float UserInterfaceScale = 1f;

		// Button Prefab used to create a button for each directory in the current path
		public GameObject DirectoryButtonPrefab;

		// Button Prefab used to create a button for each file in the current path
		public GameObject FileButtonPrefab;

		// Sprite used to represent the save button
		public Sprite SaveImage;

		// Sprite used to represent the load button
		public Sprite LoadImage;

		// Height of the directory and file buttons
		[Range(0.0f, 500.0f)] public int ItemButtonHeight = 120;

		// Font size used for the directory and file buttons
		[Range(0.0f, 72.0f)] public int ItemFontSize = 32;

		// Font size used for the path, load and save text
		[Range(0.0f, 72.0f)] public int PanelTextFontSize = 24;

		// Color used for the Directory Panel (and ItemPanel for Portrait mode)
		public Color DirectoryPanelColor = Color.gray;

		// Color used for the File Panel
		public Color FilePanelColor = Color.gray;

		// Color used for the directory and file texts
		public Color ItemFontColor = Color.white;

		// The file browser using this user interface
		private FileBrowser _fileBrowser;

		// Button used to select a file to save/load
		private GameObject _selectFileButton;

		// Game object that represents the current path
		private GameObject _pathText;

		// Game object and InputField that represents the name of the file to save
		private GameObject _saveFileText;

		// Game object (Text) that represents the name of the file to load
		private GameObject _loadFileText;

		// Game object used as the parent for all the Directories of the current path
		protected GameObject DirectoriesParent;

		// Game object used as the parent for all the Files of the current path
		protected GameObject FilesParent;

		// Input field and variable to allow file search
		private InputField _searchInputField;

		// Setup the file browser user interface
		public void Setup(FileBrowser fileBrowser) {
			_fileBrowser = fileBrowser;
			name = "FileBrowserUI";
			SetupDirectoryAndFilePrefab();
			SetupClickListeners();
			SetupTextLabels();
			SetupParents();
		}

		// Sets the font size and color for the directory and file texts
		private void SetupDirectoryAndFilePrefab() {
			DirectoryButtonPrefab.transform.GetChild(0).gameObject.GetComponent<Text>().fontSize = ItemFontSize;
			FileButtonPrefab.transform.GetChild(0).gameObject.GetComponent<Text>().fontSize = ItemFontSize;
			DirectoryButtonPrefab.transform.GetChild(0).gameObject.GetComponent<Text>().color = ItemFontColor;
			FileButtonPrefab.transform.GetChild(0).gameObject.GetComponent<Text>().color = ItemFontColor;
		}

		// Setup click listeners for buttons
		private void SetupClickListeners()
        {
			// Hook up Directory Navigation methods to Directory Navigation Buttons
			Utilities.FindButtonAndAddOnClickListener("DirectoryUpButton", _fileBrowser.DirectoryUp);

			// Hook up CloseFileBrowser method to CloseFileBrowserButton
			Utilities.FindButtonAndAddOnClickListener("CloseFileBrowserButton", _fileBrowser.CloseFileBrowser);
		}

		// Setup path, load and save file text
		private void SetupTextLabels() {
			// Find the path and file label (path label optional in Portrait UI)
			GameObject pathLabel = GameObject.Find("PathLabel");

			// Find pathText game object to update path on clicks
			_pathText = Utilities.FindGameObjectOrError("PathText");
			
			// Set font size for labels and texts
			if (pathLabel != null) {
				pathLabel.GetComponent<Text>().fontSize = PanelTextFontSize;
			}

			_pathText.GetComponent<Text>().fontSize = PanelTextFontSize;
		}

		// Setup parents object to hold directories and files (implemented in Landscape and Portrait version)
		protected abstract void SetupParents();
        
		// Sets the height of a GridLayoutGroup located in the game object (parent of directies and files object)
		protected void SetButtonParentHeight(GameObject parent, int height) {
			Vector2 cellSize = parent.GetComponent<GridLayoutGroup>().cellSize;
			cellSize = new Vector2(cellSize.x, height);
			parent.GetComponent<GridLayoutGroup>().cellSize = cellSize;
		}

		// Update the path text
		public void UpdatePathText(string newPath) {

            string[] directories = newPath.Split('\\');

			_pathText.GetComponent<Text>().text = directories[directories.Length - 1];
		}

		// Resets the directories and files parent game objects
		public void ResetParents() {
			ResetParent(DirectoriesParent);
			ResetParent(FilesParent);
		}

		// Removes all current game objects under the parent game object
		private void ResetParent(GameObject parent) {
			if (parent.transform.childCount > 0) {
				foreach (Transform child in parent.transform) {
					Destroy(child.gameObject);
				}
			}
		}

		// Creates a directory button given a directory
		public void CreateDirectoryButton(string directory) {
			GameObject button = Instantiate(DirectoryButtonPrefab, Vector3.zero, Quaternion.identity);
			SetupButton(button, new DirectoryInfo(directory).Name, DirectoriesParent.transform);
            // Setup FileBrowser DirectoryClick method to onClick event
            button.GetComponent<FileButton>().Setup(_fileBrowser, directory);
		}

		// Creates a file button given a file
		public void CreateFileButton(string file) {
			GameObject button = Instantiate(FileButtonPrefab, Vector3.zero, Quaternion.identity);
			// When in Load mode, disable the buttons with different extension than the given file extension
			if (_fileBrowser.GetMode() == FileBrowserMode.Load) {
				DisableWrongExtensionFiles(button, file);
			}

			SetupButton(button, Path.GetFileName(file), FilesParent.transform);
			// Setup FileButton script for file button (handles click and double click event)
			button.GetComponent<FileButton>().Setup(_fileBrowser, file);
		}

		// Generic method used to extract common code for creating a directory or file button
		private void SetupButton(GameObject button, string text, Transform parent) {
			button.transform.GetChild(0).gameObject.GetComponent<Text>().text = text;
			button.transform.SetParent(parent, false);
			button.transform.localScale = Vector3.one;
		}

		// Disables file buttons with files that have a different file extension (than given to the OpenFilePanel)
		private void DisableWrongExtensionFiles(GameObject button, string file) {
			if (!_fileBrowser.CompatibleFileExtension(file)) {
				button.GetComponent<Button>().interactable = false;
			}
		}
	}
}