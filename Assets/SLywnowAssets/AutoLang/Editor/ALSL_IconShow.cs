using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ALSL_IconShow
{
	
	static Texture2D texText;
	static List<int> markedObjectsText;

	static Texture2D texDwopdown;
	static List<int> markedObjectsDwopdown;

	static Texture2D texRDwopdown;
	static List<int> markedObjectsRDwopdown;

	static ALSL_IconShow()
	{
		// Load textures
		texText = AssetDatabase.LoadAssetAtPath("Assets/SLywnowAssets/AutoLang/EditorTextures/Text.png", typeof(Texture2D)) as Texture2D;
		texDwopdown = AssetDatabase.LoadAssetAtPath("Assets/SLywnowAssets/AutoLang/EditorTextures/Dropdown.png", typeof(Texture2D)) as Texture2D;
		texRDwopdown = AssetDatabase.LoadAssetAtPath("Assets/SLywnowAssets/AutoLang/EditorTextures/ALSLDropdown.png", typeof(Texture2D)) as Texture2D;

		EditorApplication.update += UpdateCB;
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
	}

	static void UpdateCB()
	{
		if (!Application.isPlaying)
		{
			GameObject[] go = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

			markedObjectsText = new List<int>();
			markedObjectsDwopdown = new List<int>();
			markedObjectsRDwopdown = new List<int>();

			foreach (GameObject g in go)
			{
				if (g.GetComponent<ALSLText>() != null)
					markedObjectsText.Add(g.GetInstanceID());

				if (g.GetComponent<ALSLSelectTranslate>() != null)
					markedObjectsDwopdown.Add(g.GetInstanceID());

				if (g.GetComponent<ALSLAutoDropDown>() != null)
					markedObjectsRDwopdown.Add(g.GetInstanceID());
			}
		}
	}

	static void HierarchyItemCB(int instanceID, Rect selectionRect)
	{
		if (!Application.isPlaying)
		{
			// place the icoon to the right of the list:
			Rect r = new Rect(selectionRect);
			r.x = r.width - 20;
			r.width = 18;
			/*Rect r2 = new Rect (selectionRect); 
			r2.x = r2.width -2;
			r2.width = 18;*/
			if (!Application.isPlaying && markedObjectsText != null)
			{
				if (markedObjectsText.Contains(instanceID))
				{
					// Draw the texture if it's a light (e.g.)
					GUI.Label(r, texText);
				}
			}

			if (!Application.isPlaying && markedObjectsDwopdown != null)
			{
				if (markedObjectsDwopdown.Contains(instanceID))
				{
					// Draw the texture if it's a light (e.g.)
					GUI.Label(r, texDwopdown);
				}
			}

			if (!Application.isPlaying && markedObjectsRDwopdown != null)
			{
				if (markedObjectsRDwopdown.Contains(instanceID))
				{
					GUI.Label(r, texRDwopdown);
				}
			}
		}
	}
}