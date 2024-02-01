using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLywnow;
//using SimpleJSON;

namespace AutoLangSLywnow
{
	public class ALSL_Main : MonoBehaviour
	{
		public static ALSL_Perfab perfab;
		public static List<string> alllangs = new List<string>();
		public static List<string> keys_alsl = new List<string>();
		public static List<string> current_alsl = new List<string>();
		public static List<string> keysR_alsl = new List<string>();
		public static List<string> repickR_alsl = new List<string>();
		public static List<string> langsvis = new List<string>();
		public static List<string> assotiate = new List<string>();
		public static List<bool> isoutput = new List<bool>();
		public static int deflang = 0;
		public static int languagebydefault;

		public static bool forseupdateall = false;

		public static List<ALSL_Language> allwords = new List<ALSL_Language>();
		public static int currentlang;

		public static string GetWorldAndFindKeys(string key)
		{
			string output = "";

			output = GetWord(key);
			if (!string.IsNullOrEmpty(output))
				output = FindKeysInString(output);

			return output;
		}

		public static string FindKeysInString(string input)
		{
			string output = input;

			if (!string.IsNullOrEmpty(output))
			{
				for (int a = 0; a < keysR_alsl.Count; a++)
				{
					if (!string.IsNullOrEmpty(keysR_alsl[a]))
						output = output.Replace(keysR_alsl[a], repickR_alsl[a]);
				}
			}

			return output;
		}

		public static void SetReplaceWord(string key, string NewWord)
		{
			if (!string.IsNullOrEmpty(key))
			{
				if (keysR_alsl.Contains(key))
					repickR_alsl[keysR_alsl.IndexOf(key)] = NewWord;
				else
				{
					keysR_alsl.Add(key);
					repickR_alsl.Add(NewWord);
				}
			}
		}

		public static string GetReplaceFromKey(string key)
		{
			string output = "";

			if (keysR_alsl.Contains(key)) output = repickR_alsl[keysR_alsl.IndexOf(key)];

			return output;
		}

		public enum CheckType { lang, keys, current, keysR, repickR, onlang };
		public static bool CheckParam(string input, CheckType type, string lang = null)
		{
			if (type == CheckType.lang) if (alllangs.Contains(input)) return true;
			if (type == CheckType.keys) if (keys_alsl.Contains(input)) return true;
			if (type == CheckType.current) if (current_alsl.Contains(input)) return true;
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

		public static void SetLanguage(int LanduageId)
		{
			if (alllangs.Count>LanduageId)
			{
				currentlang = LanduageId;

				current_alsl = new List<string>();
				for (int i=0;i<keys_alsl.Count;i++)
					current_alsl.Add(allwords[currentlang].words[i]);
				forseupdateall = !forseupdateall;
			}
		}

		public static void SetLanguage(string LanduageName)
		{
			if (alllangs.Contains(LanduageName))
			{
				int ss = SaveSystemAlt.IsIndex();
				SaveSystemAlt.StopWorkAndClose();
				SaveSystemAlt.StartWork(14);
				currentlang = alllangs.IndexOf(LanduageName);
				SaveSystemAlt.SetInt("currentlang", currentlang);
				SaveSystemAlt.StopWorkAndClose();
				SaveSystemAlt.StartWork(ss);

				current_alsl = new List<string>();
				for (int i = 0; i < keys_alsl.Count; i++)
					current_alsl.Add(allwords[currentlang].words[i]);
				forseupdateall = !forseupdateall;
			}
		}


		public static bool IsCurrentLangOutPut()
		{
			return isoutput[currentlang];
		}

		public static bool IsLangOutPut(string lang)
		{
			if (!alllangs.Contains(lang)) return false;

			return isoutput[alllangs.IndexOf(lang)];
		}

		public static bool IsLangOutPut(int langid)
		{
			if (alllangs.Count<= langid) return false;

			return isoutput[langid];
		}

		public static string GetWord(string key)
		{
			string ret = null;
			if (keys_alsl.Contains(key) && current_alsl.Count > keys_alsl.IndexOf(key)) ret = current_alsl[keys_alsl.IndexOf(key)];
			return ret;
		}

		public static string Replace (string input)
		{
			string ret = input;
			for (int i = 0; i < keysR_alsl.Count; i++)
				if (!string.IsNullOrEmpty(keysR_alsl[i]))
					ret = ret.Replace(keysR_alsl[i], repickR_alsl[i]);

			return ret;
		}

		public static void AddLanguageFromPath(string path, string filename, bool force=false)
		{
			if (alllangs.Contains(filename) && !force) { Debug.Log("Language with this name is already exist"); return; }
			try
			{
				ALSL_Language outlang = new ALSL_Language();

				/*string json = FilesSet.LoadStream(path, filename, "LangJson", false, false);
				var R = JSON.Parse(json);
				for (int a = 0; a < keys_alsl.Count; a++)
				{
					if (!string.IsNullOrEmpty(R[keys_alsl[a]].Value))
						outlang.words.Add(R[keys_alsl[a]].Value.Replace("#nl", "\n"));
					else
						outlang.words.Add(R[keys_alsl[a]].Value);
				}*/
				string[] json = FilesSet.LoadStream(path, filename, "LangJson", false);
				for (int a = 0; a < keys_alsl.Count; a++)
				{
					string getword = json[a + 1].Replace("\"" + ALSL_Main.keys_alsl[a] + "\": \"", "");
					getword = getword.Replace("\"", "");
					if (!string.IsNullOrEmpty(getword))
						outlang.words.Add(getword.Replace("#nl", "\n"));
					else
						outlang.words.Add(getword);
				}

				if (outlang.words.Count == keys_alsl.Count)
				{
					bool n = false;
					if (force && alllangs.Contains(filename))
					{
						if (currentlang== alllangs.IndexOf(filename))
							n = true;
						forseupdateall = !forseupdateall;
						RemoveLanguage(filename);
					}

					allwords.Add(outlang);
					alllangs.Add(filename);
					assotiate.Add("none");
					langsvis.Add(filename);
					if (n) { SetLanguage(alllangs.IndexOf(filename)); forseupdateall = !forseupdateall; }
					forseupdateall = !forseupdateall;
				}
				else
					Debug.LogError("Failed to load " + filename);
			}
			catch (System.SystemException exp)
			{
				Debug.LogError("Failed to load " + filename);
			}
		}

		public static void RemoveLanguage(string name)
		{
			if (!alllangs.Contains(name)) { Debug.Log("Language not found"); return; }
			int num = alllangs.IndexOf(name);
			if (!(num >= languagebydefault)) { Debug.Log("ALSL can't remove default language"); return; }

			if (currentlang == num) { SetLanguage(deflang); forseupdateall = !forseupdateall; }

			allwords.RemoveAt(num);
			alllangs.RemoveAt(num);
			assotiate.RemoveAt(num);
			langsvis.RemoveAt(num);
			forseupdateall = !forseupdateall;
		}

		public static void RestartAll()
		{
			StartingSLAL.Restart();
			forseupdateall = !forseupdateall;
		}
	}

	public class ALSL_Language
	{
		public List<string> words = new List<string>();
	}
}
