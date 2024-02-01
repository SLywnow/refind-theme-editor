using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
public class testFileBrowser : MonoBehaviour
{
    void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Main", ".logs"));
        FileBrowser.SetDefaultFilter(".logs");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Test", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            Debug.Log(FileBrowser.Result[0]);
        }
        else
		{
            Debug.Log("Stop");
		}
    }
}
