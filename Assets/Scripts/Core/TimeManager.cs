using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent, DefaultExecutionOrder(-1)]
public class TimeManager : MonoBehaviour, IExecutable
{
	public static TimeManager	Instance;

	private float	m_InitialFixedDeltaTime;
	private float	m_GameSpeed;

	public float	GameSpeed
	{
		get => m_GameSpeed;

		set
		{
			if (value == 0.0f && m_GameSpeed != 0.0f)
				m_ResumeGameSpeed = m_GameSpeed;

			m_GameSpeed = value;
		}
	}

	[SerializeField]
	private bool	m_PauseControl;
	private bool	m_PauseInterrupted;
	private float	m_ResumeGameSpeed;

	public bool		Pause
	{
		get => m_GameSpeed == 0.0f; 

		set
		{
			if (Pause != value)
				if (value)		GameSpeed = 0.0f;
				else
				{
					m_HoldToUnPauseKeys.Clear();
					m_UnPauseObject.Clear();

					m_GameSpeed = m_ResumeGameSpeed;
				}
		}
	}

	private List<KeyCode>	m_HoldToUnPauseKeys = new List<KeyCode>();
	private HashSet<object>	m_UnPauseObject = new HashSet<object>();

	
	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		m_GameSpeed = Time.timeScale;
		m_InitialFixedDeltaTime = Time.fixedDeltaTime;

		if (m_PauseControl)
			gameObject.AddComponent<RunnerLateUpdate>().Executable = this;
	}

	private void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	private void LateUpdate()
	{
		if (Time.timeScale != m_GameSpeed)
		{
			Time.timeScale = m_GameSpeed;
			if (m_GameSpeed > 0.0f && m_GameSpeed <= 1.0f)
				Time.fixedDeltaTime = m_InitialFixedDeltaTime * m_GameSpeed;
		}
	}

	public void iExecute()
	{
		if (Pause)
		{
			if (m_UnPauseObject.Count > 0)
			{
				Pause = false;
				m_PauseInterrupted = true;
			}
			else
			foreach (var n in m_HoldToUnPauseKeys)
				if (Input.GetKey(n))
				{
					Pause = false;
					m_PauseInterrupted = true;
					break;
				}	
		}
		else
		if (m_PauseInterrupted)
		{
			bool onPause = true;
			if (m_UnPauseObject.Count > 0)
				onPause = false;
			else
			foreach (var n in m_HoldToUnPauseKeys)
				if (Input.GetKey(n))
				{
					onPause = false;
					break;
				}	

			if (onPause)
			{
				Pause = true;
				m_PauseInterrupted = false;
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public void TogglePause()
	{
		Pause = !Pause;
	}

	public void AddUnpauseObject(object obj)
	{
		m_UnPauseObject.Add(obj);
	}

	public void RemoveUnpauseObject(object obj)
	{
		m_UnPauseObject.Remove(obj);
	}

	public void AddHoldUnpauseKey(KeyCode key)
	{
		m_HoldToUnPauseKeys.Add(key);
	}

	public void RemoveHoldUnpauseKey(KeyCode m_Key)
	{
		m_HoldToUnPauseKeys.Remove(m_Key);
	}
	
}
