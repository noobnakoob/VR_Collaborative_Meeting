using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogListing : MonoBehaviour {

    [SerializeField]
    private Text _logText;
    private Text LogText
    {
        get { return _logText; }
    }
 
    public void GenerateLog(string log)
    {
        LogText.text = log;
    }
}