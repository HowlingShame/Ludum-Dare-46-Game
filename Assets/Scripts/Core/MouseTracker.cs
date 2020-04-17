using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseTracker : MonoBehaviour
{
	private void Update()
	{
		transform.position = Core.Instance.MouseWorldPosition;
	}

}
