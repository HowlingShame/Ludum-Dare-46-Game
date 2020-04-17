using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1)]
public class UITinyTooltip : GLMonoBehaviour
{
	[SerializeField]
	private TMP_Text				m_Text;

	public static UITinyTooltip		Instance;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		Instance = this;
		gameObject.SetActive(false);
	}

	private void Update()
	{
		transform.position = Core.Instance.MouseWorldPosition.ScreenPosition;
	}
	
	public void Show(string text)
	{
		gameObject.SetActive(true);
		m_Text.text = text;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
