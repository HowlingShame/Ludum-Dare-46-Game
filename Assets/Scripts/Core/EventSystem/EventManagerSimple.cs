using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// just propagates received events to Listeners
[DefaultExecutionOrder(-1)]
public class EventManagerSimple : EventManager
{
	private List<IEventListener>			            m_Listeners = new List<IEventListener>();
    //private Dictionary<string, List<IEventListener>>    m_Listeners = new Dictionary<string, List<IEventListener>>();

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		Instance = this;

		GetComponentsInChildren(false, m_Listeners);
	}

	//////////////////////////////////////////////////////////////////////////
	protected override void implSend(Event e, string channel)
	{
		if (e == null)
			return;

		foreach (var n in m_Listeners.ToList())
			n.iProcess(e);
	}

	protected override void implAddListener(IEventListener listener)
	{
		if(m_Listeners.Contains(listener) == false)
			m_Listeners.Add(listener);
	}

	protected override IEventListener implGetEventListener(string name)
    {
        return m_Listeners.FirstOrDefault(n => n.Name == name);
    }

	protected override void implRemoveEventListener(IEventListener listener)
	{
		m_Listeners.Remove(listener);
	}

    protected override List<IEventListener> GetEventListenerList()
    {
        return m_Listeners;
    }
}
