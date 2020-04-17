using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;
using System.Threading;
using System.Linq;


public class DebugText : GLMonoBehaviour
{
	public class debug_text_string
	{
		public delegate string		GetStringDelegate();
		public Coroutine			p_Coroutine;
		public string				m_Title;
		public GetStringDelegate	m_GetText;
		public float				m_FadeTime;
	}
	
	string											m_Info = "";
	LinkedList<debug_text_string>					m_ShowOnce = new LinkedList<debug_text_string>();
	SortedDictionary<string, debug_text_string>		m_Fields = new SortedDictionary<string, debug_text_string>();
	Dictionary<object, string>						m_Unnamed = new Dictionary<object, string>();

	public const float								m_DefaultFadeTime = 1.0f;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		hideFlags = HideFlags.HideInInspector;
	}
	
	private void Update ()
	{
		m_Info = string.Empty;

		foreach (var n in m_Fields)
		{
			var result = "";
			try
			{
				result += n.Value.m_Title;
				result += " ";
				result += n.Value.m_GetText?.Invoke();
			}
			catch (System.Exception ex)	{ result += "\n" + ex.Message; }

			result += "\n";
			m_Info += result;
		}
			
		foreach (var n in m_ShowOnce)
		{
			var result = "";
			try
			{
				result += n.m_Title;
				result += " ";
				result += n.m_GetText?.Invoke();
			}
			catch (System.Exception ex)	{ result += "\n" + ex.Message; }

			result += "\n";
			m_Info += result;
		}
		foreach (var n in m_ShowOnce)
			m_Unnamed.Remove(n);
		m_ShowOnce.Clear();
		

		//this.enabled = false;
	}

	private void OnGUI ()
	{
		var rect = new Rect(0,0, Screen.width / 2, Screen.height);
		GUI.Label(rect, m_Info/*, UnityEditor.EditorStyles.textField*/);
	}


	//////////////////////////////////////////////////////////////////////////
	public void ShowString(object obj)
	{
		if(obj != null)		ShowString(GetUniqeName(obj), obj.ToString, m_DefaultFadeTime);
		else				ShowString(GetUniqeName(obj), () => { return "null"; }, m_DefaultFadeTime);
	}

	public void ShowString(string title, object obj, float fadeTime = 0.0f)
	{
		if(obj != null)		ShowString(title, obj.ToString, fadeTime);
		else				ShowString(title, () => { return "null"; }, fadeTime);
	}

	public void ShowString(string title, debug_text_string.GetStringDelegate getStringDelegate, float fadeTime = 0.0f)
	{
		debug_text_string tmp;
		if (m_Fields.ContainsKey(title))
		{
			tmp = m_Fields[title];
			tmp.m_Title = title;
			tmp.m_FadeTime = fadeTime;
			tmp.m_GetText = getStringDelegate;
		} 
		else
		{
			tmp = new debug_text_string { m_Title = title, m_GetText = getStringDelegate, m_FadeTime = fadeTime };

			if (fadeTime > 0.0f)
			{
				m_Fields.Add(tmp.m_Title, tmp);
				tmp.p_Coroutine = InvokeRepeating(() => {
					if (tmp.m_FadeTime <= 0.0f)
					{
						m_Fields.Remove(tmp.m_Title);
						m_Unnamed.Remove(tmp);
						StopCoroutine(tmp.p_Coroutine);
					}
					tmp.m_FadeTime -= 1.0f;

				}, 0.0f, 1.0f);
				//Invoke(() => { m_fields.Remove(tmp_.m_priority); if(m_fields.Count == 0) this.enabled = true; }, _fadetime);
				//StartCoroutine(ExecuteAfterTime(_fadetime, () => { m_fields.Remove(tmp_.m_priority); if(m_fields.Count == 0) this.enabled = true; }));
			}
			else
				m_ShowOnce.AddLast(tmp);
		}



		this.enabled = true;
	}

	protected string GetUniqeName(object obj)
	{
		if(m_Unnamed.ContainsKey(obj))
			return m_Unnamed[obj];

		string UnnamedName = obj.GetType().Name;
		string UnnamedNameResult = UnnamedName.Substring(0);

		int index = 0;
		while(m_Unnamed.ContainsValue(UnnamedNameResult) || m_Fields.ContainsKey(UnnamedNameResult))
			UnnamedNameResult = UnnamedName + index++.ToString();

		m_Unnamed.Add(obj, UnnamedNameResult);
		return UnnamedNameResult;
	}
}
