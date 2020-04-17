﻿using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DataContainerBase), true)]
public class DataContainerDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Data"), label, true);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Data"), label, true);
	}
}