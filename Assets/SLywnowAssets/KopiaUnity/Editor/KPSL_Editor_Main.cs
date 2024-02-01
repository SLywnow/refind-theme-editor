using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SLywnow;
using System;
using System.Threading.Tasks;
using System.IO;

public class KPSL_Editor_Main : EditorWindow
{
	KPSL_Editor_MainConfig config = new KPSL_Editor_MainConfig();
	KPSL_Editor_MainConfig preconfig = new KPSL_Editor_MainConfig();
	bool enabled = false;
	List<KPSL_Editor_Snapshots> snapshots;
	string datapath;

	Texture2D refreshIcon;
	Texture2D settingsIcon;
	Texture2D mainIcon;
	Texture2D consoleIcon;

	public KPSL_Editor_ConsoleHelp consoleHelp;
	public KPSL_Editor_ConsoleHelp currHelp;

	void OnEnable()
    {
		//loadcfg

		config.Load();
		serverdir = config.serverdir;

		if (!config.snaponlyassets)
			datapath = Application.dataPath.Replace("/Assets", "");
		else
			datapath = Application.dataPath;


		if (FilesSet.CheckFile(config.serverdir + "/kopia.exe"))
		{
			enabled = true;

			//runProc("policy set +" + "\"" + Application.dataPath + "/SLywnowAssets/KopiaUnity/policy.json" + "\"");
			runProc("policy set --global --ignore-cache-dirs true");
			runProc("policy set --global --add-ignore Temp");
			runProc("policy set --global --add-dot-ignore \"AtlasCache, TempArtifacts, Temp, StateCache, ShaderCache, PlayerDataCache, PackageCache\"");

			CheckAndLoad();
		}

		//UnityEngine.Debug.Log(Application.dataPath);
		//UnityEngine.Debug.Log("snapshot list " + "\"" + Application.dataPath + "\"");

		//get icons
		if (FilesSet.CheckFile(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/refresh.png"))
		{
			refreshIcon = FilesSet.LoadSprite(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/refresh.png", false).texture;
		}
		else
			refreshIcon = null;

		if (FilesSet.CheckFile(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/settings.png"))
		{
			settingsIcon = FilesSet.LoadSprite(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/settings.png", false).texture;
		}
		else
			refreshIcon = null;

		if (FilesSet.CheckFile(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/kopiaforunity.png"))
		{
			mainIcon = FilesSet.LoadSprite(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/kopiaforunity.png", false).texture;
		}
		else
			mainIcon = null;

		if (FilesSet.CheckFile(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/console.png"))
		{
			consoleIcon = FilesSet.LoadSprite(Application.dataPath + "/SLywnowAssets/KopiaUnity/Textures/console.png", false).texture;
		}
		else
			consoleIcon = null;

		//generate console help
		consoleHelp = new KPSL_Editor_ConsoleHelp();
		consoleInput = "";

		consoleHelp.help = new List<KPSL_Editor_ConsoleHelp>();

		consoleHelp.help.Add(new KPSL_Editor_ConsoleHelp());
		{
			consoleHelp.help[0].text = "list";
			consoleHelp.help[0].help = new List<KPSL_Editor_ConsoleHelp>();
			consoleHelp.help[0].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[0].help[0].text = "-l";
				consoleHelp.help[0].help[0].help = new List<KPSL_Editor_ConsoleHelp>();
			}
		}
		consoleHelp.help.Add(new KPSL_Editor_ConsoleHelp());
		{
			consoleHelp.help[1].text = "snapshot";
			consoleHelp.help[1].help = new List<KPSL_Editor_ConsoleHelp>();
			consoleHelp.help[1].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[1].help[0].text = "create";
				consoleHelp.help[1].help[0].help = new List<KPSL_Editor_ConsoleHelp>();
				consoleHelp.help[1].help[0].help.Add(new KPSL_Editor_ConsoleHelp());
				{
					consoleHelp.help[1].help[0].help[0].text = datapath;
					consoleHelp.help[1].help[0].help[0].help = new List<KPSL_Editor_ConsoleHelp>();
				}
			}
			consoleHelp.help[1].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[1].help[1].text = "list";
				consoleHelp.help[1].help[1].help = new List<KPSL_Editor_ConsoleHelp>();
			}
			consoleHelp.help[1].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[1].help[2].text = "migrate";
				consoleHelp.help[1].help[2].help = new List<KPSL_Editor_ConsoleHelp>();
			}
			consoleHelp.help[1].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[1].help[3].text = "restore";
				consoleHelp.help[1].help[3].help = new List<KPSL_Editor_ConsoleHelp>();
			}
			consoleHelp.help[1].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[1].help[4].text = "delete";
				consoleHelp.help[1].help[4].help = new List<KPSL_Editor_ConsoleHelp>();
			}
			consoleHelp.help[1].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[1].help[5].text = "fix invalid-files";
				consoleHelp.help[1].help[5].help = new List<KPSL_Editor_ConsoleHelp>();
			}
			consoleHelp.help[1].help.Add(new KPSL_Editor_ConsoleHelp());
			{
				consoleHelp.help[1].help[6].text = "fix remove-files";
				consoleHelp.help[1].help[6].help = new List<KPSL_Editor_ConsoleHelp>();
			}
		}

		currHelp = consoleHelp;
	}


	string serverdir = "";
	string consoleInput;
	Vector2 snappos;
	Vector2 filepos;
	Vector2 conspos;
	Vector2 settpos1;
	string settinput1;
	string settinput2;
	bool working;
	string workingStatus;

	enum oM { basic, files, settings, console };
	oM openMode = oM.basic;
	int filePos;
	string currentdata;
	List<string> fileList;
	List<KPSL_Editor_File> files;

	void OnGUI()
	{
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.richText = true;

		if (openMode != oM.settings)
		{
			EditorGUILayout.BeginHorizontal();
			if (!enabled)
			{
				GUILayout.Label("Path to exe:", GUILayout.Width(70));
				serverdir = EditorGUILayout.TextField("", serverdir);
				if (GUILayout.Button("Save & Check", GUILayout.Width(110)))
				{
					if (!string.IsNullOrEmpty(serverdir))
					{
						if (serverdir[serverdir.Length - 1] == '/' || serverdir[serverdir.Length - 1] == '\\')
							serverdir.Remove(serverdir.Length - 1);
						config.serverdir = serverdir;
						config.Save();

						if (FilesSet.CheckFile(config.serverdir + "/kopia.exe"))
						{
							enabled = true;
							CheckAndLoad();
						}
						else
							enabled = false;
					}
					else
						serverdir = config.serverdir;
				}
			}
			else
			{
				GUILayout.Label("<b><size=18><color=green>●</color> <color=cyan>Connected</color></size></b>", style);
				
				if (settingsIcon == null)
				{
					if (GUILayout.Button("Settings", GUILayout.Width(70)))
					{
						preconfig = config.Copy();
						openMode = oM.settings;
					}
				}
				else
				{
					if (GUILayout.Button(new GUIContent(settingsIcon), GUILayout.Width(19), GUILayout.Height(19)))
					{
						preconfig = config.Copy();
						openMode = oM.settings;
					}
				}
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
		}

		if (enabled)
		{
			if (!working)
			{
				if (openMode == oM.basic)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Create new snapshot"))
					{
						NewSnapshotAsync();
					}

					if (refreshIcon == null)
					{
						if (GUILayout.Button("Refresh"))
						{
							CheckAndLoad();
						}
					}
					else
					{
						if (GUILayout.Button(new GUIContent(refreshIcon), GUILayout.Width(19), GUILayout.Height(19)))
						{
							CheckAndLoad();
						}
					}

					if (consoleIcon == null)
					{
						if (GUILayout.Button("Console"))
						{
							openMode = oM.console;
							consoleInput = "";
						}
					}
					else
					{
						if (GUILayout.Button(new GUIContent(consoleIcon), GUILayout.Width(19), GUILayout.Height(19)))
						{
							openMode=oM.console;
							consoleInput = "";
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();

					snappos = GUILayout.BeginScrollView(snappos, style);

					if (snapshots.Count == 0)
						GUILayout.Label("There're no snapshots, create one!");

					for (int i = snapshots.Count - 1; i >= 0; i--)
					{
						int id = i;
						KPSL_Editor_Snapshots s = snapshots[id];
						s.show = EditorGUILayout.BeginFoldoutHeaderGroup(s.show, s.data.ToString());
						if (s.show)
						{
							GUILayout.Label(s.ind);
							GUILayout.Label("Size: " + s.size);
							GUILayout.Label("Files: " + s.files);
							GUILayout.Label("Directories: " + s.dirs);

							if (GUILayout.Button("Show files"))
							{
								openMode = oM.files;
								fileList = new List<string>();
								fileList.Add(s.ind);
								currentdata = s.data.ToString();
								filePos = -1;
							}

							EditorGUILayout.BeginHorizontal();
							if (GUILayout.Button("Restore this"))
							{
								RestoreSnapshot(s);
							}
							if (GUILayout.Button("Delete"))
							{
								DeleteSnapshot(s);
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndFoldoutHeaderGroup();
					}
					GUILayout.EndScrollView();
				}
				else if (openMode == oM.files)
				{
					string labeltext = currentdata+" ";
					foreach (string s in fileList)
						labeltext += s;
					labeltext = labeltext.Replace(fileList[0], "");

					GUILayout.Label(labeltext);
					EditorGUILayout.Space();

					if (fileList.Count == 1)
					{
						if (GUILayout.Button("Close"))
						{
							openMode = oM.basic;
						}
					}
					else
					{
						GUILayout.BeginHorizontal();
						if (GUILayout.Button("To the top"))
						{
							fileList.RemoveAt(fileList.Count - 1);
						}
						if (GUILayout.Button("Close", GUILayout.Width(50)))
						{
							openMode = oM.basic;
						}
						GUILayout.EndHorizontal();
					}

					EditorGUILayout.Space();

					if (filePos != fileList.Count)
					{
						working = true;
						workingStatus = "Getting list of files";
						OpenFiles();
					}
					else
					{
						filepos = GUILayout.BeginScrollView(filepos, style);
						foreach (KPSL_Editor_File file in files)
						{
							EditorGUILayout.BeginHorizontal();

							//gen object size
							Int64 size = file.size;
							string typesize = "bytes";
							if (size/1024 !=0)
							{
								typesize = "KB";
								size /= 1024;
								if (size / 1024 != 0)
								{
									typesize = "MB";
									size /= 1024;
									if (size / 1024 != 0)
									{
										typesize = "GB";
										size /= 1024;
									}
								}
							}

							//show object
							if (file.folder)
							{
								if (GUILayout.Button(file.name + " " + size + " " + typesize))
								{
									fileList.Add(file.name);
								}
							}
							else
								GUILayout.Label(file.name + " " + size + " " + typesize);
							{
								string realfile = "";
								foreach (string s in fileList)
									realfile += s + "/";
								realfile = realfile.Replace(fileList[0], "");
								realfile += file.name;
								realfile = Application.dataPath.Replace("Assets", "") + realfile;
								realfile = realfile.Replace("//", "/");
								//UnityEngine.Debug.Log(FilesSet.CheckFile(realfile) + " "+ realfile);

								if (config.textFormats != null && config.textFormats.Contains(GetFormat(file.name)))
								{
									if (FilesSet.CheckFile(realfile))
									{
										if (GUILayout.Button("See diff", GUILayout.Width(100)))
										{
											string dir = Application.dataPath.Replace("Assets", "") + "/Temp/KopiaCheck";
											if (!FilesSet.CheckDirectory(dir))
												FilesSet.CreateDirectory(dir);

											string path = "";
											foreach (string s in fileList)
												path += s + "/";
											path += file.name;
											RestoreSnapshotTextCheck(path, dir + "/changed.txt", realfile, dir);
										}
									}
									else
									{
										if (GUILayout.Button("Preview", GUILayout.Width(100)))
										{
											string dir = Application.dataPath.Replace("Assets", "") + "/Temp/KopiaCheck";
											if (!FilesSet.CheckDirectory(dir))
												FilesSet.CreateDirectory(dir);

											string path = "";
											foreach (string s in fileList)
												path += s + "/";
											path += file.name;
											RestoreSnapshotText(path, dir + "/changed.txt");
										}
									}
								}
								else if (config.imgFormats != null && config.imgFormats.Contains(GetFormat(file.name)))
								{
									if (GUILayout.Button("Preview", GUILayout.Width(100)))
									{
										string dir = Application.dataPath.Replace("Assets", "") + "/Temp/KopiaCheck";
										if (!FilesSet.CheckDirectory(dir))
											FilesSet.CreateDirectory(dir);

										string path = "";
										foreach (string s in fileList)
											path += s + "/";
										path += file.name;
										RestoreSnapshotImage(path, dir + "/preview.png");
									}
								}
							}

							if (GUILayout.Button("Restore", GUILayout.Width(100)))
							{
								string path = "";
								foreach (string s in fileList)
									path += s + "/";
								path += file.name;

								string realpath = path.Replace(fileList[0] + "/", "");

								RestoreSnapshot(path, realpath);
							}

							EditorGUILayout.EndHorizontal();
						}
						GUILayout.EndScrollView();
					}
				}
				else if (openMode == oM.settings)
				{
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Close"))
					{
						openMode = oM.basic;
						config = preconfig.Copy();
					}
					if (GUILayout.Button("Save"))
					{
						if (!string.IsNullOrEmpty(config.serverdir))
						{
							if (config.serverdir[config.serverdir.Length - 1] == '/' || config.serverdir[config.serverdir.Length - 1] == '\\')
								config.serverdir.Remove(config.serverdir.Length - 1);

							if (FilesSet.CheckFile(config.serverdir + "/kopia.exe"))
							{
								enabled = true;
								CheckAndLoad();
							}
							else
								enabled = false;
						}
						else
							config.serverdir = preconfig.serverdir;

						if (!config.snaponlyassets)
							datapath = Application.dataPath.Replace("/Assets", "");
						else
							datapath = Application.dataPath;

						config.Save();
						openMode = oM.basic;
					}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Label("Path to exe:", GUILayout.Width(200));
					config.serverdir = EditorGUILayout.TextField("", config.serverdir);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Label("Show .meta files:", GUILayout.Width(200));
					config.showmeta = EditorGUILayout.Toggle(config.showmeta);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Label("Snapshot only Asset folder:", GUILayout.Width(200));
					config.snaponlyassets = EditorGUILayout.Toggle(config.snaponlyassets);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Label("Don't rewrite files when restore:", GUILayout.Width(200));
					config.dontrewrite = EditorGUILayout.Toggle(config.dontrewrite);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.BeginVertical();
					GUILayout.Label("Text formats");
					settpos1 = EditorGUILayout.BeginScrollView(settpos1);
					if (config.textFormats != null)
					{
						for (int i = 0; i < config.textFormats.Count; i++)
						{
							int id = i;
							GUILayout.BeginHorizontal();
							GUILayout.Label(config.textFormats[id]);
							if (GUILayout.Button("-", GUILayout.Width(20)))
							{
								config.textFormats.RemoveAt(id);
							}
							GUILayout.EndHorizontal();
						}
					}
					else 
						config.textFormats = new List<string>();
					GUILayout.BeginHorizontal();
					settinput1 = EditorGUILayout.TextField(settinput1);
					if (GUILayout.Button("Add", GUILayout.Width(40)))
					{
						config.textFormats.Add(settinput1);
						GUI.FocusControl("");
						settinput1 = "";
					}
					GUILayout.EndHorizontal();
					EditorGUILayout.EndScrollView();
					GUILayout.EndVertical();
					GUILayout.BeginVertical();
					GUILayout.Label("Image formats");
					settpos1 = EditorGUILayout.BeginScrollView(settpos1);
					if (config.imgFormats != null)
					{
						for (int i = 0; i < config.imgFormats.Count; i++)
						{
							int id = i;
							GUILayout.BeginHorizontal();
							GUILayout.Label(config.imgFormats[id]);
							if (GUILayout.Button("-", GUILayout.Width(20)))
							{
								config.imgFormats.RemoveAt(id);
							}
							GUILayout.EndHorizontal();
						}
					}
					else config.imgFormats = new List<string>();
					GUILayout.BeginHorizontal();
					settinput2 = EditorGUILayout.TextField(settinput2);
					if (GUILayout.Button("Add", GUILayout.Width(40)))
					{
						config.imgFormats.Add(settinput2);
						settinput2 = "";
						GUI.FocusControl("");
					}
					GUILayout.EndHorizontal();
					EditorGUILayout.EndScrollView();
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				else if (openMode==oM.console)
				{
					if (GUILayout.Button("Close"))
					{
						openMode = oM.basic;
					}

					EditorGUILayout.Space();

					GUILayout.BeginHorizontal();
					consoleInput = EditorGUILayout.TextField("", consoleInput);

					if (GUILayout.Button("Run", GUILayout.Width(40)))
					{
						if (!string.IsNullOrEmpty(consoleInput))
							UnityEngine.Debug.Log(runProc(consoleInput));
						GUI.FocusControl("");
					}
					GUILayout.EndHorizontal();

					EditorGUILayout.Space();

					if (GUILayout.Button("Open Documentation"))
					{
						Application.OpenURL("https://kopia.io/docs/reference/command-line/common/");
					}

					EditorGUILayout.Space();

					//generate help
					if (string.IsNullOrEmpty(consoleInput) || currHelp != null)
					{
						conspos = GUILayout.BeginScrollView(conspos, style);

						foreach (KPSL_Editor_ConsoleHelp c in currHelp.help)
						{
							if (GUILayout.Button(c.text))
							{
								currHelp = c;
								consoleInput += c.text + " ";
								GUI.FocusControl("");
							}
						}
					}
					GUILayout.EndScrollView();

					if (string.IsNullOrEmpty(consoleInput) && currHelp != consoleHelp)
						currHelp = consoleHelp;

				}
			}
			else
			{
				EditorGUI.ProgressBar(new Rect(3, 45, position.width - 6, 20), 100f, workingStatus);
				EditorGUI.LabelField(new Rect(3, 70, position.width - 6, 20), "<color=red><b>DONT CLOSE UNITY!</b></color>", style);
			}
		}
		else
		{
			GUILayout.Label("Please input path to folder with kopia.exe");
			if (mainIcon != null)
				GUILayout.Label(new GUIContent(mainIcon));
		}
	}

	string GetFormat(string input)
	{
		string ret = "";

		if (!string.IsNullOrEmpty(input))
		{
			for (int i = input.Length-1;i>=0;i--)
			{
				if (i == 0) return "";
				else
				{
					ret += input[i];
					if (input[i] == '.')
						break;
				}
			}
		}

		return Reverse(ret);
	}

	static string Reverse(string s)
	{
		char[] charArray = s.ToCharArray();
		Array.Reverse(charArray);
		return new string(charArray);
	}

	async void OpenFiles()
	{
		files = new List<KPSL_Editor_File>();
		string id = "";
		foreach (string s in fileList)
			id += s + "/";
		string output = "";
		await Task.Run(() => output = runProc("list -l \"" + id + "\""));
		foreach (string s in output.Split('\n').ToList())
		{
			if (!string.IsNullOrEmpty(s) && (!s.Contains(".meta") || config.showmeta))
			{
				List<string> str = s.Split(" ").ToList();

				//check is that directory or file
				bool file = true;
				if (str[0][0] == 'd')
				{
					file = false;
				}

				//look for data
				for (int i = 0; i < str.Count; i++)
				{
					if (Int64.TryParse(str[i], out Int64 r))
					{
						files.Add(new KPSL_Editor_File());
						files[files.Count - 1].folder = !file;
						files[files.Count - 1].size = r;
						files[files.Count - 1].ind = str[i + 4];
						if (file)
						{
							for (int n = i + 7; n < str.Count; n++)
								if (n != str.Count - 1)
									files[files.Count - 1].name += str[n] + " ";
								else
									files[files.Count - 1].name += str[n];
						}
						else
							for (int n = i + 6; n < str.Count; n++)
								if (n != str.Count - 1)
									files[files.Count - 1].name += str[n] + " ";
								else
									files[files.Count - 1].name += str[n];

						break;
					}
				}
			}
		}

		files.Sort((x, y) =>
		{
			if (x.folder && y.folder && x.name != null && y.name != null) return StringLogicalComparer.Compare(x.name, y.name);
			else if (x.folder && !y.folder) return -1;
			else if (!x.folder && y.folder) return 1;
			else if (x.name == null && y.name == null) return 0;
			else if (x.name == null) return -1;
			else if (y.name == null) return 1;
			else return StringLogicalComparer.Compare(x.name,y.name);
		});

		filePos = fileList.Count;
		working = false;
	}

	async void NewSnapshotAsync()
	{
		if (!working)
		{
			working = true;
			workingStatus = "Creating new snapshot...";
			await Task.Run(() => runProc("snapshot create " + "\"" + datapath + "\""));
			working = false;
			CheckAndLoad();
		}
	}

	async void DeleteSnapshot(KPSL_Editor_Snapshots s)
	{
		if (!working)
		{
			working = true;
			workingStatus = "Deleting snapshot " + s.data + " " + s.ind + "...";
			await Task.Run(() => runProc("snapshot delete " + s.ind + " --delete"));
			working = false;
			CheckAndLoad();
		}
	}

	async void RestoreSnapshot(KPSL_Editor_Snapshots s)
	{
		if (!working)
		{
			working = true;
			workingStatus = "Restoring snapshot " + s.data + " " + s.ind + "...";
			await Task.Run(() => runProc("snapshot restore " + s.ind + " \"" + datapath + "\"" + (config.dontrewrite? "" : " --overwrite-directories --overwrite-files")));
			working = false;
			CheckAndLoad();
		}
	}

	async void RestoreSnapshot(string filepath, string realfilepath)
	{
		if (!working)
		{
			working = true;
			workingStatus = "Restoring snapshot file " + filepath + "...";
			//UnityEngine.Debug.Log("snapshot restore " + "\"" + filepath + "\" \"" + datapath + "/" + realfilepath + "\" --overwrite-directories --overwrite-files");
			await Task.Run(() => runProc("snapshot restore " + "\"" + filepath + "\" \"" + datapath+"/"+ realfilepath + "\"" + (config.dontrewrite? "" : " --overwrite-directories --overwrite-files")));
			working = false;
			CheckAndLoad();
		}
	}

	async void RestoreSnapshotTextCheck(string filepath, string realfilepath, string realfile, string dir)
	{
		if (!working)
		{
			working = true;
			workingStatus = "Restoring snapshot file " + filepath + "...";
			//UnityEngine.Debug.Log("snapshot restore " + "\"" + filepath + "\" \"" + datapath + "/" + realfilepath + "\" --overwrite-directories --overwrite-files");
			await Task.Run(() => runProc("snapshot restore " + "\"" + filepath + "\" \"" + realfilepath + "\" --overwrite-directories --overwrite-files"));
			
			working = false;
			File.Copy(realfile, dir + "/orig.txt", true);
			EditorWindow.GetWindow(typeof(KPSL_Editor_TextSee), false, "KopiaUni Text Change View", true);
		}
	}

	async void RestoreSnapshotImage(string filepath, string realfilepath)
	{
		if (!working)
		{
			working = true;
			workingStatus = "Restoring snapshot file " + filepath + "...";
			//UnityEngine.Debug.Log("snapshot restore " + "\"" + filepath + "\" \"" + datapath + "/" + realfilepath + "\" --overwrite-directories --overwrite-files");
			await Task.Run(() => runProc("snapshot restore " + "\"" + filepath + "\" \"" + realfilepath + "\" --overwrite-directories --overwrite-files"));

			working = false;
			EditorWindow.GetWindow(typeof(KPSL_Editor_ImgPrev), false, "KopiaUni Image Preview", true);
		}
	}

	async void RestoreSnapshotText(string filepath, string realfilepath)
	{
		if (!working)
		{
			working = true;
			workingStatus = "Restoring snapshot file " + filepath + "...";
			//UnityEngine.Debug.Log("snapshot restore " + "\"" + filepath + "\" \"" + datapath + "/" + realfilepath + "\" --overwrite-directories --overwrite-files");
			await Task.Run(() => runProc("snapshot restore " + "\"" + filepath + "\" \"" + realfilepath + "\" --overwrite-directories --overwrite-files"));

			working = false;
			EditorWindow.GetWindow(typeof(KPSL_Editor_TextPreview), false, "KopiaUni Text Preview", true);
		}
	}

	string runProc(string args)
	{
		string ret = "";
		var proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = config.serverdir + "\\kopia.exe",
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				Verb = "runas"
			}
		};

		proc.Start();

		ret = proc.StandardOutput.ReadToEnd();

		return ret;
	}

	void CheckAndLoad()
	{
		
		string output = runProc("snapshot list " + "\"" + datapath + "\"");

		List<string> outputs = output.Split('\n').ToList();
		snapshots = new List<KPSL_Editor_Snapshots>();

		if (outputs.Count > 1)
			for (int i = 1; i < outputs.Count; i++)
			{
				if (!string.IsNullOrEmpty(outputs[i]))
				{
					string[] data = outputs[i].Split(' ');
					snapshots.Add(new KPSL_Editor_Snapshots());
					snapshots[snapshots.Count - 1].data = DateTime.Parse(data[2] + " " + data[3]);
					snapshots[snapshots.Count - 1].ind = data[5];
					snapshots[snapshots.Count - 1].size = data[6] + data[7];
					snapshots[snapshots.Count - 1].files = data[9].Replace("files:", "");
					snapshots[snapshots.Count - 1].dirs = data[10].Replace("dirs:", "");
				}
			}
	}
}

[System.Serializable]
public class KPSL_Editor_ConsoleHelp
{
	public string text;
	public List<KPSL_Editor_ConsoleHelp> help;
}

	[System.Serializable]
public class KPSL_Editor_File
{
	public string name;
	public Int64 size;
	public string ind;
	public bool folder;
}

[System.Serializable]
public class KPSL_Editor_Snapshots
{
	public bool show;
	public DateTime data;
	public string ind;
	public string size;
	public string files;
	public string dirs;
}

	[System.Serializable]
public class KPSL_Editor_MainConfig
{
	public string serverdir;
	public bool showmeta;
	public bool snaponlyassets;
	public bool dontrewrite;
	public List<string> textFormats;
	public List<string> imgFormats;

	public void Load()
	{
		if (FilesSet.CheckFile(Application.dataPath + "/SLywnowAssets/KopiaUnity/config.cfg"))
		{
			KPSL_Editor_MainConfig c = JsonUtility.FromJson<KPSL_Editor_MainConfig>(FilesSet.LoadStream(Application.dataPath + "/SLywnowAssets/KopiaUnity/config.cfg", false, false));
			serverdir = c.serverdir;
			showmeta = c.showmeta;
			snaponlyassets = c.snaponlyassets;
			dontrewrite = c.dontrewrite;
			textFormats = c.textFormats;
			imgFormats = c.imgFormats;
		}
		else
		{
			textFormats = new List<string>();
			textFormats.Add(".txt");
			textFormats.Add(".json");
			textFormats.Add(".cs");

			imgFormats = new List<string>();
			imgFormats.Add(".png");
			imgFormats.Add(".jpg");
			imgFormats.Add(".jpeg");
			imgFormats.Add(".bmp");
		}
	}

	public void Save()
	{
		FilesSet.SaveStream(Application.dataPath + "/SLywnowAssets/KopiaUnity/config.cfg", JsonUtility.ToJson(this, true));
	}

	public KPSL_Editor_MainConfig Copy()
	{
		KPSL_Editor_MainConfig ret = new KPSL_Editor_MainConfig();

		ret.serverdir = serverdir;
		ret.showmeta = showmeta;
		ret.snaponlyassets = snaponlyassets;
		ret.dontrewrite = dontrewrite;
		ret.textFormats = textFormats.Clone();
		ret.imgFormats = imgFormats.Clone();

		return ret;
	}
}

public class KPSL_Editor_MainManager : Editor
{
    [MenuItem("SLywnow/Kopia UI")]
    static void SetDirection()
    {
        EditorWindow.GetWindow(typeof(KPSL_Editor_Main), false, "Kopia UI", true);
    }
}
