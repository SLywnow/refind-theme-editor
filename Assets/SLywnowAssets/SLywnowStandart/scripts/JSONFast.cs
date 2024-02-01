using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Linq;

namespace SLywnow
{
	public class FastJSONTests : MonoBehaviour
	{
		/*
		/// <summary>
		/// Getting time or date from url (default jsontest)
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="type">The return type, pmam returns 0 if AM and 1 if PM</param>
		/// <param name="pmam">Whether to use the AM PM system</param>
		/// <param name="from">To specify another url, !WARNING! JSON must has "time" as string, "milliseconds_since_epoch" as int and "date" as string</param>
		public static int getTime(TimeType type, bool pmam = false, string from= "http://worldtimeapi.org/api/timezone/Europe/Moscow")
		{
			string getting ="";
			getting =EasyDo.GetWWWString(from,true);
			if (string.IsNullOrEmpty(getting)) return 0;
			RealTimeCallback rt = new RealTimeCallback();
			rt = JsonUtility.FromJson<RealTimeCallback>(getting);

			DateTime date = DateTime.Parse(rt.date+" "+ rt.time);

			if (type == TimeType.day) return date.Day;
			if (type == TimeType.mounth) return date.Month;
			if (type == TimeType.year) return date.Year;

			if (type == TimeType.sec) return date.Second;
			if (type == TimeType.min) return date.Minute;
			if (type == TimeType.hour)
			{
				if (rt.time[9].ToString() + rt.time[10].ToString() == "PM" && !pmam)
					return int.Parse(rt.time[0].ToString() + rt.time[1].ToString()) + 12;
				else return int.Parse(rt.time[0].ToString() + rt.time[1].ToString());
			}

			if (type == TimeType.pmam) if (rt.time[9].ToString() + rt.time[10].ToString() == "PM") return 1; else return 0;

			return 0;
		}*/

		/// <summary>
		/// Getting time or date from url (default jsontest)
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="from">To specify another url, !WARNING! JSON must has "time" as string, "milliseconds_since_epoch" as int and "date" as string</param>
		public static DateTime getTime(string from = "http://worldtimeapi.org/api/timezone/Europe/Moscow")
		{
			string getting = EasyDo.GetWWWString(from,true);
			if (string.IsNullOrEmpty(getting)) return DateTime.Today;
			RealTimeCallback rt = new RealTimeCallback();

			try
			{
				rt = JsonUtility.FromJson<RealTimeCallback>(getting);

				CultureInfo cc = CultureInfo.CurrentCulture;
				CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
				DateTime dt = DateTime.Parse(rt.utc_datetime);

				CultureInfo.CurrentCulture = cc;

				return dt;
			} catch (Exception ex) { return DateTime.Today; }
		}

		/// <summary>
		/// Getting ip from url (default jsontest)
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="from">To specify another url, !WARNING! JSON must has "ip" as string</param>
		public static string getIp(string from = "http://worldtimeapi.org/api/timezone/Europe/Moscow")
		{
			string ip = "";

			ip = EasyDo.GetWWWString(from,true);
			try
			{
			if (string.IsNullOrEmpty(ip)) return null;
				RealTimeCallback ipc = JsonUtility.FromJson<RealTimeCallback>(ip);
			ip = ipc.client_ip;
			return ip;
			} catch (Exception ex) { return null; }
		}



		public enum TimeType {sec,min,hour,day,mounth,year,pmam};
	}

	public static class JsonHelper
	{
		public static T[] FromJson<T>(string json)
		{
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
			return wrapper.Items;
		}

		public static string ToJson<T>(T[] array)
		{
			Wrapper<T> wrapper = new Wrapper<T>();
			wrapper.Items = array;
			return JsonUtility.ToJson(wrapper);
		}

		public static string ToJson<T>(T[] array, bool prettyPrint)
		{
			Wrapper<T> wrapper = new Wrapper<T>();
			wrapper.Items = array;
			return JsonUtility.ToJson(wrapper, prettyPrint);
		}

		[Serializable]
		private class Wrapper<T>
		{
			public T[] Items;
		}

		public static void Move<T>(this List<T> list, int i, int j)
		{
			var elem = list[i];
			list.RemoveAt(i);
			list.Insert(j, elem);
		}

		public static List<T> Clone<T>(this List<T> list)
		{
			List<T> ret = new List<T>();

			foreach (T t in list)
				ret.Add(t);

			return ret;
		}

