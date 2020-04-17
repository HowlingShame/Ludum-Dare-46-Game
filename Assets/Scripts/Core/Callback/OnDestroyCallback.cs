using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class OnDestroyCallback : MonoBehaviour
{
	public event Action<GameObject>		OnDestroyAction;
	public UnityEvent					OnDestroyEvent;

	//////////////////////////////////////////////////////////////////////////
	private void OnDestroy()
	{
		OnDestroyAction?.Invoke(gameObject);
		OnDestroyEvent?.Invoke();
	}
}
