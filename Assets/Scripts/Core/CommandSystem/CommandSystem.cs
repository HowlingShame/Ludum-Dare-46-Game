using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

// Commands
public class CommandWait : ICommandProcess
{
	protected MonoBehaviour		m_CoroutineOwner;
	protected Coroutine			m_Coroutine;
	public float				m_Time;
	
	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Coroutine = m_CoroutineOwner.StartCoroutine(Core.WaitAndDo(m_Time, () => validator.iSucceeded(this)));
	}

	public void iStop()
	{
		m_CoroutineOwner.StopCoroutine(m_Coroutine);
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandWait(float m_Time)
	{
		this.m_Time = m_Time;
		this.m_CoroutineOwner = Core.Instance;
	}

	public CommandWait(float m_Time, MonoBehaviour m_CoroutineOwner)
	{
		this.m_Time = m_Time;
		this.m_CoroutineOwner = m_CoroutineOwner ?? Core.Instance;
	}
}

public class CommandCoroutine : ICommandProcess
{
	protected Coroutine								m_Coroutine;
	protected Func<ICommandValidator, IEnumerator>	m_Action;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Coroutine = Core.Instance.StartCoroutine(m_Action(validator));
	}

	public void iStop()
	{
		Core.Instance.StopCoroutine(m_Coroutine);
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandCoroutine(Func<ICommandValidator, IEnumerator> coroutine)
	{
		m_Action = coroutine;
	}
}

public class CommandTaskProcess : ICommandProcess
{
	public Action							m_Activation;
	public CancellationTokenSource			m_TokenSource;

	public ICommandValidator				m_Validator;

	public Task								m_Task;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Validator = validator;
		
		m_Task = new Task(m_Activation);
		Core.Instance.m_TaskManager.AddTask(m_Task);
	}

	public void iStop()
	{
		if (m_TokenSource == null)
			return;

		lock (m_TokenSource)
		{

			m_TokenSource.Cancel();
			m_TokenSource.Dispose();
			m_TokenSource = null;
		}

		Core.Instance.m_TaskManager.RemoveTask(m_Task);
	}
	
	public CommandTaskProcess(Action<CancellationToken> task)
	{
		m_Activation = async () =>
		{
			if (m_TokenSource != null)
				iStop();		// restart if running

			m_TokenSource = new CancellationTokenSource();

			// run task
			var token = m_TokenSource.Token;
			await Task.Run(() => task(token), token);
			
			// could be complete of iStop
			if (m_TokenSource != null)
			{
				// clear data
				lock (m_TokenSource)
				{
					m_TokenSource.Dispose();
					m_TokenSource = null;
				}

				// notify commandValidator
				m_Validator.iSucceeded(this);
			}
		};
	}

	public CommandTaskProcess(Func<CancellationToken, bool> task)
	{
		m_Activation = async () =>
		{
			if (m_TokenSource != null)
				iStop();		// restart if running

			m_TokenSource = new CancellationTokenSource();

			// run task
			var token = m_TokenSource.Token;
			var result = await Task.Run(() => task(token), token);
			
			// could be complete of iStop
			if (m_TokenSource != null)
			{
				// clear data
				m_TokenSource.Dispose();
				m_TokenSource = null;

				// notify commandValidator
				if (result)
					m_Validator.iSucceeded(this);
				else
					m_Validator.iFailed(this);
			}
		};
	}
}

public class CommandTask : ICommand
{
	public Action							m_Activation;
	public ICommandValidator				m_Validator;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Validator = validator;
		
		m_Activation.Invoke();
	}

	public CommandTask(Action task)
	{
		m_Activation = async () =>
		{
			// run task
			await Task.Run(task);
			
			m_Validator.iSucceeded(this);
		};
	}
	public CommandTask(Func<bool> task)
	{
		m_Activation = async () =>
		{
			var result = await Task.Run(task);
			
			// notify commandValidator
			if (result)		m_Validator.iSucceeded(this);
			else			m_Validator.iFailed(this);
		};
	}
}

public class CommandLeanTween : ICommandProcess
{
    private LTDescr				m_LeanTween;
    private LTSeq               m_Seq;
    private ICommandValidator   m_CommandValidator;

    //////////////////////////////////////////////////////////////////////////
    public void iStart(ICommandValidator validator)
    {
        m_CommandValidator = validator;
        m_LeanTween.resume();
    }

    public void iStop()
    {
        // memory leak 
        //m_LeanTween.reset();
        //m_LeanTween.pause();

        // may not work
        //m_MoveID = m_LTDesc.id;
        LeanTween.cancel(m_LeanTween.id, false);
    }

