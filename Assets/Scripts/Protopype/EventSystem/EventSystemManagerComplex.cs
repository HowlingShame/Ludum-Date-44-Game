using Gamelogic.Extensions;
using Malee;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemManagerComplex : EventSystemManagerSimple
{
	[SerializeField]
	private bool						m_PropagationChild = true;	
	[SerializeField, Reorderable(null, "Propagation Nodes", null)]
	private PropagationNodeReorderableList					m_PropagationNodes;	private List<EventSystemReciver>						m_Recivers = new List<EventSystemReciver>();

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class PropagationNodeReorderableList : ReorderableArray<EventSystemNode>	{
		public PropagationNodeReorderableList()	{}
		public PropagationNodeReorderableList(params EventSystemNode[] data) : base(data)	{}
	}
	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		Instance = this;

		GetComponentsInChildren<EventSystemReciver>(true, m_Recivers);

		if(m_PropagationChild)
			foreach(var n in transform.GetChildren())
			{
				if(n.gameObject.activeSelf)
				{
					var nodes = n.GetComponents<EventSystemNode>();
					foreach(var c in nodes)
						if(m_PropagationNodes.Contains(c) == false)
							m_PropagationNodes.Add(c);
				}
			}
	}

	//////////////////////////////////////////////////////////////////////////
	public override IEventListner GetEventListener(string name)
	{
		foreach(var n in m_Recivers)
			if(n.name == name)
				return n;

		return base.GetEventListener(name);;
	}

	public override void RemoveEventListener(IEventListner listener)
	{

		base.RemoveEventListener(listener);
	}
	
	public override void Send(EventSystemManagerBase.Event e)
	{
		if(e == null)
			return;

		foreach(var n in m_PropagationNodes)
			n?.PropagateEvent(e);
		
		base.Send(e);
	}
	//////////////////////////////////////////////////////////////////////////
	public void ActivateNode(IEventListner listener)
	{
		var el = listener as EventSystemReciver;
		if(el == null)
			return;

		var node = el.GetComponent<EventSystemNode>();
		if(node == null)
			return;

		if(m_Recivers.Contains(el))
		{
			foreach(var n in m_Recivers)
			{
				var rNode = n.GetComponent<EventSystemNode>();
				if(rNode.HasConnection(node))
					rNode.ActivateNode(node);
			}
		}
	}

	public void DeactivateNode(IEventListner listener)
	{		
		var el = listener as EventSystemReciver;
		if(el == null)
			return;

		var node = el.GetComponent<EventSystemNode>();
		if(node == null)
			return;

		if(m_Recivers.Contains(el))
		{
			foreach(var n in m_Recivers)
			{
				var rNode = n.GetComponent<EventSystemNode>();
				if(rNode.HasConnection(node))
					rNode.DeactivateNode(node);
			}
		}
	}

	public void ActivateNode(string name)
	{
		ActivateNode(GetEventListener(name));
	}

	public void DeactivateNode(string name)
	{
		DeactivateNode(GetEventListener(name));
	}

	//////////////////////////////////////////////////////////////////////////
	[InspectorButton]
	void ActivateNode2Sub1()
	{
		ActivateNode("Node2Sub1");
	}
	[InspectorButton]
	void DeactivateNode2Sub1()
	{
		DeactivateNode("Node2Sub1");
	}
	[InspectorButton]
	void Test()
	{
		Send(new TestEvent());
	}
}
