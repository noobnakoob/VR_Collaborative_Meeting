using UnityEngine;

public class DDOL : MonoBehaviour {

    public static DDOL Instance;

    // Use this for initialization
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
	
}
