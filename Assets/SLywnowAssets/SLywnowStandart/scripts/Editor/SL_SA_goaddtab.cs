using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class SL_SA_goaddtab : Editor
{
	[MenuItem("GameObject/3D Object/SLywnow/Blur")]
	static void SetDirection()
	{
		GameObject main = SceneView.Instantiate(Resources.Load("perfabs/sl_blurobj") as GameObject, SceneView.lastActiveSceneView.pivot, Quaternion.identity) as GameObject;
		GameObject two = SceneView.Instantiate(main, SceneView.lastActiveSceneView.pivot, Quaternion.identity);
		two.name = "Blur";
		DestroyImmediate(main);
		Selection.activeGameObject = two;
	}
}
