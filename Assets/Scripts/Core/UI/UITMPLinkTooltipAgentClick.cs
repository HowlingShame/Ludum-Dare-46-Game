using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITMPLinkTooltipAgentClick : UITMPLinkTooltip.UITMPLinkTooltipAgent, IPointerClickHandler
{
	public UITMPLinkTooltip		m_UITMPLinkTooltip;
	public TextMeshProUGUI		m_Text;
	private Camera				m_Camera;
	private OnUpdateCallback	m_OnUpdate;

	//////////////////////////////////////////////////////////////////////////
	public override void Init(UITMPLinkTooltip owner, TextMeshProUGUI text)
	{
		hideFlags = HideFlags.HideInInspector;
		m_UITMPLinkTooltip = owner;
		m_Text = text;
		if (m_Text == null)
			m_Text = GetComponent<TextMeshProUGUI>();

		var canvas = m_Text.canvas;
		m_Camera = canvas != null
			? canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Core.Instance.m_Camera
			: Core.Instance.m_Camera;
	}

	private void implHide()
	{
		if (m_OnUpdate != null)
		{
			Destroy(m_OnUpdate);
			m_OnUpdate = null;
			m_UITMPLinkTooltip.Hide();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		var linkID = TMP_TextUtilities.FindIntersectingLink(m_Text, Core.Instance.MouseWorldPosition.ScreenPosition, m_Camera);
		if (linkID != -1)
		{
			var linkInfo = m_Text.textInfo.linkInfo[linkID];
			m_UITMPLinkTooltip.Show(linkInfo.GetLinkID());

			if (m_OnUpdate == null)
			{
				m_OnUpdate = gameObject.AddComponent<OnUpdateCallback>();
				m_OnUpdate.hideFlags = HideFlags.HideInInspector;
				m_OnUpdate.m_Action = () =>
				{
					var currentLinkID = TMP_TextUtilities.FindIntersectingLink(m_Text, Core.Instance.MouseWorldPosition.ScreenPosition, m_Camera);
					if (currentLinkID != linkID)
						implHide();
				};
			}
		}
	}
	
	private void OnDestroy()
	{
		if (m_OnUpdate != null)
		{
			Destroy(m_OnUpdate);
			m_OnUpdate = null;
		}
	}
}