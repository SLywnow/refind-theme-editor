using SLywnow;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace AutoLangEditorSLywnow
{
	[CustomEditor(typeof(ALSL_KeyManager))]
	public class ALSL_KeyManager : EditorWindow
	{
		string nowselected = "";
		string search = "";
		string filter = "";
		Vector2 scpos;
		Vector2 scposW;

		string renameCurrent;
		string renameNew;
		string newKey;
		List<bool> langsOn;
		List<string> texts;

		string error;
		bool langsUpdated;

		List<string> filtersCat;

		List<string> keysMove;

		enum tab { keys, replacments, options, import, export }
		tab tabs;

		private void OnEnable()
		{
			if (!ALSL_Editor_System.enable)
			{
				ALSL_Editor_OnStartProject.Run();
			}
			filtersCat = new List<string>();
			keysMove = new List<string>();
			LoadSettings();
		}

		ALSLEditor_Options options = new ALSLEditor_Options();

		void LoadSettings()
		{
			if (FilesSet.CheckFile(Application.dataPath + "/SLywnowAssets/AutoLang/Data/editorOptions.dat"))
			{
				options = JsonUtility.FromJson<ALSLEditor_Options>(FilesSet.LoadStream(Application.dataPath + "/SLywnowAssets/AutoLang/Data/editorOptions.dat", false, false));
			}
			else
			{
				options = new ALSLEditor_Options();
				options.colorful = false;
				options.autosave = true;
				options.categorizeChar = '_';
				options.categorizeFullName = false;
				options.categorize = false;
				FilesSet.SaveStream(Application.dataPath + "/SLywnowAssets/AutoLang/Data/editorOptions.dat", JsonUtility.ToJson(options, true));
			}
		}

		void SaveSettings()
		{
			FilesSet.SaveStream(Application.dataPath + "/SLywnowAssets/AutoLang/Data/editorOptions.dat", JsonUtility.ToJson(options, true));
			AssetDatabase.ImportAsset("Assets/SLywnowAssets/AutoLang/Data/editorOptions.dat", ImportAssetOptions.ImportRecursive);
		}

		public enum tpe
		{
			none, add, rename, delete, showW, setText
		};
		public tpe nowtpe = tpe.none;

		void OnGUI()
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.richText = true;

			GUIStyle styleB = new GUIStyle(GUI.skin.button);
			styleB.richText = true;

			if (ALSL_Editor_System.enable)
			{
				if (tabs != tab.import && tabs != tab.export)
				{
					GUILayout.BeginHorizontal();
					{
						if (GUILayout.Button(tabs == tab.keys ? "<color=cyan><b>Language keys</b></color>" : "Language keys", styleB))
						{
							tabs = tab.keys;
							GUI.FocusControl("");
						}
						if (GUILayout.Button(tabs == tab.replacments ? "<color=cyan><b>Replace keys</b></color>" : "Replace keys", styleB))
						{
							tabs = tab.replacments;
							GUI.FocusControl("");
						}
						if (GUILayout.Button(tabs == tab.options ? "<color=cyan><b>Options</b></color>" : "Options", styleB))
						{
							tabs = tab.options;
							GUI.FocusControl("");
						}
					}
					GUILayout.EndHorizontal();

					if (!langsUpdated)
					{
						langsOn = new List<bool>();
						texts = new List<string>();
						for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
						{
							langsOn.Add(false);
							texts.Add("");
						}
						langsUpdated = true;
					}

					if (tabs == tab.keys)
					{
						string fullFilter = "";

						if (!options.categorize)
						{
							GUILayout.BeginHorizontal();
							{
								filter = EditorGUILayout.TextField("Search", filter);

							}
							GUILayout.EndHorizontal();
						}
						else
						{
							for (int i = 0; i < filtersCat.Count; i++)
							{
								/*if (i == filtersCat.Count - 1)
									fullFilter += filtersCat[i];
								else*/
								fullFilter += filtersCat[i] + options.categorizeChar;
							}
							filter = fullFilter;
							GUILayout.Label(filter.Replace(options.categorizeChar, ' '));
						}

						GUILayout.BeginVertical();
						{
							if (!options.categorize && !string.IsNullOrEmpty(nowselected))
							{
								GUILayout.BeginHorizontal();
								if (GUILayout.Button("▲x5")) { ALSL_Editor_System.Move(nowselected, true, 5); if (options.autosave) ALSL_Editor_System.SaveFiles(); };
								if (GUILayout.Button("▲")) { ALSL_Editor_System.Move(nowselected, true); if (options.autosave) ALSL_Editor_System.SaveFiles(); };
								if (GUILayout.Button("▼")) { ALSL_Editor_System.Move(nowselected, false); if (options.autosave) ALSL_Editor_System.SaveFiles(); };
								if (GUILayout.Button("▼x5")) { ALSL_Editor_System.Move(nowselected, false, 5); if (options.autosave) ALSL_Editor_System.SaveFiles(); };
								GUILayout.EndHorizontal();
							}
						}
						GUILayout.EndVertical();
						EditorGUILayout.Space();

						List<string> showkeys = ALSL_Editor_System.keys_alsl;
						List<string> currCats = new List<string>();

						if (options.categorize && filtersCat.Count > 0)
						{
							if (GUILayout.Button("To Top"))
							{
								nowselected = "";
								if (nowtpe != tpe.add)
									nowtpe = tpe.none;
								GUI.FocusControl("");

								filtersCat.RemoveAt(filtersCat.Count - 1);
								keysMove = new List<string>();
							}
							EditorGUILayout.Space();
						}

						scpos = GUILayout.BeginScrollView(scpos, style);
						for (int i = 0; i < showkeys.Count; i++)
						{
							if (string.IsNullOrEmpty(filter) || showkeys[i].Contains(filter))
							{
								string show = showkeys[i];

								if (options.categorize && !options.categorizeFullName && !string.IsNullOrEmpty(fullFilter))
									show = show.Replace(fullFilter, "");

								string curkey = showkeys[i];

								if (!string.IsNullOrEmpty(fullFilter))
									curkey = curkey.Replace(fullFilter, "");

								string[] parts = curkey.Split(options.categorizeChar);

								
								if (!options.categorize || (!string.IsNullOrEmpty(fullFilter) && parts.Length < 2) || string.IsNullOrEmpty(parts[0]))
								{
									//key
									string pref = "";
									string suf = "";

									if (options.colorful)
									{
										int c = 0;
										suf = "</color>";
										for (int a = 0; a < ALSL_Editor_System.alllangs.Count; a++)
										{
											if (!string.IsNullOrEmpty(ALSL_Editor_System.allwords[a].words[i]))
												c++;
										}
										if (c == 0)
											pref = "<color=red>";
										else if (c == ALSL_Editor_System.alllangs.Count)
											pref = "<color=lime>";
										else
											pref = "<color=orange>";
									}
									else
									{
										if (nowselected == showkeys[i])
										{
											pref = "<color=lime>";
											suf = "</color>";
										}
									}

									GUILayout.BeginHorizontal();
									{
										if (GUILayout.Button
												   ((nowselected == showkeys[i] ? (pref + "<b>" + show + " </b>" + suf) : (pref + show + suf)), styleB))
										{
											nowselected = (nowselected == showkeys[i] ? "" : showkeys[i]);
											GUI.FocusControl("");
											if (!string.IsNullOrEmpty(nowselected))
											{
												if (nowtpe == tpe.rename)
													renameCurrent = nowselected;
												if (nowtpe == tpe.showW)
												{
													for (int a = 0; a < ALSL_Editor_System.alllangs.Count; a++)
													{
														texts[a] = ALSL_Editor_System.allwords[a].words[showkeys.IndexOf(nowselected)];
													}
												}
											}
										}

										if (options.categorize)
										{
											//▲▼
											int id = keysMove.IndexOf(showkeys[i]);

											if (id == -1)
											{
												keysMove.Add(showkeys[i]);
												id = keysMove.IndexOf(showkeys[i]);
											}
											
											if (GUILayout.Button("▲", styleB, GUILayout.Width(25)))
											{
												if (id - 1 >= 0)
												{
													ALSL_Editor_System.Move(keysMove[id], keysMove[id - 1]);
													keysMove.Move(id, id - 1);
												}
											}
											if (GUILayout.Button("▼", styleB, GUILayout.Width(25)))
											{
												if (id + 1 < keysMove.Count)
												{ 
													ALSL_Editor_System.Move(keysMove[id], keysMove[id + 1]);
													keysMove.Move(id, id + 1);
												}
											}
										}
									}
									GUILayout.EndHorizontal();
								}
								else
								{
									//folder
									if (!string.IsNullOrEmpty(parts[0]) && !currCats.Contains(parts[0]))
									{
										if (GUILayout.Button("<color=cyan><b>" + parts[0] + " =></b></color>", styleB))
										{
											filtersCat.Add(parts[0]);
											keysMove = new List<string>();
											nowselected = "";
											if (nowtpe != tpe.add)
												nowtpe = tpe.none;
											GUI.FocusControl("");
										}
										currCats.Add(parts[0]);
									}
								}
							}
						}
						GUILayout.EndScrollView();

						EditorGUILayout.Space();

						GUILayout.Label("<b>Now selected: " + nowselected + "</b>", style);
						GUILayout.BeginHorizontal();
						if (GUILayout.Button((nowtpe == tpe.add ? ("<b>Add</b>") : "Add"), styleB))
						{
							GUI.FocusControl("");
							nowtpe = (nowtpe == tpe.add ? tpe.none : tpe.add);
							if (options.categorize)
								newKey = filter;
						}
						if (!string.IsNullOrEmpty(nowselected))
						{
							if (GUILayout.Button((nowtpe == tpe.rename ? ("<b>Rename</b>") : "Rename"), styleB))
							{
								GUI.FocusControl("");
								if (nowtpe == tpe.rename)
								{
									nowtpe = tpe.none;
									renameCurrent = "";
									renameNew = "";
								}
								else
								{
									nowtpe = tpe.rename;
									renameCurrent = nowselected;
									renameNew = "";
								}
							}
							if (GUILayout.Button((nowtpe == tpe.delete ? ("<b>Delete</b>") : "Delete"), styleB))
							{
								GUI.FocusControl("");
								nowtpe = (nowtpe == tpe.delete ? tpe.none : tpe.delete);
							}
							if (GUILayout.Button((nowtpe == tpe.showW || nowtpe == tpe.setText ? ("<b>Set Texts</b>") : "Set Texts"), styleB))
							{
								GUI.FocusControl("");
								nowtpe = (nowtpe == tpe.showW ? tpe.none : tpe.showW);
								for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
								{
									texts[i] = ALSL_Editor_System.allwords[i].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)];
								}
							}
						}
						else
						{
							if (nowtpe == tpe.rename || nowtpe == tpe.delete || nowtpe == tpe.showW || nowtpe == tpe.setText)
								nowtpe = tpe.none;
						}
						GUILayout.EndHorizontal();
						EditorGUILayout.Space();

						if (!string.IsNullOrEmpty(error))
							EditorGUILayout.LabelField("<color=red>" + error + "</color>", style);

						EditorGUILayout.Space();
						if (nowtpe == tpe.add)
						{
							EditorGUILayout.Space();
							newKey = EditorGUILayout.TextField("New key", newKey);
							if (GUILayout.Button("Add"))
							{
								GUI.FocusControl("");
								if (string.IsNullOrEmpty(newKey)) error = "Enter some key";
								else if (ALSL_Editor_System.CheckParam(newKey, ALSL_Editor_System.CheckType.keys)) error = "This key is already exist";
								else
								{
									ALSL_Editor_System.SaveToAsset(newKey, ALSL_Editor_System.TypeOfSave.keys);
									if (options.autosave) ALSL_Editor_System.SaveFiles();
									if (options.autoSelectWorldAfterAdd)
										nowselected = newKey;

									if (options.categorize)
										newKey = filter;
									else
										newKey = "";
								}
							}
						}
						if (nowtpe == tpe.rename)
						{
							GUILayout.BeginVertical();
							renameCurrent = EditorGUILayout.TextField("Current name", renameCurrent);
							renameNew = EditorGUILayout.TextField("New name", renameNew);
							if (GUILayout.Button("Rename"))
							{
								GUI.FocusControl("");
								if (string.IsNullOrEmpty(renameNew)) error = "Enter new name";
								else if (ALSL_Editor_System.CheckParam(renameNew, ALSL_Editor_System.CheckType.keys)) error = "Key with this name is already exist";
								else
								{
									ALSL_Editor_System.RenameToAsset(renameCurrent, renameNew, ALSL_Editor_System.TypeOfSave.keys);
									if (options.autosave) ALSL_Editor_System.SaveFiles();
									nowselected = renameNew;
									renameCurrent = nowselected;
									renameNew = "";
								}
							}
							GUILayout.EndVertical();
						}
						if (nowtpe == tpe.delete)
						{
							GUILayout.BeginVertical();
							EditorGUILayout.Space();
							EditorGUILayout.LabelField("Key to delete: <b>" + nowselected + "</b>", style);
							EditorGUILayout.LabelField("Are you sure you want to <b>delete</b> this key?", style);
							EditorGUILayout.Space();

							GUILayout.BeginHorizontal();
							if (GUILayout.Button("No"))
							{
								nowtpe = tpe.none;
								GUI.FocusControl("");
							}
							if (GUILayout.Button("Yes"))
							{
								GUI.FocusControl("");
								ALSL_Editor_System.RemoveToAsset(nowselected, ALSL_Editor_System.TypeOfSave.keys);
								if (options.autosave) ALSL_Editor_System.SaveFiles();
								nowtpe = tpe.none;
								nowselected = "";
							}
							GUILayout.EndHorizontal();

							GUILayout.EndVertical();
						}
						if (nowtpe == tpe.showW)
						{
							EditorGUILayout.Space();
							GUILayout.BeginVertical(GUILayout.Height(150));
							{
								scposW = GUILayout.BeginScrollView(scposW, style);
								{
									for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
									{
										int ii = i;
										string prefix = "";
										string suffix = "";
										if (string.IsNullOrEmpty(ALSL_Editor_System.allwords[ii].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)]))
										{
											prefix = "<color=red>";
											suffix = "</color>";
										}

										GUILayout.BeginHorizontal();
										GUILayout.BeginVertical(GUILayout.Width(20));
										langsOn[ii] = EditorGUILayout.BeginFoldoutHeaderGroup(langsOn[ii], "");
										GUILayout.EndVertical();
										if (GUILayout.Button(prefix + ALSL_Editor_System.alllangs[ii] + suffix, style))
											langsOn[ii] = !langsOn[ii];
										GUILayout.EndHorizontal();

										{
											if (langsOn[ii])
											{
												if (options.autosave)
												{
													texts[ii] = GUILayout.TextArea(texts[ii]);
												}
												else
												{
													ALSL_Editor_System.allwords[ii].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)] = GUILayout.TextArea(ALSL_Editor_System.allwords[ii].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)]);
												}
												EditorGUILayout.Space();
											}
										}
										EditorGUILayout.EndFoldoutHeaderGroup();
									}
								}
								GUILayout.EndScrollView();
								if (options.autosave)
								{
									EditorGUILayout.Space();
									if (GUILayout.Button("Save"))
									{
										GUI.FocusControl("");
										for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
										{
											ALSL_Editor_System.allwords[i].words[ALSL_Editor_System.keys_alsl.IndexOf(nowselected)] = texts[i];
										}
										if (options.autosave) ALSL_Editor_System.SaveFiles();
									}
								}
							}
							GUILayout.EndVertical();
						}

						EditorGUILayout.Space();
						EditorGUILayout.Space();
						if (!options.autosave)
							if (GUILayout.Button("Save all"))
							{
								ALSL_Editor_System.SaveFiles();
							}
					}
					else if (tabs == tab.replacments)
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.BeginVertical(GUILayout.Width((position.width / 2) - 20));
							{
								GUILayout.Label("Key");
							}
							GUILayout.EndVertical();
							GUILayout.BeginVertical(GUILayout.Width((position.width / 2) - 20));
							{
								GUILayout.Label("World");
							}
							GUILayout.EndVertical();
						}
						GUILayout.EndHorizontal();

						scpos = GUILayout.BeginScrollView(scpos, style);
						{
							for (int i = 0; i < ALSL_Editor_System.keysR_alsl.Count; i++)
							{
								int ii = i;
								GUILayout.BeginHorizontal();
								{
									GUILayout.BeginVertical(GUILayout.Width((position.width / 2) - 20));
									{
										ALSL_Editor_System.keysR_alsl[ii] = EditorGUILayout.TextField(ALSL_Editor_System.keysR_alsl[ii]);
									}
									GUILayout.EndVertical();
									GUILayout.BeginVertical(GUILayout.Width((position.width / 2) - 20));
									{
										ALSL_Editor_System.repickR_alsl[ii] = EditorGUILayout.TextField(ALSL_Editor_System.repickR_alsl[ii]);
									}
									GUILayout.EndVertical();

									if (GUILayout.Button("-", GUILayout.Width(30)))
									{
										ALSL_Editor_System.repickR_alsl.RemoveAt(ii);
										ALSL_Editor_System.keysR_alsl.RemoveAt(ii);
									}
								}
								GUILayout.EndHorizontal();
							}

							if (GUILayout.Button("Add"))
							{
								ALSL_Editor_System.repickR_alsl.Add("");
								ALSL_Editor_System.keysR_alsl.Add("");
							}
						}
						GUILayout.EndScrollView();

						if (GUILayout.Button("Save"))
						{
							ALSL_Editor_System.SaveFiles();
						}
					}
					else if (tabs == tab.options)
					{
						scpos = GUILayout.BeginScrollView(scpos);
						options.colorful = EditorGUILayout.Toggle("Colorful", options.colorful);
						options.autosave = EditorGUILayout.Toggle("Autosave", options.autosave);
						options.autoSelectWorldAfterAdd = EditorGUILayout.Toggle("Select new key", options.autoSelectWorldAfterAdd);
						options.categorize = EditorGUILayout.Toggle("Cataloging", options.categorize);
						if (options.categorize)
						{
							string input;
							GUILayout.BeginHorizontal();
							{
								GUILayout.Label("Separate char", GUILayout.Width(145));
								input = GUILayout.TextField(options.categorizeChar.ToString(), maxLength: 1);
								if (!string.IsNullOrEmpty(input))
									options.categorizeChar = input[0];
							}
							GUILayout.EndHorizontal();

							options.categorizeFullName = EditorGUILayout.Toggle("Show full name", options.categorizeFullName);
						}

						EditorGUILayout.Space();

						GUILayout.BeginHorizontal();
						{
							if (GUILayout.Button("Export Keys"))
							{
								tabs = tab.export;
								importFilter = "";
								PrepareExport();
							}
							if (GUILayout.Button("Import Keys"))
							{
								tabs = tab.import;
								importFilter = "";
								bools = new List<bool>();
								doneProcess = false;
								fileData = new ALSLEditor_ExportData();
							}
						}
						GUILayout.EndHorizontal();

						GUILayout.EndScrollView();

						EditorGUILayout.Space();
						EditorGUILayout.Space();


						if (GUILayout.Button("Save"))
						{
							SaveSettings();
						}
					}
				}
				else
				{
					if (tabs == tab.import)
					{
						ImportGUI();
					}
					else if (tabs == tab.export)
					{
						ExportGUI();
					}

					if (GUILayout.Button("Done"))
					{
						tabs = tab.options;
					}
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

		TextAsset importObject;
		string fileName;
		string importFilter;
		List<bool> bools;
		List<bool> langs;
		List<int> langsA;
		GUIContent[] langsC;
		bool doneProcess;
		bool showLangs;
		Vector2 scrollProcess;
		ALSLEditor_ExportData fileData;

		void PrepareImport()
		{
			importFilter = "";
			scrollProcess = new Vector2(0, 0);
			if (fileData != null)
			{
				langsA = new List<int>();

				for (int i = 0; i < fileData.langs.Count; i++)
				{
					if (ALSL_Editor_System.alllangs.Contains(fileData.langs[i]))
						langsA.Add(ALSL_Editor_System.alllangs.IndexOf(fileData.langs[i]) + 1);
					else
						langsA.Add(0);
				}

				langsC = new GUIContent[ALSL_Editor_System.alllangs.Count + 1];
				langsC[0] = new GUIContent("Off");
				for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
					langsC[i + 1] = new GUIContent(ALSL_Editor_System.alllangs[i]);

				bools = new List<bool>();
				for (int i = 0; i < fileData.keys.Count; i++)
				{
					bools.Add(true);
				}

				doneProcess = true;
			}
		}

		void ImportGUI()
		{
			GUILayout.BeginHorizontal();
			{
				importObject = EditorGUILayout.ObjectField(importObject, typeof(TextAsset), false) as TextAsset;

				if (importObject != null && GUILayout.Button("Load"))
				{
					fileData = JsonUtility.FromJson<ALSLEditor_ExportData>(importObject.text);
					PrepareImport();
				}
				if (importObject == null)
					doneProcess = false;
			}
			GUILayout.EndHorizontal();

			if (doneProcess)
			{
				scrollProcess = GUILayout.BeginScrollView(scrollProcess);
				{
					showLangs = EditorGUILayout.BeginFoldoutHeaderGroup(showLangs, "Languages to import");
					{
						if (showLangs)
						{
							for (int i = 0; i < fileData.langs.Count; i++)
							{
								int ii = i;
								GUILayout.BeginHorizontal();
								{
									GUILayout.Label(fileData.langs[ii]);
									GUILayout.Label("==");
									langsA[ii] = EditorGUILayout.Popup(langsA[ii], langsC);
								}
								GUILayout.EndHorizontal();
							}
						}
					}
					EditorGUILayout.EndFoldoutHeaderGroup();

					EditorGUILayout.Space();

					GUILayout.Label("Keys");
					{
						importFilter = EditorGUILayout.TextField("Search", importFilter);
						GUILayout.BeginHorizontal();
						{
							if (GUILayout.Button("Select all"))
							{
								for (int i = 0; i < bools.Count; i++)
									if (string.IsNullOrEmpty(importFilter) || fileData.keys[i].key.Contains(importFilter))
										bools[i] = true;
							}
							if (GUILayout.Button("Deselect all"))
							{
								for (int i = 0; i < bools.Count; i++)
									if (string.IsNullOrEmpty(importFilter) || fileData.keys[i].key.Contains(importFilter))
										bools[i] = false;
							}
						}
						GUILayout.EndHorizontal();


						for (int i = 0; i < fileData.keys.Count; i++)
						{
							int ii = i;
							if (string.IsNullOrEmpty(importFilter) || fileData.keys[ii].key.Contains(importFilter))
								bools[ii] = EditorGUILayout.Toggle(fileData.keys[ii].key, bools[ii]);
						}
					}
				}
				GUILayout.EndScrollView();
				if (bools.Find(f => f == true) && langsA.Find(f => f != 0) > 0 && GUILayout.Button("Import"))
				{
					for (int i = 0; i < fileData.keys.Count; i++)
					{
						int ii = i;
						if (bools[ii])
						{
							int keyid = -1;
							if (!ALSL_Editor_System.CheckParam(fileData.keys[ii].key, ALSL_Editor_System.CheckType.keys))
							{
								ALSL_Editor_System.SaveToAsset(fileData.keys[ii].key, ALSL_Editor_System.TypeOfSave.keys);
							}
							keyid = ALSL_Editor_System.keys_alsl.IndexOf(fileData.keys[ii].key);

							for (int l = 0; l < fileData.langs.Count; l++)
							{
								int ll = l;
								if (langsA[ll] > 0)
								{
									int langId = langsA[ll] - 1;
									ALSL_Editor_System.allwords[langId].words[keyid] = fileData.keys[ii].langs[ll].text;
								}
							}
						}
					}

					if (options.autosave)
						ALSL_Editor_System.SaveFiles();
				}
			}
		}

		void PrepareExport()
		{
			bools = new List<bool>();
			langs = new List<bool>();
			fileData = new ALSLEditor_ExportData();
			fileName = "";
			importFilter = "";
			scrollProcess = new Vector2(0, 0);

			for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
			{
				langs.Add(true);
			}

			for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
			{
				bools.Add(false);
			}
		}

		void ExportGUI()
		{
			fileName = EditorGUILayout.TextField("File name", fileName);

			scrollProcess = GUILayout.BeginScrollView(scrollProcess);
			{
				showLangs = EditorGUILayout.BeginFoldoutHeaderGroup(showLangs, "Languages to export");
				{
					if (showLangs)
					{
						for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
						{
							int ii = i;
							langs[ii] = EditorGUILayout.Toggle(ALSL_Editor_System.alllangs[ii] + " (" + ALSL_Editor_System.langsvis[ii] + ")", langs[ii]);
						}
					}
				}
				EditorGUILayout.EndFoldoutHeaderGroup();
				EditorGUILayout.Space();
				GUILayout.Label("Keys:");
				importFilter = EditorGUILayout.TextField("Search", importFilter);
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Select all"))
					{
						for (int i = 0; i < bools.Count; i++)
							if (string.IsNullOrEmpty(importFilter) || ALSL_Editor_System.keys_alsl[i].Contains(importFilter))
								bools[i] = true;
					}
					if (GUILayout.Button("Deselect all"))
					{
						for (int i = 0; i < bools.Count; i++)
							if (string.IsNullOrEmpty(importFilter) || ALSL_Editor_System.keys_alsl[i].Contains(importFilter))
								bools[i] = false;
					}
				}
				GUILayout.EndHorizontal();

				for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
				{
					int ii = i;
					if (string.IsNullOrEmpty(importFilter) || ALSL_Editor_System.keys_alsl[ii].Contains(importFilter))
						bools[ii] = EditorGUILayout.Toggle(ALSL_Editor_System.keys_alsl[ii], bools[ii]);
				}
			}
			GUILayout.EndScrollView();
			if (!string.IsNullOrEmpty(fileName) && bools.Find(f => f == true) && langs.Find(f => f == true) && GUILayout.Button("Export"))
			{
				fileData = new ALSLEditor_ExportData();
				fileData.langs = new List<string>();
				for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
				{
					int ii = i;
					if (langs[ii])
						fileData.langs.Add(ALSL_Editor_System.alllangs[ii]);
				}

				fileData.keys = new List<ALSLEditor_ExportDataBlock>();
				for (int i = 0; i < ALSL_Editor_System.keys_alsl.Count; i++)
				{
					int ii = i;
					if (bools[ii])
					{
						fileData.keys.Add(new ALSLEditor_ExportDataBlock());
						ALSLEditor_ExportDataBlock k = fileData.keys[fileData.keys.Count - 1];
						k.key = ALSL_Editor_System.keys_alsl[ii];
						k.langs = new List<ALSLEditor_ExportDataBlocklang>();
						for (int l = 0; l < ALSL_Editor_System.alllangs.Count; l++)
						{
							int ll = l;
							if (langs[ll])
							{
								k.langs.Add(new ALSLEditor_ExportDataBlocklang());
								k.langs[k.langs.Count - 1].lang = ALSL_Editor_System.alllangs[ll];
								k.langs[k.langs.Count - 1].text = ALSL_Editor_System.allwords[ll].words[ii];
							}
						}
					}
				}

				if (FilesSet.CheckDirectory(Application.dataPath + "/SLywnowAssets/AutoLang/Data/Exports"))
					FilesSet.CreateDirectory(Application.dataPath + "/SLywnowAssets/AutoLang/Data/Exports");

				FilesSet.SaveStream(Application.dataPath + "/SLywnowAssets/AutoLang/Data/Exports/" + fileName + ".json", JsonUtility.ToJson(fileData, true), false, false);
				AssetDatabase.ImportAsset("Assets/SLywnowAssets/AutoLang/Data/Exports/" + fileName + ".json", ImportAssetOptions.ImportRecursive);
			}
		}
	}


	public class ALSLOpenWindManager : Editor
	{
		[MenuItem("SLywnow/AutoLang Key Manager (beta)")]
		static void SetDirection()
		{
			EditorWindow.GetWindow(typeof(ALSL_KeyManager), false, "AutoLang Key Manager", true);
		}
	}

	[System.Serializable]
	class ALSLEditor_Options
	{
		public bool colorful;
		public bool categorize;
		public char categorizeChar = '_';
		public bool categorizeFullName;
		public bool autosave = true;
		public bool autoSelectWorldAfterAdd;
	}

	[System.Serializable]
	class ALSLEditor_ExportData
	{
		public List<string> langs;
		public List<ALSLEditor_ExportDataBlock> keys;
	}

	[System.Serializable]
	class ALSLEditor_ExportDataBlock
	{
		public string key;
		public List<ALSLEditor_ExportDataBlocklang> langs;
	}

	[System.Serializable]
	class ALSLEditor_ExportDataBlocklang
	{
		public string lang;
		public string text;
	}
}
