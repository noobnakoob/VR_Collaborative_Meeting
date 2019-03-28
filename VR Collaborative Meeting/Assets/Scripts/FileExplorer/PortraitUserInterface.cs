using UnityEngine.UI;
using System.Collections;
using UnityEngine;

public class PortraitUserInterface : UserInterface
{
    private void Awake()
    {
        MainManager.Instance.SetPortraitUserInterface(this);
    }

    protected override void SetupParents()
    {
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

    private void LateUpdate()
    {
        for (int i = 0; i < DirectoriesParent.transform.childCount; i++)
        {
            var child = DirectoriesParent.transform.GetChild(i).transform.gameObject;
            if (child.GetComponent<RectTransform>().position.y > 3.8f || child.GetComponent<RectTransform>().position.y < 2.3f)
            {
                for (int j = 0; j < child.transform.childCount; j++)
                {
                    var pos = child.transform.GetChild(j).transform.gameObject.GetComponent<RectTransform>().anchoredPosition3D;
                    child.transform.GetChild(j).transform.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos.x, pos.y, 10000);
                }
            }
            else
            {
                for (int j = 0; j < child.transform.childCount; j++)
                {
                    var pos = child.transform.GetChild(j).transform.gameObject.GetComponent<RectTransform>().anchoredPosition3D;
                    child.transform.GetChild(j).transform.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos.x, pos.y, 0);
                }
            }
        }
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
            MainManager.Instance.FileBrowser.DirectoryUp();
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
            MainManager.Instance.FileBrowserManager.CloseFileBrowser();
            MainManager.Instance.FileBrowser.CloseFileBrowser();
        }
    }

    public void OnScrollDown()
    {
        StartCoroutine(ScrollDown());
    }

    public void ScrollDownVoice()
    {
        float cellSizeY = DirectoriesParent.GetComponent<GridLayoutGroup>().cellSize.y;
        float totalSize = DirectoriesParent.transform.childCount * cellSizeY;

        if (DirectoriesParent.GetComponent<RectTransform>().offsetMax.y < totalSize)
            DirectoriesParent.GetComponent<RectTransform>().offsetMax = new Vector2(DirectoriesParent.GetComponent<RectTransform>().offsetMax.x,
                DirectoriesParent.GetComponent<RectTransform>().offsetMax.y + cellSizeY * 4);
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
                    DirectoriesParent.GetComponent<RectTransform>().offsetMax.y + cellSizeY * 4);
            
            yield return new WaitForSeconds(2f);
        }
        while (scrollDown_Gazed);
    }

    public void OnScrollUp()
    {
        StartCoroutine(ScrollUp());
    }

    public void ScrollUpVoice()
    {
        float cellSizeY = DirectoriesParent.GetComponent<GridLayoutGroup>().cellSize.y;

        if (DirectoriesParent.GetComponent<RectTransform>().offsetMax.y > 0)
            DirectoriesParent.GetComponent<RectTransform>().offsetMax = new Vector2(DirectoriesParent.GetComponent<RectTransform>().offsetMax.x,
                DirectoriesParent.GetComponent<RectTransform>().offsetMax.y - cellSizeY * 4);
    }

    IEnumerator ScrollUp()
    {
        yield return new WaitForSeconds(2f);

        float cellSizeY = DirectoriesParent.GetComponent<GridLayoutGroup>().cellSize.y;

        do
        {
            if (DirectoriesParent.GetComponent<RectTransform>().offsetMax.y > 0)
                DirectoriesParent.GetComponent<RectTransform>().offsetMax = new Vector2(DirectoriesParent.GetComponent<RectTransform>().offsetMax.x,
                    DirectoriesParent.GetComponent<RectTransform>().offsetMax.y - cellSizeY * 4);

            yield return new WaitForSeconds(2f);
        }
        while (scrollUp_Gazed);
    }
}