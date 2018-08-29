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

    private List<LogListing> _logListingList = new List<LogListing>();
    private List<LogListing> LogListingList
    {
        get { return _logListingList; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void AddNewLog(string log)
    {
        GameObject logObj = Instantiate(LogListingPrefab);
        logObj.transform.SetParent(transform, false);

        LogListing newLog = logObj.GetComponent<LogListing>();
        newLog.SetLog(log);
        LogListingList.Add(newLog);
    }
}
