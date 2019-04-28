using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventSystemManagerSimple : EventSystemManagerBase
{
	private List<EventSystemManagerBase.IEventListner>			m_Listners = new List<IEventListner>();

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		Instance = this;
	}

	//////////////////////////////////////////////////////////////////////////
	public override void Send(EventSystemManagerBase.Event e)
	{
		if(e == null)
			return;

		foreach(var n in m_Listners)
			n.iProcess(e);
	}

	public override void AddListner(EventSystemManagerBase.IEventListner listner)
	{
		if(m_Listners.Contains(listner) == false)
			m_Listners.Add(listner);
	}

	public override IEventListner GetEventListener(string name)
	{
		foreach(var n in m_Listners)
			if(n.Name == name)
				return n;

		return null;
	}

	public override void RemoveEventListener(IEventListner listener)
	{
		m_Listners.Remove(listener);
	}

	//////////////////////////////////////////////////////////////////////////
	public class TestListner : IEventListner
	{
		public string m_Name;
		public string Name
		{
			get
			{
				return m_Name;
			}
		}

		public void iProcess(Event e)
		{
			Debug.Log(Name);
		}
	}
	
	public class TestEvent : EventSystemManagerBase.Event
	{
	}

	[InspectorButton]
	void Test()
	{
		{
			Send(new TestEvent());
		}
		{
			m_Listners.Add(new TestListner(){ m_Name = "First" });
			m_Listners.Add(new TestListner(){ m_Name = "Second" });

			Send(new TestEvent());
		}
		{
			RemoveEventListener("First");
			Send(new TestEvent());
		}
	}
}
