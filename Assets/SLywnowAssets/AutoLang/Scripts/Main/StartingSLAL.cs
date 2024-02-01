using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLywnow;
using UnityEngine.UI;
//using SimpleJSON;

namespace AutoLangSLywnow
{
	public class StartingSLAL
	{

		static ALSL_ToSaveJSON keys= new ALSL_ToSaveJSON();
		static bool editor = false;
		public static int SSALevel = 0;

		public static void Restart()
		{
			OnRuntimeMethodLoad();
		}

		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		static void OnRuntimeMethodLoad()
		{
			//FilesSet.SaveStream(Application.streamingAssetsPath + "/ALSL", "keys", "alsldata", JsonUtility.ToJson(keys), false);

			ALSL_Main.perfab = (Resources.Load("alsl_perfab", typeof(GameObject)) as GameObject).GetComponent<ALSL_Perfab>();

			SaveSystemAlt.StartWork(SSALevel);
			//SaveSystemAlt.DeleteKey("LangPath");
			FirstParamALSL fp = new FirstParamALSL();
			fp = JsonUtility.FromJson<FirstParamALSL>(ALSL_Main.perfab.options.text);
#if UNITY_EDITOR
			editor = true;
#endif
			if (SaveSystemAlt.GetInt("versionT") < fp.verison)
			{
				SaveSystemAlt.SetInt("versionT", fp.verison);
				editor = true;
			}

			if (!SaveSystemAlt.HasKey("OutPutFiles") || SaveSystemAlt.GetString("OutPutFiles", "") != fp.output || editor)
			{
				if (string.IsNullOrEmpty(fp.output)) SaveSystemAlt.SetString("OutPutFiles", null);
				else
				{
					fp.output = fp.output.Replace("#sf", FastFind.GetDefaultPath());
					fp.output = fp.output.Replace("#appdata", Application.persistentDataPath);
					fp.output = fp.output.Replace("#app", Application.dataPath);
					fp.output = fp.output.Replace("#project", Application.productName);
					SaveSystemAlt.SetString("OutPutFiles", fp.output);
				}
			}

			SaveSystemAlt.SaveUpdatesNotClose();

			keys = JsonUtility.FromJson<ALSL_ToSaveJSON>(ALSL_Main.perfab.keys.text);
			SetUp();

			SaveSystemAlt.SaveUpdatesNotClose();
		}

		static void SetUp()
		{
			ALSL_Main.alllangs = keys.alllangs;
			ALSL_Main.keysR_alsl = keys.keysR_alsl;
			ALSL_Main.keys_alsl = keys.keys_alsl;
			ALSL_Main.repickR_alsl = keys.repickR_alsl;
			ALSL_Main.langsvis = keys.langsvis;
			ALSL_Main.deflang = keys.deflang;
			ALSL_Main.assotiate = keys.assotiate;

			ALSL_Main.isoutput = new List<bool>();
			foreach (string a in keys.alllangs)
				ALSL_Main.isoutput.Add(false);

			if (SaveSystemAlt.HasKey("currentlang"))
				ALSL_Main.currentlang = SaveSystemAlt.GetInt("currentlang");
			else
			{
				if (SaveSystemAlt.GetInt("LangFromSystem")==0)
					ALSL_Main.currentlang = keys.deflang;
				else
				{
					string sysL= Application.systemLanguage.ToString();
					ALSL_Main.currentlang = -1;
					for (int i = 0; i < ALSL_Main.assotiate.Count;i++) if (ALSL_Main.assotiate[i] == sysL) ALSL_Main.currentlang = i;
					if (ALSL_Main.currentlang==-1) ALSL_Main.currentlang = keys.deflang;
				}
				SaveSystemAlt.SetInt("currentlang", ALSL_Main.currentlang);
			}

			//inbuild langs
			ALSL_Main.allwords = new List<ALSL_Language>();
			for (int i = 0; i < ALSL_Main.alllangs.Count; i++)
			{
				ALSL_Main.allwords.Add(new ALSL_Language());
				/*string json = FilesSet.LoadStream(path + "/Langs Files", ALSL_Main.alllangs[i], "json", false, false);
				var R = JSON.Parse(json);
				for (int a = 0; a < ALSL_Main.keys_alsl.Count; a++)
				{
					if (!string.IsNullOrEmpty(R[ALSL_Main.keys_alsl[a]].Value))
						ALSL_Main.allwords[i].words.Add(R[ALSL_Main.keys_alsl[a]].Value.Replace("▬", "\n"));
					else
						ALSL_Main.allwords[i].words.Add(R[ALSL_Main.keys_alsl[a]].Value);
				}*/
				string[] json = ALSL_Main.perfab.langFiles.Find(f => f.name == ALSL_Main.alllangs[i]).asset.text.Split('\n');
				for (int a = 0; a < ALSL_Main.keys_alsl.Count; a++)
				{
					string getword = json[a + 1].Replace("\"" + ALSL_Main.keys_alsl[a] + "\": \"", "");
					getword = getword.Replace("\"", "");
					if (!string.IsNullOrEmpty(getword))
						ALSL_Main.allwords[i].words.Add(getword.Replace("▬", "\n"));
					else
						ALSL_Main.allwords[i].words.Add("");
				}
			}

			ALSL_Main.languagebydefault = ALSL_Main.alllangs.Count;

			//custom langs
			if (!string.IsNullOrEmpty(SaveSystemAlt.GetString("OutPutFiles")))
			{
				if (!FilesSet.CheckDirectory(SaveSystemAlt.GetString("OutPutFiles"))) FilesSet.CreateDirectory(SaveSystemAlt.GetString("OutPutFiles"));

				string[] names = FilesSet.GetFilesFromdirectories(SaveSystemAlt.GetString("OutPutFiles"), "langjson",false,FilesSet.TypeOfGet.NamesOfFiles);
				if (names != null && names.Length > 0)
				{
					for (int i = 0; i < names.Length; i++)
					{
						if (names[i] != "example")
						{
							try
							{
								addCustomLang(JsonUtility.FromJson<ALSL_CustomLangJSON>(FilesSet.LoadStream(SaveSystemAlt.GetString("OutPutFiles"), names[i], "langjson", false, false)), names[i]);
							}
							catch (System.Exception ex)
							{
								Debug.LogError("Failed to load " + names[i] + " error: " + ex.ToString());
							}
						}
					}
				}
				else
				{
					try
					{
						GenerateExample();
					}
					catch (System.Exception ex)
					{
						Debug.LogError(ex);
					}
				}

				if (editor)
				{
					GenerateExample();
				}
			}

			if (ALSL_Main.currentlang < ALSL_Main.alllangs.Count)
				ALSL_Main.SetLanguage(ALSL_Main.alllangs[ALSL_Main.currentlang]);
			else
				ALSL_Main.SetLanguage(ALSL_Main.alllangs[ALSL_Main.deflang]);
		}

