using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoLangSLywnow;
using UnityEditor;
using SLywnow;
using System;

namespace AutoLangEditorSLywnow
{

	public class ALSL_Editor_System : Editor
	{
		public static bool enable;
		public int repeatCount=0;

		public static string folderlink = Application.dataPath + "/SLywnowAssets/AutoLang/Data";
		public static string folderlinkA = "Assets/SLywnowAssets/AutoLang/Data";
		public static string folderlanglink = Application.dataPath + "/SLywnowAssets/AutoLang/Data/Langs Files";
		public static string folderlanglinkA = "Assets/SLywnowAssets/AutoLang/Data/Langs Files";
		public static string folderreslink = "Assets/SLywnowAssets/AutoLang/Resources";
		public static ALSL_Perfab perfab;

		public static ALSL_Params options = new ALSL_Params();
		public static List<string> alllangs = new List<string>();
		public static List<string> keys_alsl = new List<string>();
		public static List<string> keysR_alsl = new List<string>();
		public static List<string> repickR_alsl = new List<string>();
		public static List<string> langsvis = new List<string>();
		public static List<string> assotiate = new List<string>();
		public static int deflang = 0;

		public static List<ALSL_Language> allwords = new List<ALSL_Language>();

		

		public static List<string> GetStringsByKey(string key)
		{
			List<string> ret = new List<string>();

			if (keys_alsl.Contains(key))
			{
				int id = keys_alsl.IndexOf(key);

				foreach (ALSL_Language l in allwords)
					ret.Add(l.words[id]);
			}

			return ret;
		}

		public enum CheckType { lang, keys, keysR, repickR, onlang };
		public static bool CheckParam(string input, CheckType type, string lang = null)
		{
			if (type == CheckType.lang) if (alllangs.Contains(input)) return true;
			if (type == CheckType.keys) if (keys_alsl.Contains(input)) return true;
			if (type == CheckType.keysR) if (repickR_alsl.Contains(input)) return true;
			if (type == CheckType.repickR) if (repickR_alsl.Contains(input)) return true;
			if (type == CheckType.onlang)
			{
				if (string.IsNullOrEmpty(lang)) for (int i = 0; i < allwords.Count; i++)
					{
						if (allwords[i].words.Contains(input)) return true;
					}
				else
				{
					if (alllangs.Contains(lang)) { int i = alllangs.IndexOf(lang); if (allwords[i].words.Contains(input)) return true; }
				}
			}
			return false;
		}
		public enum TypeOfSave { lang, keys, keysR, repickR, word };
		public static void SaveToAsset(string input, TypeOfSave type, string input2 = "", bool def = false, string inputAL="none", string lang = null)
		{
			if (string.IsNullOrEmpty(input)) return;

			if (type == TypeOfSave.keys)
			{
				keys_alsl.Add(input);
				for (int i = 0; i < allwords.Count; i++) allwords[i].words.Add("");
			}

			if (type == TypeOfSave.lang)
			{
				if (def) deflang = alllangs.Count;
				alllangs.Add(input);
				langsvis.Add(input2);
				assotiate.Add(inputAL);

				allwords.Add(new ALSL_Language());
				for (int i = 0; i < keys_alsl.Count; i++) allwords[allwords.Count-1].words.Add("");
			}
		}

		public static void RemoveToAsset(string input, TypeOfSave type, string lang = null)
		{
			if (string.IsNullOrEmpty(input)) return;

			if (type == TypeOfSave.keys && keys_alsl.Contains(input))
			{
				int a = keys_alsl.IndexOf(input);
				keys_alsl.Remove(input);
				for (int i = 0; i < allwords.Count; i++) allwords[i].words.RemoveAt(a);
			}

			if (type == TypeOfSave.lang && alllangs.Contains(input))
			{
				int a = alllangs.IndexOf(input);
				FilesSet.DelStream(folderlanglink, alllangs[a], "json",false,false);
				alllangs.Remove(input);
				langsvis.RemoveAt(a);
				if (deflang == a) deflang = 0;
				allwords.RemoveAt(a);
				assotiate.RemoveAt(a);
			}
		}

		public static void RenameToAsset(string input, string input2, TypeOfSave type, string input3 = "", bool def = false, string inputAL="none", string lang = null)
		{
			if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(input2)) return;

			if (type == TypeOfSave.keys && keys_alsl.Contains(input) && !keys_alsl.Contains(input2))
			{
				keys_alsl[keys_alsl.IndexOf(input)] = input2;
			}

