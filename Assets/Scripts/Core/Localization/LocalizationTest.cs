using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class LocalizationTest : MonoBehaviour
{
	[LocalizedStringTextArea(3)]
	public LocalizedString	m_LocalizedStringTest;

	public TMP_InputField	m_Input;
	public TMP_Dropdown		m_Dropdown;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		GetComponent<TMP_Text>().text = m_LocalizedStringTest.Data;
	}

	[Button]
	public void Refresh()
	{
		LocalizationManager.Language = m_Dropdown.options[m_Dropdown.value].text;

		m_LocalizedStringTest.Key = m_Input.text;
		GetComponent<TMP_Text>().text = m_LocalizedStringTest.Data;
	}
}