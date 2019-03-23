using UnityEngine;
using UnityEngine.UI;
using System.IO;

// The UI used in the file browser. 
public abstract class UserInterface : MonoBehaviour
{
    // Dimension used to set the scale of the UI
    [Range(0.1f, 1.0f)] public float UserInterfaceScale = 1f;

    // Button Prefab used to create a button for each directory in the current path
    public GameObject DirectoryButtonPrefab;

    // Button Prefab used to create a button for each file in the current path
    public GameObject FileButtonPrefab;

    // Height of the directory and file buttons
    [Range(0.0f, 500.0f)] public int ItemButtonHeight = 120;

    // Color used for the Directory Panel (and ItemPanel for Portrait mode)
    public Color DirectoryPanelColor = Color.gray;

    // Color used for the File Panel
    public Color FilePanelColor = Color.gray;

    // Color used for the directory and file texts
    public Color ItemFontColor = Color.white;

    // Game object that represents the current path
    private GameObject _pathText;

    // Game object used as the parent for all the Directories of the current path
    protected GameObject DirectoriesParent;

    // Game object used as the parent for all the Files of the current path
    protected GameObject FilesParent;

    private void Awake()
    {
        MainManager.Instance.SetUserInterface(this);
    }

    // Setup the file browser user interface
    public void Setup()
    {
        name = "FileBrowserUI";
        SetupDirectoryAndFilePrefab();
        SetupClickListeners();
        SetupTextLabels();
        SetupParents();
    }

    // Sets the font size and color for the directory and file texts
    private void SetupDirectoryAndFilePrefab()
    {
        DirectoryButtonPrefab.transform.GetChild(0).gameObject.GetComponent<Text>().color = ItemFontColor;
        FileButtonPrefab.transform.GetChild(0).gameObject.GetComponent<Text>().color = ItemFontColor;
    }

    // Setup click listeners for buttons
    private void SetupClickListeners()
    {
        Utilities.FindButtonAndAddOnClickListener("DirectoryUpButton", MainManager.Instance.FileBrowser.DirectoryUp);
        Utilities.FindButtonAndAddOnClickListener("CloseFileBrowserButton", MainManager.Instance.FileBrowser.CloseFileBrowser);
        Utilities.FindButtonAndAddOnClickListener("ScrollUp", MainManager.Instance.PortraitUserInterface.OnScrollUp);
        Utilities.FindButtonAndAddOnClickListener("ScrollDown", MainManager.Instance.PortraitUserInterface.OnScrollDown);
    }

    // Setup path, load and save file text
    private void SetupTextLabels()
    {
        // Find pathText game object to update path on clicks
        _pathText = Utilities.FindGameObjectOrError("PathText");
    }

    // Setup parents object to hold directories and files (implemented in Landscape and Portrait version)
    protected abstract void SetupParents();

    // Sets the height of a GridLayoutGroup located in the game object (parent of directies and files object)
    protected void SetButtonParentHeight(GameObject parent, int height)
    {
        Vector2 cellSize = parent.GetComponent<GridLayoutGroup>().cellSize;
        cellSize = new Vector2(cellSize.x, height);
        parent.GetComponent<GridLayoutGroup>().cellSize = cellSize;
    }

    // Update the path text
    public void UpdatePathText(string newPath)
    {
        string[] directories = newPath.Split('\\');
        _pathText.GetComponent<Text>().text = directories[directories.Length - 1];
    }

    // Resets the directories and files parent game objects
    public void ResetParents()
    {
        ResetParent(DirectoriesParent);
        ResetParent(FilesParent);
    }

    // Removes all current game objects under the parent game object
    private void ResetParent(GameObject parent)
    {
        if (parent.transform.childCount > 0)
        {
            foreach (Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // Creates a directory button given a directory
    public void CreateDirectoryButton(string directory)
    {
        GameObject button = Instantiate(DirectoryButtonPrefab, Vector3.zero, Quaternion.identity);
        SetupButton(button, new DirectoryInfo(directory).Name, DirectoriesParent.transform);
        // Setup FileBrowser DirectoryClick method to onClick event
        button.GetComponent<FileButton>().Setup(directory);
    }

    // Creates a file button given a file
    public void CreateFileButton(string file)
    {
        GameObject button = Instantiate(FileButtonPrefab, Vector3.zero, Quaternion.identity);
        // When in Load mode, disable the buttons with different extension than the given file extension
        DisableWrongExtensionFiles(button, file);
        SetupButton(button, Path.GetFileName(file), FilesParent.transform);
        // Setup FileButton script for file button (handles click and double click event)
        button.GetComponent<FileButton>().Setup(file);
    }

    // Generic method used to extract common code for creating a directory or file button
    private void SetupButton(GameObject button, string text, Transform parent)
    {
        button.transform.GetChild(0).gameObject.GetComponent<Text>().text = text;
        button.transform.SetParent(parent, false);
        button.transform.localScale = Vector3.one;
    }

    // Disables file buttons with files that have a different file extension (than given to the OpenFilePanel)
    private void DisableWrongExtensionFiles(GameObject button, string file)
    {
        if (!MainManager.Instance.FileBrowser.CompatibleFileExtension(file))
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    public void SelectDirectoryVoice(string directory)
    {
        for (int i = 0; i < DirectoriesParent.transform.childCount; i++)
        {
            if (DirectoriesParent.transform.GetChild(i)
                .gameObject.transform.GetChild(0)
                .gameObject.GetComponent<Text>().text.Contains(directory))
                DirectoriesParent.transform.GetChild(i)
                    .gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void SelectFileVoice(string directory)
    {
        for (int i = 0; i < FilesParent.transform.childCount; i++)
        {
            if (FilesParent.transform.GetChild(i)
                .gameObject.transform.GetChild(0)
                .gameObject.GetComponent<Text>().text.Contains(directory))
                FilesParent.transform.GetChild(i)
                    .gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }
}