		public static void Swap<T>(this List<T> list, int i, int j)
		{
			var elem1 = list[i];
			var elem2 = list[j];

			list[i] = elem2;
			list[j] = elem1;
		}
	}

	public class SaveSystemAlt : MonoBehaviour
	{
		[SerializeField]
		public static SaveSystemSL ss_sl;
		[SerializeField]
		public static int sindex_sl;
		[SerializeField]
		public static bool useDebug;

		/// <summary>
		/// Use debug in SaveSystemAlt
		/// </summary>
		public static void UseDebug(bool value)
		{
			useDebug = value;
		}

		/// <summary>
		/// Initialize this before you start working with SaveSystemAlt (SSA) in your code
		/// </summary>
		/// <param name="i">Index of SSA</param>
		public static void StartWork(int i=0)
		{
			if (!(ss_sl == null) && useDebug) Debug.LogError("Warning, you haven't closed old save");

			ss_sl = new SaveSystemSL();
			sindex_sl = i;
			if (FilesSet.CheckFile("SaveSystemSL/", "Save" + sindex_sl, "slsave", true))
			{
				if (useDebug) Debug.Log(FilesSet.LoadStream("SaveSystemSL/", "Save" + sindex_sl, "slsave", true, false));
				ss_sl = JsonUtility.FromJson<SaveSystemSL>(FilesSet.LoadStream("SaveSystemSL/", "Save" + sindex_sl, "slsave", true, false));
			}
		}

		/// <summary>
		/// Setting string by name (like PlayerPrefs), need StartWork before initialize
		/// </summary>
		/// <param name="key">Name of saving</param>
		/// <param name="value">Value to save</param>
		public static void SetString(string key, string value)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }

