using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Register
{
	protected uint						m_MaxIndex;
	protected Dictionary<uint, object>	m_Data = new Dictionary<uint, object>(c_DictionarySize);

	protected const int					c_DictionarySize = 2048;

	//////////////////////////////////////////////////////////////////////////
	public uint Add(uint id, object obj)
	{
		if(m_MaxIndex < id)
			m_MaxIndex = id;
		
		if(m_Data.ContainsKey(id))
			return Add(obj);
		else
			m_Data[id] = obj;

		return id;
	}

	public uint Add(object obj)
	{
		m_Data.Add(++ m_MaxIndex, obj);
		return m_MaxIndex;
	}

	public object Get(uint id)
	{
		object value;
		m_Data.TryGetValue(id, out value);

		return value;
	}
}

public class ID
{
	public uint		m_ID;

	//////////////////////////////////////////////////////////////////////////
	public void Register(object obj)
	{
		m_ID = Core.Instance.m_Registry.Add(obj);
	}

	public void Serialize(SerializationInfo info)
	{
		info.AddValue("ID", m_ID);
	}
	
	public void Deserialize(SerializationInfo info)
	{
		m_ID = info.GetUInt32("ID");
	}
}
