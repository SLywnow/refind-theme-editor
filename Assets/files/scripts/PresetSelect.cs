using SFB;
using SLywnow;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PresetSelect : MonoBehaviour
{
    public MakePreset sc;
	public List<PresetSelectIcon> icons;
	
	public string path;
	public string to;
	public int curid;

	public void OpenSelect()
	{
		for (int i =0;i<icons.Count;i++)
		{
			string pathopen = sc.curProj.dir;
			if (!icons[i].nonicon)
				pathopen += "/icons";
			pathopen += "/" + icons[i].filename + ".png";

			if (FilesSet.CheckFile(pathopen))
			{
				FileStream streamimg = File.Open(pathopen, FileMode.Open);
				byte[] imgbt = new byte[streamimg.Length];
				streamimg.Read(imgbt, 0, imgbt.Length);
				streamimg.Close();

				Texture2D imgtex = new Texture2D(0, 0);
				imgtex.LoadImage(imgbt);
				Sprite sp = (Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f)));
				icons[i].Image.sprite = sp;
			}
			else
			{
				icons[i].Image.sprite = icons[i].deficon;
			}
		}
	}

	public void SelectPNG(int id)
	{
		to = sc.curProj.dir;
		if (!icons[id].nonicon)
			to += "/icons";
		to += "/" + icons[id].filename + ".png";
		var extensions = new[] {
				new ExtensionFilter("Image Files", "png" ),
			};
		curid = id;
		string[] str = StandaloneFileBrowser.OpenFilePanel("Select " + icons[id].filename, sc.curProj.dir, extensions, false);
		if (str.Length>0)
		path = str[0];
	}

	public void Update()
	{
		if (!string.IsNullOrEmpty(path))
		{
			if (FilesSet.CheckFile(path))
			{
				if (!FilesSet.CheckDirectory(sc.curProj.dir + "/icons/")) FilesSet.CreateDirectory(sc.curProj.dir + "/icons/");

				File.Copy(path, to, true);

				UpdateAll(curid);

				path = "";
				to = "";
				curid = 0;
			}
		}
	}

	public void UpdateAll (int id)
	{
		string pathopen = sc.curProj.dir;
		if (!icons[id].nonicon)
			pathopen += "/icons";
		pathopen += "/" + icons[id].filename + ".png";

		FileStream streamimg = File.Open(pathopen, FileMode.Open);
		byte[] imgbt = new byte[streamimg.Length];
		streamimg.Read(imgbt, 0, imgbt.Length);
		streamimg.Close();

		Texture2D imgtex = new Texture2D(0, 0);
		imgtex.LoadImage(imgbt);
		Sprite sp= (Sprite.Create(imgtex, new Rect(0.0f, 0.0f, imgtex.width, imgtex.height), new Vector2(0.5f, 0.5f)));
		icons[id].Image.sprite = sp;
	}
}

[System.Serializable]
public class PresetSelectIcon
{
	public Image Image;
	public string filename;
	public bool nonicon;
	public Sprite deficon;
}
