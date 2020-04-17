using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundExecutor : MonoBehaviour
{
	public int				m_ID;
	public float			m_StartTime;
	public float			m_CurrentTime;
	public float			m_EndTime;
	public AudioSource		m_AudioSource;


	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public enum CancelMode
	{
		Destroy,
		Repeat,
	}

	[Serializable]
	public enum UpdateMode
	{
		DeltaTime,
		RealTime,
	}

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_AudioSource = GetComponent<AudioSource>();
	}


	private void Update()
	{
		// must be implement thru state machine or coroutine
		if (m_CurrentTime >= m_StartTime && m_CurrentTime <= m_EndTime)
		{
			if (m_AudioSource.isPlaying == false)
				m_AudioSource.Play();
		}
		else
		{
			if (m_AudioSource.isPlaying)
				m_AudioSource.Stop();
		}

		m_CurrentTime += Time.deltaTime;
	}
}