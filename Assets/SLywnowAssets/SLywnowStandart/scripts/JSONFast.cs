using System.Collections.Generic;
using UnityEngine;
using System;

namespace SLywnow
{
	public class FastJSONTests : MonoBehaviour
	{
		/// <summary>
		/// Getting time or date from url (default jsontest)
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="type">The return type, pmam returns 0 if AM and 1 if PM</param>
		/// <param name="pmam">Whether to use the AM PM system</param>
		/// <param name="from">To specify another url, !WARNING! JSON must has "time" as string, "milliseconds_since_epoch" as int and "date" as string</param>
		public static int getTime(TimeType type, bool pmam = false, string from= "http://date.jsontest.com/")
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
		}

		/// <summary>
		/// Getting time or date from url (default jsontest)
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="type">The return type, pmam returns 0 if AM and 1 if PM</param>
		/// <param name="pmam">Whether to use the AM PM system</param>
		/// <param name="from">To specify another url, !WARNING! JSON must has "time" as string, "milliseconds_since_epoch" as int and "date" as string</param>
		public static DateTime getTime(string from = "http://date.jsontest.com/")
		{
			string getting = EasyDo.GetWWWString(from,true);
			if (string.IsNullOrEmpty(getting)) return DateTime.Today;
			RealTimeCallback rt = new RealTimeCallback();

			try
			{
				rt = JsonUtility.FromJson<RealTimeCallback>(getting);

				return DateTime.Parse(rt.date + " " + rt.time);
			} catch (Exception ex) { return DateTime.Today; }
		}

		/// <summary>
		/// Getting ip from url (default jsontest)
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="from">To specify another url, !WARNING! JSON must has "ip" as string</param>
		public static string getIp(string from = "http://ip.jsontest.com/")
		{
			string ip = "";

			ip = EasyDo.GetWWWString(from,true);
			try
			{
			if (string.IsNullOrEmpty(ip)) return null;
			IpCallback ipc = JsonUtility.FromJson<IpCallback>(ip);
			ip = ipc.ip;
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
				ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.stringS;
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
				ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.intS;
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
				ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.floatS;
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
				ss_sl.type[ss_sl.name.IndexOf(key)] = SaveSystemSL.SSLTpe.boolS;
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
		public static string GetString(string key, string def=null)
		{
			if (ss_sl == null) { if (useDebug) Debug.LogError("Use SaveSystemAlt.StartWork(); first"); return def; }

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.stringS)
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

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.intS)
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

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.floatS)
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

			if (ss_sl.name.Contains(key)) if (ss_sl.type[ss_sl.name.IndexOf(key)] == SaveSystemSL.SSLTpe.boolS)
				{
					try { return bool.Parse(ss_sl.contain[ss_sl.name.IndexOf(key)]); }
					catch { return def; }

				}
				else { if (useDebug) Debug.LogError("Type of value not float"); return def; }
			else { if (useDebug) Debug.LogError("There is no such key in SSA file with " + sindex_sl + " index"); return def; }
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

	}

	public class RealTimeCallback
	{
		public string time="00:00:00 AM";
		public int milliseconds_since_epoch=0;
		public string date= "0001-01-01";
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
		public enum SSLTpe { intS, stringS, floatS, boolS };
		
	}
}