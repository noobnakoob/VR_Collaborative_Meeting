using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = 3.0F;
    void Update()
    {
        float forward = Input.GetAxis("Vertical") * speed;
        float side = Input.GetAxis("Horizontal") * speed;
        forward *= Time.deltaTime;
        side *= Time.deltaTime;
        //transform.Translate(0, 0, translation);
        //cameraHolder.Rotate(0, rotation, 0);
        Vector3 camForward = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
        Vector3 camSide = Vector3.Scale(-Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = forward * camForward + side * camSide;
        transform.Translate(move);
    }
}
