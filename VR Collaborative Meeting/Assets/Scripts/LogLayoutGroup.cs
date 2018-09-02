using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogLayoutGroup : MonoBehaviour {

    public static LogLayoutGroup Instance;

    [SerializeField]
    private GameObject _logListingPrefab;
    private GameObject LogListingPrefab
    {
        get { return _logListingPrefab; }
    }

    private List<LogListing> _logList = new List<LogListing>();
    private List<LogListing> LogList
    {
        get { return _logList; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void AddNewLog(string log)
    {
        if (LogList.Count >= 9)
        {
            foreach (Transform tr in transform)
            {
                if (tr != this)
                    Destroy(tr.gameObject);
            }
            LogList.Clear();
        }

        GameObject newLog = Instantiate(LogListingPrefab, transform, false);

        LogListing logListing = newLog.GetComponent<LogListing>();
        logListing.GenerateLog(log);
        
        LogList.Add(logListing);
    }
}