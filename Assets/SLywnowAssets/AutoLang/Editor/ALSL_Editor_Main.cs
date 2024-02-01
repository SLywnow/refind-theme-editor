using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AutoLangSLywnow;
using System;

namespace AutoLangEditorSLywnow
{
	[CustomEditor(typeof(ALSL_Editor_Main))]
	public class ALSL_Editor_Main : EditorWindow
	{

		string input;
		string input2;
		string inputAL;

		bool inputtoggle;

		bool show;
		int noweditid = -1;
		int noweditidl = -1;

		Vector2 scpos = new Vector2();

		public enum tpe
		{
			begin, paramdata, editW, showW,
			addK, editK, removeK,
			editRK, addRK,
			addL, editL, removeL, showL
		};
		public tpe nowtpe = tpe.begin;

		void OnEnable()
		{
			EditorWindow w = GetWindow<ALSL_Editor_Main>();
			w.maxSize = new Vector2(1000, 1000);
			w.minSize = new Vector2(200, 200);

			if (!ALSL_Editor_System.enable)
			{
				ALSL_Editor_OnStartProject.Run();
			}
		}

		/*
		Fast travel, find:
		start widow - starting window of GUI

		show replaces -  show and edit all replaces

		add key - adding key window
		rename key - rename key window
		remove key - removing key window

		add language - adding language window
		edit language - rename language window
		remove language - removing language window

		edit word - list of all word
		edit word2 - editing word
		*/
		bool clear;
		bool firstedit;
		bool error;
		string errorText;

		bool paramreset;
		int inputver;
		string inputoutput;
		bool langonstart;
		UnityEngine.Object jsonfile;

		List<string> keysInput= new List<string>();
		List<string> keysWInput = new List<string>();

		void OnGUI()
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.richText = true;

