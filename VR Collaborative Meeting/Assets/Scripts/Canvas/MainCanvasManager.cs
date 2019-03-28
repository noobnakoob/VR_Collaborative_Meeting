using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager Instance;

    [SerializeField]
    private LobbyCanvas _lobbyCanvas;
    public LobbyCanvas LobbyCanvas
    {
        get { return _lobbyCanvas; }
    }

    [SerializeField]
    private CurrentRoomCanvas _currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas
    {
        get { return _currentRoomCanvas; }
    }

    [SerializeField]
    private PlayerLayoutGroup _playerLayoutGroup;
    public PlayerLayoutGroup PlayerLayoutGroup
    {
        get { return _playerLayoutGroup; }
    }

    [SerializeField]
    private CreateRoom _createRoom;
    public CreateRoom CreateRoom
    {
        get { return _createRoom; }
    }

    [SerializeField]
    private LogLayoutGroup _logLayoutGroup;
    public LogLayoutGroup LogLayoytGroup
    {
        get { return _logLayoutGroup; }
    }

    [SerializeField]
    private RoomLayoutGroup _roomLayoutGroup;
    public RoomLayoutGroup RoomLayoutGroup
    {
        get { return _roomLayoutGroup; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}