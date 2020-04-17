using System;
using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EventManager : GLMonoBehaviour
{
	public static EventManager		Instance;
    public const string             c_DefaultChannel = "";

	//////////////////////////////////////////////////////////////////////////
	public class Event 
	{
        public string Key { get; set; }

        //////////////////////////////////////////////////////////////////////////
        public T GetData<T>()
        {
            if (this is EventData eventData)
                return eventData.GetDataAs<T>();

            return default;
        }

        public Event()
		{
		}

		public Event(string key)
		{
			Key = key;
		}
	}

    public class EventData : Event
    {
        public object Data { get; set; }

        //////////////////////////////////////////////////////////////////////////
        public T GetDataAs<T>()
        {
            return (T)Data;
        }

        public EventData(string key, object data) : base(key)
        {
            Data = data;
        }
    }

	public interface IEventListener
	{
		string Name { get; }
        string Channel { get; }

		void iProcess(Event e);
	}

	public class EventListenerAction : IEventListener
	{
        private string      m_Name;
        private string      m_Key;
        private string      m_Channel;
        public Action       m_Action;

		public string Name => m_Name;
        public string Channel => m_Channel;

        //////////////////////////////////////////////////////////////////////////
		public void iProcess(Event e)
		{
            // if key matches invoke action
			if (e.Key == m_Key)
				m_Action.Invoke();
		}

		public EventListenerAction(string name, string key, Action action, string channel = "")
		{
			m_Name = string.IsNullOrEmpty(name) ? new Guid().ToString() : name ;
			m_Key = key;
            m_Channel = channel;
			m_Action = action;
		}
	}
	
	//////////////////////////////////////////////////////////////////////////
	protected abstract void implSend(Event e, string channel);

	protected abstract void implAddListener(IEventListener listener);

	protected abstract IEventListener implGetEventListener(string name);

	protected abstract void implRemoveEventListener(IEventListener listener);

    protected abstract List<IEventListener> GetEventListenerList();

	//////////////////////////////////////////////////////////////////////////
	public static void RemoveListener(string name)
	{
        // remove by name
		var el = Instance.implGetEventListener(name);
		if (el != null)
			Instance.implRemoveEventListener(el);
	}
    
    public static void RemoveListeners<T>()
    {
        // remove all of type
        RemoveListeners((listener) => listener is T);
    }

    public static void RemoveListeners(string name)
    {
        // remove with name
        RemoveListeners((listener) => listener.Name == name);
    }

    public static void RemoveListeners(Func<IEventListener, bool> condition)
    {
        // remove all matched listeners
        foreach (var eventListener in Instance.GetEventListenerList().ToList())
        {
            if (condition(eventListener))
                Instance.implRemoveEventListener(eventListener);
        }
    }

	public static void Send(string key)
	{
		Instance.implSend(new Event(key), c_DefaultChannel);
	}

    public static void Send<T>(string key, T data)
    {
        Instance.implSend(new EventData(key, data), c_DefaultChannel);
    }

    public static void Send<T>(string key, T data, string channel = c_DefaultChannel)
    {
        Instance.implSend(new EventData(key, data), channel);
    }
    
    public static void Send(params Event[] events)
    { 
        foreach (var n in events)
            Send(events);
    }
	
	public static void AddListener(IEventListener listener)
	{
		Instance.implAddListener(listener);
	}

    public static void AddReaction(string key, Action action, string name = "")
    {
        // add single time executed event
        var el = new EventListenerAction(name, key,  null);
        el.m_Action = () =>
        {
            // remove from event manager, invoke event
            RemoveListener(el);
            action.Invoke();
        };

        // add listener
        Instance.implAddListener(el);
    }

	public static void AddListener(string descKey, Action action, string name = "")
	{
		Instance.implAddListener(new EventListenerAction(name, descKey, action));
	}

	public static IEventListener GetEventListener(string name)
	{
		return Instance.implGetEventListener(name);
	}

	public static void RemoveListener(IEventListener listener)
	{
		Instance.implRemoveEventListener(listener);
	}
}