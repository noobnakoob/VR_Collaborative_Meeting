using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject head;
    public GameObject player;
    //public GameObject head2
        

	// Use this for initialization
	void Start () {

        //transform.position = head.transform.position;
        //transform.rotation = new Quaternion(0f, 90f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {

        head.transform.localRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        player.transform.localRotation = new Quaternion(player.transform.localRotation.x, transform.rotation.y, player.transform.localRotation.z, player.transform.localRotation.w);
        transform.parent.position = head.transform.position;
    }
}