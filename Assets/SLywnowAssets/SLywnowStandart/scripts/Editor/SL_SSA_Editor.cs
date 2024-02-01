using SLywnow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;



public class SL_SSA_Editor : EditorWindow
{

	string open = "";
	string add = "";
	int addtpe = 0;
	int currentId = -1;
	bool canOpen;
	SaveSystemSL loaded;
	List<string> inputs;
	List<bool> inputsBools;
	Vector2 scpos;
	string error="";
	string savecoml = "";
	bool saveonce=false;
	string search = "";
	string filter = "";
	bool AddOpen;

	//settings
	bool optOpen;
	SL_SSA_Config config;

	private void OnEnable()
	{
		if (FilesSet.CheckFile(Application.dataPath + "/SLywnowAssets/SLywnowStandart/config.cfg"))
			config = JsonUtility.FromJson<SL_SSA_Config>(FilesSet.LoadStream(Application.dataPath + "/SLywnowAssets/SLywnowStandart/config.cfg", false, false));
		else
			config = new SL_SSA_Config();

		open = config.lastOpen + "";
	}

	void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.richText = true;

		GUIStyle styleB = new GUIStyle();
		styleB = GUI.skin.GetStyle("Button");
		styleB.richText = true;

		GUILayout.BeginHorizontal();
		open = EditorGUILayout.TextField("", open);
		if (GUILayout.Button("Open") && !Application.isPlaying)
		{
			try
			{
				currentId = int.Parse(open);
				if (FilesSet.CheckFile("SaveSystemSL/", "Save" + currentId, "slsave", true))
				{

					loaded = JsonUtility.FromJson<SaveSystemSL>(FilesSet.LoadStream("SaveSystemSL/", "Save" + currentId, "slsave", true, false));
					canOpen = true;
					error = "";
					savecoml = "";
					saveonce = false;
					inputs = new List<string>();
					inputsBools = new List<bool>();
					for (int i = 0; i < loaded.name.Count; i++)
					{
						inputs.Add(loaded.contain[i]);
						if (loaded.type[i] == SaveSystemSL.SSLTpe.boolS)
							inputsBools.Add(bool.Parse(loaded.contain[i]));
						else
							inputsBools.Add(false);
					}

					config.lastOpen = open;
					FilesSet.SaveStream(Application.dataPath + "/SLywnowAssets/SLywnowStandart/config.cfg", JsonUtility.ToJson(config, true), false, false);
				}
				else
					canOpen = false;

			}
			catch
			{
				open = "";
				canOpen = false;
			}
		}
		GUILayout.EndHorizontal();

		EditorGUILayout.Space();

		//Debug.Log(canOpen);

		if (Application.isPlaying)
		{
			GUILayout.Label("Not available in Play mode");
		}

