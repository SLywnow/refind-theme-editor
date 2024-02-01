using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SLywnow.UI
{
	public class HideGroup : EventTrigger
	{
		public CanvasGroup group;
		public float without = 0.5f;
		public float with = 1;

		public override void OnPointerEnter(PointerEventData eventData)
		{
			group.alpha = with;
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			group.alpha = without;
		}

	}

#if UNITY_EDITOR
	[CustomEditor(typeof(HideGroup))]
	[CanEditMultipleObjects]
	public class HideGroup_Editor : Editor
	{
		SerializedProperty group;
		SerializedProperty without;
		SerializedProperty with;

		void OnEnable()
		{
			group = serializedObject.FindProperty("group");
			without = serializedObject.FindProperty("without");
			with = serializedObject.FindProperty("with");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(group);
			EditorGUILayout.PropertyField(without);
			EditorGUILayout.PropertyField(with);

			serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}