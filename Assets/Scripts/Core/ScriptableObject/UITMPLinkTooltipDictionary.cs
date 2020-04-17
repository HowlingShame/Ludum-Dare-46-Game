using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

//[CreateAssetMenu(fileName = "UITMPLinkTooltipDictionary", menuName = "UITMPLinkTooltipDictionary")]
public class UITMPLinkTooltipDictionary : ScriptableObject, UITMPLinkTooltip.ITooltipSource
{
	[Tooltip("Link description tooltip")]
	public LinkDisctionary		m_LinkDictionary;

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class LinkDisctionary : SerializableDictionaryBase<string, string> {};

	//////////////////////////////////////////////////////////////////////////
	public string iGetTooltip(string linkID)
	{
		if (m_LinkDictionary.TryGetValue(linkID, out var str))
			return str;

		return "unknown";
	}
}