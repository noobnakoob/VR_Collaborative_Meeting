using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.UI;

public class PlayerNetwork : MonoBehaviour {

    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; }
    private PhotonView PhotonView;
    private int PlayersInGame = 0;
    private ExitGames.Client.Photon.Hashtable m_playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
    private Coroutine m_pingCoroutine;

    // Use this for initialization
    private void Awake()
    {
        Instance = this;

        PhotonView = GetComponent<PhotonView>();

        PlayerName = "Client#" + Random.Range(1000, 9999);
        
        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 30;

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            if (PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
    }
	
    private void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        XRSettings.enabled = true;
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    private void RPC_LoadedGameScene(PhotonPlayer photonPlayer)
    {
        PlayersInGame++;
        if (PlayersInGame == PhotonNetwork.playerList.Length)
        {
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
            XRSettings.enabled = true;
        }
    } 

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        int ID = PlayerLayoutGroup.Instance.PlayerListings.FindIndex(player => player.PlayerName.text == PlayerName);

        GameObject positions = GameObject.FindGameObjectWithTag("Respawn");
        GameObject posObj = positions.transform.GetChild(ID).gameObject;
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), posObj.transform.position, posObj.transform.rotation, 0);
    }
    
    private IEnumerator C_SetPing()
    {
        while (PhotonNetwork.connected)
        {
            m_playerCustomProperties["Ping"] = PhotonNetwork.GetPing();
            PhotonNetwork.player.SetCustomProperties(m_playerCustomProperties);

            yield return new WaitForSeconds(5f);
        }

        yield break;
    }


    //When connected to the master server (photon).
    private void OnConnectedToMaster()
    {
        if (m_pingCoroutine != null)
            StopCoroutine(m_pingCoroutine);
        m_pingCoroutine = StartCoroutine(C_SetPing());
    }

    [PunRPC]
    private void RPC_ApplyTexture(byte[] data)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(data);
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        ImageManager.Instance.sharedImage.enabled = true;
        ImageManager.Instance.sharedImage.sprite = newSprite;
    }

    [PunRPC]
    private void RPC_RemoveImage()
    {
        ImageManager.Instance.sharedImage.enabled = false;
    }
}