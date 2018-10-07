using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class DDOL : MonoBehaviour {

    public static DDOL Instance;

    // Use this for initialization
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {          
            Instance = this;
            DontDestroyOnLoad(this);
        }
        XRSettings.enabled = false;

        SceneManager.LoadScene(1);
    }	
}