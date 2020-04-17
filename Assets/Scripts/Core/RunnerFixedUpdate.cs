using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RunnerFixedUpdate : RunnerBase
{
	//////////////////////////////////////////////////////////////////////////
	public void FixedUpdate()
	{
		m_Executable.iExecute();
	}
}
