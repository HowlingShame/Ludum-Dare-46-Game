using System;
using UnityEngine;

[Serializable]
public class LocalizedString
{
	public const string c_KeyProperty = "m_Key";
	
	[SerializeField]
	private string	m_Key;
	[NonSerialized]
	private string	m_Data;

	public string Data
	{
		get
		{
			if (m_Data == null)
				m_Data = LocalizationManager.Localize(m_Key);

			return m_Data;
		}
	}

	public string Key
	{
		get => m_Key;
		set
		{
			m_Key = value;
			m_Data = LocalizationManager.Localize(m_Key);
		}
	}

	public static implicit operator string(LocalizedString localizedString)
	{
		return localizedString.Data;
	}
}