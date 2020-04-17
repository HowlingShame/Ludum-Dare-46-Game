using System;
using System.Collections;
using UnityEngine;

public class DirtyUpdate
{
	public Dirty		m_CurrentState{ get; private set; }
	private	Coroutine	m_UpdateCoroutine;

	public bool			IsDirty{ get{ return m_CurrentState.HasFlag(Dirty.Dirty); } }

	//////////////////////////////////////////////////////////////////////////
	private IEnumerator implWaitFixedUpdateAndDo(Action action) 
	{
		yield return new WaitForFixedUpdate();
		action();
		m_CurrentState = Dirty.Clean;
		m_UpdateCoroutine = null;
	}

	private IEnumerator implCleanUpdate(Action action) 
	{
		yield return null;
		action();
		m_CurrentState = Dirty.Clean;
		m_UpdateCoroutine = null;
	}

	private void implStopUpdate()
	{
		if(m_UpdateCoroutine != null)
		{
			Core.Instance.StopCoroutine(m_UpdateCoroutine);
			m_UpdateCoroutine = null;
		}
	}
	
	public void SetDirty(Dirty dirty)
	{
		switch(dirty)
		{
			case Dirty.Clean:
			case Dirty.Dirty:
			case Dirty.None:
				implStopUpdate();
			break;

			case Dirty.RequireUpdate:
			case Dirty.FixedUpdate:
				new ErrorResultDescriptor("invalid parameter use action argument function");
			break;

		}
		
		m_CurrentState = dirty;
	}

	public void SetDirty(Dirty dirty, Action updateCall)
	{
		switch(dirty)
		{
			case Dirty.RequireUpdate:
				if(IsDirty == false)
					m_UpdateCoroutine = Core.Start(implCleanUpdate(updateCall));
			break;
			case Dirty.FixedUpdate:
				if(IsDirty == false)
					m_UpdateCoroutine = Core.Start(implWaitFixedUpdateAndDo(updateCall));
			break;

			case Dirty.Clean:
			case Dirty.Dirty:
			case Dirty.None:
				implStopUpdate();
			break;
		}
		
		m_CurrentState = dirty;
	}

}

public enum Dirty
{
	None	= 0,
	Clean	= 1,
	Dirty	= 1<<1,
	RequireUpdate	= 1<<2 | Dirty,
	FixedUpdate		= 1<<3 | Dirty,
}


