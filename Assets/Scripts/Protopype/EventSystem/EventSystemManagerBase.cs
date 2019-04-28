using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventSystemManagerBase : GLMonoBehaviour
{
	public static EventSystemManagerBase		Instance;

	//////////////////////////////////////////////////////////////////////////
	public abstract class Event {}

	public interface IEventListner
	{
		string Name{ get; }

		void iProcess(Event e);
	}
	
	//////////////////////////////////////////////////////////////////////////
	public abstract void Send(Event e);

	public abstract void AddListner(IEventListner listner);

	public abstract IEventListner GetEventListener(string name);

	public abstract void RemoveEventListener(IEventListner listener);

	//////////////////////////////////////////////////////////////////////////
	public void Send(params Event[] e)
	{ 
		foreach(var n in e)
			Send(e);
	}
	
	public void RemoveEventListener(string name)
	{
		var el = GetEventListener(name);
		if(el != null)
			RemoveEventListener(el);
	}
}