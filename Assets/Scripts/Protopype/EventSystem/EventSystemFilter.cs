using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventSystemFilter : MonoBehaviour
{
	public abstract bool Check(EventSystemManagerBase.Event e);
}
