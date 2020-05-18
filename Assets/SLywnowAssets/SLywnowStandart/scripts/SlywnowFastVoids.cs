using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace SLywnow
{
	public class FilesSet : MonoBehaviour
	{
		/*[DllImport("user32.dll")]
		private static extern int GetActiveWindow();

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(int hWnd, int nIndex, uint dwNewLong);

		[DllImport("user32.dll")]
		static extern bool ShowWindowAsync(int hWnd, int nCmdShow);

		[DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
		static extern int SetLayeredWindowAttributes(int hwnd, int crKey, byte bAlpha, int dwFlags);

		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		private static extern int SetWindowPos(int hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

		[DllImport("Dwmapi.dll")]
		private static extern uint DwmExtendFrameIntoClientArea(int hWnd, ref MARGINS margins);

		bool hide;
		int fWidth = Screen.width;
		int fHeight = Screen.height;

		const uint WS_POPUP = 0x80000000;
		const uint WS_VISIBLE = 0x10000000;
		const uint WS_CAPTION = 0x00C0000;
		const uint WS_OVERLAPPED = 0x00000000;
		const uint WS_SYSMENU = 0x00080000;
		const uint WS_THICKFRAME = 0x00040000;
		const uint WS_MINIMIZEBOX = 0x00020000;
		const uint WS_MAXIMIZEBOX = 0x00010000;
		const int GWL_STYLE = -16;

		/// <summary>
		/// Do the window invisibling (Work only on Windows Standart build)
		/// </summary>
		public static void DoWinInvisible ()
		{
//#if !UNITY_EDITOR 
#if UNITY_STANDALONE_WIN
			var margins = new MARGINS() { cxLeftWidth = -1 };
			var hwnd = GetActiveWindow();
			SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
			SetLayeredWindowAttributes(hwnd, 0, 0, 2);
			DwmExtendFrameIntoClientArea(hwnd, ref margins);
#endif 
//#endif
		}


		/// <summary>
		/// Do the window visibling (Work only on Windows Standart build)
		/// </summary>
		public static void DoWinVisible()
		{
//#if !UNITY_EDITOR
#if UNITY_STANDALONE_WIN
			var margins = new MARGINS() { cxLeftWidth = -1 };
			var hwnd = GetActiveWindow();
			SetWindowLong(hwnd, GWL_STYLE, (WS_OVERLAPPED | WS_CAPTION / WS_SYSMENU / WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX) | WS_VISIBLE);
			SetLayeredWindowAttributes(hwnd, 0, 255, 2);
			DwmExtendFrameIntoClientArea(hwnd, ref margins);
#endif
//#endif
		}

		/// <summary> 
		/// Sets the window alpha (Work only on Windows Standart build)
		/// </summary>
		public static void DoWinAlpha(byte alpha)
		{
#if !UNITY_EDITOR
#if UNITY_STANDALONE_WIN
			var margins = new MARGINS() { cxLeftWidth = -1 };
			var hwnd = GetActiveWindow();
			SetWindowLong(hwnd, GWL_STYLE, WS_CAPTION | WS_OVERLAPPED | WS_VISIBLE);
			SetLayeredWindowAttributes(hwnd, 0, alpha, 2);
			DwmExtendFrameIntoClientArea(hwnd, ref margins);
#endif
#endif
		}*/

		/// <summary>
		/// Save some strings to path
		/// </summary>
		/// <param name="path">Path to save ("/" automaticly added)</param>
		/// <param name="name">Name of file</param>
		/// <param name="format">Format of file</param>
		/// <param name="saves">Strings to save</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <param name="add">dont rewrite file</param>
		public static void SaveStream(string path, string name, string format, string[] saves, bool datapath = false, bool add = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link + "/" + name + "." + format);
			if (!fi.Directory.Exists) Directory.CreateDirectory(link);
			StreamWriter sw = new StreamWriter(link + "/" + name + "." + format);
			if (!fi.Exists) fi.Create();
			if (saves.Length > 0)
			{
				if (add) sw = fi.AppendText();
				for (int i = 0; i < saves.Length; i++) sw.WriteLine(saves[i]);
			}
			sw.Close();
		}

		/// <summary>
		/// Save some strings to path
		/// </summary>
		/// <param name="path">Path to save</param>
		/// <param name="saves">Strings to save</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <param name="add">dont rewrite file</param>
		public static void SaveStream(string path, string[] saves, bool datapath = false, bool add = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link);
			if (!fi.Directory.Exists) Directory.CreateDirectory(link);
			StreamWriter sw = new StreamWriter(link);
			if (!fi.Exists) fi.Create();
			if (saves.Length > 0)
			{
				if (add) sw = fi.AppendText();
				for (int i = 0; i < saves.Length; i++) sw.WriteLine(saves[i]);
			}
			sw.Close();
		}

		/// <summary>
		/// Save some string to path
		/// </summary>
		/// <param name="path">Path to save ("/" automaticly added)</param>
		/// <param name="name">Name of file</param>
		/// <param name="format">Format of file</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <param name="add">dont rewrite file</param>
		public static void SaveStream(string path, string name, string format, string save, bool datapath = false, bool add = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link + "/" + name + "." + format);
			if (!fi.Directory.Exists) Directory.CreateDirectory(link);
			StreamWriter sw = new StreamWriter(link + "/" + name + "." + format);
			if (!fi.Exists) fi.Create();
			if (!string.IsNullOrEmpty(save))
			{
				if (add) sw = fi.AppendText();
				sw.WriteLine(save);
			}
			sw.Close();
		}

		/// <summary>
		/// Save some string to path
		/// </summary>
		/// <param name="path">Path to save</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <param name="add">dont rewrite file</param>
		public static void SaveStream(string path, string save, bool datapath = false, bool add = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link);
			if (!fi.Directory.Exists) Directory.CreateDirectory(link);
			StreamWriter sw = new StreamWriter(link);
			if (!fi.Exists) fi.Create();
			if (!string.IsNullOrEmpty(save))
			{
				if (add) sw = fi.AppendText();
				sw.WriteLine(save);
			}
			sw.Close();
		}

		/// <summary>
		/// Loading some strings from path
		/// </summary>
		/// <param name="path">Path to load ("/" automaticly added)</param>
		/// <param name="name">Name of file</param>
		/// <param name="format">Format of file</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <returns>Returns all lines from file</returns>
		public static string[] LoadStream(string path, string name, string format, bool datapath = false)
		{
			string[] ret = new string[0];
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link + "/" + name + "." + format);
			if (!fi.Directory.Exists) return null;
			StreamReader sr = new StreamReader(link + "/" + name + "." + format);
			List<string> lt = new List<string>();
			if (fi.Exists)
			{
				while (!sr.EndOfStream) lt.Add(sr.ReadLine());
				sr.Close();
				ret = lt.ToArray();
			}
			else return null;
			return ret;
		}

		/// <summary>
		/// Loading some strings from path
		/// </summary>
		/// <param name="path">Path to load</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <returns>Returns all lines from file</returns>
		public static string[] LoadStream(string path, bool datapath = false)
		{
			string[] ret = new string[0];
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link);
			if (!fi.Directory.Exists) return null;
			StreamReader sr = new StreamReader(link);
			List<string> lt = new List<string>();
			if (fi.Exists)
			{
				while (!sr.EndOfStream) lt.Add(sr.ReadLine());
				sr.Close();
				ret = lt.ToArray();
			}
			else return null;
			return ret;
		}

		/// <summary>
		/// Loading some string from path
		/// </summary>
		/// <param name="path">Path to load ("/" automaticly added)</param>
		/// <param name="name">Name of file</param>
		/// <param name="format">Format of file</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <returns>Returns all lines from file</returns>
		public static string LoadStream(string path, string name, string format, bool datapath = false, bool onlyoneline = false)
		{
			string ret = null;
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link + "/" + name + "." + format);
			if (!fi.Directory.Exists) return null;
			StreamReader sr = new StreamReader(link + "/" + name + "." + format);
			if (fi.Exists)
			{
				ret = "";
				if (!onlyoneline) while (!sr.EndOfStream) ret+= sr.ReadLine();
				else ret = sr.ReadLine();
				sr.Close();
			}
			else return null;
			return ret;
		}

		/// <summary>
		/// Loading some string from path
		/// </summary>
		/// <param name="path">Path to load</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <returns>Returns all lines from file</returns>
		public static string LoadStream(string path, bool datapath = false, bool onlyoneline = false)
		{
			string ret = null;
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link);
			if (!fi.Directory.Exists) return null;
			StreamReader sr = new StreamReader(link);
			if (fi.Exists)
			{
				ret = "";
				if (!onlyoneline) while (!sr.EndOfStream) ret += sr.ReadLine();
				else ret = sr.ReadLine();
				sr.Close();
			}
			else return null;
			return ret;
		}

		/// <summary>
		/// Checking file in path
		/// </summary>
		/// <param name="path">Path to file ("/" automaticly added)</param>
		/// <param name="name">Name of file</param>
		/// <param name="format">Format of file</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		public static bool CheckFile(string path, string name, string format, bool datapath = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link + "/" + name + "." + format);
			if (fi.Exists) return true;
			return false;
		}

		/// <summary>
		/// Checking file in path
		/// </summary>
		/// <param name="path">Path to file</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		public static bool CheckFile(string path, bool datapath = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link);
			if (fi.Exists) return true;
			return false;
		}

		/// <summary>
		/// Delete file from path
		/// </summary>
		/// <param name="path">Path to file ("/" automaticly added)</param>
		/// <param name="name">Name of file</param>
		/// <param name="format">Format of file</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <param name="dirtoo">Removing directory too</param>
		/// <param name="forse">Removing directory when that has another files</param>
		public static void DelStream(string path, string name, string format, bool datapath = false, bool dirtoo = false, bool forse = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link + "/" + name + "." + format);
			if (fi.Exists && fi.Directory.Exists) { fi.Delete(); if (dirtoo) fi.Directory.Delete(forse); }
		}

		/// <summary>
		/// Delete file from path
		/// </summary>
		/// <param name="path">Path to file</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <param name="dirtoo">Removing directory too</param>
		/// <param name="forse">Removing directory when that has another files</param>
		public static void DelStream(string path, bool datapath = false, bool dirtoo = false, bool forse = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			FileInfo fi = new FileInfo(link);
			if (fi.Exists && fi.Directory.Exists) { fi.Delete(); if (dirtoo) fi.Directory.Delete(forse); }
		}

		/// <summary>
		/// Checking directory by path
		/// </summary>
		/// <param name="path">Path to directory ("/" automaticly added)</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		public static bool CheckDirectory(string path, bool datapath = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			if (Directory.Exists(link)) return true;
			return false;
		}

		/// <summary>
		/// Gets all paths from directory
		/// WARNING! DONT USE THIS IN Update OR FixedUpdate VOIDS
		/// </summary>
		/// <param name="path">Path to directory ("/" automaticly added)</param>
		/// <param name="format">Format of files, "" for all</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		/// <param name="type">Type of search, all, only directories, only files</param>
		public static string[] GetFilesFromdirectories(string path, string format, bool datapath = false, TypeOfGet type = TypeOfGet.Files)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			if (!Directory.Exists(link)) return null;

			string[] str = new string[0];
			string[] dir = new string[0];
			string[] fil = new string[0];
			if (!(type == TypeOfGet.Files)) dir = Directory.GetDirectories(link);
			if (!(type == TypeOfGet.Directory)) {
				if (!string.IsNullOrEmpty(format)) fil = Directory.GetFiles(link, "*." + format);
				else fil = Directory.GetFiles(link);
			}

			if (type == TypeOfGet.Files) str = fil;
			if (type == TypeOfGet.Directory) str = dir;
			if (type == TypeOfGet.All) str = ConcatArray(dir,fil);
			if (type == TypeOfGet.NamesOfFiles)
			{
				for (int i = 0; i < fil.Length; i++)
				{
					fil[i] = fil[i].Replace(link + "\\", "");
					if (!string.IsNullOrEmpty(format))
						fil[i] = fil[i].Replace("."+format, "");
					else
					{
						for (int c = fil[i].Length; c >= 0; c--)
						{
							if (!(fil[i][c] == '.'))
								fil[i] = fil[i].Remove(c);
							else
							{ fil[i] = fil[i].Remove(c); break; }
						}
					}
				}
				str = fil;
			}
			if (type == TypeOfGet.Formats)
			{
				for (int i = 0; i < fil.Length; i++)
				{
					fil[i] = fil[i].Replace(link + "\\", "");
					for (int c = fil[i].Length; c >= 0; c--)
					{
						if (!(fil[i][c] == '.'))
							fil[i] = fil[i].Remove(c);
						else
						{ fil[i] = fil[i].Remove(c); break; }
					}
				}
				str = fil;
			}
			if (type == TypeOfGet.NamesOfFilesWithFormat)
			{
				for (int i = 0; i < fil.Length; i++)
				{
					fil[i] = fil[i].Replace(link + "\\", "");
				}
				str = fil;
			}
			if (type == TypeOfGet.NamesOfDirectories)
			{
				for (int i = 0; i < dir.Length; i++)
				{
					dir[i] = dir[i].Replace(link + "\\", "");
				}
				str = dir;
			}

			return str;
		}

		public enum TypeOfGet { All, Files, Directory, NamesOfFiles, NamesOfFilesWithFormat, NamesOfDirectories, Formats };

		/// <summary>
		/// Contact 2 arrays
		public static string[] ConcatArray(string[] array1, string[] array2)
		{
			if (array1 == null) throw new ArgumentNullException("array1");
			if (array2 == null) throw new ArgumentNullException("array2");
			int oldLen = array1.Length;
			Array.Resize<string> (ref array1, array1.Length + array2.Length);
			Array.Copy(array2, 0, array1, oldLen, array2.Length);
			return array1;
		}

		/// <summary>
		/// Create directory by path
		/// </summary>
		/// <param name="path">Path to add</param>
		/// <param name="datapath">Added Application.persistentDataPath to path ("/" automaticly added)</param>
		public static void CreateDirectory(string path, bool datapath = false)
		{
			string link = path;
			if (datapath) link = Application.persistentDataPath + "/" + path;
			Directory.CreateDirectory(link);
		}

	private struct MARGINS
		{
			public int cxLeftWidth;
			public int cxRightWidth;
			public int cyTopHeight;
			public int cyBottomHeight;
		}
	}

	public class FastFind : MonoBehaviour
	{
		/// <summary>
		/// Finding object by cords
		/// WARNING! DONT USE THIS IN Update OR FixedUpdate VOIDS
		/// </summary>
		/// <param name="position">Cords of object</param>
		/// <param name="first">Return a first object in this cords</param>
		/// <param name="tag">Checking a tag, set "" to use blocktags, default null</param>
		/// <param name="blocktags">Blocking this tags for search, deault null</param>
		public static GameObject InCords(Vector3 position, bool first, string tag = null, string[] blocktags = null)
		{
			GameObject ret = null;
			GameObject[] objs = FindObjectsOfType<GameObject>() as GameObject[];

			for (int i = 0; i < objs.Length; i++) if (objs[i].transform.position == position)
				{
					if (tag != null) if (objs[i].tag == tag) if (first) return objs[i]; else { ret = objs[i]; continue; }
					if (blocktags != null) for (int a = 0; a < blocktags.Length; a++) if (objs[i].tag == blocktags[a]) continue;
					ret = objs[i];
					if (first) break;
				}
			return ret;
		}

		/// <summary>
		/// Finding all objects by cords 
		/// WARNING! DONT USE THIS IN Update OR FixedUpdate VOIDS
		/// </summary>
		/// <param name="position">Cords of objects</param>
		/// <param name="tag">Checking a tag, set "" to use blocktags, default null</param>
		/// <param name="blocktags">Blocking this tags for search, deault null</param>
		public static GameObject[] AllInCords(Vector3 position, string tag = null, string[] blocktags = null)
		{
			List<GameObject> ret = null;
			GameObject[] objs = FindObjectsOfType<GameObject>() as GameObject[];

			for (int i = 0; i < objs.Length; i++) if (objs[i].transform.position == position)
				{
					if (tag != null) if (objs[i].tag == tag) { ret.Add(objs[i]); continue; }
					if (blocktags != null) for (int a = 0; a < blocktags.Length; a++) if (objs[i].tag == blocktags[a]) continue;
					ret.Add(objs[i]);

				}
			GameObject[] rets = ret.ToArray();
			return rets;
		}

		/*/// <summary>
		/// Finding object by collider in cords 
		/// WARNING! DONT USE THIS IN Update OR FixedUpdate 
		/// </summary>
		/// <param name="position">Cords of collider</param>
		/// <param name="dir">Direction to check, use Vector3.zero for sphere search</param>
		public static GameObject CollinderInCords(Vector3 position, Vector3 dir, float radius = 0.1f)
		{
			RaycastHit hit;

			if (dir == Vector3.zero)
			{
				if (Physics.Raycast(position, Vector3.forward, out hit, radius))
				{
					return hit.collider.gameObject;
				}
				else if (Physics.Raycast(position, Vector3.back, out hit, radius))
				{
					return hit.collider.gameObject;
				}
				else if (Physics.Raycast(position, Vector3.up, out hit, radius))
				{
					return hit.collider.gameObject;
				}
				else if (Physics.Raycast(position, Vector3.down, out hit, radius))
				{
					return hit.collider.gameObject;
				}
				else if (Physics.Raycast(position, Vector3.left, out hit, radius))
				{
					return hit.collider.gameObject;
				}
				else if (Physics.Raycast(position, Vector3.right, out hit, radius))
				{
					return hit.collider.gameObject;
				}
			} else
			{
				if (Physics.Raycast(position, dir, out hit, radius))
				{
					return hit.collider.gameObject;
				}
			}

			return null;
		}

		/// <summary>
		/// Finding object by collider in cords 
		/// WARNING! DONT USE THIS IN Update OR FixedUpdate 
		/// </summary>
		/// <param name="position">Cords of collider</param>
		/// <param name="dir">Direction to check, use Vector3.zero for sphere search</param>
		public static GameObject[] AllCollindersInCords(Vector3 position, Vector3 dir, float radius = 0.1f)
		{
			RaycastHit hit;
			List<GameObject> gms = new List<GameObject>();

			if (dir == Vector3.zero)
			{
				if (Physics.Raycast(position, Vector3.forward, out hit, radius))
				{
					if (!gms.Contains(hit.collider.gameObject))
						gms.Add(hit.collider.gameObject);
				}
				else if (Physics.Raycast(position, Vector3.back, out hit, radius))
				{
					if (!gms.Contains(hit.collider.gameObject))
						gms.Add(hit.collider.gameObject);
				}
				else if (Physics.Raycast(position, Vector3.up, out hit, radius))
				{
					if (!gms.Contains(hit.collider.gameObject))
						gms.Add(hit.collider.gameObject);
				}
				else if (Physics.Raycast(position, Vector3.down, out hit, radius))
				{
					if (!gms.Contains(hit.collider.gameObject))
						gms.Add(hit.collider.gameObject);
				}
				else if (Physics.Raycast(position, Vector3.left, out hit, radius))
				{
					if (!gms.Contains(hit.collider.gameObject))
						gms.Add(hit.collider.gameObject);
				}
				else if (Physics.Raycast(position, Vector3.right, out hit, radius))
				{
					if (!gms.Contains(hit.collider.gameObject))
						gms.Add(hit.collider.gameObject);
				}
			}
			else
			{
				if (Physics.Raycast(position, dir, out hit, radius))
				{
					gms.Add(hit.collider.gameObject);
				}
			}

			return gms.ToArray();
		} */

		/// <summary>
		/// Return default path 
		/// /sdcard on android
		/// Program folder on PC
		/// </summary>
		public static string GetDefaultPath()
		{
			string path = "";
#if UNITY_ANDROID && !UNITY_EDITOR
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Environment"))
						{
							path = androidJavaClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
						}
#endif
#if !UNITY_ANDROID || UNITY_EDITOR
			path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#endif
			return path;
		}

		/// <summary>
		/// Looking for a child in the whole hierarchy under the parent
		/// </summary>
		/// <param name="parent">Transform where will the search begin</param>
		/// <param name="name">Name of child</param>
		public static Transform FindChild(Transform parent, string name)
		{
			foreach (Transform child in parent)
			{
				if (child.name == name)
				{
					return child;
				}
				else
				{
					var result = FindChild(child, name);
					if (result != null)
					{
						return result;
					}
				}
			}
			return null;
		}
	}

	public class EasyDo : MonoBehaviour
	{
		public static List<List<T>> RemoveIndex<T>(List<List<T>> tomove, List<int> indexs, int index, int newplace)
		{
			int i = indexs.IndexOf(index);
			foreach (List<T> a in tomove)
			{
				a.RemoveAt(i);
			}
			indexs.RemoveAt(i);
			return tomove;
		}

		public static List<List<T>> MoveIndex<T>(List<List<T>> tomove, List<int> indexs, int index, int newplace)
		{
			int i = indexs.IndexOf(index);
			foreach (List<T> a in tomove) {
				a.Move(i, newplace);
			}
			indexs.Move(i, newplace);
			return tomove;
		}

		public static void CreateIndex(out List<int> output, List<int> input, int min = 100000, int max = 999999)
		{
			output = input;
			output.Add(Random.Range(min, max));
		}

		/// <summary>
		/// Placed all lines to string Array
		/// Don't use it on Update or FixedUpdate
		/// </summary>
		/// <param name="input">Input string from InputField</param>
		/// <param name="enter">The symbol of the transition to a new line, default \n</param>
		public static string[] UIMultiLineToStringArray(string input, string enter = "\n")
		{
			string inside = input.Replace(enter, "■");
			List<string> ls = new List<string>();
			input = "";
			for (int i = 0; i < inside.Length; i++)
			{
				if (inside[i] == '■')
				{
					ls.Add(input);
					input = "";
				}
				else input += inside[i];
			}
			ls.Add(input);
			return ls.ToArray();
		}

		/// <summary>
		/// Placed string Array to multiline text
		/// Don't use it on Update or FixedUpdate
		/// </summary>
		/// <param name="input">Input string from InputField</param>
		/// <param name="enter">The symbol of the transition to a new line, default \n</param>
		public static string StringArrayToUIMultiLine(string[] input, string enter = "\n")
		{
			string output = "";
			for (int i = 0; i < input.Length; i++)
			{
				output += input[i] + enter;
			}
			return output;
		}

		/// <summary>
		/// Smoothly changes color
		/// Use on Update or FixedUpdate
		/// </summary>
		/// <param name="from">Color to change</param>
		/// <param name="to">Finish color</param>
		/// <param name="speed">Speed of change</param>
		/// <param name="r">Red intensity of change</param>
		/// <param name="g">Green intensity of change</param>
		/// <param name="b">Blue intensity of change</param>
		public static Color MoveToColor(Color from, Color to, float speed, byte r = 255, byte g = 255, byte b = 255)
		{

			Color col = from;

			if (col.r < to.r && !(col.r > to.r + speed * (r / 255))) col.r += speed * (r / 255);
			else if (col.r > to.r && !(col.r < to.r - speed * (r / 255))) col.r -= speed * (r / 255);
			else col.r = to.r;

			if (col.g < to.g && !(col.g > to.g + speed * (g / 255))) col.g += speed * (g / 255);
			else if (col.g > to.g && !(col.g < to.g - speed * (g / 255))) col.g -= speed * (g / 255);
			else col.g = to.g;

			if (col.b < to.b && !(col.b > to.b + speed * (b / 255))) col.b += speed * (b / 255);
			else if (col.b > to.b && !(col.b < to.b - speed * (b / 255))) col.b -= speed * (b / 255);
			else col.b = to.b;

			return col;
		}

		/// <summary>
		/// Smoothly changes color with alpha
		/// Use on Update or FixedUpdate
		/// </summary>
		/// <param name="from">Color to change</param>
		/// <param name="to">Finish color</param>
		/// <param name="speed">Speed of change</param>
		/// <param name="r">Red intensity of change</param>
		/// <param name="g">Green intensity of change</param>
		/// <param name="b">Blue intensity of change</param>
		/// <param name="a">Alpha intensity of change</param>
		public static Color MoveToColorWithAlpha(Color from, Color to, float speed, byte r = 255, byte g = 255, byte b = 255, byte a = 255)
		{

			Color col = from;

			if (col.r < to.r && !(col.r > to.r + speed * (r / 255))) col.r += speed * (r / 255);
			else if (col.r > to.r && !(col.r < to.r - speed * (r / 255))) col.r -= speed * (r / 255);
			else col.r = to.r;

			if (col.g < to.g && !(col.g > to.g + speed * (g / 255))) col.g += speed * (g / 255);
			else if (col.g > to.g && !(col.g < to.g - speed * (g / 255))) col.g -= speed * (g / 255);
			else col.g = to.g;

			if (col.b < to.b && !(col.b > to.b + speed * (b / 255))) col.b += speed * (b / 255);
			else if (col.b > to.b && !(col.b < to.b - speed * (b / 255))) col.b -= speed * (b / 255);
			else col.b = to.b;

			if (col.a < to.a && !(col.a > to.a + speed * (a / 255))) col.a += speed * (a / 255);
			else if (col.a > to.a && !(col.a < to.a - speed * (a / 255))) col.a -= speed * (a / 255);
			else col.a = to.a;

			return col;
		}

		/// <summary>
		/// Returns Color between from and to in position
		/// Use on Update or FixedUpdate
		/// </summary>
		/// <param name="from">Color to change</param>
		/// <param name="to">Finish color</param>
		/// <param name="position">Position 0 to 1 only</param>
		public static Color SetPositionColor(Color from, Color to, float position)
		{
			Color ret=from;
			Color col;
			col.r = ((from.r < to.r ? to.r : from.r) - (from.r < to.r ? from.r : to.r)) * position;
			col.g = ((from.g < to.g ? to.g : from.g) - (from.g < to.g ? from.g : to.g)) * position;
			col.b = ((from.b < to.b ? to.b : from.b) - (from.b < to.b ? from.b : to.b)) * position;
			col.a = ((from.a < to.a ? to.a : from.a) - (from.a < to.a ? from.a : to.a)) * position;

			ret.r = (from.r < to.r ? from.r + col.r : from.r - col.r);
			ret.g = (from.g < to.g ? from.g + col.g : from.g - col.g);
			ret.b = (from.b < to.b ? from.b + col.b : from.b - col.b);
			ret.a = (from.a < to.a ? from.a + col.a : from.a - col.a);

			return ret;
		}

		/// <summary>
		/// Getting string from url
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="url">url to string</param>
		/// <param name="maxtime">Maximum time to wait</param>
		/// <returns>Returns string from url</returns>
		public static Texture GetWWWTexture2D(string url, bool unlimited = false, float maxtime = 1000f)
		{
			
			WWW www = new WWW(url);
			float time = 0;
			while (!www.isDone)
			{
				if (!string.IsNullOrEmpty(www.error)) return null;
				if (!unlimited)
				{
					time += Time.deltaTime;
					if (time > maxtime) return null;
				}
				else continue;
			}
			return www.texture;
		}

		/// <summary>
		/// Getting audio from url
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="url">url to audio</param>
		/// <param name="maxtime">Maximum time to wait</param>
		/// <returns>Returns audio from url</returns>
		public static AudioClip GetWWWAudio(string url, bool unlimited = false, float maxtime = 1000f)
		{
			WWW www = new WWW(url);
			float time = 0;
			while (!www.isDone)
			{
				if (!string.IsNullOrEmpty(www.error)) return null;
				if (!unlimited)
				{
					time += Time.deltaTime;
					if (time > maxtime) return null;
				}
				else continue;
			}
			return www.GetAudioClip();
		}

		/// <summary>
		/// Getting texture2D from url
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="url">url to texture2D</param>
		/// <param name="maxtime">Maximum time to wait</param>
		/// <returns>Returns texture2D from url</returns>
		public static string GetWWWString(string url, bool unlimited = false, float maxtime=1000f)
		{
			WWW www = new WWW(url);
			float time=0;
			while (!www.isDone) {
				if (!string.IsNullOrEmpty(www.error)) return null;
				if (!unlimited)
				{
					time += Time.deltaTime;
					if (time > maxtime) return null;
				}
				else continue;
			}
			return www.text;
		}

		/// <summary>
		/// Getting bytes from url
		/// Dont use on Update or FixedUpdate
		/// </summary>
		/// <param name="url">url to site</param>
		/// <param name="maxtime">Maximum time to wait</param>
		/// <returns>Returns arrays byte from url</returns>
		public static byte[] GetWWWBytes(string url, bool unlimited = false, float maxtime = 1000f)
		{
			WWW www = new WWW(url);
			float time = 0;
			while (!www.isDone)
			{
				if (!string.IsNullOrEmpty(www.error)) return null;
				if (!unlimited)
				{
					time += Time.deltaTime;
					if (time > maxtime) return null;
				}
				else continue;
			}
			return www.bytes;
		}

		/// <summary>
		/// Not works now
		/// </summary>
		public static T[] JSONtoArray<T>(string json)
		{
			string newJson = "{ \"array\": " + json + "}";
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
			return wrapper.array;
		}

		[System.Serializable]
		private class Wrapper<T>
		{
			public T[] array;
		}

	}
}
