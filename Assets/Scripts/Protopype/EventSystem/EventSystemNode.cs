using Gamelogic.Extensions;
using Malee;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemNode : GLMonoBehaviour
{
	[SerializeField]
	private bool						m_PropagationChild = true;

	[SerializeField, Reorderable(null, "Propagation Nodes", null)]
	private EventSystemManagerComplex.PropagationNodeReorderableList                    m_PropagationNodes;

	protected List<EventSystemNode>	m_Connections = new List<EventSystemNode>();
	protected bool					m_Lock = false;

	public List<EventSystemFilter>		m_Filters{ get; private set; }	= new List<EventSystemFilter>();
	public List<EventSystemReciver>		m_Recivers{ get; private set; } = new List<EventSystemReciver>();

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_Connections.AddRange(m_PropagationNodes.ToArray());

		GetComponents<EventSystemFilter>(m_Filters);
		GetComponents<EventSystemReciver>(m_Recivers);

		if(m_PropagationChild)
			foreach(var n in transform.GetChildren())
			{
				var nodes = n.GetComponents<EventSystemNode>();
				m_Connections.AddRange(nodes);

				if(n.gameObject.activeSelf)
				{
					foreach(var c in nodes)
						if(m_PropagationNodes.Contains(c) == false)
							m_PropagationNodes.Add(c);
				}
			}
	}

	//////////////////////////////////////////////////////////////////////////
	public void PropagateEvent(EventSystemManagerBase.Event e)
	{
		if(m_Lock)
			return;

		m_Lock = true;
		{
			foreach(var n in m_Filters)
				if(n.Check(e) == false)
					return;
		
			foreach(var n in m_Recivers)
				n.iProcess(e);
		
			foreach(var n in m_PropagationNodes)
				n.PropagateEvent(e);
		}
		m_Lock = false;
	}

	public void NodeDestroyed(EventSystemNode node)
	{ 
		foreach(var n in m_PropagationNodes)
			if(n == node)
				m_PropagationNodes.Remove(n);

		m_Connections.Remove(node);
	}

	public bool HasConnection(EventSystemNode node)
	{
		return m_Connections.Contains(node);
	}

	public void ActivateNode(EventSystemNode node)
	{
		node.gameObject.SetActive(true);

		if(m_PropagationNodes.Contains(node))
			return;

		m_PropagationNodes.Insert(m_Connections.IndexOf(node), node);
	}

	public void DeactivateNode(EventSystemNode node)
	{
		foreach(var n in m_PropagationNodes)
			if(n == node)
			{
				m_PropagationNodes.Remove(n);
				n.gameObject.SetActive(false);
				return;
			}
	}
}