			GUIStyle styleB = new GUIStyle(GUI.skin.button);
			styleB.richText = true;
			if (ALSL_Editor_System.enable)
			{
				//start widow
				if (nowtpe == tpe.begin)
				{
					//Debug.Log(input);
					input = null;
					input = "";
					input2 = "";
					noweditid = -1;
					noweditidl = -1;
					GUILayout.Label("Select options", EditorStyles.boldLabel);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Add key")) nowtpe = tpe.addK;
					if (GUILayout.Button("Edit key")) nowtpe = tpe.editK;
					if (GUILayout.Button("Remove key")) nowtpe = tpe.removeK;
					//if (GUILayout.Button("Add language")) nowtpe = tpe.addL;
					GUILayout.EndHorizontal();

					/*GUILayout.BeginHorizontal();
					if (GUILayout.Button("Edit language")) nowtpe = tpe.editL;
					GUILayout.EndHorizontal();*/

					GUILayout.BeginHorizontal();
					//if (GUILayout.Button("Remove language")) nowtpe = tpe.removeL;
					if (GUILayout.Button("Edit language")) nowtpe = tpe.showL;
					if (GUILayout.Button("Edit words")) nowtpe = tpe.showW;
					if (GUILayout.Button("Edit replaces")) { firstedit = true; nowtpe = tpe.editRK; }
					GUILayout.EndHorizontal();


					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Set Up Starting data")) { paramreset = true; nowtpe = tpe.paramdata; }
					GUILayout.EndHorizontal();
				}

				//show replaces
				if (nowtpe == tpe.editRK)
				{
					if (firstedit)
					{
						keysInput = new List<string>();
						keysWInput = new List<string>();
						for (int i = 0; i < ALSL_Editor_System.keysR_alsl.Count; i++)
						{
							keysWInput.Add(ALSL_Editor_System.repickR_alsl[i]);
							keysInput.Add(ALSL_Editor_System.keysR_alsl[i]);
						}
						firstedit = false;
					}
					GUILayout.BeginVertical();
					if (GUILayout.Button("Add Replace Key"))
					{
						keysInput.Add("Replace key " + keysInput.Count);
						keysWInput.Add("");
					}
					if (keysInput.Count == 0) EditorGUILayout.LabelField("<color=red> Replaces not founded </color>", style);
					else
					{
						GUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("< Key ", style);
						EditorGUILayout.LabelField("Replace Word", style);
						GUILayout.EndHorizontal();
						scpos = GUILayout.BeginScrollView(scpos, style);
						for (int i = 0; i < keysInput.Count; i++)
						{
							GUILayout.BeginHorizontal();
							keysInput[i] = EditorGUILayout.TextField("", keysInput[i]);
							keysWInput[i] = GUILayout.TextArea(keysWInput[i]);
							if (GUILayout.Button("Remove key"))
							{
								keysInput.RemoveAt(i);
								keysWInput.RemoveAt(i);
							}
							GUILayout.EndHorizontal();
						}
						GUILayout.EndScrollView();
					}

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Cancel")) nowtpe = tpe.begin;
					if (GUILayout.Button("Save"))
					{
						ALSL_Editor_System.keysR_alsl = keysInput;
						ALSL_Editor_System.repickR_alsl = keysWInput;
						ALSL_Editor_System.SaveFiles();
						firstedit = true;
						nowtpe = tpe.begin;
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}

				//add key
				if (nowtpe == tpe.addK)
				{

					if (clear) { input = ""; input2 = ""; show = false; clear = false; }

					GUILayout.BeginVertical();
					input = EditorGUILayout.TextField("Key name", input);
					if (error) EditorGUILayout.LabelField("<color=red>" + errorText + "</color>", style);

					EditorGUILayout.Space();
					show = EditorGUILayout.BeginToggleGroup("Show all keys", show);
					if (show)
					{
						scpos = GUILayout.BeginScrollView(scpos, style);
						for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
							if (GUILayout.Button(ALSL_Editor_System.keys_alsl[i])) { input = ALSL_Editor_System.keys_alsl[i]; }
						GUILayout.EndScrollView();
					}
					EditorGUILayout.EndToggleGroup();

					EditorGUILayout.Space();
					GUILayout.EndVertical();

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Cancel")) { if (!clear) clear = true; nowtpe = tpe.begin; error = false; }
					if (GUILayout.Button("Add"))
					{
						if (string.IsNullOrEmpty(input)) { error = true; errorText = "Enter some key or press Cancel"; }
						else if (ALSL_Editor_System.CheckParam(input, ALSL_Editor_System.CheckType.keys)) { error = true; errorText = "This key is already exist"; }
						else
						{
							ALSL_Editor_System.SaveToAsset(input, ALSL_Editor_System.TypeOfSave.keys);
							ALSL_Editor_System.SaveFiles();
						}
						if (!error) { nowtpe = tpe.begin; if (!clear) clear = true; }
					}
					GUILayout.EndHorizontal();
				}

				//rename key
				if (nowtpe == tpe.editK)
				{
					if (clear) { input = ""; input2 = ""; show = false; clear = false; }

					GUILayout.BeginVertical();
					input = EditorGUILayout.TextField("Old Key name", input);
					input2 = EditorGUILayout.TextField("New Key name", input2);
					if (error) EditorGUILayout.LabelField("<color=red>" + errorText + "</color>", style);

					EditorGUILayout.Space();
					show = EditorGUILayout.BeginToggleGroup("Show all keys", show);
					if (show)
					{
						scpos = GUILayout.BeginScrollView(scpos, style);
						for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
							if (GUILayout.Button(ALSL_Editor_System.keys_alsl[i])) { input = ALSL_Editor_System.keys_alsl[i]; }
						GUILayout.EndScrollView();
					}
					EditorGUILayout.EndToggleGroup();

					EditorGUILayout.Space();
					GUILayout.EndVertical();

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Cancel")) { if (!clear) clear = true; nowtpe = tpe.begin; error = false; }
					if (GUILayout.Button("Rename"))
					{
						if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(input2)) { error = true; errorText = "Enter two keys or press Cancel"; }
						else if (!ALSL_Editor_System.CheckParam(input, ALSL_Editor_System.CheckType.keys)) { error = true; errorText = "This key isn't already exist"; }
						else
						{
							ALSL_Editor_System.RenameToAsset(input, input2, ALSL_Editor_System.TypeOfSave.keys);
							ALSL_Editor_System.SaveFiles();
						}
						if (!error) nowtpe = tpe.begin;
					}
					GUILayout.EndHorizontal();
				}

				//remove key
				if (nowtpe == tpe.removeK)
				{
					if (clear) { input = ""; input2 = ""; show = false; clear = false; }

					GUILayout.BeginVertical();
					input = EditorGUILayout.TextField("Key name", input);
					if (error) EditorGUILayout.LabelField("<color=red>" + errorText + "</color>", style);

					EditorGUILayout.Space();
					show = EditorGUILayout.BeginToggleGroup("Show all keys", show);
					if (show)
					{
						scpos = GUILayout.BeginScrollView(scpos, style);
						for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
							if (GUILayout.Button(ALSL_Editor_System.keys_alsl[i])) { input = ALSL_Editor_System.keys_alsl[i]; }
						GUILayout.EndScrollView();
					}
					EditorGUILayout.EndToggleGroup();

					EditorGUILayout.Space();
					GUILayout.EndVertical();

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Cancel")) { if (!clear) clear = true; nowtpe = tpe.begin; error = false; }
					if (GUILayout.Button("Revove"))
					{
						if (string.IsNullOrEmpty(input)) { error = true; errorText = "Enter some key or press Cancel"; }
						else if (!ALSL_Editor_System.CheckParam(input, ALSL_Editor_System.CheckType.keys)) { error = true; errorText = "This key isn't already exist"; }
						else
						{
							ALSL_Editor_System.RemoveToAsset(input, ALSL_Editor_System.TypeOfSave.keys);
							ALSL_Editor_System.SaveFiles();
						}
						if (!error) nowtpe = tpe.begin;
					}
					GUILayout.EndHorizontal();
				}

				//add language
				if (nowtpe == tpe.addL)
				{
					if (firstedit)
					{
						inputAL = "none";
						input = null;
						input2 = null;
						inputtoggle = false;
						firstedit = false;
					}

					GUILayout.BeginVertical();
					input = EditorGUILayout.TextField("Language name", input);

					EditorGUILayout.LabelField("Params: ", style);
					inputtoggle = GUILayout.Toggle(inputtoggle, "Default language");
					input2 = EditorGUILayout.TextField("Display name", input2);

					if (error) EditorGUILayout.LabelField("<color=red>" + errorText + "</color>", style);

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Association with SystemLanguage", style);
					EditorGUILayout.LabelField("Now: " + inputAL, style);
					scpos = GUILayout.BeginScrollView(scpos, style);
					if (GUILayout.Button("none")) inputAL = "none";
					string[] allSL = SystemLanguage.GetNames(typeof(SystemLanguage));
					for (int i = 0; i < allSL.Length; i++) if (GUILayout.Button(allSL[i])) inputAL = allSL[i];
					GUILayout.EndScrollView();
					EditorGUILayout.Space();

					GUILayout.EndVertical();

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Add"))
					{
						if (string.IsNullOrEmpty(input)) { error = true; errorText = "Enter some language or press Cancel"; }
						else if (ALSL_Editor_System.CheckParam(input, ALSL_Editor_System.CheckType.lang)) { error = true; errorText = "This language is already exist"; }
						else
						{
							ALSL_Editor_System.SaveToAsset(input, ALSL_Editor_System.TypeOfSave.lang, input2, inputtoggle, inputAL);
							ALSL_Editor_System.SaveFiles();
							scpos = Vector2.zero;
							input = "";
							input2 = "";
							inputtoggle = false;
						}
						if (!error) nowtpe = tpe.showL;
					}
					if (GUILayout.Button("Cancel"))
					{
						scpos = Vector2.zero;
						input = "";
						input2 = "";
						inputtoggle = false;
						nowtpe = tpe.showL;
						error = false;
					}
					GUILayout.EndHorizontal();
				}

				//edit language
				if (nowtpe == tpe.editL)
				{
					if (noweditid != -1)
					{
						if (firstedit)
						{
							input = ALSL_Editor_System.alllangs[noweditid];
							input2 = ALSL_Editor_System.langsvis[noweditid];
							inputAL = ALSL_Editor_System.assotiate[noweditid];
							if (noweditid == ALSL_Editor_System.deflang) inputtoggle = true;
							firstedit = false;
						}

						if (error) EditorGUILayout.LabelField("<color=red>" + errorText + "</color>", style);
						input = EditorGUILayout.TextField("Language name", input);

						EditorGUILayout.LabelField("Params", style);
						inputtoggle = GUILayout.Toggle(inputtoggle, "Default language");
						input2 = EditorGUILayout.TextField("Display name", input2);

						EditorGUILayout.Space();
						EditorGUILayout.LabelField(" Association with SystemLanguage", style);
						EditorGUILayout.LabelField(" Now: " + inputAL, style);
						scpos = GUILayout.BeginScrollView(scpos, style);
						if (GUILayout.Button("none")) inputAL = "none";
						string[] allSL = SystemLanguage.GetNames(typeof(SystemLanguage));
						for (int i = 0; i < allSL.Length; i++) if (GUILayout.Button(allSL[i])) inputAL = allSL[i];
						GUILayout.EndScrollView();
						EditorGUILayout.Space();

						GUILayout.BeginHorizontal();
						if (GUILayout.Button("Save"))
						{
							if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(input2)) { error = true; errorText = "Enter name of language and display name or press Cancel"; }
							else if (!ALSL_Editor_System.CheckParam(ALSL_Editor_System.alllangs[noweditid], ALSL_Editor_System.CheckType.lang)) { error = true; errorText = "This language isn't already exist"; }
							else
							{
								ALSL_Editor_System.RenameToAsset(ALSL_Editor_System.alllangs[noweditid], input, ALSL_Editor_System.TypeOfSave.lang, input2, inputtoggle, inputAL);
								ALSL_Editor_System.SaveFiles();

								scpos = Vector2.zero;
								input = "";
								input2 = "";
								inputtoggle = false;
								noweditid = -1;
							}
							if (!error) nowtpe = tpe.showL;
						}
					}
					else
					{
						EditorGUILayout.LabelField("<color=red> Something wrong, select language again </color>", style);
						GUILayout.BeginHorizontal();
					}
					if (GUILayout.Button("Cancel"))
					{
						scpos = Vector2.zero;
						input = "";
						input2 = "";
						inputtoggle = false;
						noweditid = -1;
						nowtpe = tpe.showL;
						error = false;
					}
					GUILayout.EndHorizontal();
				}