			if (type == TypeOfSave.lang && alllangs.Contains(input))
			{
				if (alllangs[alllangs.IndexOf(input)] != input2)
				{
					List<string> allwordss = new List<string>();
					allwordss.Add("{");
					for (int a = 0; a < keys_alsl.Count; a++)
					{
						//Debug.Log(keys_alsl.Count + " " + allwords[i].words.Count);
						string wordsave = allwords[alllangs.IndexOf(input)].words[a].Replace("\n", "▬");
						allwordss.Add("\"" + keys_alsl[a] + "\": \"" + wordsave + "\"");
						if (a < keys_alsl.Count - 1) allwordss[a + 1] += ",";
					}
					allwordss.Add("}");
					FilesSet.DelStream(folderlanglink, alllangs[alllangs.IndexOf(input)], "json", false, false);
					FilesSet.SaveStream(folderlanglink, input2, "json", allwordss.ToArray(), false);
				}

				langsvis[alllangs.IndexOf(input)] = input3;
				assotiate[alllangs.IndexOf(input)] = inputAL;
				if (def) deflang = alllangs.IndexOf(input);
				alllangs[alllangs.IndexOf(input)] = input2;
			}
		}

		public static void AddFromJSON(UnityEngine.Object json)
		{
			string input = "";

			string path = AssetDatabase.GetAssetPath(json);
			//input = FilesSet.LoadStream(path.Replace());
			Debug.Log(path);
		}

		public static void Move(string key, bool up, int count=1)
		{
			if (keys_alsl.Contains(key))
			{
				int id = keys_alsl.IndexOf(key);
				int to = id;
				if (up)
					to = id - count;
				else
					to = id + count;
				if (to >= 0 && to < keys_alsl.Count)
				{
					keys_alsl.Move(id, to);
					for (int i = 0; i < alllangs.Count; i++)
					{
						allwords[i].words.Move(id, to);
					}
				}
			}
		}

		public static void Move(string key, int to)
		{
			if (keys_alsl.Contains(key))
			{
				int id = keys_alsl.IndexOf(key);
				if (to >= 0 && to < keys_alsl.Count)
				{
					keys_alsl.Move(id, to);
					for (int i = 0; i < alllangs.Count; i++)
					{
						allwords[i].words.Move(id, to);
					}
				}
			}
		}

		public static void Move(string key, string toKey)
		{
			if (keys_alsl.Contains(key))
			{
				int id = keys_alsl.IndexOf(key);
				int to = keys_alsl.IndexOf(toKey);

				/*if (!up)
					to += 1;
				*/

				if (to >= 0 && to < keys_alsl.Count)
				{
					keys_alsl.Move(id, to);
					for (int i = 0; i < alllangs.Count; i++)
					{
						allwords[i].words.Move(id, to);
					}
				}
			}
		}

		public static void SaveFiles()
		{
			if (alllangs.Count > 0)
			{
				for (int i = 0; i < allwords.Count; i++)
				{
					List<string> allwordss = new List<string>();
					allwordss.Add("{");
					for (int a = 0; a < keys_alsl.Count; a++)
					{
						//Debug.Log(keys_alsl.Count + " " + allwords[i].words.Count);
						string wordsave = allwords[i].words[a].Replace("\n", "▬");
						allwordss.Add("\"" + keys_alsl[a] + "\": \"" + wordsave + "\"");
						//if (a < keys_alsl.Count-1) allwordss[a+1] += ",";
					}
					allwordss.Add("}");
					FilesSet.SaveStream(folderlanglink, alllangs[i], "json", allwordss.ToArray(), false);
				}
			}
			ALSL_ToSaveJSON ts = new ALSL_ToSaveJSON();
			ts.alllangs = alllangs;
			ts.keys_alsl = keys_alsl;
			ts.keysR_alsl = keysR_alsl;
			ts.repickR_alsl = repickR_alsl;
			ts.deflang = deflang;
			ts.langsvis = langsvis;
			ts.assotiate = assotiate;

			string tosave = JsonUtility.ToJson(ts, true);
			FilesSet.SaveStream(folderlink, "params", "txt", JsonUtility.ToJson(options, true), false);

			FilesSet.SaveStream(folderlink, "keys", "txt", new string[1] { tosave }, false);
			AssetDatabase.ImportAsset(folderlinkA + "/keys.txt", ImportAssetOptions.ForceUpdate);
			AssetDatabase.ImportAsset(folderlinkA + "/params.txt", ImportAssetOptions.ForceUpdate);
			for (int i = 0; i < alllangs.Count; i++)
				AssetDatabase.ImportAsset(folderlanglinkA + "/" + alllangs[i] + ".json", ImportAssetOptions.ImportRecursive);

			perfab.langFiles = new List<ALSL_Perfab_Lang>();

			for (int i = 0; i < alllangs.Count; i++)
			{
				if (FilesSet.CheckFile(folderlanglink, alllangs[i], "json", false))
				{
					perfab.langFiles.Add(new ALSL_Perfab_Lang());
					perfab.langFiles[perfab.langFiles.Count - 1].name = alllangs[i];
					perfab.langFiles[perfab.langFiles.Count - 1].asset = AssetDatabase.LoadAssetAtPath(folderlanglinkA + "/" + alllangs[i] + ".json", typeof(TextAsset)) as TextAsset;
				}
			}
		}
	}


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

	public class ALSL_Params
	{
		public int verison = 1;
		public int LangFromSystem = 0;
		public string output = "#sf/Games/#project/CustomTranslate";
	}

}
