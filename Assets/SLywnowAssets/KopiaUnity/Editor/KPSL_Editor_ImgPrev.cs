using SLywnow;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KPSL_Editor_ImgPrev : EditorWindow
{

	private void OnEnable()
	{
		OnFocus();
	}

	Texture2D loaded;

	private void OnFocus()
	{
		string path = Application.dataPath.Replace("/Assets", "/Temp/KopiaCheck/");

		loaded = null;
		if (FilesSet.CheckFile(path + "preview.png")) 
		{
			loaded = FilesSet.LoadSprite(path + "preview.png", false).texture;
		}
	}

	Vector2 pos;

	private void OnGUI()
	{
		if (loaded !=null)
		{
			pos = GUILayout.BeginScrollView(pos);

			GUILayout.Label(new GUIContent(loaded));

			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label("Image not found! Try again!");
		}
	}

}
