using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

    public class FileButton : MonoBehaviour {

        public bool directory;

        // The file browser using this file button
        private FileBrowser _fileBrowser;
        
        // The path of the button
        private string _path = "";

        private bool gazed;

        // click and double click variables
        private int _clickCount;
        private float _firstClickTime;
        // Change this constant to tweak the time between single and double clicks
        private const float DoubleClickInterval = 0.25f;

        // Set variables, called by UserInterface script
        public void Setup(FileBrowser fileBrowser, string path) {
            _fileBrowser = fileBrowser;
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
            yield return new WaitForSeconds(5f);

            if (gazed && !directory)
                _fileBrowser.FileClick(_path);
            else if (gazed && directory)
                _fileBrowser.DirectoryClick(_path);
        }
    }
}
