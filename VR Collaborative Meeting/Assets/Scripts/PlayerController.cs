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
        Vector3 camForward = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
        Vector3 camSide = Vector3.Scale(-Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = forward * camForward + side * camSide;
        transform.Translate(move);

        if (Input.GetKey(KeyCode.E))
            Camera.main.transform.Rotate(new Vector3(0f, speed ,0f));
        else if (Input.GetKey(KeyCode.Q))
            Camera.main.transform.Rotate(new Vector3(0f, -speed, 0f));
    }
}
