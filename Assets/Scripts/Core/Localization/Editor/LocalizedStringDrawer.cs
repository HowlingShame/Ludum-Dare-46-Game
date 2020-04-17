using System;
using Gamelogic.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LocalizedStringTextAreaAttribute))]
[CustomPropertyDrawer(typeof(LocalizedString))]
public class LocalizedStringDrawer : UnityEditor.PropertyDrawer
{
	private const int	c_LanguagePopupWidth = 50;

	//////////////////////////////////////////////////////////////////////////
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var textArea = attribute as LocalizedStringTextAreaAttribute;

		var key = property.FindPropertyRelative(LocalizedString.c_KeyProperty);

		// current language
		var languages = LocalizationManager.GetLanguages();
		var selectedLanguage = EditorGUI.Popup(new Rect(position.x + position.width - c_LanguagePopupWidth, position.y, c_LanguagePopupWidth, EditorGUIUtility.singleLineHeight),
			languages.IndexOf(LocalizationManager.Language), languages.ToArray());

		// change current localization
		if (selectedLanguage != -1 && languages[selectedLanguage] != LocalizationManager.Language)
			LocalizationManager.Language = languages[selectedLanguage];

		// update text
		var keyValue = EditorGUI.TextField(new Rect(position.x, position.y, position.width - c_LanguagePopupWidth, EditorGUIUtility.singleLineHeight), label, key.stringValue);
		var hasKey = LocalizationManager.ContainsKey(keyValue);
		if (keyValue != key.stringValue)
		{	// rename key
			if (LocalizationManager.ChangeLocalizationKey(key.stringValue, keyValue, LocalizationManager.ChangeLocalizationKeyMode.None))
			{	// save changes
				key.stringValue = keyValue;
				key.serializedObject.ApplyModifiedProperties();
			}

		}

		//GUI.SetNextControlName("LocalizedStringTextArea");
		//&& GUI.GetNameOfFocusedControl() != "LocalizedStringTextArea";

		if (hasKey)
		{
			// update data
			var value = LocalizationManager.Localize(key.stringValue);
			var localizationValue = EditorGUI.TextArea(new Rect(position.x,
				position.y + EditorGUIUtility.singleLineHeight
				, position.width, EditorGUIUtility.singleLineHeight * (textArea?.m_LinesCount ?? 1)), value);

			if (localizationValue != value)
			{
				LocalizationManager.ChangeLocalizationKeyValue(key.stringValue, localizationValue);
			}
		}
		else
		{	// show placeholder, if changed then create new key
			var localizationValue = EditorGUI.TextArea(new Rect(position.x,
				position.y + EditorGUIUtility.singleLineHeight
				, position.width, EditorGUIUtility.singleLineHeight * (textArea?.m_LinesCount ?? 1)), LocalizationManager.c_DefaultKeyValue);

			// initialize key
			if (localizationValue != LocalizationManager.c_DefaultKeyValue)
			{
				LocalizationManager.GenerateNewLocalizationKey(key.stringValue, localizationValue);
			}
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (attribute is LocalizedStringTextAreaAttribute textArea)
			return EditorGUIUtility.singleLineHeight * (1 + textArea.m_LinesCount);

		return EditorGUIUtility.singleLineHeight * 2;
	}
}