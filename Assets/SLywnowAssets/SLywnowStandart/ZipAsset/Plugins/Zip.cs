using UnityEngine;
using Ionic.Zip;
using System.IO;
using SLywnow;
using System.Collections.Generic;

public class ZipUtil
{
#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void unzip (string zipFilePath, string location);

	[DllImport("__Internal")]
	private static extern void zip (string zipFilePath);

	[DllImport("__Internal")]
	private static extern void addZipFile (string addFile);

#endif

	public static void Unzip(string zipFilePath, string location)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
		Directory.CreateDirectory(location);

		using (ZipFile zip = ZipFile.Read(zipFilePath))
		{

			zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
		}
#elif UNITY_ANDROID
		using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
			zipper.CallStatic ("unzip", zipFilePath, location);
		}
#elif UNITY_IPHONE
		unzip (zipFilePath, location);
#endif
	}

	public static void Zip(string zipFileName, params string[] files)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
		string path = Path.GetDirectoryName(zipFileName);
		Directory.CreateDirectory(path);

		using (ZipFile zip = new ZipFile())
		{
			foreach (string file in files)
			{
				zip.AddFile(file, "");
			}
			zip.Save(zipFileName);
		}
#elif UNITY_ANDROID
		using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
			{
				zipper.CallStatic ("zip", zipFileName, files);
			}
		}
#elif UNITY_IPHONE
		foreach (string file in files) {
			addZipFile (file);
		}
		zip (zipFileName);
#endif
	}

	public static void ZipInDir(string zipFileName, string directoryForZip, int max =5)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
		string path = Path.GetDirectoryName(zipFileName);
		Directory.CreateDirectory(path);

		List<string> dirs = new List<string>();
		List<List<string>> files = new List<List<string>>();

		GetFilesAndDirs(directoryForZip, ref dirs, ref files, max, 0);

		using (ZipFile zip = new ZipFile())
		{
			for (int i = 0; i < dirs.Count; i++)
			{
				zip.AddDirectoryByName(dirs[i]);

				foreach (string s in files[i])
					zip.AddFile(s, dirs[i]);
			}
			zip.Save(zipFileName);
		}

		//Debug
		/*for (int i=0;i<dirs.Count;i++)
		{
			Debug.Log("-----");
			Debug.Log(dirs[i] + " =>");

			foreach (string s in files[i])
				Debug.Log(s);

		}*/
#endif
	}

	static void GetFilesAndDirs(string dir, ref List<string> dirs, ref List<List<string>> files, int max, int cur, string curdir="/")
	{
		string[] drs = FilesSet.GetFilesFromdirectories(dir, "", false, FilesSet.TypeOfGet.Directory);
		string[] fls = FilesSet.GetFilesFromdirectories(dir, "", false, FilesSet.TypeOfGet.NamesOfFilesWithFormat);

		files.Add(new List<string>());
		dirs.Add(curdir.Replace("\\", ""));

		for (int i = 0; i < fls.Length; i++)
			files[files.Count-1].Add(dir+"/" + fls[i]);

		List<string> ndrs= new List<string>();
		for (int i = 0; i < drs.Length; i++)
			ndrs.Add(drs[i]);

		foreach (string s in ndrs)
		{
			string rd = s.Replace(dir, "");
			if (cur < max)
				GetFilesAndDirs(s, ref dirs, ref files, max, cur++, curdir + rd + "/");
			//dirs[dirs.Count - 1].Add((curdir + rd).Replace("\\", ""));
		}
	}
}
