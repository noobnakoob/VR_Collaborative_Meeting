using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FileButton : MonoBehaviour
{
    public bool directory;

    // The path of the button
    private string _path = "";

    private bool gazed;

    // click and double click variables
    private int _clickCount;
    private float _firstClickTime;

    // Change this constant to tweak the time between single and double clicks
    private const float DoubleClickInterval = 0.25f;

    // Set variables, called by UserInterface script
    public void Setup(string path)
    {
        _path = path;
    }

    public void OnGazeEnter()
    {
        gazed = true;
        StartCoroutine(ClickRoutine());
        transform.GetChild(0).gameObject.GetComponent<Text>().color = Color.green;
    }

    public void OnGazeExit()
    {
        gazed = false;
        transform.GetChild(0).gameObject.GetComponent<Text>().color = Color.white;
    }

    private IEnumerator ClickRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (gazed && !directory)
            MainManager.Instance.FileBrowser.FileClick(_path);
        else if (gazed && directory)
            MainManager.Instance.FileBrowser.DirectoryClick(_path);
    }

    public void FileClickedVoice()
    {
        if (!directory)
            MainManager.Instance.FileBrowser.FileClick(_path);
        else if (directory)
            MainManager.Instance.FileBrowser.DirectoryClick(_path);
    }
}