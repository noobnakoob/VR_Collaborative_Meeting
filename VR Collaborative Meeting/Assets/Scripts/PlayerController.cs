using UnityEngine;

public class PlayerController : Photon.MonoBehaviour
{
    [SerializeField]
    private GameObject _head;
    private GameObject Head
    {
        get { return _head; }
    }
    
    private PhotonView PhotonView;
    private readonly float speed = 3.0f;
    private Vector3 TargetPosition;
    private Quaternion TargetRotation;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    void LateUpdate()
    {
        if (PhotonView.isMine)
            CheckInput();
    }

    void CheckInput()
    {
        float forward = Input.GetAxis("Vertical");

        Head.transform.rotation = Camera.main.transform.rotation;
        transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

        Camera.main.transform.parent.position = new Vector3(transform.position.x,
            Camera.main.transform.parent.transform.position.y,
            transform.position.z);

        transform.position += Vector3.Scale(Camera.main.transform.forward, new Vector3(1f, 0f, 1f)) * (forward * speed * Time.deltaTime);

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.E))
            Camera.main.transform.Rotate(new Vector3(0f, speed, 0f));
        else if (Input.GetKey(KeyCode.Q))
            Camera.main.transform.Rotate(new Vector3(0f, -speed, 0f));
        else if (Input.GetKey(KeyCode.A))
            Camera.main.transform.Rotate(new Vector3(0f, 0f, speed));
        else if (Input.GetKey(KeyCode.D))
            Camera.main.transform.Rotate(new Vector3(0f, 0f, -speed));
        else if (Input.GetKey(KeyCode.Z))
            Camera.main.transform.Rotate(new Vector3(speed, 0f, 0f));
        else if (Input.GetKey(KeyCode.C))
            Camera.main.transform.Rotate(new Vector3(-speed, 0f, 0f));
#endif
    }
}