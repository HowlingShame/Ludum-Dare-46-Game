using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(Core.c_ManagerDefaultExecutionOrder)]
public class SoundManager : MonoBehaviour
{
	public static SoundManager          Instance;

    [SerializeField]
    private ChanelDictionary            m_ChanelDictionary;
    [SerializeField]
    private SoundDictionary             m_SoundDictionary;

    [SerializeField]
    private AudioSource                 m_MusicChanel;
    [SerializeField]
    private AudioSource                 m_SoundChanel;

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class ChanelDictionary: SerializableDictionaryBase<string, AudioSource> {};

	[Serializable]
	public class SoundDictionary: SerializableDictionaryBase<string, AudioClip> {};
	
	[Serializable]
	public enum PlayMode
	{
		OneShot,
		Source,
	}

    //////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		Instance = this;
	}

	public void Stop(string chanel)
	{
		if (m_ChanelDictionary.TryGetValue(chanel, out var audioSource))
			audioSource.Stop();
	}

	public void Play(string chanel, float time = 0.0f)
	{
		if (m_ChanelDictionary.TryGetValue(chanel, out var audioSource))
		{
            audioSource.Play();
            audioSource.time = time;
		}
	}

	public bool Play(string chanel, string sound, PlayMode mode = PlayMode.OneShot)
	{
		// try get data
		if (m_SoundDictionary.TryGetValue(sound, out var audioClip) 
		    && m_ChanelDictionary.TryGetValue(chanel, out var audioSource))
		{
			switch (mode)
			{
				case PlayMode.OneShot:
					audioSource.PlayOneShot(audioClip);
					return true;
				case PlayMode.Source:
					audioSource.clip = audioClip;
					audioSource.Play();
					return true;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}

		return false;
	}

    public void PlayMusic(string sound)
    {
        if (string.IsNullOrEmpty(sound) == false && m_SoundDictionary.TryGetValue(sound, out var audioClip))
        {
            m_MusicChanel.clip = audioClip;
            m_MusicChanel.Play();
        }
        else
        {
            m_MusicChanel.Stop();
        }
    }

    public void PlaySound(string sound, float volumeScale = 1.0f)
    {
        if (sound != null && m_SoundDictionary.TryGetValue(sound, out var audioClip))
            m_SoundChanel.PlayOneShot(audioClip, volumeScale);
    }
}
