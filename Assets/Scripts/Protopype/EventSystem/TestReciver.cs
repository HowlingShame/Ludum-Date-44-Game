using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestReciver : EventSystemReciver
{
	public override void iProcess(EventSystemManagerBase.Event e)
	{
		Debug.Log(gameObject.name);
	}
}
