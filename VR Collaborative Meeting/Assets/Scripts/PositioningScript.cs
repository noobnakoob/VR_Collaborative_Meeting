using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositioningScript : Photon.MonoBehaviour {

    public GameObject head;
    public GameObject body;

	// Use this for initialization
	void Start () {

        body.transform.parent.position = CameraManager.instance.head.transform.position - new Vector3(0f, 1.65f, 0f);
    }

    // Update is called once per frame
    void LateUpdate () {
		
        if (photonView.isMine)
        {
            head.transform.position = CameraManager.instance.head.transform.position;
            head.transform.localRotation = CameraManager.instance.head.transform.rotation;
            //body.transform.localRotation = new Quaternion(0f, 0f, CameraManager.instance.head.transform.rotation.y, Mathf.Cos(CameraManager.instance.head.transform.rotation.y / 2) + 
            //    Mathf.Sin(CameraManager.instance.head.transform.rotation.y / 2));
            body.transform.localRotation = Quaternion.Euler(0f, CameraManager.instance.head.transform.rotation.eulerAngles.y, 0f);
            CameraManager.instance.head.transform.parent.position = new Vector3(body.transform.parent.position.x,
                CameraManager.instance.head.transform.parent.transform.position.y,
                body.transform.parent.position.z);
        }
    }
}