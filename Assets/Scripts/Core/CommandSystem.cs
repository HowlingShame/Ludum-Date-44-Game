using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public abstract class BaseCommand : ICommand, ICommandValidator
//////////////////////////////////////////////////////////////////////////
public abstract class CommandBase : ICommand, ICommandValidator, IEnumerable
{
	public event Action		OnFailed;
	public event Action		OnSucceeded;
	private CommandBase		m_NextCommand;

	public CommandBase		Last
	{
		get
		{
			CommandBase last = this;
			while(last.m_NextCommand != null)
				last = last.m_NextCommand;

			return last;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private class Enumerator : IEnumerator
	{
		private CommandBase		m_Start;
		private CommandBase		m_Current;

		public object Current
		{
			get
			{
				return m_Current;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		public bool MoveNext()
		{
			m_Current = m_Current.m_NextCommand;
			return m_Current != null;
		}

		public void Reset()
		{
			m_Current = m_Start;
		}

		//////////////////////////////////////////////////////////////////////////
		public Enumerator(CommandBase m_Start)
		{
			this.m_Start = m_Start;
			this.m_Current = m_Start;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public virtual CommandBase Add(CommandBase command)
	{
		if(command == null)
		{
			return this;
		}
		
		CommandBase endOfPack = command.Last;

		endOfPack.m_NextCommand = m_NextCommand;
		m_NextCommand = command;

		return endOfPack;
	}

	public abstract void iStart(ICommandValidator validator);

	public void iSucceeded()
	{
		OnSucceeded?.Invoke();		
		if(m_NextCommand != null)
			m_NextCommand.Run();
	}

	public void iFailed()
	{
		OnFailed?.Invoke();
	}

	public void Run()
	{
		iStart(null);
	}

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
}

public abstract class CommandBaseEx : ICommand, ICommandValidator, IEnumerable
{
	public event Action		OnFailed;
	public event Action		OnSucceeded;
	private CommandBaseEx		m_NextCommand;
	private CommandBaseEx		m_PreviousCommand;

	public CommandBaseEx		Last
	{
		get
		{
			CommandBaseEx last = this;
			while(last.m_NextCommand != null)
				last = last.m_NextCommand;

			return last;
		}
	}

	public CommandBaseEx		First
	{
		get
		{
			CommandBaseEx first = this;
			while(first.m_PreviousCommand != null)
				first = first.m_PreviousCommand;

			return first;
		}
	}

	protected CommandSequenceInfo	m_CommandSequence;
	public CommandSequenceInfo		CommandSequence{ get{ return GetCommandSequence(); } private set{ m_CommandSequence = value;  m_CommandSequence.Add(this);} }
	
	//////////////////////////////////////////////////////////////////////////
	private class Enumerator : IEnumerator
	{
		private CommandBaseEx		m_Start;
		private CommandBaseEx		m_Current;

		public object Current
		{
			get
			{
				return m_Current;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		public bool MoveNext()
		{
			m_Current = m_Current.m_NextCommand;
			return m_Current != null;
		}

		public void Reset()
		{
			m_Current = m_Start;
		}

		//////////////////////////////////////////////////////////////////////////
		public Enumerator(CommandBaseEx m_Start)
		{
			this.m_Start = m_Start;
			this.m_Current = m_Start;
		}
	}
	
	public class CommandSequenceInfo
	{
		public bool				IsRunning{ get{ return Current != null; } }
		public int				Count { get; private set; }
		public CommandBaseEx		Current { get; private set; }

		protected CommandBaseEx	m_Command;
		
		public event Action		OnBreak;
		public event Action		OnStart
		{
			add
			{
				new FirstCommand(value, m_Command);
			}

			remove
			{
				throw new NotImplementedException();
			}
		}
		public event Action		OnFinish
		{
			add
			{
				new LastCommand(value, m_Command);
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		//////////////////////////////////////////////////////////////////////////
		public class LastCommand : CommandBaseEx
		{
			public Action		m_OnStart;

			//////////////////////////////////////////////////////////////////////////
			public override CommandBaseEx Add(CommandBaseEx command)
			{
				return m_PreviousCommand != null ? m_PreviousCommand.Add(command) : command.Add(this);
			}

			public override void iStart(ICommandValidator validator)
			{
				m_OnStart.Invoke();
				iSucceeded();
			}

			//////////////////////////////////////////////////////////////////////////
			public LastCommand(Action onStart, CommandBaseEx command)
			{
				Add(command.Last);
				this.m_OnStart = onStart;
			}
		}
		
		public class FirstCommand : CommandBaseEx
		{
			public Action		m_OnStart;

			//////////////////////////////////////////////////////////////////////////
			public override CommandBaseEx Add(CommandBaseEx command)
			{
				return m_NextCommand != null ? m_NextCommand.Add(command) : base.Add(command);
			}

			public override void iStart(ICommandValidator validator)
			{
				m_OnStart.Invoke();
				iSucceeded();
			}
			
//			public override void Connect(BaseCommandEx command)
//			{
//				if(m_NextCommand != null)
//				{
//					m_NextCommand.Connect(command);
//				}
//				else
//					command.m_NextCommand = null;
//
//				m_NextCommand = null;
//				m_PreviousCommand = null;
//				Add(command.First);
//			}

			//////////////////////////////////////////////////////////////////////////
			public FirstCommand(Action onStart, CommandBaseEx command)
			{
				Add(command.First);
				this.m_OnStart = onStart;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		public void Add(CommandBaseEx command)
		{
			Count++;
		}

		public void Remove(CommandBaseEx command)
		{
			command.m_CommandSequence = null;
			Count --;
		}

		public void Break()
		{
			OnBreak?.Invoke();
			Current = null;
		}

		public void RunningCommand(CommandBaseEx command)
		{ 
			Current = command;
		}

		public void Merge(CommandSequenceInfo commandSequence)
		{
			Count += commandSequence.Count;
			OnBreak += commandSequence.OnBreak;
		}

		public IEnumerable<CommandBaseEx> GetEnumerable()
		{
			for(var n = m_Command.First; n != null; n = n.m_NextCommand)
				yield return n;
		}
		
		//////////////////////////////////////////////////////////////////////////
		public CommandSequenceInfo(CommandBaseEx m_Command)
		{
			this.m_Command = m_Command;
		}
	}

	protected class CommandFirstLast
	{
		public CommandBaseEx		m_First;
		public CommandBaseEx		m_Last;
	}

	//////////////////////////////////////////////////////////////////////////
	public abstract void iStart(ICommandValidator validator);
	
	public virtual CommandBaseEx Add(CommandBaseEx command)
	{
		if(command == null)
		{
			return this;
		}
		
		if(m_CommandSequence != command.m_CommandSequence)
			if(m_CommandSequence != null && command.m_CommandSequence != null)		// merge sequence info
			{
				m_CommandSequence.Merge(command.m_CommandSequence);
				for(var forward = command; forward != null; forward = forward.m_NextCommand)
					forward.CommandSequence = m_CommandSequence;
			}
			else if(m_CommandSequence == null && command.m_CommandSequence != null)	// assign sequence info
			{
				CommandSequence = command.m_CommandSequence;
				for(var back = m_PreviousCommand; back != null; back = back.m_PreviousCommand)
					back.CommandSequence = m_CommandSequence;
				for(var forward = m_NextCommand; forward != null; forward = forward.m_NextCommand)
					forward.CommandSequence = m_CommandSequence;
			}
			else if(m_CommandSequence != null && command.m_CommandSequence == null)
			{
				for(var forward = command; forward != null; forward = forward.m_NextCommand)
					forward.CommandSequence = m_CommandSequence;
			}
		
		CommandBaseEx endOfPack = command.Last;

		m_NextCommand?.Connect(endOfPack);
		command.Connect(this);

		return endOfPack;
	}

	public CommandBaseEx Add(ICommand command)
	{
		return Add(new CommandExWrapperICommand(command));
	}

	public void iSucceeded()
	{
		OnSucceeded?.Invoke();		
		if(m_NextCommand != null)
			m_NextCommand.Run();
		else
			m_CommandSequence?.RunningCommand(null);
	}

	public void iFailed()
	{
		OnFailed?.Invoke();
		m_CommandSequence?.Break();
	}

	public void Run()
	{
		m_CommandSequence?.RunningCommand(this);
		iStart(null);
	}
	
	public void RunFromStart()
	{
		First.Run();
	}

	public void Remove()
	{
		if(m_PreviousCommand != null)
			if(m_NextCommand != null)
			{
				m_PreviousCommand.m_NextCommand = m_NextCommand;
				m_NextCommand.m_PreviousCommand = m_PreviousCommand;

				m_NextCommand = null;
				m_PreviousCommand = null;
			}
			else
			{
				m_PreviousCommand.m_NextCommand = null;
				m_PreviousCommand = null;
			}
		else 
		if(m_NextCommand != null)
		{
			m_NextCommand.m_PreviousCommand = null;
			m_NextCommand = null;
		}

		m_CommandSequence?.Remove(this);
	}

	public void Stop()
	{
		(this as ICommandProcess)?.iStop();
	}

	private CommandSequenceInfo GetCommandSequence()
	{
		if(m_CommandSequence == null)
		{
			m_CommandSequence = new CommandSequenceInfo(this);
			for(var back = m_PreviousCommand; back != null; back = back.m_PreviousCommand)
				back.CommandSequence = m_CommandSequence;
			for(var forward = this; forward != null; forward = forward.m_NextCommand)
				forward.CommandSequence = m_CommandSequence;
		}

		return m_CommandSequence;
	}
	
	private void Connect(CommandBaseEx command)
	{
		m_PreviousCommand = command;
		command.m_NextCommand = this;
	}

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(First);
	}

	//////////////////////////////////////////////////////////////////////////
	public static CommandBaseEx Build(params CommandBaseEx[] commands)
	{
		var result = commands[0];
		for(var n = 1; n < commands.Length; ++ n)
			result.Add(commands[n]);

		return result;
	}
}

//////////////////////////////////////////////////////////////////////////
// Wrapper's for CommandPack
public class CommandWrapperICommand : CommandBase
{
	public ICommand		m_Command;

	//////////////////////////////////////////////////////////////////////////
	public override void iStart(ICommandValidator validator)
	{
		m_Command.iStart(this);
	}
	
	//////////////////////////////////////////////////////////////////////////
	public CommandWrapperICommand(ICommand command)
	{
		this.m_Command = command;
	}
}

public class CommandExWrapperICommand : CommandBaseEx
{
	public ICommand		m_Command;
	
	//////////////////////////////////////////////////////////////////////////
	public override void iStart(ICommandValidator validator)
	{
		m_Command.iStart(this);
	}
	
	//////////////////////////////////////////////////////////////////////////
	public CommandExWrapperICommand(ICommand command)
	{
		this.m_Command = command;
	}
}

// Simple Commands
public class WaitCommand : ICommandProcess
{
	protected Coroutine	m_Coroutine;
	public float		m_Time;
	
	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Coroutine = Core.StartWaitTimeAndDo(m_Time, () => validator.iSucceeded());
	}

	public void iStop()
	{
		Core.Instance.StopCoroutine(m_Coroutine);
	}

	//////////////////////////////////////////////////////////////////////////
	public WaitCommand(float m_Time)
	{
		this.m_Time = m_Time;
	}
}

public class CoroutineCommand : ICommandProcess
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
	public CoroutineCommand(Func<ICommandValidator, IEnumerator> coroutine)
	{
		m_Action = coroutine;
	}
}

public class LogCommand : ICommand
{
	public string		m_LogText;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		Debug.Log(m_LogText);
		validator.iSucceeded();
	}

	//////////////////////////////////////////////////////////////////////////
	public LogCommand(string m_LogText)
	{
		this.m_LogText = m_LogText;
	}
}

public class FailedCommand : ICommand
{
	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		Debug.Log("Failed");
		validator.iFailed();
	}
}

public class LambdaCommand : ICommand
{
	public Action		m_Action;

	//////////////////////////////////////////////////////////////////////////
	public void iStart(ICommandValidator validator)
	{
		m_Action?.Invoke();
	}

	//////////////////////////////////////////////////////////////////////////
	public LambdaCommand(Action m_Action)
	{
		this.m_Action = m_Action;
	}
}

//////////////////////////////////////////////////////////////////////////
public interface ICommandValidator
{
	void iSucceeded();
	void iFailed();
}

public interface ICommandInvoker
{
	void Invoke(ICommand command, ICommandValidator validator);
}

public interface ICommand
{
	void iStart(ICommandValidator validator);
}

public interface ICommandProcess : ICommand
{
	void iStop();
}

public interface IRouteKeeper
{
	List<IWorldPosition> iGetRoute();
}

//////////////////////////////////////////////////////////////////////////
public class CommandWrapperTask : ICommand, ICommandValidator
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
		m_Task.iStart(this);
	}

	public void iSucceeded()
	{
		OnSucceeded?.Invoke();
		m_TaskHandle.iSucceeded();
	}

	public void iFailed()
	{
		OnFailed?.Invoke();
		m_TaskHandle.iFailed();
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandWrapperTask(ICommand task, Action onStart = null, Action onSucceeded = null, Action onFailed = null)
	{
		OnStart = onStart;
		OnSucceeded = onSucceeded;
		OnFailed = onFailed;

		m_Task = task;
	}
}

[Serializable]
public class CommandPack : ICommandValidator, IEnumerable
{
	public List<Task>		m_TaskList = new List<Task>();
	public Task				m_CurrentTask;
	public int				m_CurrentTaskIndex;
	public event Action		m_OnFail;
	public event Action		m_OnSuccess;

	public State		State
	{
		get
		{
			return m_CurrentTask.m_State;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public class Task
	{
		public State		m_State;
		public ICommand		m_Task;
	}

	//////////////////////////////////////////////////////////////////////////
	public void Start()
	{
		Start(0);
	}

	public void Start(int index)
	{
		m_CurrentTaskIndex = index;
		if(m_CurrentTaskIndex < m_TaskList.Count)
		{
			m_CurrentTask = m_TaskList[m_CurrentTaskIndex];
			m_CurrentTask.m_State = State.Running;

			m_CurrentTask.m_Task.iStart(this);
		}
	}
	
	public CommandPack Add(ICommand command)
	{
		m_TaskList.Add(new Task(){ m_State = State.None, m_Task = command });
		return this;
	}

	public CommandPack Inject(CommandPack taskHandle)
	{
		m_TaskList.AddRange(taskHandle.m_TaskList);
		m_OnFail += taskHandle.m_OnFail;
		m_OnSuccess += taskHandle.m_OnSuccess;

		return this;
	}

	public void iSucceeded()
	{
		m_CurrentTask.m_State = State.Succeeded;

		Start(m_CurrentTaskIndex + 1);

		if(m_CurrentTaskIndex == m_TaskList.Count)
			m_OnSuccess?.Invoke();
	}

	public void iFailed()
	{
		m_CurrentTask.m_State = State.Failed;
		m_OnFail?.Invoke();
	}

	public IEnumerator GetEnumerator()
	{
		return m_TaskList.GetEnumerator();
	}
};


public class CommandReciver : ICommandValidator, IEnumerable
{
	public LinkedList<ICommand>		m_CommandList = new LinkedList<ICommand>();
	public LinkedListNode<ICommand>	m_Current;

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

	private ICommandNotifier			m_CommandNotifier;
	public ICommandNotifier				CommandNotifier
	{
		get
		{ 
			return m_CommandNotifier;
		}

		set
		{ 
			m_CommandNotifier.Detach(this);
			m_CommandNotifier = value; 
			m_CommandNotifier.Attach(this);
		}
	}

	public ICommand						CurrentCommand
	{
		get{ return m_Current?.Value; }
	}

	public bool IsRunning
	{ 
		get
		{
			return m_Current != null;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public interface ICommandNotifier
	{
		void Attach(CommandReciver commandReciver);
		void Detach(CommandReciver commandReciver);

		void Succeeded();
		void Failed();
		void Interrupted();
	}

	public class CommandInvokerDefault : ICommandInvoker
	{
		public void Invoke(ICommand command, ICommandValidator validator)
		{
			command.iStart(validator);
		}
	}

	public class CommandNotifierDefault : ICommandNotifier
	{
		public void Attach(CommandReciver commandReciver)
		{
		}

		public void Detach(CommandReciver commandReciver)
		{
		}

		public void Succeeded()
		{
		}

		public void Failed()
		{
		}

		public void Interrupted()
		{
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public void Start()
	{
		m_Current = m_CommandList.First;
		m_Current.Value.iStart(this);
	}

	public void Stop()
	{
		if(m_Current != null)
		{
			(m_Current.Value as ICommandProcess)?.iStop();
			m_CommandNotifier.Interrupted();
		}

		m_Current = null;
	}

	public void Clear()
	{
		Stop();
		m_CommandList.Clear();
	}

	public void AddLast(ICommand command)
	{
		m_CommandList.AddLast(command);
	}

	public void AddFirst(ICommand command)
	{
		m_CommandList.AddFirst(command);
	}

	public void AddNext(ICommand command)
	{
		if(m_Current == null)
		{
			AddLast(command);
			return;
		}

		m_CommandList.AddAfter(m_Current, command);
	}

	public void AddNext(params ICommand[] commands)
	{
		if(m_Current == null)
		{
			foreach(var n in commands)
				AddLast(n);
			return;
		}
		
		for(var n = commands.GetLength(0) - 1; n >= 0; -- n)
			m_CommandList.AddAfter(m_Current, commands[n]);
	}

	//////////////////////////////////////////////////////////////////////////
	public void iSucceeded()
	{
		m_Current = m_Current?.Next;

		if(m_Current == null)			m_CommandNotifier.Succeeded();
		else							m_Current.Value.iStart(this);
	}

	public void iFailed()
	{
		(m_Current.Value as ICommandProcess)?.iStop();
		m_CommandNotifier.Failed();
		m_Current = null;
	}

	public IEnumerator GetEnumerator()
	{
		return m_CommandList.GetEnumerator();
	}

	//////////////////////////////////////////////////////////////////////////
	public CommandReciver(ICommandInvoker m_CommandInvoker = null, ICommandNotifier m_CommandNotifier = null)
	{
		this.m_CommandInvoker = m_CommandInvoker ?? new CommandInvokerDefault();
		this.m_CommandNotifier = m_CommandNotifier ?? new CommandNotifierDefault();
		this.m_CommandNotifier.Attach(this);
	}
}