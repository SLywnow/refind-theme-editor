using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SLywnow;
using System.Linq;
using UnityEngine.WSA;
using System.Xml;

public class openfile : MonoBehaviour
{
    public FileBrowser browser;
    public CurentState state;
    public static OutPutFile output;

    public List<string> dirs;
    public List<string> dirsnames;
    public List<string> files;
    public List<string> filesnames;

	public void Start()
	{
        OpenFolder(true);
    }

	void Update()
    {
        if (string.IsNullOrEmpty(state.select))
		{
            browser.select.interactable = false;
            browser.selecttext.text = "";
        }
        else
		{
            browser.select.interactable = true;
            if (state.folder)
                browser.selecttext.text = browser.openfolder;
            else
                browser.selecttext.text = browser.openfile;
        }
    }

    public void CheckSelect()
    {
        if (state.folder)
            OpenFolder(false);
        else
            SetFile();
    }

    public void MoveUp()
	{
        state.select = state.prevpath;
        OpenFolder(false);
    }

    public void OpenFolder(bool first)
    {
        string path;
        if (first)
		{
            path = FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/projects";
            state.path = path;
        }
        else
		{
            path = state.select;
            state.path = path;
        }
        state.prevpath = path;
        for (int i = state.prevpath.Length - 1; i >= 0; i--)
        {
            if (!(state.prevpath[i] == '/' || state.prevpath[i] == '\\'))
            {
                //Debug.Log(state.prevpath[i]);
                state.prevpath = state.prevpath.Remove(i);
            }
            else
            {
                state.prevpath = state.prevpath.Remove(i);
                break;
            }
        }
        browser.path.text = path;

        state.select = "";

        dirs = FilesSet.GetFilesFromdirectories(path, "", false, FilesSet.TypeOfGet.Directory).ToList<string>();
        dirsnames = FilesSet.GetFilesFromdirectories(path, "", false, FilesSet.TypeOfGet.NamesOfDirectories).ToList<string>();

        string tpe="";
        if (state.CurrentType == CurentState.current.image)
            tpe = state.imagefiles;
        if (state.CurrentType == CurentState.current.text)
            tpe = state.textfiles;
        if (state.CurrentType == CurentState.current.audio)
            tpe = state.audfiles;

        files = FilesSet.GetFilesFromdirectories(path, tpe, false, FilesSet.TypeOfGet.Files).ToList<string>();
        filesnames = FilesSet.GetFilesFromdirectories(path, tpe, false, FilesSet.TypeOfGet.NamesOfFiles).ToList<string>();


        for (int i = browser.parrent.childCount - 1; i >= 0; i--)
        {
            Destroy(browser.parrent.GetChild(i).gameObject);
        }

        //setup
		{
			{
                GameObject obj = Instantiate(browser.FileBlock, browser.parrent);
                obj.SetActive(true);
                obj.GetComponent<openFileBlock>().id = -1;
                obj.GetComponent<openFileBlock>().mypath = state.prevpath;
                obj.GetComponent<openFileBlock>().folder = true;
                if (string.IsNullOrEmpty(state.prevpath))
                    obj.GetComponent<Button>().interactable = false;
                
                FastFind.FindChild(obj.transform, "Name").GetComponent<Text>().text = browser.moveuptext;
                FastFind.FindChild(obj.transform, "Icon").GetComponent<Image>().sprite = browser.moveup;
            }

            int idcur = 0;
            for (int i=0;i<dirs.Count;i++)
			{
                GameObject obj = Instantiate(browser.FileBlock, browser.parrent);
                obj.SetActive(true);
                obj.GetComponent<openFileBlock>().id= idcur;
                obj.GetComponent<openFileBlock>().mypath = dirs[i];
                obj.GetComponent<openFileBlock>().folder = true;
                FastFind.FindChild(obj.transform, "Name").GetComponent<Text>().text = dirsnames[i];
                FastFind.FindChild(obj.transform, "Icon").GetComponent<Image>().sprite = browser.folder;
                idcur++;
			}
            for (int i = 0; i < files.Count; i++)
            {
                GameObject obj = Instantiate(browser.FileBlock, browser.parrent);
                obj.SetActive(true);
                obj.GetComponent<openFileBlock>().id = idcur;
                obj.GetComponent<openFileBlock>().mypath = files[i];
                obj.GetComponent<openFileBlock>().folder = false;
                FastFind.FindChild(obj.transform, "Name").GetComponent<Text>().text = filesnames[i];
                FastFind.FindChild(obj.transform, "Icon").GetComponent<Image>().sprite = browser.file;
                idcur++;
            }
        }
    }


    public void SetFile ()
	{
        if (state.CurrentType == CurentState.current.image)
		{
            FileStream streamimg = File.Open(state.select, FileMode.Open);
            byte[] imgbt = new byte[streamimg.Length];
            streamimg.Read(imgbt, 0, imgbt.Length);
            streamimg.Close();

            output.tex = new Texture2D(0, 0);
            output.tex.LoadImage(imgbt);
        }
        /*if (state.CurrentType == CurentState.current.audio)
        {
            FileStream streamimg = File.Open(state.select, FileMode.Open);
            byte[] imgbt = new byte[streamimg.Length];
            streamimg.Read(imgbt, 0, imgbt.Length);
            streamimg.Close();

            output.aud = new AudioClip();
            output.aud.LoadAudioData(imgbt);
        }*/
        if (state.CurrentType == CurentState.current.text)
        {
            FileStream streamimg = File.Open(state.select, FileMode.Open);
            byte[] imgbt = new byte[streamimg.Length];
            streamimg.Read(imgbt, 0, imgbt.Length);
            streamimg.Close();

            output.text = imgbt.ToString();
        }
    }

    public static Texture2D GetFileTexture()
    {
        Texture2D ret = output.tex;
        output.Clear();
        return ret;
    }

    public static string GetFileString()
    {
        string ret = output.text;
        output.Clear();
        return ret;
    }

    public static AudioClip GetFileAudio()
    {
        AudioClip ret = output.aud;
        output.Clear();
        return ret;
    }
}

[System.Serializable]
public class FileBrowser
{
    public GameObject main;
    public Button select;
    public Text selecttext;
    public InputField path;
    public GameObject FileBlock;
    public Transform parrent;
    public string moveuptext;
    public string openfolder;
    public string openfile;
    public Sprite folder;
    public Sprite file;
    public Sprite moveup;

    public int selectid;
}

[System.Serializable]
public class OutPutFile
{
    public Texture2D tex= new Texture2D(0,0);
    public string text;
    public AudioClip aud;
    public bool hasfile;

    public void Clear()
	{
        tex = null;
        text = "";
        aud = null;
        hasfile = false;
    }
}

[System.Serializable]
public class CurentState
{
    public string prevpath;
    public string path;
    public string select;
    public bool folder;
    public enum current { image, text, audio };
    public current CurrentType;

    public string imagefiles;
    public string textfiles;
    public string audfiles;
}
