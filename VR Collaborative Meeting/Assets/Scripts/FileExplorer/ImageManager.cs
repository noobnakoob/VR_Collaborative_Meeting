using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour {

    public static ImageManager Instance;

    PhotonView PhotonView;

	// Use this for initialization
	void Awake () {

        Instance = this;
        PhotonView = GameObject.Find("PlayerNetwork").GetComponent<PhotonView>();
    }

    public void GenerateTexture(byte[] generated_Texture)
    {        
        if (PhotonNetwork.isMasterClient)
        {
            PhotonView.RPC("RPC_ApplyTexture", PhotonTargets.MasterClient, generated_Texture);
            //PhotonView.RPC("RPC_ApplyTexture", PhotonTargets.Others, generated_Texture);
        }
    }  
    
    public void RemoveTexture()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonView.RPC("RPC_RemoveImage", PhotonTargets.MasterClient);
            PhotonView.RPC("RPC_RemoveImage", PhotonTargets.Others);
        }
    }
}
