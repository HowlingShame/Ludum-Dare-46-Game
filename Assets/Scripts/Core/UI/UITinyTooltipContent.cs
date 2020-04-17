using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITinyTooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[TextArea(4, 10)] 
	public string m_TooltipContent;

	//////////////////////////////////////////////////////////////////////////
	public void OnPointerEnter(PointerEventData eventData)
	{
		UITinyTooltip.Instance.Show(m_TooltipContent);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		UITinyTooltip.Instance.Hide();
	}
}
