using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject head;

    public static CameraManager instance;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
}