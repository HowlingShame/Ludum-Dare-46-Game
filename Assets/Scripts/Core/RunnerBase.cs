using UnityEngine;
using Gamelogic.Extensions;

public abstract class RunnerBase : GLMonoBehaviour
{
	protected IExecutable m_Executable;

	public IExecutable Executable
	{
		get
		{
			return m_Executable;
		}

		set
		{
			m_Executable = value;
			if (m_Executable != null && enabled == false)
				enabled = true;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	protected void Start()
	{
		hideFlags = HideFlags.HideInInspector;

		if(m_Executable == null)
			enabled = false;
	}
}