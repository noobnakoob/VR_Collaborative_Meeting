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

    private void Start()
    {
        startPPTXAplication();
    }

    private void startPPTXAplication()
    {
        bool fail = false;
        string bundleId = "com.petar.pptx";
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
            if (launchIntent == null)
                fail = true;
        }
        catch (System.Exception e)
        {
            fail = true;
        }

        if (fail)
        {
            InstalPPTXViewerAPK();            
        }
        else
        {
            string apkPath = Application.persistentDataPath + "/PPTXViewer.apk";
            if (File.Exists(apkPath))
            {
                File.Delete(apkPath);
            }
            currentActivity.Call("startActivity", launchIntent);
        }

        unityPlayer.Dispose();
        currentActivity.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
    }

    private void InstalPPTXViewerAPK()
    {
        string apkPath = Application.persistentDataPath + "/PPTXViewer.apk";
        string apkPathSA = Path.Combine(Application.streamingAssetsPath, "PPTXViewer.apk");
        
        byte[] result = null;
        if (apkPathSA.Contains("://") || apkPathSA.Contains(":///"))
        {
            WWW file = new WWW(apkPathSA);
            while (!file.isDone) { }
            result = file.bytes;
        }
        else
            result = File.ReadAllBytes(apkPathSA);        

        if (!File.Exists(apkPath) || new FileInfo(apkPath).Length == 0)
        {
            File.WriteAllBytes(apkPath, result);
        } 

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject unityContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        //Get the package Name
        string packageName = unityContext.Call<string>("getPackageName");
        string authority = packageName + ".fileprovider";

        AndroidJavaClass intentObj = new AndroidJavaClass("android.content.Intent");
        string ACTION_VIEW = intentObj.GetStatic<string>("ACTION_VIEW");
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", ACTION_VIEW);


        int FLAG_ACTIVITY_NEW_TASK = intentObj.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK");
        int FLAG_GRANT_READ_URI_PERMISSION = intentObj.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION");

        //File fileObj = new File(String pathname);
        AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", apkPath);
        //FileProvider object that will be used to call it static function
        AndroidJavaClass fileProvider = new AndroidJavaClass("android.support.v4.content.FileProvider");
        //getUriForFile(Context context, String authority, File file)
        AndroidJavaObject uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", unityContext, authority, fileObj);

        intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive");
        intent.Call<AndroidJavaObject>("addFlags", FLAG_ACTIVITY_NEW_TASK);
        intent.Call<AndroidJavaObject>("addFlags", FLAG_GRANT_READ_URI_PERMISSION);
        currentActivity.Call("startActivity", intent);

        unityPlayer.Dispose();
        currentActivity.Dispose();
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        PlayersInGame = 0;
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
        }
    } 

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        XRSettings.enabled = true;

        int ID = MainCanvasManager.Instance.PlayerLayoutGroup.PlayerListings.FindIndex(player => player.PlayerName.text == PlayerName);

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
        ImageManager.Instance.sharedImage.sprite = null;
        ImageManager.Instance.sharedImage.sprite = newSprite;
    }

    [PunRPC]
    private void RPC_RemoveImage()
    {
        ImageManager.Instance.sharedImage.enabled = false;
    }
}