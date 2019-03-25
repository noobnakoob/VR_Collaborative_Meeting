using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour {

    public static ImageManager Instance;

    PhotonView PhotonView;
    public Image sharedImage;

    // Use this for initialization
    void Awake () {

        Instance = this;
        PhotonView = GameObject.Find("PlayerNetwork").GetComponent<PhotonView>();
    }

    public void GenerateTexture(Texture2D texture)
    {        
        if (PhotonNetwork.isMasterClient)
        {
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sharedImage.enabled = true;
            sharedImage.sprite = null;
            sharedImage.sprite = newSprite;            

            PhotonView.RPC("RPC_ApplyTexture", PhotonTargets.Others, texture.EncodeToPNG());            
        }
    }  
    
    public void RemoveTexture()
    {
        if (PhotonNetwork.isMasterClient)
        {
            sharedImage.sprite = null;
            sharedImage.enabled = false;
            PhotonView.RPC("RPC_RemoveImage", PhotonTargets.All);
        }
    }
}
