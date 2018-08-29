using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{

    [SerializeField]
    private Text _roomName;
    private Text RoomName
    {
        get { return _roomName; }
    }

    [SerializeField]
    private Text _playerNumber;
    private Text PlayerNumber
    {
        get { return _playerNumber; }
    }

    public void OnClick_CreateRoom()
    {
        if (int.Parse(PlayerNumber.text) >= 2 && int.Parse(PlayerNumber.text) <= 10)
        {
            RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = byte.Parse(PlayerNumber.text) };
            if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
            {
                LogLayoutGroup.Instance.AddNewLog("Creating room " + RoomName.text + " for " + PlayerNumber.text + " clients");
                print("create room successfully sent." + " " + PlayerNumber);
            }
            else
            {
                LogLayoutGroup.Instance.AddNewLog("Create room failed to send.");
                print("Create room failed to send");
            }
        } 
        else
        {
            LogLayoutGroup.Instance.AddNewLog("Client number must be between 2 and 10.");
            print("Client number must be between 2 and 10.");
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        LogLayoutGroup.Instance.AddNewLog("Create room failed: " + codeAndMessage[1]);
        print("create room failed: " + codeAndMessage[1]);
    }
}
