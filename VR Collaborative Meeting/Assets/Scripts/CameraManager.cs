using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject head;
    public GameObject player;

    float angley, rotx, rotz;

	// Use this for initialization
	void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        angley = transform.rotation.y;
        head.transform.localRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        player.transform.localRotation = new Quaternion(0f, 0f, transform.rotation.y, transform.rotation.w);
        transform.position = head.transform.position;
    }
}