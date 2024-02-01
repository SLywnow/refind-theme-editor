using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SLywnow;
using AutoLangSLywnow;
using UnityEngine.UI.Extensions;

namespace AutoLangEditorSLywnow
{
	[InitializeOnLoad]
	public class ALSL_Editor_OnStartProject
	{

		static ALSL_Editor_OnStartProject()
		{
			Run();
		}

		public static void Run()
		{
			if ((AssetDatabase.LoadAssetAtPath(ALSL_Editor_System.folderreslink + "/alsl_perfab.prefab", typeof(GameObject)) as GameObject) != null)
			{
				ALSL_Editor_System.perfab = (AssetDatabase.LoadAssetAtPath(ALSL_Editor_System.folderreslink + "/alsl_perfab.prefab", typeof(GameObject)) as GameObject).GetComponent<ALSL_Perfab>();
				ALSL_Editor_System.enable = true;
			}
			else
			{
				ALSL_Editor_System.enable = false;
			}

			if (ALSL_Editor_System.enable)
			{
				ALSL_ToSaveJSON dw = new ALSL_ToSaveJSON();
				ALSL_Params pw = new ALSL_Params();
				if (FilesSet.CheckFile(ALSL_Editor_System.folderlink, "keys", "txt", false))
					dw = JsonUtility.FromJson<ALSL_ToSaveJSON>(FilesSet.LoadStream(ALSL_Editor_System.folderlink, "keys", "txt", false, false));
				else
					FilesSet.SaveStream(ALSL_Editor_System.folderlink, "keys", "txt", new string[0], false);

				if (FilesSet.CheckFile(ALSL_Editor_System.folderlink, "params", "txt", false))
					pw = JsonUtility.FromJson<ALSL_Params>(FilesSet.LoadStream(ALSL_Editor_System.folderlink, "params", "txt", false, false));
				else
					FilesSet.SaveStream(ALSL_Editor_System.folderlink, "params", "txt", JsonUtility.ToJson(pw, true), false);

				if (ALSL_Editor_System.perfab.keys == null)
					ALSL_Editor_System.perfab.keys = AssetDatabase.LoadAssetAtPath(ALSL_Editor_System.folderlinkA + "/keys.txt", typeof(TextAsset)) as TextAsset;
				if (ALSL_Editor_System.perfab.options == null)
					ALSL_Editor_System.perfab.options = AssetDatabase.LoadAssetAtPath(ALSL_Editor_System.folderlinkA + "/params.txt", typeof(TextAsset)) as TextAsset;

				ALSL_Editor_System.alllangs = dw.alllangs;
				ALSL_Editor_System.keysR_alsl = dw.keysR_alsl;
				ALSL_Editor_System.keys_alsl = dw.keys_alsl;
				ALSL_Editor_System.repickR_alsl = dw.repickR_alsl;
				ALSL_Editor_System.langsvis = dw.langsvis;
				ALSL_Editor_System.deflang = dw.deflang;
				ALSL_Editor_System.assotiate = dw.assotiate;

				ALSL_Editor_System.options = pw;

				if (ALSL_Editor_System.perfab.langFiles == null)
					ALSL_Editor_System.perfab.langFiles = new List<ALSL_Perfab_Lang>();

				ALSL_Editor_System.allwords = new List<AutoLangSLywnow.ALSL_Language>();
				for (int i = 0; i < ALSL_Editor_System.alllangs.Count; i++)
				{
					ALSL_Editor_System.allwords.Add(new AutoLangSLywnow.ALSL_Language());
					if (FilesSet.CheckFile(ALSL_Editor_System.folderlanglink, ALSL_Editor_System.alllangs[i], "json", false))
					{
						string[] loadjson = FilesSet.LoadStream(ALSL_Editor_System.folderlanglink, ALSL_Editor_System.alllangs[i], "json", false);
						for (int a = 0; a < ALSL_Editor_System.keys_alsl.Count; a++)
						{
							string getword = loadjson[a + 1].Replace("\"" + ALSL_Editor_System.keys_alsl[a] + "\": \"", "");
							getword = getword.Replace("\"", "");
							ALSL_Editor_System.allwords[i].words.Add(getword.Replace("▬", "\n"));
						}

						if (ALSL_Editor_System.perfab.langFiles.Find(f => f.name == ALSL_Editor_System.alllangs[i]) == null)
						{
							ALSL_Editor_System.perfab.langFiles.Add(new ALSL_Perfab_Lang());
							ALSL_Editor_System.perfab.langFiles[ALSL_Editor_System.perfab.langFiles.Count - 1].name = ALSL_Editor_System.alllangs[i];
							ALSL_Editor_System.perfab.langFiles[ALSL_Editor_System.perfab.langFiles.Count - 1].asset = AssetDatabase.LoadAssetAtPath(ALSL_Editor_System.folderlanglinkA + "/" + ALSL_Editor_System.alllangs[i] + ".json", typeof(TextAsset)) as TextAsset;
						}
					}
					else Debug.LogError("File not found: " + ALSL_Editor_System.folderlanglink + "/" + ALSL_Editor_System.alllangs[i] + ".json");
				}
			}
		}
	}

	
}
