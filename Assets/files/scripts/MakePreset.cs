using SLywnow;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MakePreset : MonoBehaviour
{
    public GameObject showui;
    public GameObject selectproj;
    public GameObject projprev;
    public AudioSource makescreen;
    public CanvasScaler canvas;

    public PreviewSetup prew;
    public SceneOptions prevopt;
    public SceneSetUp scene;
    public SceneObjs sceneobj;

    public enum md {select,editshow, show }
    public md mode;

    public ProjectFiles curProj;
    List<Sprite> bgs;
    List<Sprite> ics;
    List<Sprite> sls;

    public void Start()
    {
        GetProjects();
        mode = md.select;
        sceneobj.canvas.SetActive(false);
        selectproj.SetActive(true);
        showui.SetActive(false);
        projprev.SetActive(false);
    }

    public void GetProject(int i)
    {
        mode = md.editshow;
        //Debug.Log(i);
        projprev.SetActive(true);
        selectproj.SetActive(false);

        curProj.dir = FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/projects/" + dirs[i];

        //load structure
        //load bg
        if (FilesSet.CheckFile(curProj.dir + "/background.png", false))
        {
            FileStream streamimg = File.Open(curProj.dir + "/background.png", FileMode.Open);
            byte[] imgbt = new byte[streamimg.Length];
            streamimg.Read(imgbt, 0, imgbt.Length);
            streamimg.Close();

            Texture2D imgtex = new Texture2D(0, 0);
            imgtex.LoadImage(imgbt);
            curProj.bg = Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            curProj.bg = prew.defbg;
        }
        //load sel
        if (FilesSet.CheckFile(curProj.dir + "/selection_big.png", false))
        {
            FileStream streamimg = File.Open(curProj.dir + "/selection_big.png", FileMode.Open);
            byte[] imgbt = new byte[streamimg.Length];
            streamimg.Read(imgbt, 0, imgbt.Length);
            streamimg.Close();

            Texture2D imgtex = new Texture2D(0, 0);
            imgtex.LoadImage(imgbt);
            curProj.b_select = Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            curProj.b_select = prew.defsel;
        }
        if (FilesSet.CheckFile(curProj.dir + "/selection_small.png", false))
        {
            FileStream streamimg = File.Open(curProj.dir + "/selection_small.png", FileMode.Open);
            byte[] imgbt = new byte[streamimg.Length];
            streamimg.Read(imgbt, 0, imgbt.Length);
            streamimg.Close();

            Texture2D imgtex = new Texture2D(0, 0);
            imgtex.LoadImage(imgbt);
            curProj.l_select = Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            curProj.l_select = prew.defsel;
        }
        //load ico
        string[] icons = null;
        if (FilesSet.CheckDirectory(curProj.dir + "/icons"))
        {
            icons = FilesSet.GetFilesFromdirectories(curProj.dir + "/icons", "png", false, FilesSet.TypeOfGet.NamesOfFiles);
            for (int a = 0; a < icons.Length; a++)
            {
                if (icons[a].IndexOf("os_") >= 0)
                {
                    FileStream streamimg = File.Open(curProj.dir + "/icons/" + icons[a] + ".png", FileMode.Open);
                    byte[] imgbt = new byte[streamimg.Length];
                    streamimg.Read(imgbt, 0, imgbt.Length);
                    streamimg.Close();

                    Texture2D imgtex = new Texture2D(0, 0);
                    imgtex.LoadImage(imgbt);
                    curProj.oss.Add(Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f)));
                    curProj.ossp.Add(icons[a].Replace("os_", ""));
                }
                if (icons[a].IndexOf("func_") >= 0)
                {
                    FileStream streamimg = File.Open(curProj.dir + "/icons/" + icons[a] + ".png", FileMode.Open);
                    byte[] imgbt = new byte[streamimg.Length];
                    streamimg.Read(imgbt, 0, imgbt.Length);
                    streamimg.Close();

                    Texture2D imgtex = new Texture2D(0, 0);
                    imgtex.LoadImage(imgbt);
                    curProj.funcs.Add(Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f)));
                    curProj.funcsp.Add(icons[a].Replace("func_", ""));
                }
            }
        }

        //get theme.conf
        if (FilesSet.CheckFile(curProj.dir + "/theme.conf", false))
        {
            List<string> conf = FilesSet.LoadStream(curProj.dir + "/theme.conf", false).ToList();
            for (int c=0;c<conf.Count;c++)
            {
                if (!((conf[c].IndexOf("#")) >= 0) && (conf[c].IndexOf("big_icon_size ") >= 0)) curProj.bigicon = int.Parse(conf[c].Replace("big_icon_size ", ""));
                if (!((conf[c].IndexOf("#")) >= 0) && (conf[c].IndexOf("small_icon_size ") >= 0)) curProj.smallicon = int.Parse(conf[c].Replace("small_icon_size ", ""));
            }
            if (curProj.bigicon==0)
                curProj.bigicon = 256;
            if (curProj.smallicon == 0)
                curProj.smallicon = 96;
        }
        else
        {
            curProj.bigicon = 256;
            curProj.smallicon = 96;
        }

        //setup workspace
        scene.bg.sprite = curProj.bg;
        prevopt.bigicon.text = curProj.bigicon+"";
        prevopt.smallicon.text = curProj.smallicon + "";
        prevopt.screenX.text = Screen.width + "";
        prevopt.screenY.text = Screen.height + "";
        prevopt.selbig.sprite = curProj.b_select;
        prevopt.selsmall.sprite = curProj.l_select;

        for (int o=0;o< curProj.oss.Count;o++)
        {
            GameObject obj = Instantiate(prevopt.togpresset,prevopt.osscroll);
            obj.SetActive(true);
            FastFind.FindChild(obj.transform, "Icon").GetComponent<Image>().sprite = curProj.oss[o];
            FastFind.FindChild(obj.transform, "Name").GetComponent<Text>().text = curProj.ossp[o];
            obj.GetComponent<ProjPrevToggle>().i = o;
            obj.GetComponent<ProjPrevToggle>().tpe = ProjPrevToggle.tp.os;
            prevopt.ostog.Add(obj.GetComponent<Toggle>());
        }
        for (int f = 0; f < curProj.funcs.Count; f++)
        {
            GameObject obj = Instantiate(prevopt.togpresset, prevopt.funcscroll);
            obj.SetActive(true);
            FastFind.FindChild(obj.transform, "Icon").GetComponent<Image>().sprite = curProj.funcs[f];
            FastFind.FindChild(obj.transform, "Name").GetComponent<Text>().text = curProj.funcsp[f];
            obj.GetComponent<ProjPrevToggle>().i = f;
            obj.GetComponent<ProjPrevToggle>().tpe = ProjPrevToggle.tp.func;
            prevopt.functog.Add(obj.GetComponent<Toggle>());
        }
    }

    public void CloseProject()
    {
        mode = md.select;
        projprev.SetActive(false);
        selectproj.SetActive(true);

        for (int i= prevopt.osscroll.childCount-1; i>=0;i--)
        {
            Destroy(prevopt.osscroll.GetChild(i).gameObject);
        }
        for (int i = prevopt.funcscroll.childCount - 1; i >= 0; i--)
        {
            Destroy(prevopt.funcscroll.GetChild(i).gameObject);
        }
        scene.bg.sprite = prew.defbg;
        prevopt.bigicon.text = "0";
        prevopt.smallicon.text = "0";



        curProj = new ProjectFiles();
        prevopt.functog = new List<Toggle>();
        prevopt.ostog = new List<Toggle>();

        GetProjects();
    }

    public List<string> dirs;
    void GetProjects()
    {

        for (int i = prew.parent.childCount - 1; i >= 0; i--)
        {
            Destroy(prew.parent.GetChild(i).gameObject);
        }

        string workdir = FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/projects";
        if (!FilesSet.CheckDirectory(workdir))
            FilesSet.CreateDirectory(workdir);

        dirs = FilesSet.GetFilesFromdirectories(workdir, "", false, FilesSet.TypeOfGet.NamesOfDirectories).ToList();

        bgs = new List<Sprite>();
        ics = new List<Sprite>();
        sls = new List<Sprite>();
        for (int i=0; i<dirs.Count; i++)
        {
            string curdir = workdir + "/" + dirs[i];
            //create button
            GameObject obj = Instantiate(prew.block, prew.parent);
            obj.SetActive(true);

            FastFind.FindChild(obj.transform, "Name").GetComponent<Text>().text= dirs[i];

            obj.GetComponent<ProjSelButton>().i = i;

            Image bg = FastFind.FindChild(obj.transform, "Bg").GetComponent<Image>();
            Image ico = FastFind.FindChild(obj.transform, "Icon").GetComponent<Image>();
            Image sel = FastFind.FindChild(obj.transform, "Selector").GetComponent<Image>();

            //load bg
            if (FilesSet.CheckFile(curdir + "/background.png",false))
            {
                FileStream streamimg = File.Open(curdir + "/background.png", FileMode.Open);
                byte[] imgbt = new byte[streamimg.Length];
                streamimg.Read(imgbt, 0, imgbt.Length);
                streamimg.Close();

                Texture2D imgtex = new Texture2D(0, 0);
                imgtex.LoadImage(imgbt);
                bgs.Add(Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f)));
                bg.sprite = bgs[bgs.Count-1];
            } else
            {
                bgs.Add(prew.defbg);
                bg.sprite = bgs[bgs.Count - 1];
            }
            //load sel
            if (FilesSet.CheckFile(curdir + "/selection_big.png", false))
            {
                FileStream streamimg = File.Open(curdir + "/selection_big.png", FileMode.Open);
                byte[] imgbt = new byte[streamimg.Length];
                streamimg.Read(imgbt, 0, imgbt.Length);
                streamimg.Close();

                Texture2D imgtex = new Texture2D(0, 0);
                imgtex.LoadImage(imgbt);
                sls.Add(Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f)));
                sel.sprite = sls[sls.Count - 1];
            }
            else
            {
                sls.Add(prew.defsel);
                sel.sprite = sls[sls.Count - 1];
            }
            //load ico
            int ind = -1;
            string[] icons = null;
            if (FilesSet.CheckDirectory(curdir + "/icons"))
            {
                icons = FilesSet.GetFilesFromdirectories(curdir + "/icons", "png", false, FilesSet.TypeOfGet.NamesOfFiles);
                for (int a = 0; a < icons.Length; a++)
                {
                    if (icons[a].IndexOf("os_") >= 0) { ind = a; break; }
                }
            }

            if ((ind > -1) && (FilesSet.CheckFile(curdir + "/icons/"+ icons[ind]+".png", false)))
            {
                FileStream streamimg = File.Open(curdir + "/icons/" + icons[ind] + ".png", FileMode.Open);
                byte[] imgbt = new byte[streamimg.Length];
                streamimg.Read(imgbt, 0, imgbt.Length);
                streamimg.Close();

                Texture2D imgtex = new Texture2D(0, 0);
                imgtex.LoadImage(imgbt);
                ics.Add(Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f)));
                ico.sprite = ics[ics.Count - 1];
            }
            else
            {
                ics.Add(prew.defico);
                ico.sprite = ics[ics.Count - 1];
            }
        }
        
    }

    public void OpenFolder()
    {
        Application.OpenURL(FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/Screenshots/");
    }

    public void OpenPFolder()
    {
        Application.OpenURL(curProj.dir+"/");
    }

    public void Run()
    {
        
        mode = md.show;
        sceneobj.canvas.SetActive(true);
        sceneobj.bg.sprite = curProj.bg;

        Screen.SetResolution(int.Parse(prevopt.screenX.text), int.Parse(prevopt.screenY.text), true);

        showui.SetActive(true);
        projprev.SetActive(false);

        sceneobj.osbg.sizeDelta = new Vector2(sceneobj.osbg.sizeDelta.x, curProj.bigicon);
        sceneobj.funcbg.sizeDelta = new Vector2(sceneobj.funcbg.sizeDelta.x, curProj.smallicon);

        for (int i=0;i<prevopt.ostog.Count;i++)
        {
            if (prevopt.ostog[i].isOn) {
                RectTransform obj = Instantiate(sceneobj.img, sceneobj.ostr).GetComponent<RectTransform>();
                obj.gameObject.SetActive(true);
                obj.sizeDelta = new Vector2(curProj.bigicon, curProj.bigicon);
                obj.GetComponent<Image>().sprite = curProj.oss[prevopt.ostog[i].GetComponent<ProjPrevToggle>().i];
                sceneobj.oss.Add(obj);
            }
        }
        for (int i = 0; i < prevopt.functog.Count; i++)
        {
            if (prevopt.functog[i].isOn)
            {
                RectTransform obj = Instantiate(sceneobj.img, sceneobj.functr).GetComponent<RectTransform>();
                obj.gameObject.SetActive(true);
                obj.sizeDelta = new Vector2(curProj.smallicon, curProj.smallicon);
                obj.GetComponent<Image>().sprite = curProj.funcs[prevopt.functog[i].GetComponent<ProjPrevToggle>().i];
                sceneobj.funcs.Add(obj);
            }
        }

        sceneobj.possel = 0;
        if (sceneobj.oss.Count == 0)
            sceneobj.func = true;
        else
            sceneobj.func = false;

        if (!(sceneobj.oss.Count == 0 && sceneobj.funcs.Count == 0))
        {
            sceneobj.cursel = Instantiate(sceneobj.img, sceneobj.selzone);
            sceneobj.cursel.SetActive(true);
            sceneobj.imgsel = sceneobj.cursel.GetComponent<Image>();
            sceneobj.rcsel = sceneobj.cursel.GetComponent<RectTransform>();
            sceneobj.bigsel = new Vector2(curProj.bigicon, curProj.bigicon);
            sceneobj.smsel = new Vector2(curProj.smallicon, curProj.smallicon);
            if (sceneobj.oss.Count>0)
            {
                sceneobj.imgsel.sprite = curProj.b_select;
                sceneobj.rcsel.sizeDelta = new Vector2(curProj.bigicon, curProj.bigicon);
                sceneobj.rcsel.position = sceneobj.oss[sceneobj.possel].position;
            }
            else
            {
                sceneobj.imgsel.sprite = curProj.l_select;
                sceneobj.rcsel.sizeDelta = new Vector2(curProj.smallicon, curProj.smallicon);
                sceneobj.rcsel.position = sceneobj.funcs[sceneobj.possel].position;
            }
        }

    }

    public void ExitShow()
    {
        mode = md.editshow;
        sceneobj.canvas.SetActive(false);
        Screen.SetResolution(1280,720,false);

        for (int i = sceneobj.ostr.childCount - 1; i >= 0; i--)
        {
            Destroy(sceneobj.ostr.GetChild(i).gameObject);
        }
        for (int i = sceneobj.functr.childCount - 1; i >= 0; i--)
        {
            Destroy(sceneobj.functr.GetChild(i).gameObject);
        }

        sceneobj.oss = new List<RectTransform>();
        sceneobj.funcs = new List<RectTransform>();
        if (sceneobj.cursel != null)
            Destroy(sceneobj.cursel);

        showui.SetActive(false);
        projprev.SetActive(true);
    }
    public void MakePhoto()
    {
        showui.SetActive(false);
        StartCoroutine(PhotoCor());
    }
    IEnumerator PhotoCor()
    {
        yield return new WaitForSeconds(0.1f);
        string pname = "Screenshot " + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + ".png";
        if (!FilesSet.CheckDirectory(FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/Screenshots"))
            FilesSet.CreateDirectory(FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/Screenshots");
        ScreenCapture.CaptureScreenshot(FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/Screenshots/" + pname);
        //Debug.Log(FastFind.GetDefaultPath() + "/SLywnow/rEFInd theme editor/Screenshots/" + pname);
        makescreen.Play();
        yield return new WaitForSeconds(0.1f);
        showui.SetActive(true);
    }

    private void Update()
    {
        /*if (mode==md.editshow)
        {
            if (prevopt.ostog.Count > 6) prevopt.ostog.RemoveAt(prevopt.ostog.Count - 1);
            if (prevopt.functog.Count > 7) prevopt.ostog.RemoveAt(prevopt.ostog.Count - 1);
        }*/
        
        if (mode == md.show)
        {
            //exit
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                    ExitShow();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                ExitShow();

            //move selector
            if (!(sceneobj.cursel == null))
            {
                if (sceneobj.func)
                {
                    if (sceneobj.imgsel.sprite != curProj.l_select)
                        sceneobj.imgsel.sprite = curProj.l_select;

                    if (sceneobj.rcsel.sizeDelta != sceneobj.smsel)
                        sceneobj.rcsel.sizeDelta = sceneobj.smsel;

                    if (sceneobj.rcsel.position != sceneobj.funcs[sceneobj.possel].position)
                        sceneobj.rcsel.position = sceneobj.funcs[sceneobj.possel].position;
                }
                else {
                    if (sceneobj.imgsel.sprite != curProj.b_select)
                        sceneobj.imgsel.sprite = curProj.b_select;

                    if (sceneobj.rcsel.sizeDelta != sceneobj.bigsel)
                        sceneobj.rcsel.sizeDelta = sceneobj.bigsel;

                    if (sceneobj.rcsel.position != sceneobj.oss[sceneobj.possel].position)
                        sceneobj.rcsel.position = sceneobj.oss[sceneobj.possel].position;
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (sceneobj.func)
                    {
                        if (sceneobj.possel< sceneobj.funcs.Count-1)
                        {
                            sceneobj.possel++;
                        }
                    }
                    else
                    {
                        if (sceneobj.possel < sceneobj.oss.Count-1)
                        {
                            sceneobj.possel++;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (sceneobj.func)
                    {
                        if (sceneobj.possel > 0)
                        {
                            sceneobj.possel--;
                        }
                    }
                    else
                    {
                        if (sceneobj.possel > 0)
                        {
                            sceneobj.possel--;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (!sceneobj.func)
                    {
                        sceneobj.possel = 0;
                        sceneobj.func = true;

                        Destroy(sceneobj.cursel.gameObject);
                        sceneobj.cursel = Instantiate(sceneobj.img, sceneobj.selzone);
                        sceneobj.cursel.SetActive(true);
                        sceneobj.imgsel = sceneobj.cursel.GetComponent<Image>();
                        sceneobj.rcsel = sceneobj.cursel.GetComponent<RectTransform>();
                        sceneobj.imgsel.sprite = curProj.l_select;
                        sceneobj.rcsel.sizeDelta = new Vector2(curProj.smallicon, curProj.smallicon);
                        sceneobj.rcsel.position = sceneobj.funcs[sceneobj.possel].position;
                    }
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (sceneobj.func)
                    {
                        sceneobj.possel = 0;
                        sceneobj.func = false;
                        
                        Destroy(sceneobj.cursel.gameObject);
                        sceneobj.cursel = Instantiate(sceneobj.img, sceneobj.selzone);
                        sceneobj.cursel.SetActive(true);
                        sceneobj.imgsel = sceneobj.cursel.GetComponent<Image>();
                        sceneobj.rcsel = sceneobj.cursel.GetComponent<RectTransform>();
                        sceneobj.imgsel.sprite = curProj.b_select;
                        sceneobj.rcsel.sizeDelta = new Vector2(curProj.bigicon, curProj.bigicon);
                        sceneobj.rcsel.position = sceneobj.oss[sceneobj.possel].position;
                    }
                }
            }
        }

        /*if (Input.GetButton("Ctrl"))
        {
            if (Input.GetButtonDown("X"))
            {
                
            }
        }*/
    }
}

[System.Serializable]
public class PreviewSetup
{
    public GameObject block;
    public Transform parent;
    public Sprite defbg;
    public Sprite defico;
    public Sprite defsel;
}

[System.Serializable]
public class ProjectFiles
{
    public string dir;
    public int bigicon;
    public int smallicon;

    public List<Sprite> oss = new List<Sprite>();
    public List<string> ossp = new List<string>();
    public List<Sprite> funcs = new List<Sprite>();
    public List<string> funcsp = new List<string>();
    public Sprite bg = null;
    public Sprite b_select = null;
    public Sprite l_select = null;
}

[System.Serializable]
public class SceneOptions
{
    public InputField bigicon;
    public InputField smallicon;
    public InputField screenX;
    public InputField screenY;
    public Image selbig;
    public Image selsmall;
    public List<Toggle> ostog;
    public List<Toggle> functog;
    public Transform osscroll;
    public Transform funcscroll;
    public GameObject togpresset;
}

[System.Serializable]
public class SceneSetUp
{
    public Image bg;
    public Transform paros;
    public Transform parfunc;
    public GameObject img;
}

[System.Serializable]
public class SceneObjs
{
    public GameObject canvas;
    public Image bg;
    public RectTransform osbg;
    public RectTransform funcbg;
    public Transform ostr;
    public Transform functr;
    public GameObject img;
    public List<RectTransform> oss;
    public List<RectTransform> funcs;
    public GameObject cursel;
    public Transform selzone;
    public int possel;
    public bool func;
    public Image imgsel;
    public RectTransform rcsel;
    public Vector2 bigsel;
    public Vector2 smsel;
}