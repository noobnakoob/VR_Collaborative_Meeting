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
    private Text _clientNumber;
    private Text ClientNumber
    {
        get { return _clientNumber; }
    }

    public void OnClick_CreateRoom()
    {
        if (ClientNumber.text != "")
        {
            if (int.Parse(ClientNumber.text) > 1)
            {
                RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = byte.Parse(ClientNumber.text) };

                if (RoomName.text != "")
                {
                    if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
                        LogLayoutGroup.Instance.AddNewLog("Create room successfully sent.");
                    else
                        LogLayoutGroup.Instance.AddNewLog("Create room failed to send.");
                }
                else
                    LogLayoutGroup.Instance.AddNewLog("Enter room name first.");
            }
            else
                LogLayoutGroup.Instance.AddNewLog("Client number must be greater than 1.");
        }
        else
            LogLayoutGroup.Instance.AddNewLog("Client number must be greater than 1.");        
    }

    public void SetRoomName(string roomName)
    {
        _roomName.transform.parent.gameObject.GetComponent<InputField>().text = roomName;
    }

    public void SetClientNumber(string number)
    {
        _clientNumber.transform.parent.gameObject.GetComponent<InputField>().text = number;
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        LogLayoutGroup.Instance.AddNewLog("Create room failed: " + codeAndMessage[1]);
    }
}