				//remove language
				if (nowtpe == tpe.removeL)
				{

					GUILayout.BeginVertical();
					if (noweditid != -1)
					{
						EditorGUILayout.LabelField("Removing " + ALSL_Editor_System.alllangs[noweditid] + ". Remove?", style);
						EditorGUILayout.LabelField("<color=red> Warning! Remove language removing all words for this language</color>", style);
						EditorGUILayout.Space();

						GUILayout.BeginHorizontal();
						if (GUILayout.Button("Yes"))
						{
							ALSL_Editor_System.RemoveToAsset(ALSL_Editor_System.alllangs[noweditid], ALSL_Editor_System.TypeOfSave.lang);
							ALSL_Editor_System.SaveFiles();

							scpos = Vector2.zero;
							noweditid = -1;
							nowtpe = tpe.showL;
						}
					}
					else
						EditorGUILayout.LabelField("<color=red>Something wrong, select language again </color>", style);

					if (GUILayout.Button("No")) { scpos = Vector2.zero; noweditid = -1; if (!clear) clear = true; nowtpe = tpe.showL; error = false; }
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}

				//show language
				if (nowtpe == tpe.showL)
				{

					GUILayout.BeginVertical();
					if (GUILayout.Button("Add language"))
					{ firstedit = true; nowtpe = tpe.addL; }
					if (ALSL_Editor_System.alllangs.Count == 0) EditorGUILayout.LabelField("<color=red> Languages not founded </color>", style);
					else
					{
						scpos = GUILayout.BeginScrollView(scpos, style);
						for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
						{
							GUILayout.BeginHorizontal();
							if (ALSL_Editor_System.deflang == i)
								EditorGUILayout.LabelField("<color=green>" + ALSL_Editor_System.alllangs[i] + "</color>", style);
							else EditorGUILayout.LabelField(ALSL_Editor_System.alllangs[i], style);
							if (GUILayout.Button("Edit"))
							{ int b = i; noweditid = b; firstedit = true; nowtpe = tpe.editL; }
							if (GUILayout.Button("Remove"))
							{ int b = i; noweditid = b; nowtpe = tpe.removeL; }
							GUILayout.EndHorizontal();
						}
						GUILayout.EndScrollView();
					}

					if (GUILayout.Button("Cancel")) nowtpe = tpe.begin;
					GUILayout.EndVertical();
				}

