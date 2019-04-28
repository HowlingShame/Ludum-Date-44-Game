using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System.Runtime.Serialization;
using UnityEditor;
using System.IO;
using UnityEngine.Tilemaps;

public class UnityCommon_Serializator
{
	public class SerializationSurrogate_Vector3 : ISerializationSurrogate
	{
		// Method called to serialize a Vector3 object
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			Vector3 v = (Vector3)obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
			info.AddValue("z", v.z);
		}
 
		// Method called to deserialize a Vector3 object
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			Vector3 v = (Vector3)obj;
			v.x = (float)info.GetValue("x", typeof(float));
			v.y = (float)info.GetValue("y", typeof(float));
			v.z = (float)info.GetValue("z", typeof(float));

			//obj = v3;
			return v;
		}
	}
	public class SerializationSurrogate_Vector3Int : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Vector3Int)obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
			info.AddValue("z", v.z);
		}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var v = (Vector3Int)obj;
			v.x = info.GetInt32("x");
			v.y = info.GetInt32("y");
			v.z = info.GetInt32("z");

			return v;
		}
	}
	public class SerializationSurrogate_Vector2Int : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Vector2Int)obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
		}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var v = (Vector2Int)obj;
			v.x = info.GetInt32("x");
			v.y = info.GetInt32("y");

			return v;
		}
	}
	public class SerializationSurrogate_Vector2 : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Vector2)obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
		}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var v = (Vector2)obj;
			v.x = info.GetInt32("x");
			v.y = info.GetInt32("y");

			return v;
		}
	}
	public class SerializationSurrogate_Quaternion : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			var q = (Quaternion)obj;
			info.AddValue("x", q.x);
			info.AddValue("y", q.y);
			info.AddValue("z", q.z);
			info.AddValue("w", q.w);
		}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var q = (Quaternion)obj;
			q.x = (float)info.GetValue("x", typeof(float));
			q.y = (float)info.GetValue("y", typeof(float));
			q.z = (float)info.GetValue("z", typeof(float));
			q.w = (float)info.GetValue("w", typeof(float));

			return q;
		}
	}
	

	public static void Reg(SurrogateSelector selector)
	{
		selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector3());
		selector.AddSurrogate(typeof(Vector3Int), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector3Int());
		selector.AddSurrogate(typeof(Vector2Int), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector2Int());
		selector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector2());
		selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Quaternion());
	}
}



public class Empty_Serializator<T>
{
	private class Serializator_Empty : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)	{}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)	{return obj;}
	}

	public static void Reg(SurrogateSelector selector)
	{
		selector.AddSurrogate(typeof(T), new StreamingContext(StreamingContextStates.All), new Serializator_Empty());
	}
}

/*
public class BaseGameObject_Serializator
{
	private class BaseGameObject_ReadWriteEmpty : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)	{}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)	{return obj;}
	}

	public static void Reg(SurrogateSelector selector)
	{
		selector.AddSurrogate(typeof(BaseGameObject), new StreamingContext(StreamingContextStates.All), new BaseGameObject_ReadWriteEmpty());
	}
}*/
/*
public class IBaseTileAsset_Serializator<T> where T : IBaseTileAsset
{
	public class WRITE_DATA{}

	private class IBaseTileAsset_Write : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			var id = (obj as IBaseTileAsset).ID();
		
			info.AddValue("id", id);
			info.SetType(typeof(WRITE_DATA));
		}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)	{new NotImplementedException();	return null;}
	}
	
	private class IBaseTileAsset_Read : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)											{new NotImplementedException();}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var id = info.GetInt32("id");
			var result = Core.Instance.m_TileAssetDataBase.m_TileAssetDataBaseDic[id];

			return result;
		}
	}

	public static void Reg(SurrogateSelector selector)
	{
		selector.AddSurrogate(typeof(T), new StreamingContext(StreamingContextStates.All), new IBaseTileAsset_Write());
		selector.AddSurrogate(typeof(WRITE_DATA), new StreamingContext(StreamingContextStates.All), new IBaseTileAsset_Read());
	}
}*/

/*
public class BaseGameObject_Serializator
{
	private class BaseGameObject_ReadWriteEmpty : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)	{}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)	{return obj;}
	}

	public static void Reg(SurrogateSelector selector)
	{
		selector.AddSurrogate(typeof(BaseGameObject), new StreamingContext(StreamingContextStates.All), new BaseGameObject_ReadWriteEmpty());
	}
}*/
