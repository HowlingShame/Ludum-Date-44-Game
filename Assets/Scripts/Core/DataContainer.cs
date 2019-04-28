using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataContainer
{
	[SerializeField]
	private DataDictionary		m_Data = new DataDictionary();
	
	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	private class DataElement
	{
		[SerializeField]
		private string		m_DataBuffer;
		public string		DataBuffer
		{
			get
			{
				if(string.IsNullOrEmpty(m_DataBuffer))
					m_DataBuffer = m_Data.ToString();

				return m_DataBuffer;
			}
		}
		[NonSerialized]
		public object		m_Data;

		
		//////////////////////////////////////////////////////////////////////////
		public int GetInt()
		{
			if(m_Data == null)
				m_Data = int.Parse(m_DataBuffer);

			return (int)m_Data;
		}
		public float GetFloat()
		{
			if(m_Data == null)
				m_Data = float.Parse(m_DataBuffer);

			return (float)m_Data;
		}
		public string GetString()
		{
			return m_DataBuffer;
		}
		
		public void Set<T>(T value)
		{
			m_DataBuffer = null;
			m_Data = value;
		}
	}

	[Serializable]
	private class DataDictionary : SerializableDictionaryBase<DataIndex, DataElement> { }

	////////////////////////////////////////////////////////////////////////
	public int GetInt(DataIndex index)
	{
		DataElement result;
		if(m_Data.TryGetValue(index, out result))
			return result.GetInt();

		return 0;
	}
	
	public float GetFloat(DataIndex index)
	{
		DataElement result;
		if(m_Data.TryGetValue(index, out result))
			return result.GetFloat();

		return 0.0f;
	}
	
	public string GetString(DataIndex index)
	{
		DataElement result;
		if(m_Data.TryGetValue(index, out result))
			return result.GetString();

		return string.Empty;
	}

	public void SetInt(DataIndex index, int value)
	{
		m_Data[index].Set(value);
	}

	public void SetFloat(DataIndex index, float value)
	{
		m_Data[index].Set(value);
	}

	public void SetString(DataIndex index, string value)
	{
		m_Data[index].Set(value);
	}

	public void RemoveKey(DataIndex index)
	{
		m_Data.Remove(index);
	}
	
}