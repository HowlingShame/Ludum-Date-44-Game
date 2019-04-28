using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class LoadableResourceMarker : MonoBehaviour, ILoadableResource 
{
	public string	m_ResourcePath;

	//////////////////////////////////////////////////////////////////////////
	public void iDeserialize(SerializationInfo info)
	{		
		transform.position = (Vector3)info.GetValue("pos", typeof(Vector3));
		foreach(var n in  GetComponents<ISerializebleObject>())
			if(n as ILoadableResource == null)		//GetType().IsAssignableFrom()
				n.iDeserialize(info);
	}

	public void iSerialize(SerializationInfo info)
	{
		info.AddValue("pos", transform.position, typeof(Vector3));
		foreach(var n in  GetComponents<ISerializebleObject>())
			if(n as ILoadableResource == null)
				n.iSerialize(info);
	}

	public Object iGetObject()
	{
		return gameObject;
	}

	public string iGetResourcePath()
	{
		return m_ResourcePath;
	}


	public void iSetResourcePath(string resourcePath)
	{
		m_ResourcePath = resourcePath;
	}
}
