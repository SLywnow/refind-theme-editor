using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toorah.Files;


[RequireComponent(typeof(RectTransform))]
public class DragFilesOntoMe : MonoBehaviour {

	[SerializeField] FileHopper m_fileHopper;
	[SerializeField] Text m_text;

	// Use this for initialization
	void Start () { 
		m_fileHopper.OnFilesDropped.AddListener(DroppedFilesOnMe);
	}

	void DroppedFilesOnMe(List<string> files, Vector2 pos)
	{
		//See if mouse is inside of Rect of this RectTransform while dropping files
		if(RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, pos))
		{
			m_text.text = "Dropped on me: " + name + ", file count: " + files.Count.ToString()
				+ "\n First File: " + files[0];
		}
	}
}