		if (canOpen && !Application.isPlaying && loaded != null)
		{
			GUILayout.BeginHorizontal();
			search = EditorGUILayout.TextField("", search);
			if (GUILayout.Button("Search")) filter = search;
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GUILayout.BeginVertical();
			scpos = GUILayout.BeginScrollView(scpos, style);
			for (int i = 0; i < loaded.name.Count; i++)
			{
				int id = i;
				if (string.IsNullOrEmpty(filter) || loaded.name[id].Contains(filter))
				{
					GUILayout.BeginHorizontal();
					string visname = "";
					switch (loaded.type[id])
					{
						case SaveSystemSL.SSLTpe.stringS:
							visname = "(string) " + loaded.name[id];
							break;
						case SaveSystemSL.SSLTpe.undefinedS:
							visname = "(undefined) " + loaded.name[id];
							break;
						case SaveSystemSL.SSLTpe.intS:
							visname = "(int) " + loaded.name[id];
							break;
						case SaveSystemSL.SSLTpe.floatS:
							visname = "(float) " + loaded.name[id];
							break;
						case SaveSystemSL.SSLTpe.boolS:
							visname = "(bool) " + loaded.name[id];
							break;
						default:
							break;
					}
					if (!config.showLabelAsInput)
					{
						if (loaded.type[id] == SaveSystemSL.SSLTpe.boolS)
							inputsBools[id] = EditorGUILayout.Toggle(visname, inputsBools[id]);
						else
							inputs[id] = EditorGUILayout.TextField(visname, inputs[id]);
					}
					else
					{
						EditorGUILayout.TextField(visname);
						if (loaded.type[id] == SaveSystemSL.SSLTpe.boolS)
							inputsBools[id] = EditorGUILayout.Toggle(inputsBools[id]);
						else
							inputs[id] = EditorGUILayout.TextField(inputs[id]);
					}

					if (GUILayout.Button("-", GUILayout.Width(15)))
					{
						loaded.contain.RemoveAt(id);
						loaded.name.RemoveAt(id);
						loaded.type.RemoveAt(id);
						inputs.RemoveAt(id);
					}
					GUILayout.EndHorizontal();
				}
			}
			if (GUILayout.Button("Add"))
			{
				AddOpen = !AddOpen;
			}
			if (AddOpen)
			{
				GUILayout.BeginHorizontal();
				add = EditorGUILayout.TextField("", add);
				addtpe = EditorGUILayout.Popup(addtpe, new string[] { "String","Int","Float","Bool", "Undefined" });
				if (!string.IsNullOrEmpty(add) && !loaded.name.Contains(add) && GUILayout.Button("Create"))
				{
					loaded.name.Add(add);
					switch (addtpe)
					{
						case 0: //string
							loaded.type.Add(SaveSystemSL.SSLTpe.stringS);
							loaded.contain.Add("");
							inputs.Add("");
							break;
						case 1: //int
							loaded.type.Add(SaveSystemSL.SSLTpe.intS);
							loaded.contain.Add("0");
							inputs.Add("0");
							break;
						case 2: //float
							loaded.type.Add(SaveSystemSL.SSLTpe.floatS);
							loaded.contain.Add("0");
							inputs.Add("0");
							break;
						case 3: //bool
							loaded.type.Add(SaveSystemSL.SSLTpe.boolS);
							loaded.contain.Add("False");
							inputsBools.Add(false);
							break;
						case 4: //und
							loaded.type.Add(SaveSystemSL.SSLTpe.undefinedS);
							loaded.contain.Add("");
							inputs.Add("");
							break;
						default:
							break;
					}
					add = "";
				}
				GUILayout.EndHorizontal();
				/*GUILayout.BeginHorizontal();
				if (GUILayout.Button(addtpe==0 ? "<color=green>String</color>" : "String",styleB))
				{
					addtpe = 0;
				}
				if (GUILayout.Button(addtpe == 1 ? "<color=green>Int</color>" : "Int", styleB))
				{
					addtpe = 1;
				}
				if (GUILayout.Button(addtpe == 2 ? "<color=green>Float</color>" : "Float", styleB))
				{
					addtpe = 2;
				}
				if (GUILayout.Button(addtpe == 3 ? "<color=green>Bool</color>" : "Bool", styleB))
				{
					addtpe = 3;
				}
				if (GUILayout.Button(addtpe == 4 ? "<color=green>Undefined</color>" : "Undefined", styleB))
				{
					addtpe = 4;
				}
				GUILayout.EndHorizontal();*/
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save"))
			{
				bool can = false;
				string errorName = "";
				if (!string.IsNullOrEmpty(config.CultureInf))
				{
					CultureInfo.CurrentCulture = new CultureInfo(config.CultureInf, false);
				}
				else
					CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

				try
				{
					for (int i = 0; i < loaded.name.Count; i++)
					{
						errorName = loaded.name[i];
						if (loaded.type[i] == SaveSystemSL.SSLTpe.boolS)
							loaded.contain[i] = inputsBools[i].ToString();

						if (loaded.type[i] == SaveSystemSL.SSLTpe.undefinedS || loaded.type[i] == SaveSystemSL.SSLTpe.stringS)
							loaded.contain[i] = inputs[i];

						if (loaded.type[i] == SaveSystemSL.SSLTpe.intS)
							loaded.contain[i] = int.Parse(inputs[i]).ToString();

						if (loaded.type[i] == SaveSystemSL.SSLTpe.floatS)
							loaded.contain[i] = float.Parse(inputs[i]).ToString();
					}
					can = true;
				}
				catch
				{
					error = "Error converting a value in</color><color=yellow> " + errorName + "</color> <color=red>variable";
				}

				if (can)
				{
					if (loaded.name.Count == loaded.type.Count && loaded.type.Count == loaded.contain.Count)
					{
						string[] save = new string[1];
						save[0] = JsonUtility.ToJson(loaded, true);

						FilesSet.SaveStream("SaveSystemSL/", "Save" + currentId, "slsave", save, true);
						savecoml = "Save complete " + DateTime.Now.ToString();
						error = "";
						saveonce = !saveonce;
					}
				}
			}
			if (GUILayout.Button("Options"))
			{
				optOpen = !optOpen;
			}
			GUILayout.EndHorizontal();
			if (optOpen)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Show labels as Input (read-only)");
				config.showLabelAsInput = EditorGUILayout.Toggle(config.showLabelAsInput);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Force set culture info (empty - default)");
				config.CultureInf = EditorGUILayout.TextField(config.CultureInf);
				GUILayout.EndHorizontal();

				if (GUILayout.Button("Save"))
				{
					optOpen = !optOpen;
					FilesSet.SaveStream(Application.dataPath + "/SLywnowAssets/SLywnowStandart/config.cfg", JsonUtility.ToJson(config, true), false, false);
				}
			}


			if (!string.IsNullOrEmpty(error))
			{
				savecoml = "";
				EditorGUILayout.LabelField("<b><color=red>" + error + "</color></b>", style);
			}
			if (!string.IsNullOrEmpty(savecoml))
			{
				if (!saveonce)
					EditorGUILayout.LabelField("<b><color=green>" + savecoml + "</color></b>", style);
				else
					EditorGUILayout.LabelField("<b><color=cyan>" + savecoml + "</color></b>", style);
			}
		}

	}
}

[System.Serializable]
public class SL_SSA_Config
{
	public bool showLabelAsInput;
	public string CultureInf;
	public string lastOpen;
}

public class SL_SSA_EditorManager : Editor
{
	[MenuItem("SLywnow/SaveSystemAlt Editor")]
	static void SetDirection()
	{
		EditorWindow.GetWindow(typeof(SL_SSA_Editor), false, "SaveSystemAlt Editor", true);
	}
}
