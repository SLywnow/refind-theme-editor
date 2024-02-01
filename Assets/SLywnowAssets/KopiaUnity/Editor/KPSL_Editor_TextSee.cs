using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SLywnow;
using System.Linq;

public class KPSL_Editor_TextSee : EditorWindow
{
    List<string> stringOrig;
    List<string> stringChanged;
    List<int> origs;
    List<int> origsinChanged;
    List<int> edited;
    Vector2 pos;
    bool worked;

    GUIStyle style;
    GUIStyle styleEdit;
    GUIStyle styleDelete;
    GUIStyle styleAdded;

    Color editC;
    Color deleteC;
    Color addedC;
    Texture2D editT;
    Texture2D deleteT;
    Texture2D addedT;

    private void OnEnable()
	{
        editC = new Color(204f / 255f, 204f / 255f, 0, 0.2f);
        deleteC = new Color(204f / 255f, 0, 0, 0.2f);
        addedC = new Color(0, 204f / 255f, 0, 0.2f);
        editT = UIEditor.GetTextureWithColor(editC, 600, 10);
        deleteT = UIEditor.GetTextureWithColor(deleteC, 600, 10);
        addedT = UIEditor.GetTextureWithColor(addedC, 600, 10);

        //Debug.Log("ok");
        OnFocus();
        //EditorWindow.DestroyImmediate(this);
    }



	void OnFocus()
    {
        string path = Application.dataPath.Replace("/Assets", "/Temp/KopiaCheck/");
        //Debug.Log(path);
        if (FilesSet.CheckFile(path + "changed.txt") && FilesSet.CheckFile(path + "orig.txt"))
        {
            stringOrig = FilesSet.LoadStream(path + "orig.txt", false).ToList();
            stringChanged = FilesSet.LoadStream(path + "changed.txt", false).ToList();

            origs = new List<int>();
            origsinChanged = new List<int>();
            edited = new List<int>();

            //get same lines
            for (int i =0;i< stringOrig.Count;i++)
			{
                int id = i;
                if (stringChanged.Count>i)
				{
                    if (stringOrig[i].Equals(stringChanged[i]))
                    {
                        origs.Add(id);
                        origsinChanged.Add(id);
                    }
                    else if (stringChanged.IndexOf(stringOrig[i]) != -1)
                    {
                        origs.Add(id);
                        origsinChanged.Add(stringChanged.IndexOf(stringOrig[i]));
                    }
                    else 
                        edited.Add(id);
                }
			}
            
            worked = true;
        }
        else 
            worked = false;
    }

	private void OnGUI()
	{
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.richText = true;

		float linesize = position.width - 100;
        float numsize = 60;

        style = new GUIStyle(GUI.skin.label);

        styleEdit = new GUIStyle(GUI.skin.label);
        styleEdit.normal.background = editT;

        styleDelete = new GUIStyle(GUI.skin.label);
        styleDelete.normal.background = deleteT;

        styleAdded = new GUIStyle(GUI.skin.label);
        styleAdded.normal.background = addedT;

        if (worked)
        {
            if (GUILayout.Button("Open preview"))
			{
                EditorWindow.GetWindow(typeof(KPSL_Editor_TextPreview), false, "Kopia Unity Text Preview", true);
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b><size=15></size></b>", style, GUILayout.Width(numsize));
            GUILayout.Label("<b><size=15>In project</size></b>", style, GUILayout.Width((linesize) / 2));
            GUILayout.Label("<b><size=15>Snapshot</size></b>", style, GUILayout.Width((linesize) / 2));
            GUILayout.EndHorizontal();

            int max = stringOrig.Count >= stringChanged.Count ? stringOrig.Count : stringChanged.Count;

            pos = EditorGUILayout.BeginScrollView(pos);
            for (int i = 0; i < max; i++)
			{
                int id = i;
                GUILayout.BeginHorizontal();
                //number lines
                {
                    if (stringOrig.Count > id)
                    {
                        if (edited.Contains(id))
                        {
                            GUILayout.Label("<b><color=yellow>" + id + "</color></b>", style, GUILayout.Width(numsize));
                        }
                        else if (origs.Contains(id))
                        {
                            if (origsinChanged[origs.IndexOf(id)] == origs.IndexOf(id))
                                GUILayout.Label("<b>" + id + "</b>", style, GUILayout.Width(numsize));
                            else if (!string.IsNullOrEmpty(stringOrig[id]))
                                GUILayout.Label("<b>" + id + " (" + origsinChanged[origs.IndexOf(id)] + ")</b>", style, GUILayout.Width(numsize));
                            else
                                GUILayout.Label("<b>" + id + "</b>", style, GUILayout.Width(numsize));
                        }
                        else
                            GUILayout.Label("<b><color=green>" + id + "</color></b>", style, GUILayout.Width(numsize));
                    }
                    else
                        GUILayout.Label("<b><color=red>" + id + "</color></b>", style, GUILayout.Width(numsize));
                }

                //orig
                if (stringOrig.Count > id)
                {
                    if (edited.Contains(id))
                        GUILayout.TextField(stringOrig[id], styleEdit, GUILayout.Width((linesize) / 2));
                    else if (origs.Contains(id))
                    {
                        if (origsinChanged[origs.IndexOf(id)] == origs.IndexOf(id))
                            GUILayout.TextField(stringOrig[id], style, GUILayout.Width((linesize) / 2));
                        else if (!string.IsNullOrEmpty(stringOrig[id]))
                            GUILayout.TextField(stringOrig[id], style, GUILayout.Width((linesize) / 2));
                        else
                            GUILayout.TextField(stringOrig[id], style, GUILayout.Width((linesize) / 2));
                    }
                    else
                        GUILayout.TextField(stringOrig[id], styleAdded, GUILayout.Width((linesize) / 2));
                }
                else
                    GUILayout.TextField("", styleDelete, GUILayout.Width((linesize) / 2));

                //changed
                if (stringChanged.Count > id)
                    GUILayout.TextField(stringChanged[id], style, GUILayout.Width((linesize) / 2));
                else
                    GUILayout.TextField("", style, GUILayout.Width((linesize) / 2));


                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}


public class KPSL_Editor_TextPreview : EditorWindow
{
    private void OnEnable()
    {
        OnFocus();
    }

    List<string> loaded;

    private void OnFocus()
    {
        string path = Application.dataPath.Replace("/Assets", "/Temp/KopiaCheck/");

        loaded = null;
        if (FilesSet.CheckFile(path + "changed.txt"))
        {
            loaded = FilesSet.LoadStream(path + "changed.txt", false).ToList();
        }
    }

    Vector2 pos;

    private void OnGUI()
    {
        if (loaded != null)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);

            pos = GUILayout.BeginScrollView(pos);
            string text = "";
			GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.Width(60));
                {
                    for (int i = 0; i < loaded.Count; i++)
                    {
                        int id = i;



                        text += loaded[id] + "\n";
                        GUILayout.Label(id + "", style);


                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    EditorGUILayout.TextArea(text, style);
                }
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Text not found! Try again!");
        }
    }
}