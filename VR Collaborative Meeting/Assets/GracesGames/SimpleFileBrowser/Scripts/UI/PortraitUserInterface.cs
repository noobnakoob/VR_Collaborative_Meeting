using UnityEngine.UI;
using System.Collections;
using GracesGames.Common.Scripts;
using UnityEngine;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

	public class PortraitUserInterface : UserInterface {

		protected override void SetupParents() {
			// Find directories parent to group directory buttons
			DirectoriesParent = Utilities.FindGameObjectOrError("Items");
			// Find files parent to group file buttons
			FilesParent = Utilities.FindGameObjectOrError("Items");
			// Set the button height
			SetButtonParentHeight(DirectoriesParent, ItemButtonHeight);
			SetButtonParentHeight(FilesParent, ItemButtonHeight);
			// Set the panel color
			Utilities.FindGameObjectOrError("ItemPanel").GetComponent<Image>().color = DirectoryPanelColor;
		}

        bool gazed;

        public void OnGazeEnter()
        {
            gazed = true;
        }

        public void OnGazeExit()
        {
            gazed = false;
        }

        public void OnLeaveDirectory()
        {
            StartCoroutine(LeaveDirectoryRoutine());
        }

        private IEnumerator LeaveDirectoryRoutine()
        {
            yield return new WaitForSeconds(2f);

            if (gazed)
            {
                FileBrowser fileBrowserScript = GameObject.Find("FileBrowser").gameObject.GetComponent<FileBrowser>();
                fileBrowserScript.DirectoryUp();
            }
        }

        public void OnCloseFileBrowser()
        {
            StartCoroutine(CloseFileBrowserRoutine());
        }

        private IEnumerator CloseFileBrowserRoutine()
        {
            yield return new WaitForSeconds(2f);

            if (gazed)
            {
                FileBrowser fileBrowserScript = GameObject.Find("FileBrowser").gameObject.GetComponent<FileBrowser>();
                DemoCaller.Instance.CloseFileBrowser();
                fileBrowserScript.CloseFileBrowser();
            }
        }
    }
}
