using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFilter : EventSystemFilter
{
	public bool m_Open;

	public override bool Check(EventSystemManagerBase.Event e)
	{
		return m_Open;
	}
}