    public CommandLeanTween(LTDescr leanTween)
    {
        m_LeanTween = leanTween;
        m_LeanTween.pause();

        m_Seq = LeanTween.sequence();
        m_Seq
            .append(m_LeanTween)
            .append(() => m_CommandValidator.iSucceeded(this));
    }
}

public class CommandLog : ICommand
{
	public string		m_LogText;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		Debug.Log(m_LogText);
		validator.iSucceeded(this);
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandLog(string m_LogText)
	{
		this.m_LogText = m_LogText;
	}
}

public class CommandFailed : ICommand
{
	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		validator.iFailed(this);
	}
}

public class CommandSucceeded : ICommand
{
	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		validator.iSucceeded(this);
	}
}

public class CommandAction : ICommand
{
	public Action		m_Action;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Action?.Invoke();
		validator.iSucceeded(this);
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandAction(Action m_Action)
	{
		this.m_Action = m_Action;
	}
}

public class CommandActionDelayed : ICommand
{
	public Action		m_Action;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		validator.iSucceeded(this);
		m_Action?.Invoke();
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandActionDelayed(Action m_Action)
	{
		this.m_Action = m_Action;
	}
}

public class CommandActionPersistent : ICommand
{
	public Action		m_Action;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Action?.Invoke();
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandActionPersistent(Action m_Action)
	{
		this.m_Action = m_Action;
	}
}

public class CommandCicle : ICommandProcess, ICommandValidator
{
	public ICommand				m_Action;
	public ICommandValidator	m_CommandValidator;

	public int					m_IterationsCurrent;
	public int					m_IterationsCount;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_CommandValidator = validator;
		iSucceeded(this);
	}

	public void iStop()
	{
		m_IterationsCurrent = 0;
		(m_Action as ICommandProcess)?.iStop();
	}

	public void iSucceeded(ICommand command)
	{
		if (m_IterationsCurrent >= m_IterationsCount)
		{
			m_CommandValidator.iSucceeded(command);
		}
		else
		{
			m_IterationsCount ++;
			m_Action.iStart(this);
		}
	}

	public void iFailed(ICommand command)
	{
		m_CommandValidator.iFailed(command);
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandCicle(ICommand m_Action, int m_IterationsCount)
	{
		this.m_Action = m_Action;
		this.m_IterationsCount = m_IterationsCount;
	}
}

public class CommandConditionalBranching : ICommandProcess
{
	public ICommand				m_ActionTrue;
	public ICommand				m_ActionFalse;
	public ICommand				m_ActionSelected;

	public Func<bool>			m_Condition;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		if (m_Condition())		m_ActionSelected = m_ActionTrue;
		else					m_ActionSelected = m_ActionFalse;

		m_ActionSelected.iStart(validator);
	}

	public void iStop()
	{
		(m_ActionSelected as ICommandProcess)?.iStop();
		m_ActionSelected = null;
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandConditionalBranching(Func<bool> m_Condition, ICommand m_ActionTrue, ICommand m_ActionFalse)
	{
		this.m_Condition = m_Condition;
		this.m_ActionTrue = m_ActionTrue;
		this.m_ActionFalse = m_ActionFalse;

		m_ActionSelected = null;
	}
}

//////////////////////////////////////////////////////////////////////////
public interface ICommandValidator
{
	void iSucceeded(ICommand command);
	void iFailed(ICommand command);
}

public interface ICommand
{
	void iStart(ICommandValidator validator);
}

public interface ICommandProcess : ICommand
{
	void iStop();
}

public interface ICommandProcessFlexible : ICommandProcess, ICommandListenerLinked<ICommand>
{
}

public interface ICommandInvoker
{
	void iInvoke(ICommand command, ICommandValidator validator);
}

public interface ICommandListener
{
	void iSucceeded(ICommand command);
	void iFailed(ICommand command);			// calls on failed ending
	void iInterrupted(ICommand command);	// calls on pause or stop
	void iFinished();						// calls on succeeded ending
}

public interface ICommandListenerLinked<T> : ICommandListener
{
	void iAttach(T commandScope);
	void iDetach(T commandRunner);
}

public class CommandInvokerDefault : ICommandInvoker
{
	public void iInvoke(ICommand command, ICommandValidator validator)
	{
		command.iStart(validator);
	}
}

public sealed class CommandListenerDefault<T> : ICommandListenerLinked<T>
{
	public void iAttach(T commandScope){}
	public void iDetach(T commandReciver){}
	public void iSucceeded(ICommand command){}
	public void iFailed(ICommand command){}
	public void iInterrupted(ICommand command){}
	public void iFinished(){}
}

