using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

    public class FileButton : MonoBehaviour {

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
        }

        public void OnGazeExit()
        {
            gazed = false;
        }          

        private IEnumerator ClickRoutine()
        {
            yield return new WaitForSeconds(2f);

            if (gazed)
                _fileBrowser.FileClick(_path);
        }
    }
}
