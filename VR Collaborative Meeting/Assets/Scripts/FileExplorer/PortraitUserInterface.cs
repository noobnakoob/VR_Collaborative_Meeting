using UnityEngine.UI;
using System.Collections;
using GracesGames.Common.Scripts;
using UnityEngine;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

	public class PortraitUserInterface : UserInterface {

        public static PortraitUserInterface Instance;

        private void Awake()
        {
            Instance = this;
        }

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

        bool directory_Gazed;
        bool scrollUp_Gazed;
        bool scrollDown_Gazed;
        bool close_Gazed;

        public void OnGazeEnterDirectoryUp()
        {
            directory_Gazed = true;
            scrollUp_Gazed = false;
            scrollDown_Gazed = false;
            close_Gazed = false;
        }

        public void OnGazeEnterScrollUp()
        {
            directory_Gazed = false;
            scrollUp_Gazed = true;
            scrollDown_Gazed = false;
            close_Gazed = false;
        }

        public void OnGazeEnterScrollDown()
        {
            directory_Gazed = false;
            scrollUp_Gazed = false;
            scrollDown_Gazed = true;
            close_Gazed = false;
        }

        public void OnGazeEnterClose()
        {
            directory_Gazed = false;
            scrollUp_Gazed = false;
            scrollDown_Gazed = false;
            close_Gazed = true;
        }

        public void OnGazeExit()
        {
            directory_Gazed = false;
            scrollUp_Gazed = false;
            scrollDown_Gazed = false;
            close_Gazed = false;
        }

        public void OnLeaveDirectory()
        {
            StartCoroutine(LeaveDirectoryRoutine());
        }

        private IEnumerator LeaveDirectoryRoutine()
        {
            yield return new WaitForSeconds(2f);

            do
            {               
                FileBrowser fileBrowserScript = GameObject.Find("FileBrowser").gameObject.GetComponent<FileBrowser>();
                fileBrowserScript.DirectoryUp();
                yield return new WaitForSeconds(1.5f);
            }
            while (directory_Gazed);
        }

        public void OnCloseFileBrowser()
        {
            StartCoroutine(CloseFileBrowserRoutine());
        }

        private IEnumerator CloseFileBrowserRoutine()
        {
            yield return new WaitForSeconds(2f);

            if (close_Gazed)
            {
                FileBrowser fileBrowserScript = GameObject.Find("FileBrowser").gameObject.GetComponent<FileBrowser>();
                DemoCaller.Instance.CloseFileBrowser();
                fileBrowserScript.CloseFileBrowser();
            }
        }

        public void OnScrollDown()
        {
            StartCoroutine(ScrollDown());
        }

        IEnumerator ScrollDown()
        {
            yield return new WaitForSeconds(2f);
            
            float cellSizeY = DirectoriesParent.GetComponent<GridLayoutGroup>().cellSize.y;
            float totalSize = DirectoriesParent.transform.childCount * cellSizeY;

            do
            {
                if (DirectoriesParent.GetComponent<RectTransform>().offsetMax.y < totalSize)
                    DirectoriesParent.GetComponent<RectTransform>().offsetMax = new Vector2(DirectoriesParent.GetComponent<RectTransform>().offsetMax.x,
                        DirectoriesParent.GetComponent<RectTransform>().offsetMax.y + cellSizeY);

                yield return new WaitForSeconds(1.5f);
            }
            while (scrollDown_Gazed);
        }

        public void OnScrollUp()
        {
            StartCoroutine(ScrollUp());
        }

        IEnumerator ScrollUp()
        {
            yield return new WaitForSeconds(2f);

            float cellSizeY = DirectoriesParent.GetComponent<GridLayoutGroup>().cellSize.y;

            do
            {
                if (DirectoriesParent.GetComponent<RectTransform>().offsetMax.y > 0)
                    DirectoriesParent.GetComponent<RectTransform>().offsetMax = new Vector2(DirectoriesParent.GetComponent<RectTransform>().offsetMax.x,
                        DirectoriesParent.GetComponent<RectTransform>().offsetMax.y - cellSizeY);

                yield return new WaitForSeconds(1.5f);
            }
            while (scrollUp_Gazed);
        }
    }
}
