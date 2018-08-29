using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogListing : MonoBehaviour {

    [SerializeField]
    private Text _logListingText;
    private Text LogListingText
    {
        get { return _logListingText; }
    }

    public void SetLog(string log)
    {
        LogListingText.text = log;
    }
}
