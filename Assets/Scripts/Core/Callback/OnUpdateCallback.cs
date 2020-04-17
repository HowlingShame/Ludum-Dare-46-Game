using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUpdateCallback : MonoBehaviour
{
	public Action m_Action;
	
	private void Update()
	{
		m_Action.Invoke();
	}
}
