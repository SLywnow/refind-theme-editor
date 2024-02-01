using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using System.Linq;

namespace SLywnow
{
    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty FindParentProperty(this SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
            {
                return default;
            }

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                    {
                        // reached the end
                        break;
                    }
                    if (propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        var arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }
    }

    [CustomPropertyDrawer(typeof(ShowFromEnumAttribute))]
    public class HideAttributeEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ShowFromEnumAttribute att = (ShowFromEnumAttribute)attribute;

            bool wasEnabled = GUI.enabled;

            bool enabled = CheckProp(att, property);
            GUI.enabled = enabled;
            if (enabled)
            {
                    EditorGUI.PropertyField(position, property, label, true);
            }
            else
			{
                if (property.displayName.Contains("Element") && (property.name == "data" || property.name == "missing"))
                {
                    //name.Array.data[n].
					EditorGUI.PropertyField(position, property, new GUIContent("Disabled"), false);
                }
			}

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowFromEnumAttribute att = (ShowFromEnumAttribute)attribute;
            bool enabled = CheckProp(att, property);

            if (enabled)
            {
                    return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool CheckProp(ShowFromEnumAttribute att, SerializedProperty property)
        {
            bool ret = false;

            if (property != null)
            {
                SerializedProperty checkp = null;

                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, att.TargetProperty);
                checkp = property.serializedObject.FindProperty(conditionPath);
                if (checkp == null)
                {
                    propertyPath = SerializedPropertyExtensions.FindParentProperty(property).propertyPath;
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, att.TargetProperty);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }


                if (checkp != null)
                {
                    if (!att.not)
                    {
                        if (checkp.enumValueIndex == att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
                    {
                        if (checkp.enumValueIndex != att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                }
            }
            else
            {

            }


            return ret;
        }
    }


    [CustomPropertyDrawer(typeof(ShowFromIntAttribute))]
    public class HideAttributeIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ShowFromIntAttribute att = (ShowFromIntAttribute)attribute;

            bool wasEnabled = GUI.enabled;

            bool enabled = CheckProp(att, property);
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                if (property.displayName.Contains("Element") && (property.name == "data" || property.name == "missing"))
                {
                    EditorGUI.PropertyField(position, property, new GUIContent("Disabled"), false);
                }
            }

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowFromIntAttribute att = (ShowFromIntAttribute)attribute;
            bool enabled = CheckProp(att, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool CheckProp(ShowFromIntAttribute att, SerializedProperty property)
        {
            bool ret = false;

            if (property != null)
            {
                SerializedProperty checkp = null;
                /*if (!property.isArray || property.type == "string")
                {
                    string propertyPath = property.propertyPath;
                    string conditionPath = propertyPath.Replace(property.name, att.TargetProperty);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }
                else
                {
                    string propertyPath = property.propertyPath;
                    propertyPath = propertyPath.Remove(propertyPath.LastIndexOf(".Array.data"));
                    propertyPath = propertyPath.Remove(propertyPath.LastIndexOf("."));
                    propertyPath += "." + att.TargetProperty;

                    checkp = property.serializedObject.FindProperty(propertyPath);
                }*/

                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, att.TargetProperty);
                checkp = property.serializedObject.FindProperty(conditionPath);
                if (checkp == null)
                {
                    propertyPath = SerializedPropertyExtensions.FindParentProperty(property).propertyPath;
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, att.TargetProperty);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }

                if (checkp != null)
                {
                    if (!att.not)
                    {
                        if (checkp.intValue == att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
					{
                        if (checkp.intValue != att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                }
            }
            else
            {

            }


            return ret;
        }
    }

    [CustomPropertyDrawer(typeof(ShowFromFloatAttribute))]
    public class HideAttributeFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ShowFromFloatAttribute att = (ShowFromFloatAttribute)attribute;

            bool wasEnabled = GUI.enabled;

            bool enabled = CheckProp(att, property);
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                if (property.displayName.Contains("Element") && (property.name == "data" || property.name == "missing"))
                {
                    EditorGUI.PropertyField(position, property, new GUIContent("Disabled"), false);
                }
            }

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowFromFloatAttribute att = (ShowFromFloatAttribute)attribute;
            bool enabled = CheckProp(att, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool CheckProp(ShowFromFloatAttribute att, SerializedProperty property)
        {
            bool ret = false;

            if (property != null)
            {
                SerializedProperty checkp = null;
                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, att.TargetProperty);
                checkp = property.serializedObject.FindProperty(conditionPath);
                if (checkp == null)
                {
                    propertyPath = SerializedPropertyExtensions.FindParentProperty(property).propertyPath;
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, att.TargetProperty);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }

                if (checkp != null)
                {
                    if (!att.not)
					{
                        if (checkp.floatValue == att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
					{
                        if (checkp.floatValue != att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                }
            }
            else
            {

            }


            return ret;
        }
    }


    [CustomPropertyDrawer(typeof(ShowFromStringAttribute))]
    public class HideFromStringAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ShowFromStringAttribute att = (ShowFromStringAttribute)attribute;

            bool wasEnabled = GUI.enabled;

            bool enabled = CheckProp(att, property);
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                if (property.displayName.Contains("Element") && (property.name == "data" || property.name == "missing"))
                {
                    EditorGUI.PropertyField(position, property, new GUIContent("Disabled"), false);
                }
            }

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowFromStringAttribute att = (ShowFromStringAttribute)attribute;
            bool enabled = CheckProp(att, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool CheckProp(ShowFromStringAttribute att, SerializedProperty property)
        {
            bool ret = false;

            if (property != null)
            {
                SerializedProperty checkp = null;
                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, att.TargetProperty);
                checkp = property.serializedObject.FindProperty(conditionPath);
                if (checkp == null)
                {
                    propertyPath = SerializedPropertyExtensions.FindParentProperty(property).propertyPath;
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, att.TargetProperty);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }

                if (checkp != null)
                {
                    if (!att.not)
                    {
                        if (checkp.stringValue == att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
                    {
                        if (checkp.stringValue != att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                }
            }
            else
            {

            }


            return ret;
        }
    }


    [CustomPropertyDrawer(typeof(ShowFromBoolAttribute))]
    public class HideFromBoolAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ShowFromBoolAttribute att = (ShowFromBoolAttribute)attribute;

            bool wasEnabled = GUI.enabled;

            bool enabled = CheckProp(att, property);
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                if (property.displayName.Contains("Element") && (property.name == "data" || property.name == "missing"))
                {
                    EditorGUI.PropertyField(position, property, new GUIContent("Disabled"), false);
                }
            }

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowFromBoolAttribute att = (ShowFromBoolAttribute)attribute;
            bool enabled = CheckProp(att, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool CheckProp(ShowFromBoolAttribute att, SerializedProperty property)
        {
            bool ret = false;

            if (property != null)
            {
                SerializedProperty checkp = null;
                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, att.TargetProperty);
                checkp = property.serializedObject.FindProperty(conditionPath);
                if (checkp == null)
                {
                    propertyPath = SerializedPropertyExtensions.FindParentProperty(property).propertyPath;
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, att.TargetProperty);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }

                if (checkp != null)
                {
                    if (!att.not)
                    {
                        if (checkp.boolValue == att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
                    {
                        if (checkp.boolValue != att.targetval)
                            ret = true;
                        else
                            ret = false;
                    }
                }
            }
            else
            {

            }


            return ret;
        }
    }


    [CustomPropertyDrawer(typeof(ShowFromObjectNotNullAttribute))]
    public class HideFromObjectNotNullAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ShowFromObjectNotNullAttribute att = (ShowFromObjectNotNullAttribute)attribute;

            bool wasEnabled = GUI.enabled;

            bool enabled = CheckProp(att, property);
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                if (property.displayName.Contains("Element") && (property.name == "data" || property.name == "missing"))
                {
                    EditorGUI.PropertyField(position, property, new GUIContent("Disabled"), false);
                }
            }

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowFromObjectNotNullAttribute att = (ShowFromObjectNotNullAttribute)attribute;
            bool enabled = CheckProp(att, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool CheckProp(ShowFromObjectNotNullAttribute att, SerializedProperty property)
        {
            bool ret = false;

            if (property != null)
            {
                SerializedProperty checkp = null;
                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, att.TargetProperty);
                checkp = property.serializedObject.FindProperty(conditionPath);
                if (checkp == null)
                {
                    propertyPath = SerializedPropertyExtensions.FindParentProperty(property).propertyPath;
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, att.TargetProperty);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }

                if (checkp != null)
                {
                    if (att.targetval)
                    {
                        if (checkp.objectReferenceValue ==null)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
					{
                        if (checkp.objectReferenceValue != null)
                            ret = true;
                        else
                            ret = false;
                    }
                }
            }
            else
            {

            }


            return ret;
        }
    }


    [CustomPropertyDrawer(typeof(ShowFromMultipleAttribute))]
    public class ShowFromMultipleAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ShowFromMultipleAttribute att = (ShowFromMultipleAttribute)attribute;

            bool wasEnabled = GUI.enabled;

            bool enabled = CheckProp(att, property);
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                if (property.displayName.Contains("Element") && (property.name == "data" || property.name == "missing"))
                {
                    EditorGUI.PropertyField(position, property, new GUIContent("Disabled"), false);
                }
            }

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowFromMultipleAttribute att = (ShowFromMultipleAttribute)attribute;
            bool enabled = CheckProp(att, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool CheckProp(ShowFromMultipleAttribute att, SerializedProperty property)
        {
            bool ret = false;

            if (property != null)
            {
                string tocheck = att.TargetProperty[0];
                SerializedProperty checkp = null;
                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, tocheck);
                checkp = property.serializedObject.FindProperty(conditionPath);
                if (checkp == null)
                {
                    propertyPath = SerializedPropertyExtensions.FindParentProperty(property).propertyPath;
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, tocheck);
                    checkp = property.serializedObject.FindProperty(conditionPath);
                }

                int max = att.TargetProperty.Count;

                if (max > att.valss.Count)
                    max = att.valss.Count;

                if (max > att.typess.Count)
                    max = att.typess.Count;

                List<bool> bools = new List<bool>();
                for (int i =0;i<max;i++)
				{
                    conditionPath = propertyPath.Replace(property.serializedObject.FindProperty(propertyPath).name, att.TargetProperty[i]);
                    checkp = property.serializedObject.FindProperty(conditionPath);

                    if (checkp !=null)
					{
                        //"string","int","float","bool","enum","object"

                        switch (att.typess[i])
						{
                            case "string":
								{
                                    if (checkp.stringValue == att.valss[i])
                                        bools.Add(true);
                                    else
                                        bools.Add(false);
                                    break;
								}
                            case "int":
                                {
                                    try
                                    {
                                        if (checkp.intValue == int.Parse(att.valss[i]))
                                            bools.Add(true);
                                        else
                                            bools.Add(false);
                                    }
                                    catch { }
                                    break;
                                }
                            case "float":
                                {
                                    try
                                    {
                                        if (checkp.floatValue == float.Parse(att.valss[i]))
                                            bools.Add(true);
                                        else
                                            bools.Add(false);
                                    }
                                    catch { }
                                    break;
                                }
                            case "bool":
                                {
                                    try
                                    {
                                        if (checkp.boolValue == bool.Parse(att.valss[i]))
                                            bools.Add(true);
                                        else
                                            bools.Add(false);
                                    }
                                    catch { }
                                    break;
                                }
                            case "enum":
                                {
                                    try
                                    {
                                        if (checkp.enumValueIndex == int.Parse(att.valss[i]))
                                            bools.Add(true);
                                        else
                                            bools.Add(false);
                                    }
                                    catch { }
                                    break;
                                }
                            case "object":
                                {
                                    try
                                    {
                                        if (bool.Parse(att.valss[i]))
                                        {
                                            if (checkp.objectReferenceValue == null)
                                                bools.Add(true);
                                            else
                                                bools.Add(false);
                                        }
                                        else
                                        {
                                            if (checkp.objectReferenceValue != null)
                                                bools.Add(true);
                                            else
                                                bools.Add(false);
                                        }
                                    }
                                    catch { }
                                break;
                                }
                        }
                    }
                }

                if (att.Md==ShowFromMultipleAttribute.mode.and)
				{
                    if (bools.Contains(false))
                        ret = false;
                    else
                        ret = true;
				}
                else if (att.Md == ShowFromMultipleAttribute.mode.or)
				{
                    if (bools.Contains(true))
                        ret = true;
                    else
                        ret = false;
                }
            }
            else
            {

            }


            return ret;
        }
    }
}