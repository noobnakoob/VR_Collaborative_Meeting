using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecognitionManager : MonoBehaviour {

    [HideInInspector]
    public bool IsRecording { get; private set; }

    private static readonly string COMMAND_ROOM_NAME = "ROOM NAME";
    private static readonly string COMMAND_CLIENTS = "CLIENTS";
    //private static readonly string COMMAND_PLAYER_NAME = "PLAYER NAME";
    private static readonly string COMMAND_CREATE_ROOM = "CREATE ROOM";
    private static readonly string COMMAND_JOIN_ROOM = "JOIN";
    private static readonly string COMMAND_LEAVE_ROOM = "LEAVE ROOM";
    private static readonly string COMMAND_ENTER = "ENTER";
    private static readonly string COMMAND_ENTER_LOCK = "ENTER LOCK";
    private static readonly string COMMAND_ROOM_STATE = "CHANGE STATE";
    private static readonly string COMMAND_EXIT = "EXIT";
    private static readonly string COMMAND_OPEN_FILE = "OPEN BROWSER";
    private static readonly string COMMAND_CLOSE_FILE = "CLOSE BROWSER";
    private static readonly string COMMAND_SELECT_FILE = "SELECT FILE";
    private static readonly string COMMAND_SELECT_FOLDER = "SELECT FOLDER";
    private static readonly string COMMAND_NEXT_SLIDE = "NEXT SLIDE";
    private static readonly string COMMAND_PREVIOUS_SLIDE = "PREVIOUS SLIDE";
    private static readonly string COMMAND_DIRECTORY_UP = "BACK";
    private static readonly string COMMAND_SCROLL_UP = "SCROLL UP";
    private static readonly string COMMAND_SCROLL_DOWN = "SCROLL DOWN";    

    AndroidJavaClass androidJC;
    AndroidJavaObject jo;
    AndroidJavaClass jc;

    private void Start()
    {
        androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = androidJC.GetStatic<AndroidJavaObject>("currentActivity");
        jc = new AndroidJavaClass("com.plugin.speech.pluginlibrary.SpeechClass");
    }

    public void StartMessage()
    {
        if (!IsRecording)
        {
            IsRecording = true;
            jc.CallStatic("StartListenning", jo);
        }
    }

    public void StopMessage()
    {
        if (IsRecording)
        {
            IsRecording = false;
            jc.CallStatic("StopListening");
        }
    }

    void GetMessage(string message_)
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            message_ = message_.ToUpper();
            
            if (message_.Contains(COMMAND_ROOM_NAME))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                string roomName = message_.Replace(COMMAND_ROOM_NAME + " ", "");
                MainCanvasManager.Instance.CreateRoom.SetRoomName(roomName);
            }
            else if (message_.Contains(COMMAND_CLIENTS))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                string temp = message_.Replace(COMMAND_CLIENTS + " ", "");
                string[] number = temp.Split(' ');
                MainCanvasManager.Instance.CreateRoom.SetClientNumber(number[0]);
            }
            else if (message_.Contains(COMMAND_CREATE_ROOM))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                MainCanvasManager.Instance.CreateRoom.OnClick_CreateRoom();
            }
            else if (message_.Contains(COMMAND_ENTER_LOCK))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                MainCanvasManager.Instance.CurrentRoomCanvas.OnClickStartAndLock();
            }
            else if (message_.Contains(COMMAND_ENTER))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                MainCanvasManager.Instance.CurrentRoomCanvas.OnClickStart();
            }
            else if (message_.Contains(COMMAND_LEAVE_ROOM))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                MainCanvasManager.Instance.PlayerLayoutGroup.OnClickLeaveRoom();
            }
            else if (message_.Contains(COMMAND_ROOM_STATE))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                MainCanvasManager.Instance.PlayerLayoutGroup.OnClickRoomState();
            }
            else if (message_.Contains(COMMAND_JOIN_ROOM))
            {
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message_);
                string roomName = message_.Replace(COMMAND_JOIN_ROOM + " ", "");
                MainCanvasManager.Instance.RoomLayoutGroup.JoinRoomVoice(roomName);
            }
            else if (message_.Contains(COMMAND_EXIT))
            {
                string message = "Exiting application!";
                MainCanvasManager.Instance.LogLayoytGroup.AddNewLog(message);
                MainCanvasManager.Instance.OnClickExit();
            }
        }

        if (SceneManager.GetActiveScene().name == "Game")
        {
            message_ = message_.ToUpper();
            if (message_.Contains(COMMAND_OPEN_FILE))
                MainManager.Instance.FileBrowserManager.OpenFileBrowserVoice();
            else if (message_.Contains(COMMAND_CLOSE_FILE))
            {
                MainManager.Instance.FileBrowserManager.CloseFileBrowser();
                MainManager.Instance.FileBrowser.CloseFileBrowser();
            }
            else if (message_.Contains(COMMAND_LEAVE_ROOM))
                MainManager.Instance.FileBrowserManager.OnExitRoom();
            else if (message_.Contains(COMMAND_NEXT_SLIDE))
                MainManager.Instance.FileBrowserManager.OnNextSlide();
            else if (message_.Contains(COMMAND_PREVIOUS_SLIDE))
                MainManager.Instance.FileBrowserManager.OnPreviousSlide();
            else if (message_.Contains(COMMAND_SELECT_FILE))
            {
                string fileName = message_.Replace(COMMAND_SELECT_FILE + " ", "");
                MainManager.Instance.UserInterface.SelectDirectoryVoice(fileName);
            }
            else if (message_.Contains(COMMAND_SELECT_FOLDER))
            {
                string dirName = message_.Replace(COMMAND_SELECT_FOLDER + " ", "");
                MainManager.Instance.UserInterface.SelectFileVoice(dirName);
            }
            else if (message_.Contains(COMMAND_DIRECTORY_UP))
                MainManager.Instance.FileBrowser.DirectoryUp();
            else if (message_.Contains(COMMAND_SCROLL_DOWN))
                MainManager.Instance.PortraitUserInterface.ScrollDownVoice();
            else if (message_.Contains(COMMAND_SCROLL_UP))
                MainManager.Instance.PortraitUserInterface.ScrollUpVoice();
        }
    }

    void GetError(string message_)
    {
        Debug.Log(message_);
    }

    void StopWriting(string message_)
    {
        IsRecording = false;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button4))
            StartMessage();
        else if (Input.GetKeyUp(KeyCode.Joystick1Button5))
            StopMessage();
    }
}