public sealed class CommandListenerDelegate<T> : ICommandListenerLinked<T>
{
	public Action	m_OnInterrupted;
	public Action	m_OnFinished;
	public Action	m_OnSucceeded;
	public Action	m_OnFailed;

	public void iAttach(T commandScope){}
	public void iDetach(T commandReciver){}
	public void iSucceeded(ICommand command){m_OnSucceeded?.Invoke();}
	public void iFailed(ICommand command){m_OnInterrupted?.Invoke();}
	public void iInterrupted(ICommand command){m_OnInterrupted?.Invoke();}
	public void iFinished(){m_OnFinished?.Invoke();}

	public CommandListenerDelegate(Action onInterrupted = null, Action onFinished = null, Action onSucceeded = null, Action onFailed = null)
	{
		m_OnInterrupted = onInterrupted;
		m_OnFinished = onFinished;
		m_OnSucceeded = onSucceeded;
		m_OnFailed = onFailed;
	}
}

public class CommandListenerReseter<T> : ICommandListenerLinked<T>
{
	public Action		m_ResetAction;

	//////////////////////////////////////////////////////////////////////////
	public void iAttach(T commandScope)
	{
	}

	public void iDetach(T commandRunner)
	{
	}

	public void iFailed(ICommand command)
	{
		iFinished();
	}

	public void iFinished()
	{
		m_ResetAction?.Invoke();
	}

	public void iInterrupted(ICommand command)
	{
	}

	public void iSucceeded(ICommand command)
	{
	}

	public CommandListenerReseter(Action resetAction = null)
	{
		m_ResetAction = resetAction;
	}
}

public sealed class CommandListenerFlexibleProcessManager<T> : ICommandListenerLinked<T>
{
	public List<ICommandProcessFlexible>	m_FlexibleProcessList = new List<ICommandProcessFlexible>();

	//////////////////////////////////////////////////////////////////////////
	public void iAttach(T commandScope){}
	public void iDetach(T commandReciver){}

	public void iSucceeded(ICommand command)
	{
		foreach (var n in m_FlexibleProcessList)
			n.iSucceeded(command);

		if (command is ICommandProcessFlexible flexibleProcess)
			m_FlexibleProcessList.Add(flexibleProcess);
	}

	public void iFailed(ICommand command)
	{
		foreach (var n in m_FlexibleProcessList)
			n.iFailed(command);

		m_FlexibleProcessList.Clear();
	}

	public void iInterrupted(ICommand command)
	{
		foreach (var n in m_FlexibleProcessList)
			n.iInterrupted(command);

		m_FlexibleProcessList.Clear();
	}

	public void iFinished()
	{
		foreach (var n in m_FlexibleProcessList)
			n.iFinished();
		
		m_FlexibleProcessList.Clear();
	}
}

public class CommandNotifierRedirect<T> : ICommandListenerLinked<T>
{
	public ICommandListenerLinked<T>		m_RedirectionTarget;
	
	public void iAttach(T commandScope){}
	public void iDetach(T commandReciver){}
	public void iSucceeded(ICommand command)
	{
		if (command != null)
			m_RedirectionTarget.iSucceeded(command);
	}
	public void iFailed(ICommand command) => m_RedirectionTarget.iFailed(command);
	public void iInterrupted(ICommand command) => m_RedirectionTarget.iInterrupted(command);

	public void iFinished()
	{
		m_RedirectionTarget.iFinished();
	}

	public CommandNotifierRedirect(ICommandListenerLinked<T> redirectionTarget)
	{
		m_RedirectionTarget = redirectionTarget;
	}
}

//////////////////////////////////////////////////////////////////////////
public class CommandWrapperCallback : ICommand, ICommandValidator
{
	protected ICommandValidator		m_TaskHandle;
	protected ICommand				m_Task;
	public event Action				OnStart;
	public event Action				OnSucceeded;
	public event Action				OnFailed;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator taskHandle)
	{
		m_TaskHandle = taskHandle;
		OnStart?.Invoke();
		m_Task.iStart(this);
	}

	public void iSucceeded(ICommand command)
	{
		OnSucceeded?.Invoke();
		m_TaskHandle.iSucceeded(command);
	}

	public void iFailed(ICommand command)
	{
		OnFailed?.Invoke();
		m_TaskHandle.iFailed(command);
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandWrapperCallback(ICommand task, Action onStart = null, Action onSucceeded = null, Action onFailed = null)
	{
		OnStart = onStart;
		OnSucceeded = onSucceeded;
		OnFailed = onFailed;

		m_Task = task;
	}
}
