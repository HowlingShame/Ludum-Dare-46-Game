using System.Collections;
using System.Collections.Generic;

public class CommandPack : ICommandProcess, ICommandValidator, IEnumerable<ICommand>
{
	private State						m_State = State.None;
	private ICommandValidator			m_CommandValidator;
	private LinkedList<ICommand>		m_CommandList = new LinkedList<ICommand>();
	private LinkedListNode<ICommand>	m_CurrentCommand;

	private ICommandInvoker				m_CommandInvoker;
	public ICommandInvoker				CommandInvoker
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

	private ICommandListenerLinked<CommandPack>			m_CommandNotifier;
	public ICommandListenerLinked<CommandPack>			CommandNotifier
	{
		get
		{ 
			return m_CommandNotifier;
		}

		set
		{ 
			m_CommandNotifier.iDetach(this);
			m_CommandNotifier = value; 
			m_CommandNotifier.iAttach(this);
		}
	}

	public ICommand				CurrentCommand => m_CurrentCommand?.Value;
	public ICommandValidator	CommandValidator => m_CommandValidator;
	public State				State => m_State;

	//////////////////////////////////////////////////////////////////////////
	public CommandPack AddLast(ICommand command)
	{
		m_CommandList.AddLast(command);
		return this;
	}

	public CommandPack AddFirst(ICommand command)
	{
		m_CommandList.AddLast(command);
		return this;
	}
	
	public CommandPack AddAfter(ICommand key, ICommand command)
	{
		for (var n = m_CommandList.First; n != null; n = n.Next)
			if (n.Value == key)
			{
				m_CommandList.AddAfter(n, command);
				break;
			}

		return this;
	}
	
	public CommandPack AddNext(ICommand command)
	{
		if (m_CurrentCommand == null)
		{
			m_CommandList.AddLast(command);
			return this;
		}

		m_CommandList.AddAfter(m_CurrentCommand, command);

		return this;
	}

	public CommandPack AddNext(params ICommand[] commands)
	{
		if (m_CurrentCommand == null)
		{
			foreach (var n in commands)
				AddLast(n);
			return this;
		}
		
		for (var n = commands.GetLength(0) - 1; n >= 0; -- n)
			m_CommandList.AddAfter(m_CurrentCommand, commands[n]);
		
		return this;
	}

	public void iSucceeded(ICommand command)
	{
		if (m_CurrentCommand.Next != null)
		{
			m_CommandNotifier.iSucceeded(m_CurrentCommand.Value);
			m_CurrentCommand = m_CurrentCommand.Next;
			m_CommandInvoker.iInvoke(m_CurrentCommand.Value, this);
		}
		else
		{
			m_State = State.Succeeded;
			m_CommandNotifier.iSucceeded(null);
			m_CommandValidator?.iSucceeded(command);
		}
	}

	public void iFailed(ICommand command)
	{
		m_State = State.Failed;
		m_CommandNotifier.iFailed(m_CurrentCommand.Value);
		m_CommandValidator?.iFailed(command);
	}

	public void iStop()
	{
		if (m_CurrentCommand != null && m_State != State.Interrupted)
		{
			m_State = State.Interrupted;
			(m_CurrentCommand.Value as ICommandProcess)?.iStop();
			m_CommandNotifier.iInterrupted(m_CurrentCommand.Value);
		}
	}

	public void iStart(ICommandValidator validator)
	{
		m_CommandValidator = validator;
		if (m_CommandList.Count > 0)		// continue or start from first
		{
			m_State = State.Running;
			m_CurrentCommand = m_CurrentCommand ?? m_CommandList.First;
			m_CommandInvoker.iInvoke(m_CurrentCommand.Value, this);
		}
	}

	public void iStart()
	{
		iStart(null);
	}

	IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator()
	{
		return m_CommandList.GetEnumerator();
	}

	public IEnumerator GetEnumerator()
	{
		return m_CommandList.GetEnumerator();
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandPack(ICommandInvoker m_CommandInvoker = null, ICommandListenerLinked<CommandPack> m_CommandNotifier = null)
	{
		this.m_CommandInvoker = m_CommandInvoker ?? new CommandInvokerDefault();
		this.m_CommandNotifier = m_CommandNotifier ?? new CommandListenerDefault<CommandPack>();
		this.m_CommandNotifier.iAttach(this);
	}
};