using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    public static MainManager Instance;

    [SerializeField]
    private FileBrowserManager _fileBrowserManager;
    public FileBrowserManager FileBrowserManager
    {
        get { return _fileBrowserManager; }
    }

    [SerializeField]
    private FileBrowser _fileBrowser;
    public FileBrowser FileBrowser
    {
        get { return _fileBrowser; }
        private set { _fileBrowser = value; }
    }

    [SerializeField]
    private PortraitUserInterface _portraitUserInterface;
    public PortraitUserInterface PortraitUserInterface
    {
        get { return _portraitUserInterface; }
        private set { _portraitUserInterface = value; }
    }

    [SerializeField]
    private UserInterface _userInterface;
    public UserInterface UserInterface
    {
        get { return _userInterface; }
        private set { _userInterface = value; }
    }

	// Use this for initialization
	void Awake () {

        Instance = this;
	}

    public void SetFileBrowser(FileBrowser browser)
    {
        FileBrowser = browser;
    }

    public void SetPortraitUserInterface(PortraitUserInterface inter)
    {
        PortraitUserInterface = inter;
    }

    public void SetUserInterface(UserInterface inter)
    {
        UserInterface = inter;
    }
}