using UnityEngine;

public class LobbyNetwork : MonoBehaviour
{

    // Use this for initialization
    private void Start()
    {
        if (!PhotonNetwork.connected)
        {
            LogLayoutGroup.Instance.AddNewLog("Connecting to server...");
            print("Connecting to server..");
            PhotonNetwork.ConnectUsingSettings("1.0.0");
        }
    }

    private void OnConnectedToMaster()
    {
        LogLayoutGroup.Instance.AddNewLog("Connected to master.");
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = PlayerNetwork.Instance.PlayerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    private void OnJoinedLobby()
    {
        LogLayoutGroup.Instance.AddNewLog("Joined lobby.");
        print("Joined lobby.");

        if (!PhotonNetwork.inRoom)
            MainCanvasManager.Instance.LobbyCanvas.transform.SetAsLastSibling();
    }
}