				//edit word
				if (nowtpe == tpe.showW)
				{
					GUILayout.BeginVertical();
					if (ALSL_Editor_System.keys_alsl.Count == 0) EditorGUILayout.LabelField("<color=red> Keys not founded </color>", style);
					else
					{
						scpos = GUILayout.BeginScrollView(scpos, style);
						for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
						{
							GUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(ALSL_Editor_System.keys_alsl[i], style);
							for (int a = 0; a < ALSL_Editor_System.alllangs.Count; a++) if (GUILayout.Button
								   ((string.IsNullOrEmpty(ALSL_Editor_System.allwords[a].words[i]) ? ("<color=red>" + ALSL_Editor_System.alllangs[a] + "</color>") : (ALSL_Editor_System.alllangs[a])), styleB))
								{ int b = a; noweditid = b; int c = i; noweditidl = c; firstedit = true; nowtpe = tpe.editW; }
							GUILayout.EndHorizontal();
						}
						GUILayout.EndScrollView();
					}

					if (GUILayout.Button("Cancel")) nowtpe = tpe.begin;
					GUILayout.EndVertical();
				}

				//edit word2
				if (nowtpe == tpe.editW)
				{
					GUILayout.BeginVertical();
					if (!(noweditid == -1 || noweditidl == -1))
					{
						//Debug.Log(noweditid + " "+ noweditidl);
						//Debug.Log(ALSL_Editor_System.keys_alsl.Count+" "+ ALSL_Editor_System.alllangs.Count);
						if (firstedit) { input = ALSL_Editor_System.allwords[noweditid].words[noweditidl]; firstedit = false; }
						EditorGUILayout.LabelField("key \"" + ALSL_Editor_System.keys_alsl[noweditidl] + "\" in " + ALSL_Editor_System.alllangs[noweditid], style);
						input = GUILayout.TextArea(input);

					}
					else EditorGUILayout.LabelField("<color=red> Something wrong, select word again </color>", style);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Cancel"))
					{
						input = "";
						input2 = "";
						noweditid = -1;
						noweditidl = -1;
						nowtpe = tpe.showW;
					}
					if (!(noweditid == -1 || noweditidl == -1))
					{
						if (GUILayout.Button("Save"))
						{
							ALSL_Editor_System.allwords[noweditid].words[noweditidl] = input;
							ALSL_Editor_System.SaveFiles();

							input = "";
							input2 = "";
							noweditid = -1;
							noweditidl = -1;
							nowtpe = tpe.showW;
						}
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}

				//edit params
				if (nowtpe == tpe.paramdata)
				{
					if (paramreset)
					{
						if (ALSL_Editor_System.options != null)
						{
							inputver = ALSL_Editor_System.options.verison;
							inputoutput = ALSL_Editor_System.options.output;
							if (ALSL_Editor_System.options.LangFromSystem == 1) langonstart = true; else langonstart = false;
						}
						else
						{
							inputver = 1;
							inputoutput = "#sf/Games/#project/CustomTranslate";
							langonstart = false;
						}
						paramreset = false;
					}

					GUILayout.BeginVertical();
					inputoutput = EditorGUILayout.TextField("Translate path", inputoutput);
					EditorGUILayout.LabelField("Path to user's folder with custom translate, set empty for disable");
					EditorGUILayout.LabelField("#st - Standart folder (sdcard on android, Documents on PC)");
					EditorGUILayout.LabelField("#project - Project name");
					EditorGUILayout.LabelField("#dp - Not work in this string");
					EditorGUILayout.Space();

					inputver = EditorGUILayout.IntField("Version", inputver);
					EditorGUILayout.LabelField("For checking updates");
					EditorGUILayout.Space();

					langonstart = EditorGUILayout.Toggle("Associate language", langonstart);
					EditorGUILayout.LabelField("Set true if you want, that would asset found the best language on associations");
					EditorGUILayout.Space();

					jsonfile = EditorGUILayout.ObjectField("langJSON to import", jsonfile, typeof(UnityEngine.Object), false);
					EditorGUILayout.LabelField("Import .LangJson to add new language");
					if (GUILayout.Button("Add"))
					{
						if (jsonfile != null)
							ALSL_Editor_System.AddFromJSON(jsonfile);
					}
					EditorGUILayout.Space();

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Cancel"))
					{
						nowtpe = tpe.begin;
					}

					if (GUILayout.Button("Save"))
					{
						ALSL_Editor_System.options.verison = inputver;
						if (langonstart) ALSL_Editor_System.options.LangFromSystem = 1; else ALSL_Editor_System.options.LangFromSystem = 0;

						ALSL_Editor_System.SaveFiles();
						nowtpe = tpe.begin;
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}
			}
			else
			{
				EditorGUILayout.LabelField("Error while loading! Do you delete perfab?");
				if (GUILayout.Button("Reload"))
				{
					ALSL_Editor_OnStartProject.Run();
				}
			}
		}
	}

	public class ALSLOpenWind : Editor
	{
		[MenuItem("SLywnow/AutoLang SetUp")]
		static void SetDirection()
		{
			EditorWindow.GetWindow(typeof(ALSL_Editor_Main), false, "AutoLang SetUp", true);
		}
	}

}
