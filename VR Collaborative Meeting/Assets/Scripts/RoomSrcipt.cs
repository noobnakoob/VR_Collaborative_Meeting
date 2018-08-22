using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomSrcipt : MonoBehaviour {

    public GameObject[] Lights;
    bool isLightUp = true;

	// Use this for initialization
	void Start () {
        Lights = GameObject.FindGameObjectsWithTag("light");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isLightUp = !isLightUp;
            foreach(GameObject Light in Lights)
            {
                Light.SetActive(isLightUp);
            }
        }
	}
}
