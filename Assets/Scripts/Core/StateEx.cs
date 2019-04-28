using System;
using System.Collections.Generic;

[Serializable]
public class StateEx<StateType, TagType>
{
	public Queue<StateType>				m_StateStack = new Queue<StateType>();
	public StateType					m_CurrentState;
	public Action<StateType>			m_OnStateChange;
	public Tags<TagType>				m_Tags;

	//////////////////////////////////////////////////////////////////////////
	public void AddState(StateType state)
	{
		m_StateStack.Enqueue(state);
		m_CurrentState = state;
	}

	public void SetState(StateType state)
	{
		if(state.Equals(m_CurrentState) == false)
		{
			m_CurrentState = state;
			m_OnStateChange?.Invoke(state);
		}
		else
			m_CurrentState = state;
	}

	public void Clear()
	{
		m_StateStack.Clear();
		m_CurrentState = default(StateType);
	}
};

[Serializable]
public class StateEx<StateType>
{
	public Queue<StateType>				m_StateStack = new Queue<StateType>();
	public StateType					m_CurrentState;

	//////////////////////////////////////////////////////////////////////////
	public void AddState(StateType state)
	{
		m_StateStack.Enqueue(state);
		m_CurrentState = state;
	}

	public void SetState(StateType state)
	{
		m_CurrentState = state;
	}

	public void Clear()
	{
		m_StateStack.Clear();
		m_CurrentState = default(StateType);
	}
};

