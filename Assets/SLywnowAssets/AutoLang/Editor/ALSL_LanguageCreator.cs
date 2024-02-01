using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AutoLangEditorSLywnow
{
	[CustomEditor(typeof(ALSL_LanguageCreator))]
	public class ALSL_LanguageCreator : EditorWindow
	{
		enum tpe
		{
			edit, create, langOpen
		};
		tpe type = tpe.edit;

		bool showDefault;
		bool showAsInput;
		int langid;
		Vector2 scpos;
		Vector2 scpos2;
		string search = "";
		string filter = "";
		bool advance;
		bool defaultlang = false;
		string visibleName = "";
		string associate;
		string mainname = "";
		bool deletethis;


		List<string> texts;
		List<string> def;

		private void OnEnable()
		{
			if (!ALSL_Editor_System.enable)
			{
				ALSL_Editor_OnStartProject.Run();
			}
		}

		void OnGUI()
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.richText = true;

			GUIStyle styleB = new GUIStyle(GUI.skin.button);
			styleB.richText = true;

			if (ALSL_Editor_System.enable)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(type == tpe.edit || type == tpe.langOpen ? "<b>Edit</b>" : "Edit", styleB)) type = tpe.edit;
				if (GUILayout.Button(type == tpe.create ? "<b>Create</b>" : "Create", styleB))
				{
					texts = new List<string>();
					def = new List<string>();
					for (int l = 0; l < ALSL_Editor_System.keys_alsl.Count; l++)
					{
						texts.Add("");
						def.Add(ALSL_Editor_System.allwords[ALSL_Editor_System.deflang].words[l]);
					}
					filter = "";
					search = "";
					advance = false;
					defaultlang = false;
					mainname = "";
					visibleName = "";
					associate = "";

					type = tpe.create;
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (type == tpe.edit)
				{
					scpos = GUILayout.BeginScrollView(scpos, style);
					for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
					{
						if (GUILayout.Button(ALSL_Editor_System.alllangs[i]))
						{
							texts = new List<string>();
							def = new List<string>();
							for (int l = 0; l < ALSL_Editor_System.keys_alsl.Count; l++)
							{
								texts.Add(ALSL_Editor_System.allwords[i].words[l]);
								def.Add(ALSL_Editor_System.allwords[ALSL_Editor_System.deflang].words[l]);
							}
							filter = "";
							search = "";
							advance = false;
							mainname = ALSL_Editor_System.alllangs[i];
							defaultlang = i == ALSL_Editor_System.deflang ? true : false;
							visibleName = ALSL_Editor_System.langsvis[i];
							associate = ALSL_Editor_System.assotiate[i];
							deletethis = false;

							langid = i;
							type = tpe.langOpen;
						}
					}
					GUILayout.EndScrollView();
				}
				if (type == tpe.langOpen)
				{
					string editing = ALSL_Editor_System.alllangs[langid];
					if (!string.IsNullOrEmpty(ALSL_Editor_System.langsvis[langid]))
						editing = ALSL_Editor_System.langsvis[langid];
					EditorGUILayout.LabelField("The modified language: " + editing, style);
					EditorGUILayout.Space();
					showDefault = EditorGUILayout.Toggle("Show Default strings", showDefault);
					if (showDefault)
						showAsInput = EditorGUILayout.Toggle("Allow copy default strings", showAsInput);
					EditorGUILayout.Space();

					scpos = GUILayout.BeginScrollView(scpos, style);

					GUILayout.BeginHorizontal();
					search = EditorGUILayout.TextField("", search);
					if (GUILayout.Button("Search")) filter = search;
					GUILayout.EndHorizontal();
					EditorGUILayout.Space();

					for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
					{
						if (string.IsNullOrEmpty(filter) || ALSL_Editor_System.keys_alsl[i].IndexOf(filter) >= 0)
						{
							EditorGUILayout.LabelField("<b>" + ALSL_Editor_System.keys_alsl[i] + "</b>", style);
							if (showDefault)
							{
								if (showAsInput)
									def[i] = GUILayout.TextArea(def[i]);
								else
									EditorGUILayout.LabelField(def[i], style);
							}
							texts[i] = GUILayout.TextArea(texts[i]);
							EditorGUILayout.Space();
						}
					}

					GUILayout.EndScrollView();
					EditorGUILayout.Space();

					advance = EditorGUILayout.Toggle("Advance Options", advance);
					if (advance)
					{
						visibleName = EditorGUILayout.TextField("Visible name", visibleName);
						defaultlang = EditorGUILayout.Toggle("Default language", defaultlang);
						EditorGUILayout.LabelField("<color=black> Association with SystemLanguage </color>", style);
						EditorGUILayout.LabelField("<color=black> Now: " + associate + " </color>", style);
						scpos2 = GUILayout.BeginScrollView(scpos2, style);
						if (GUILayout.Button("none")) associate = "none";
						string[] allSL = SystemLanguage.GetNames(typeof(SystemLanguage));
						for (int i = 0; i < allSL.Length; i++) if (GUILayout.Button(allSL[i])) associate = allSL[i];

						GUILayout.EndScrollView();

						deletethis = EditorGUILayout.Toggle("Unlock delete button", deletethis);
						if (deletethis)
						{
							EditorGUILayout.LabelField("<b> Are you sure, that you want delete this language (can't be undo)? </b>", style);
							if (GUILayout.Button("<color=red><b>Yes, i want delete language</b></color>", styleB))
							{
								ALSL_Editor_System.RemoveToAsset(ALSL_Editor_System.alllangs[langid], ALSL_Editor_System.TypeOfSave.lang);
								ALSL_Editor_System.SaveFiles();
								type = tpe.edit;
							}
						}

						EditorGUILayout.Space();
					}

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Cancel", styleB)) type = tpe.edit;
					if (GUILayout.Button("Save", styleB))
					{
						for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
						{
							ALSL_Editor_System.allwords[langid].words[i] = texts[i];
						}
						ALSL_Editor_System.RenameToAsset(mainname, mainname, ALSL_Editor_System.TypeOfSave.lang, visibleName, defaultlang, associate);

						ALSL_Editor_System.SaveFiles();
						type = tpe.edit;
					}
					GUILayout.EndHorizontal();
				}
				if (type == tpe.create)
				{
					EditorGUILayout.LabelField("Creating new language", style);
					mainname = EditorGUILayout.TextField("Name", mainname);
					EditorGUILayout.Space();
					showDefault = EditorGUILayout.Toggle("Show Default strings", showDefault);
					if (showDefault)
						showAsInput = EditorGUILayout.Toggle("Allow copy default strings", showAsInput);
					EditorGUILayout.Space();

					scpos = GUILayout.BeginScrollView(scpos, style);

					GUILayout.BeginHorizontal();
					search = EditorGUILayout.TextField("", search);
					if (GUILayout.Button("Search")) filter = search;
					GUILayout.EndHorizontal();
					EditorGUILayout.Space();

					for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
					{
						if (string.IsNullOrEmpty(filter) || ALSL_Editor_System.keys_alsl[i].IndexOf(filter) >= 0)
						{
							EditorGUILayout.LabelField("<b>" + ALSL_Editor_System.keys_alsl[i] + "</b>", style);
							if (showDefault)
							{
								if (showAsInput)
									def[i] = GUILayout.TextArea(def[i]);
								else
									EditorGUILayout.LabelField(def[i], style);
							}
							texts[i] = GUILayout.TextArea(texts[i]);
							EditorGUILayout.Space();
						}
					}

					GUILayout.EndScrollView();
					EditorGUILayout.Space();

					advance = EditorGUILayout.Toggle("Advance Options", advance);
					if (advance)
					{
						visibleName = EditorGUILayout.TextField("Visible name", visibleName);
						defaultlang = EditorGUILayout.Toggle("Default language", defaultlang);
						EditorGUILayout.LabelField("<color=black> Association with SystemLanguage </color>", style);
						EditorGUILayout.LabelField("<color=black> Now: " + associate + " </color>", style);
						scpos2 = GUILayout.BeginScrollView(scpos2, style);
						if (GUILayout.Button("none")) associate = "none";
						string[] allSL = SystemLanguage.GetNames(typeof(SystemLanguage));
						for (int i = 0; i < allSL.Length; i++) if (GUILayout.Button(allSL[i])) associate = allSL[i];

						GUILayout.EndScrollView();
						EditorGUILayout.Space();
					}

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Create", styleB))
					{
						if (!string.IsNullOrEmpty(mainname))
						{
							ALSL_Editor_System.SaveToAsset(mainname, ALSL_Editor_System.TypeOfSave.lang, visibleName, defaultlang, associate);
							for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
							{
								ALSL_Editor_System.allwords[ALSL_Editor_System.alllangs.IndexOf(mainname)].words[i] = texts[i];
							}
							ALSL_Editor_System.SaveFiles();
							type = tpe.edit;
						}
					}
					GUILayout.EndHorizontal();
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

	public class ALSLOpenWindLangEditor : Editor
	{
		[MenuItem("SLywnow/AutoLang Language editor (beta)")]
		static void SetDirection()
		{
			EditorWindow.GetWindow(typeof(ALSL_LanguageCreator), false, "AutoLang Language Editor", true);
		}
	}
}