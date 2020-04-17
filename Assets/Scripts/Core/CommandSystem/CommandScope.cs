public class CommandScope : CommandReceiver, ICommandProcess
{
	public sealed class CommandListenerScope : ICommandListenerLinked<CommandReceiver>
	{
		public CommandScope		m_Owner;
		//public ICommandListenerLinked<CommandReciver>	m_ParentValidator;

		//////////////////////////////////////////////////////////////////////////
		public void iAttach(CommandReceiver commandScope)
		{
			m_Owner = commandScope as CommandScope;
		}
		public void iDetach(CommandReceiver commandScope)
		{
			m_Owner = null;
		}

		public void iSucceeded(ICommand command)
		{
		}

		public void iFailed(ICommand command)
		{
			m_Owner.m_Validator.iFailed(m_Owner);
		}

		public void iInterrupted(ICommand command)
		{
		}

		public void iFinished()
		{
			m_Owner.m_Validator.iSucceeded(m_Owner);
		}
	}

	//////////////////////////////////////////////////////////////////////////
	protected CommandReceiver		m_CommandReceiver;
	protected ICommandValidator		m_Validator;
	//protected bool					m_UseParentInvoker;
	//protected bool					m_UseParentValidator;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Validator = validator;
		m_CommandReceiver = validator as CommandReceiver;
		/*if (m_CommandReciver != null)
		{	// write this if needed
			if (m_UseParentInvoker)
			if (m_UseParentValidator)
		}*/
		Start();
	}

	public void iStop()
	{
		Stop();
	}

	public CommandScope(ICommandInvoker m_CommandInvoker = null) : base(m_CommandInvoker, new CommandListenerScope())
	{
	}
};