			if (ss_sl.name.Contains(key))
			{
				ss_sl.contain[ss_sl.name.IndexOf(key)] = value;
				if (ss_sl.type[ss_sl.name.IndexOf(key)] != SaveSystemSL.SSLTpe.undefinedS) ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.stringS;
			}
			else
			{
				ss_sl.name.Add(key);
				ss_sl.contain.Add(value);
				ss_sl.type.Add(SaveSystemSL.SSLTpe.stringS);
			}
		}

		/// <summary>
		/// Setting int by name (like PlayerPrefs), need StartWork before initialize
		/// </summary>
		/// <param name="key">Name of saving</param>
		/// <param name="value">Value to save</param>
		public static void SetInt(string key, int value)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }

			if (ss_sl.name.Contains(key))
			{
				ss_sl.contain[ss_sl.name.IndexOf(key)] = value + "";
				if (ss_sl.type[ss_sl.name.IndexOf(key)] != SaveSystemSL.SSLTpe.undefinedS) ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.intS;
			}
			else
			{
				ss_sl.name.Add(key);
				ss_sl.contain.Add(value+"");
				ss_sl.type.Add(SaveSystemSL.SSLTpe.intS);
			}
		}

		/// <summary>
		/// Setting float by name (like PlayerPrefs), need StartWork before initialize
		/// </summary>
		/// <param name="key">Name of saving</param>
		/// <param name="value">Value to save</param>
		public static void SetFloat(string key, float value)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }

			if (ss_sl.name.Contains(key))
			{
				ss_sl.contain[ss_sl.name.IndexOf(key)] = value.ToString();
				if (ss_sl.type[ss_sl.name.IndexOf(key)] != SaveSystemSL.SSLTpe.undefinedS) ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.floatS;
			}
			else
			{
				ss_sl.name.Add(key);
				ss_sl.contain.Add(value.ToString());
				ss_sl.type.Add(SaveSystemSL.SSLTpe.floatS);
			}
		}

		/// <summary>
		/// Setting bool by name, need StartWork before initialize
		/// </summary>
		/// <param name="key">Name of saving</param>
		/// <param name="value">Value to save</param>
		public static void SetBool(string key, bool value)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }

			if (ss_sl.name.Contains(key))
			{
				ss_sl.contain[ss_sl.name.IndexOf(key)] = value.ToString();
				if (ss_sl.type[ss_sl.name.IndexOf(key)] != SaveSystemSL.SSLTpe.undefinedS) ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.boolS;
			}
			else
			{
				ss_sl.name.Add(key);
				ss_sl.contain.Add(value.ToString());
				ss_sl.type.Add(SaveSystemSL.SSLTpe.boolS);
			}
		}

		/// <summary>
		/// Getting string by name (like PlayerPrefs), need StartWork before initialize
		/// </summary>
		/// <param name="key">Name to loading</param>
		/// <param name="def">Default value if name does't exist</param>
		public static string GetString(string key, string def=null,bool fromanytype=false)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return def; }

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.stringS || ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.undefinedS || fromanytype)
				{
					return ss_sl.contain[ss_sl.name.IndexOf(key)];
				}
				else { if (useDebug) Debug.LogError("Type of value not string"); return def; }
			else { if (useDebug) Debug.LogError("There is no such key in SSA file with " + sindex_sl + " index"); return def; }
		}

		/// <summary>
		/// Getting int by name (like PlayerPrefs), need StartWork before initialize
		/// </summary>
		/// <param name="key">Name to loading</param>
		/// <param name="def">Default value if name does't exist</param>
		public static int GetInt(string key, int def = 0)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return def; }

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.intS || ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.undefinedS)
				{
					try { return int.Parse(ss_sl.contain[ss_sl.name.IndexOf(key)]); }
					catch { return def; }
					
				}
				else { if (useDebug) Debug.LogError("Type of value not int"); return def; }
			else { if (useDebug) Debug.LogError("There is no such key in SSA file with " + sindex_sl + " index"); return def; }
		}

		/// <summary>
		/// Getting float by name (like PlayerPrefs), need StartWork before initialize
		/// </summary>
		/// <param name="key">Name to loading</param>
		/// <param name="def">Default value if name does't exist</param>
		public static float GetFloat(string key, float def = 0)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return def; }

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.floatS || ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.intS || ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.undefinedS)
				{
					try { return float.Parse(ss_sl.contain[ss_sl.name.IndexOf(key)]); }
					catch { return def; }

				}
				else { if (useDebug) Debug.LogError("Type of value not float"); return def; }
			else { if (useDebug) Debug.LogError("There is no such key in SSA file with " + sindex_sl+" index"); return def; }
		}

		/// <summary>
		/// Getting bool by name, need StartWork before initialize
		/// </summary>
		/// <param name="key">Name to loading</param>
		/// <param name="def">Default value if name does't exist</param>
		public static bool GetBool(string key, bool def = false)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return def; }

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.boolS || ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.undefinedS)
				{
					try { return bool.Parse(ss_sl.contain[ss_sl.name.IndexOf(key)]); }
					catch { return def; }

				}
				else { if (useDebug) Debug.LogError("Type of value not float"); return def; }
			else { if (useDebug) Debug.LogError("There is no such key in SSA file with " + sindex_sl + " index"); return def; }
		}

		/// <summary>
		/// Transform value type to Undefined, after that you can use this value like any type (except array)
		/// </summary>
		/// <param name="key">Name to get</param>
		public static void SetValueToUndefined(string key)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }
			if (ss_sl.name.Contains(key))
			{
				int index = ss_sl.name.IndexOf(key);
				ss_sl.type[index] = SaveSystemSL.SSLTpe.undefinedS;
			}
			else if (useDebug) Debug.Log("There is no such key in SSA file with " + sindex_sl + " index");
		}

		/// <summary>
		/// Transform value type from Undefined to some type
		/// </summary>
		/// <param name="key">Name to get</param>
		public static void SetValueToSomeType(string key, SaveSystemSL.SSLTpe type)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }
			if (ss_sl.name.Contains(key))
			{
				int index = ss_sl.name.IndexOf(key);
				if (ss_sl.type[index] == SaveSystemSL.SSLTpe.undefinedS)
				{
					bool ok = false;

					try
					{
						if (type ==SaveSystemSL.SSLTpe.boolS)
						{
							ss_sl.contain[index] = bool.Parse(ss_sl.contain[index]).ToString();
							ok = true;
						}
						else if (type == SaveSystemSL.SSLTpe.stringS)
						{
							ok = true;
						}
						else if (type == SaveSystemSL.SSLTpe.intS)
						{
							ss_sl.contain[index] = int.Parse(ss_sl.contain[index]).ToString();
							ok = true;
						}
						else if(type == SaveSystemSL.SSLTpe.floatS)
						{
							ss_sl.contain[index] = float.Parse(ss_sl.contain[index]).ToString();
							ok = true;
						}
					}
					catch { ok = false; }

					if (ok)
					{
						ss_sl.type[index] = type;
					}
					else if (useDebug) Debug.Log("The variable cannot be converted or you have selected an Undefined type");
					
				}
			}
			else if (useDebug) Debug.Log("There is no such key in SSA file with " + sindex_sl + " index");
		}

		/// <summary>
		/// Return true if key exist,  need StartWork before initialize
		/// </summary>
		/// <param name="key">Name to check</param>
		public static bool HasKey(string key)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return false; }
			if (ss_sl.name.Contains(key)) return true;
			else return false;
		}

		/// <summary>
		/// Deleting key by name
		/// </summary>
		/// <param name="key">Name to delete</param>
		public static void DeleteKey(string key)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }
			if (ss_sl.name.Contains(key))
			{
				int index = ss_sl.name.IndexOf(key);
				ss_sl.name.RemoveAt(index);
				ss_sl.contain.RemoveAt(index);
				ss_sl.type.RemoveAt(index);
			}
			else if (useDebug) Debug.Log("There is no such key in SSA file with " + sindex_sl + " index");
		}

		/// <summary>
		/// Delete all kays from SSA file
		/// </summary>
		/// <param name="key">Name to delete</param>
		/// <param name="withFile">Delete file? (if true, don't use SaveSystemAlt.StopWorkAndClose();)</param>
		public static void DeleteAll(string key, bool withFile = false)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }
			if (withFile)
			{
				FilesSet.DelStream("SaveSystemSL/", "Save" + sindex_sl, "slsave", true);
				ss_sl = null;
				sindex_sl = 0;
				Debug.Log("SSA file deleted");
			} else
			{
				ss_sl.name = new List<string>();
				ss_sl.contain = new List<string>();
				ss_sl.type = new List<SaveSystemSL.SSLTpe>();
				Debug.Log("All data deleted, use SaveSystemAlt.StopWorkAndClose(); to save");
			}
		}

		/// <summary>
		/// Return true if you're working with some SSA file
		/// </summary>
		public static bool IsWorking()
		{
			if (ss_sl == null) return false;
			else return true;
		}

		/// <summary>
		/// Return index from SaveSystemAlt.StartWork();
		/// </summary>
		public static int IsIndex()
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first, returning 0"); return 0; }
			else return sindex_sl;
		}

		/// <summary>
		/// Rename key
		/// </summary>
		/// <param name="key">Name to rename</param>
		/// <param name="newName">New name of key</param>
		public static void RenameKey(string key, string newName)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }
			else
			{
				if (ss_sl.name.Contains(key))
				{
					ss_sl.name[ss_sl.name.IndexOf(key)] = newName;
				}
				else if (useDebug) Debug.Log("There is no such key in SSA file with " + sindex_sl + " index");
			}
		}

		/// <summary>
		/// Close file, initialize this if you want saving new data or set new index
		/// Like PlayerPrefs.Save()
		/// </summary>
		public static void StopWorkAndClose()
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("You're not starting the work with SaveSystemAlt"); return; }
			string[] save = new string[1];
			save[0] = JsonUtility.ToJson(ss_sl,true);

			FilesSet.SaveStream("SaveSystemSL/", "Save" + sindex_sl, "slsave", save, true);

			ss_sl = null;
			sindex_sl = 0;
		}

		/// <summary>
		/// Alternative to StopWorkAndClose() in which you do not need to call StartWork() again to Get or Set data
		/// Like PlayerPrefs.Save()
		/// Don't call StartWork() after this function, use first StopWorkAndClose()
		/// </summary>
		public static void SaveUpdatesNotClose()
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("You're not starting the work with SaveSystemAlt"); return; }
			string[] save = new string[1];
			save[0] = JsonUtility.ToJson(ss_sl, true);

			FilesSet.SaveStream("SaveSystemSL/", "Save" + sindex_sl, "slsave", save, true);
		}

		/// <summary>
		/// Don't need SaveSystemAlt.StartWork(); and SaveSystemAlt.StopWorkAndClose();
		/// Saving array with key, use only English
		/// Don't use this on Update or FixedUpdate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">Name to save</param>
		/// <param name="array">Any array to save</param>
		public static void SetArray<T>(string key, T[] array)
		{
			if (array == null) { if (useDebug) Debug.LogError("Array is null"); return; }
			string[] save = new string[1];
			save[0] = JsonHelper.ToJson(array, true);
			FilesSet.SaveStream("SaveSystemSL/Arrays/", "Array_" + key, "slsave", save, true);
		}

		/// <summary>
		/// Don't need SaveSystemAlt.StartWork(); and SaveSystemAlt.StopWorkAndClose();
		/// Loading array with key, use only English
		/// Don't use this on Update or FixedUpdate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">Name to save</param>
		/// <param name="array">Any array to save</param>
		public static T[] GetArray<T>(string key)
		{
			if (FilesSet.CheckFile("SaveSystemSL/Arrays/", "Array_" + key, "slsave", true)) { if (useDebug) Debug.LogError("Key not exist"); return null; }
			else
			{
				return JsonHelper.FromJson<T>(EasyDo.GetWWWString(Application.persistentDataPath + "/SaveSystemSL/Arrays/Array_" + key + ".slsave"));
			}
		}

		/// <summary>
		/// Allows you to get a save cell for further work with it, for example, to transfer to another save index
		/// </summary>
		/// <param name="key">Name to save</param>
		/// <returns></returns>
		public static OutputSSAData GetData(string key)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return null; }

			OutputSSAData ret = null;

			if (HasKey(key))
			{
				ret = new OutputSSAData();
				ret.key = key;
				ret.data = ss_sl.contain[ss_sl.name.IndexOf(key)];
				ret.type = (int) ss_sl.type[ss_sl.name.IndexOf(key)];
			}
			else if (useDebug) Debug.Log("There is no such key in SSA file with " + sindex_sl + " index");

			return ret;
		}

		public static void WriteData(OutputSSAData input)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return; }

			if (input !=null)
			{
				int id = ss_sl.name.IndexOf(input.key);
				
				if (id ==-1)
				{
					ss_sl.name.Add(input.key);
					ss_sl.contain.Add(input.data);
					ss_sl.type.Add((SaveSystemSL.SSLTpe)input.type);
				}
				else
				{
					ss_sl.contain[id] = input.data;
					ss_sl.type[id] = (SaveSystemSL.SSLTpe)input.type;
				}
			}
			else if (useDebug) Debug.Log("You need to put some OutputSSAData to add it!");
		}

	}

	public class RealTimeCallback
	{
		public string utc_datetime = "00:00:00 AM";
		public string client_ip = "0001-01-01";
	}
	public class IpCallback
	{
		public string ip;
	}

	public class SaveSystemSL
	{
		public List<string> name=new List<string>();
		public List<string> contain = new List<string>();
		public List<SSLTpe> type = new List<SSLTpe>();
		public enum SSLTpe { intS, stringS, floatS, boolS, undefinedS };
		
	}

	public class OutputSSAData
	{
		public string key;
		public string data;
		public int type;
	}

	public class JSONFile
	{
		static int pt = -1;//parse at
		static char[] toParse;
		public string value;

		public JSONFile parent = null;
		public Dictionary<string, JSONFile> map;
		public JSONFile this[string ind_]
		{
			get
			{
				if (map.ContainsKey(ind_))
					return map[ind_];
				else
					return new JSONFile();
			}//get
		}
		public bool Has(params string[] path_)
		{
			Dictionary<string, JSONFile> m = map;
			for (int i = 0; i < path_.Length; i++)
			{
				//Debug.Log(path_[i]+":"+(m!=null));
				if (m == null || !m.ContainsKey(path_[i]))
					return false;
				m = m[path_[i]].map;
			}
			return true;
		}

		public string Get(params string[] path_)
		{
			JSONFile m = this;
			for (int i = 0; i < path_.Length; i++)
			{
				//Debug.Log(path_[i]+":"+(m!=null));
				if (!m.map.ContainsKey(path_[i]))
					return null;
				m = m.map[path_[i]];
			}
			return m.value;
		}

		public string GetKeysCount(params string[] path_)
		{
			JSONFile m = this;
			for (int i = 0; i < path_.Length; i++)
			{
				//Debug.Log(path_[i]+":"+(m!=null));
				if (!m.map.ContainsKey(path_[i]))
					return null;
				m = m.map[path_[i]];
			}
			if (m.map == null)
				return null;
			else
				return m.map.Keys.Count + "";
		}

		public List<string> GetKeysNames(params string[] path_)
		{
			JSONFile m = this;
			for (int i = 0; i < path_.Length; i++)
			{
				//Debug.Log(path_[i]+":"+(m!=null));
				if (!m.map.ContainsKey(path_[i]))
					return null;
				m = m.map[path_[i]];
			}
			if (m.map == null)
				return new List<string>();
			else
			{
				List<string> ret = new List<string>();
				foreach (var k in m.map.Keys)
				{
					ret.Add(k);
				}
				return ret;
			}
		}

		public string GenrateJSONTree(string tab = "| ", params string[] path_)
		{
			string ret = "";

			JSONFile m = this;

			string prefix = string.Join("", Enumerable.Repeat(tab, path_.Length));

			foreach (string s in GetKeysNames(path_))
			{
				ret += prefix + s + "\n";
				List<string> newpath = path_.ToList();
				newpath.Add(s);

				ret += GenrateJSONTree(tab, newpath.ToArray());
			}

			return ret;
		}

		public char valueType = 'v';//v:value o:object a:array
		public JSONFile Parse(string txt = "")
		{
			if (!string.IsNullOrEmpty(txt))
			{
				value = txt;
				toParse = txt.ToCharArray();
				pt = -1;
			}
			string key = "";
			while (pt < toParse.Length)
			{
				//STEP 1:look for key 
				switch (toParse[++pt])
				{
					case '{'://initialize object
						valueType = 'o';
						map = new Dictionary<string, JSONFile>();
						continue;
					//break;
					case '['://initialize array
						valueType = 'a';
						map = new Dictionary<string, JSONFile>();
						//TODO: exit this procedure and do array lookup
						break;

					//termination
					//TODO:terminate object
					case '}':
						return this;//break;
										  //TODO:terminate array
					case ']':
						return this;//break;
										  //TODO: next item
					case ',':
						if (valueType == 'o')
						{
							key = "";
							continue;
						}
						break;


					//skip empty
					case ' ': case '\n': case '\t': continue;


					// (")
					case '"':
						key = "";
						//get key name
						while (toParse[++pt] != '"') key += toParse[pt];
						//skip till colon
						while (toParse[++pt] != ':') ;
						break;
					// (")
					case '\'':
						key = "";
						//get key name
						while (toParse[++pt] != '\'') key += toParse[pt];
						//skip till colon
						while (toParse[++pt] != ':') ;
						break;

					//keys that has no (")
					default:
						key = "";
						pt--;
						//Debug.Log("tt:"+toParse[pt]);
						while (toParse[++pt] != ':')
							if (toParse[pt] != ' ' && toParse[pt] != '\n' && toParse[pt] != '\t')
								key += toParse[pt];
						break;
				}
				//Debug.Log("Key>"+key);

				//if no key found yet, keep looking
				if (key == "" && valueType != 'a')
					continue;
				//prepare for array
				if (valueType == 'a')
				{
					if (key == "")
						key = "0";
					else
						key = (int.Parse(key) + 1).ToString();
				}
				//STEP 2: look for value,array,object

				//skip empty area
				do { pt++; }
				while (toParse[pt] == ' ' || toParse[pt] == '\n'
										|| toParse[pt] == '\t');
				//pt--;
				//Debug.Log("value is: "+toParse[pt]);
				switch (toParse[pt])
				{
					case '{':
					case '[':
						pt--;
						map[key] = new JSONFile().Parse();
						break;
					case '"':
						map[key] = new JSONFile();
						map[key].value = "";
						bool ignorenext=false;
						while (toParse[++pt] != '"' || ignorenext)
						{
							ignorenext = false;
							if (toParse[pt] == '\\')
								ignorenext = true;
							map[key].value += toParse[pt];
						}
						break;

					case '\'':
						map[key] = new JSONFile();
						map[key].value = "";
						while (toParse[++pt] != '\'')
							map[key].value += toParse[pt];
						break;

					default:
						map[key] = new JSONFile();
						map[key].value = "";
						pt--;
						while (toParse[++pt] != ' '
							&& toParse[pt] != '\n'
							&& toParse[pt] != '\t'
							&& toParse[pt] != ','
							&& toParse[pt] != ']'
							&& toParse[pt] != '}')
							map[key].value += toParse[pt];
						//
						pt--;//next iteration needs to know it

						continue;//break;
				}
			}

			return this;
		}
	}

}