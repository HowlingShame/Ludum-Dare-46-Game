using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UltEvents;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class OnAwakeCallback : MonoBehaviour
{
	public event Action<GameObject>		OnAwakeAction;
	public CallbackEvent				OnAwakeEvent;

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class CallbackEvent : UltEvent<GameObject>
	{
	}

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		OnAwakeAction?.Invoke(gameObject);
		OnAwakeEvent?.Invoke(gameObject);
	}
}
