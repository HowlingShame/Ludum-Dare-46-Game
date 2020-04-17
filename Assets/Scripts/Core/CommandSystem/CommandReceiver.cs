using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class CommandReceiver : ICommandValidator, IEnumerable<ICommand>
{
	private LinkedList<ICommand>		m_CommandList = new LinkedList<ICommand>();
	private LinkedListNode<ICommand>	m_Current;

	private ReceiverMode					m_ReceiverMode;
	public ReceiverMode						Mode => m_ReceiverMode;

	protected ICommandInvoker				m_CommandInvoker;
	public ICommandInvoker					CommandInvoker
	{
		get
		{
			return m_CommandInvoker;
		}
		set
		{
			m_CommandInvoker = value ?? new CommandInvokerDefault();
		}
	}

	protected ICommandListenerLinked<CommandReceiver>			m_CommandListener;
	public ICommandListenerLinked<CommandReceiver>				CommandNotifier
	{
		get
		{ 
			return m_CommandListener;
		}

		set
		{ 
			m_CommandListener.iDetach(this);
			m_CommandListener = value ?? new CommandListenerDefault<CommandReceiver>(); 
			m_CommandListener.iAttach(this);
		}
	}

	public ICommand						CurrentCommand => m_Current?.Value;

	private bool						m_Running;
	public bool							IsRunning => m_Running;

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public enum ReceiverMode
	{
		Container,
		Process
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandReceiver(ReceiverMode receiverMode, ICommandInvoker commandInvoker = null, ICommandListenerLinked<CommandReceiver> commandListener = null)
	{
		implConstructor(receiverMode, commandInvoker, commandListener);
	}

	public CommandReceiver(ICommandInvoker commandInvoker = null, ICommandListenerLinked<CommandReceiver> commandListener = null)
	{
		implConstructor(ReceiverMode.Container, commandInvoker, commandListener);
	}

	public void Start()
	{
		m_Current = m_CommandList.First;

		if (m_Current != null)
		{
			m_Running = true;
			m_CommandInvoker.iInvoke(m_Current.Value, this);
		}
		else
		{
			m_Running = false;
			m_CommandListener.iFinished();
		}
	}

	public void Continue()
	{
		if (m_Current == null)
			m_Current = m_CommandList.First;
		
			
		if (m_Current != null)
		{
			m_Running = true;
			m_CommandInvoker.iInvoke(m_Current.Value, this);
		}
		else
		{
			m_Running = false;
			m_CommandListener.iFinished();
		}
	}

	public void Stop()
	{
		if (m_Running == false)
			return;
		
		m_Running = false;
		(m_Current.Value as ICommandProcess)?.iStop();
		m_CommandListener.iInterrupted(m_Current.Value);
		m_Current = null;
	}

	public void Pause()
	{
		if (m_Current != null)
		{
			(m_Current.Value as ICommandProcess)?.iStop();
			m_CommandListener.iInterrupted(m_Current.Value);
			m_Running = false;
		}
	}

	public void Clear()
	{
		Stop();
		m_CommandList.Clear();
	}
	
	public void AddLast(Action action)
	{
		AddLast(new CommandAction(action));
	}

	public void AddLast(ICommand command)
	{
		switch (m_ReceiverMode)
		{
			case ReceiverMode.Container:
			{
				m_CommandList.AddLast(command);
			}	break;
			case ReceiverMode.Process:
			{
                m_CommandList.AddLast(command);
				if (IsRunning == false)
					Start();
			}	break;
		}
	}

	public void AddFirst(ICommand command)
	{
		switch (m_ReceiverMode)
		{
			case ReceiverMode.Container:
			{
				m_CommandList.AddFirst(command);
			}	break;
			case ReceiverMode.Process:
			{
				if (IsRunning)	AddNext(command);
				else			m_CommandList.AddFirst(command);
			}	break;
		}
	}

	public void AddNext(ICommand command)
	{
		if (m_Current == null)
		{
			AddLast(command);
			return;
		}

		m_CommandList.AddAfter(m_Current, command);
	}

	public void AddNext(params ICommand[] commands)
	{
		if (m_Current == null)
		{
			foreach (var n in commands)
				AddLast(n);
			return;
		}
		
		for(var n = commands.GetLength(0) - 1; n >= 0; -- n)
			m_CommandList.AddAfter(m_Current, commands[n]);
	}

	public void Remove(ICommand command)
	{
		var cmdNode = m_CommandList.Find(command);
		implRemove(cmdNode);
	}

	public void RemoveFirst()
	{
		if (m_CommandList.Count != 0)
			implRemove(m_CommandList.First);
	}

	public void RemoveLast()
	{
		if (m_CommandList.Count != 0)
			implRemove(m_CommandList.Last);
	}

	public void RemoveNext()
	{
		var next = m_Current?.Next;
		if (next != null)
			implRemove(next);
	}

    //////////////////////////////////////////////////////////////////////////
	public void iSucceeded(ICommand command)
	{
		if (m_Running == false)
			return;

		if (command != m_Current.Value)
			return;

		m_CommandListener.iSucceeded(m_Current.Value);

		implNextCommand();
	}

	public void iFailed(ICommand command)
	{
		if (m_Running == false)
			return;

		if (command != m_Current.Value)
			return;

		var failedCommand = m_Current.Value;
		switch (m_ReceiverMode)
		{
			case ReceiverMode.Container:
			{	// stop if container
				(m_Current.Value as ICommandProcess)?.iStop();
				m_Current = null;
				m_Running = false;
			}	break;
			case ReceiverMode.Process:
			{	// move next if process
				implNextCommand();
			}	break;
		}

		m_CommandListener.iFailed(failedCommand);
	}

	public IEnumerator GetEnumerator()
	{
		return m_CommandList.GetEnumerator();
	}

	IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator()
	{
		return m_CommandList.GetEnumerator();
	}

	//////////////////////////////////////////////////////////////////////////
	private void implNextCommand()
	{
		if (m_Current.Next != null)
		{
			// start next command
			m_Current = m_Current.Next;
			m_CommandInvoker.iInvoke(m_Current.Value, this);

			// remove executed command if process mode(always first)
			if (m_ReceiverMode == ReceiverMode.Process)
				RemoveFirst();
		}
		else
		{
			// no commands left
			m_Running = false;
			m_CommandListener.iFinished();
		}
	}

	private void implRemove(LinkedListNode<ICommand> cmdNode)
	{
		if (cmdNode != null)
		{
			if (cmdNode == m_Current)
			{
				// stop current, move to next command
				if (m_Running)
				{	// stop if running
					(m_Current.Value as ICommandProcess)?.iStop();
					m_CommandListener.iInterrupted(m_Current.Value);
				}

				// move next
				m_Current = m_Current.Next;

				// try execute next
				if (m_Running && m_Current != null)
					m_CommandInvoker.iInvoke(m_Current.Value, this);
				else
					m_Running = false;
			}

			m_CommandList.Remove(cmdNode);
		}
	}

	private void implConstructor(ReceiverMode receiverMode, ICommandInvoker commandInvoker,
		ICommandListenerLinked<CommandReceiver> commandListener)
	{
		this.m_ReceiverMode = receiverMode;

		this.m_CommandInvoker = commandInvoker ?? new CommandInvokerDefault();
		this.m_CommandListener = commandListener ?? new CommandListenerDefault<CommandReceiver>();

		this.m_CommandListener.iAttach(this);
	}
}