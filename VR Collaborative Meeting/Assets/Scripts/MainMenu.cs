using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MainMenu : MonoBehaviour {

    public TextMeshProUGUI text;
    public GameObject cube;

	// Use this for initialization
	void Start () {

        text.text = "";
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetAxis("Horizontal") > 0)
            cube.transform.position += Vector3.right * Time.deltaTime;

        if (Input.GetAxis("Horizontal") < 0)
            cube.transform.position += -Vector3.right * Time.deltaTime;

        if (Input.GetAxis("Vertical") > 0)
            cube.transform.position += Vector3.up * Time.deltaTime;

        if (Input.GetAxis("Vertical") < 0)
            cube.transform.position += -Vector3.up * Time.deltaTime;

        if (Input.GetKey(KeyCode.Joystick1Button3))
            cube.transform.position += Vector3.forward * Time.deltaTime;

        if (Input.GetKey(KeyCode.Joystick1Button0))
            cube.transform.position += -Vector3.forward * Time.deltaTime;

        //text.text = "Horizontal " + Input.GetAxis("Horizontal").ToString() + " Vertical " + Input.GetAxis("Vertical").ToString();

        //if (Input.anyKey)
        //{
        //    foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
        //    {
        //      //if (Input.GetKeyDown(code))
        //      //  text.text = code.ToString();

        //        //if (Input.GetKey(code))
        //        //    text.text = code.ToString();


        //    }
        //}
    }
}
