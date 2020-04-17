using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class UIURLLinkOpener : MonoBehaviour, IPointerClickHandler 
{
	private TMP_Text		m_Text;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_Text = GetComponent<TMP_Text>();
	}

	public void OnPointerClick(PointerEventData eventData) 
	{
		
		//var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, Input.mousePosition, m_Camera == null ? null : m_Camera);

		var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, Input.mousePosition, 
			m_Text.canvas == null 
				? (Core.Instance.m_Camera != null 
					? Core.Instance.m_Camera 
					: Camera.main)
				: (m_Text.canvas.renderMode == RenderMode.ScreenSpaceOverlay 
					? null 
					: m_Text.canvas.worldCamera)
					);
		if (linkIndex != -1)
		{
			TMP_LinkInfo linkInfo = m_Text.textInfo.linkInfo[linkIndex];

			// open the link id as a url, which is the metadata we added in the text field
			Application.OpenURL(linkInfo.GetLinkID());
		}
	}
}