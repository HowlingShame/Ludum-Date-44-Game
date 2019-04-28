using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventSystemReciver : GLMonoBehaviour, EventSystemManagerBase.IEventListner
{
	public string Name
	{
		get
		{
			return gameObject.name;
		}
	}

	public abstract void iProcess(EventSystemManagerBase.Event e);
}
