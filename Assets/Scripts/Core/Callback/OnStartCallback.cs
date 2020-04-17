using UnityEngine;
using System;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class OnStartCallback : MonoBehaviour
{
	public event Action<GameObject>		OnDestroyAction;
	public UnityEvent					OnDestroyEvent;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		OnDestroyAction?.Invoke(gameObject);
		OnDestroyEvent?.Invoke();
	}
}