		public static void GenerateExample()
		{
			ALSL_CustomLangJSON file = new ALSL_CustomLangJSON();
			file.lines = new List<ALSL_CustomLangLine>();
			file.displayName = "Example";

			for (int i =0;i < ALSL_Main.keys_alsl.Count;i++)
			{
				file.lines.Add(new ALSL_CustomLangLine());
				file.lines[file.lines.Count - 1].key = ALSL_Main.keys_alsl[i];
				file.lines[file.lines.Count - 1].text = ALSL_Main.allwords[ALSL_Main.deflang].words[i].Replace("\r", "");
			}

			FilesSet.SaveStream(SaveSystemAlt.GetString("OutPutFiles"), "example", "langjson", JsonUtility.ToJson(file, true), false, false);
		}

		public static void addCustomLang(ALSL_CustomLangJSON file, string name)
		{
			string langvis = file.displayName;

			ALSL_Language outlang = new ALSL_Language();
			outlang.words = new List<string>();

			for (int i = 0; i < ALSL_Main.keys_alsl.Count; i++)
				outlang.words.Add(ALSL_Main.allwords[ALSL_Main.deflang].words[i].Replace("\r", ""));

			foreach (ALSL_CustomLangLine l in file.lines)
			{
				int id = ALSL_Main.keys_alsl.IndexOf(l.key);
				if (id >= 0)
					outlang.words[id] = l.text;
			}

			ALSL_Main.allwords.Add(outlang);
			ALSL_Main.alllangs.Add(name);
			ALSL_Main.isoutput.Add(true);
			ALSL_Main.assotiate.Add("none");
			ALSL_Main.langsvis.Add(langvis);
		}

	}

	[System.Serializable]
	public class FirstParamALSL
	{
		public int verison = 0;
		public int LangFromSystem = 0;
		public string output = "";
	}

	[System.Serializable]
	public class ALSL_ToSaveJSON
	{
		public List<string> alllangs = new List<string>();
		public List<string> keys_alsl = new List<string>();
		public List<string> keysR_alsl = new List<string>();
		public List<string> repickR_alsl = new List<string>();
		public List<string> langsvis = new List<string>();
		public List<string> assotiate = new List<string>();
		public int deflang = 0;
	}

	[System.Serializable]
	public class ALSL_CustomLangJSON
	{
		public string displayName;
		public List<ALSL_CustomLangLine> lines;
	}

	[System.Serializable]
	public class ALSL_CustomLangLine
	{
		public string key;
		public string text;
	}
}