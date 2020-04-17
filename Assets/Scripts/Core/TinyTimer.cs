using UnityEngine;
using System;

[Serializable]
public class TinyTimer
{
	[SerializeField]
	private float		m_CurrentTime = 0.0f;
	public float		TimePassed => m_CurrentTime;
	public float		Scale => m_CurrentTime / m_FinishTime;

	public bool			Running	=> m_CurrentTime < m_FinishTime;
	public bool			Expired	=> m_CurrentTime >= m_FinishTime;
	public float		Excess => Mathf.Max(m_CurrentTime - m_FinishTime, 0.0f);
	public float		Remainder => Mathf.Max(m_FinishTime - m_CurrentTime, 0.0f);

	[SerializeField]
	private float		m_FinishTime;
	public float		FinishTime
	{
		get => m_FinishTime;
		set => m_FinishTime = value;
	}

	//////////////////////////////////////////////////////////////////////////
	public bool AddTime(float time)
	{
		m_CurrentTime += time;
		return m_CurrentTime >= m_FinishTime;
	}

	/// <summary>
	/// Set current & finish time
	/// </summary>
	/// <param name="currentTimer"></param>
	/// <param name="finishTime"></param>
	public void Reset(float currentTimer, float finishTime)
	{
		m_CurrentTime = currentTimer;
		m_FinishTime = finishTime;
	}
	/// <summary>
	/// Set finish time & discard current time
	/// </summary>
	/// <param name="finishTime"></param>
	public void Reset(float finishTime)
	{
		m_CurrentTime = 0.0f;
		m_FinishTime = finishTime;
	}
	
	/// <summary>
	/// Set current time to 0.0f
	/// </summary>
	public void Reset()
	{
		m_CurrentTime = 0.0f;
	}

	/// <summary>
	/// Subtract time length from current time, clamp current tine to zero
	/// </summary>
	public void CloseCircle()
	{
		m_CurrentTime = Mathf.Max(m_CurrentTime - m_FinishTime, 0.0f);
	}
	
	public TinyTimer()
	{
	}

	public TinyTimer(float m_FinishTime)
	{
		this.m_FinishTime = m_FinishTime;
	}
}
