using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrefabManager
{
	class ResourceCell
	{
	}
	class WRITE_DATA{ }
	
	private class WriteSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			var loadableResource = (obj as ILoadableResource);
			info.AddValue("RP", loadableResource.iGetResourcePath());

			loadableResource.iSerialize(info);

			info.SetType(typeof(WRITE_DATA));
		}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)	{new NotImplementedException();	return null;}
	}
	
	private class ReadSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)											{new NotImplementedException();}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var resourcePath = info.GetString("RP");
			
			var result = Core.Instance.m_SerializationManager.Require(resourcePath);
			result?.iDeserialize(info);

			return result;
		}
	}
	
	private class EmptyReadSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)											{new NotImplementedException();}
 
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			return null;
		}
	}
		

	public class SerializationHelper
	{
		public Object				m_Prefab;
		public string				m_PrefabPath;

		//////////////////////////////////////////////////////////////////////////
		public ILoadableResource GetInstance()
		{
			var sriptableObject = m_Prefab as ScriptableObject;
			if(sriptableObject != null)
				return sriptableObject as ILoadableResource;

			var gameObject = m_Prefab as GameObject;
			if(gameObject != null)
				return GameObject.Instantiate(gameObject).GetComponent<ILoadableResource>();
			
			new ErrorResultDescriptor("can't detect resource type");
			return null;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private Dictionary<string, SerializationHelper>			m_LoadedPrefabs = new Dictionary<string, SerializationHelper>();


	//////////////////////////////////////////////////////////////////////////
	public SerializationHelper Add(string path)
	{
		var resource = Resources.Load(path);
				
		var s = new SerializationHelper(){ m_Prefab = resource, m_PrefabPath = path };
		m_LoadedPrefabs.Add(path, s);

		return s;
	}

	public ILoadableResource Require(string prefabPath)
	{
		if(prefabPath == string.Empty)		{ new ErrorResultDescriptor("string of resource path is not set to an object."); return null; }

		SerializationHelper	value;

		if(m_LoadedPrefabs.TryGetValue(prefabPath, out value))		// find existing key
			return value.GetInstance();

		value = Add(prefabPath);									// return loaded key
		return value.GetInstance();
	}


#if UNITY_EDITOR
//	public SerializationHelper Add(BaseGameObject bgo)
//	{
//		var path = GetPrefabPath(bgo.gameObject);
//		var s = new SerializationHelper(){ m_Prefab = Resources.Load(path) as BaseGameObject, m_PrefabPath = path, m_ObjectType = bgo.GetType() };
//		m_LoadedPrefabs.Add(path, s);
//
//		return s;
//	}
	static public string GetPrefabPath(Object obj)
	{
		var parentObject = PrefabUtility.GetCorrespondingObjectFromSource(obj);
		if(parentObject == null)
			return string.Empty;

		var path = AssetDatabase.GetAssetPath(parentObject);

		//path = path.Replace(".prefab", "");
		var indexStart = path.IndexOf("Resources/") + 10;
		path = path.Substring(indexStart, path.Length - indexStart - 7 );

		return path;
	}
#endif

	//////////////////////////////////////////////////////////////////////////
	static public void RegAllValidTypes(SurrogateSelector selector)
	{
		var asm = Assembly.GetExecutingAssembly();

		foreach (Type type in asm.GetTypes())
		{
			if(typeof(ILoadableResource).IsAssignableFrom(type))
				PrefabManager.RegSerializator(type, selector);
		}
	}

	static public void RegSerializator(Type objType, SurrogateSelector selector)
	{
		selector.AddSurrogate(objType, new StreamingContext(StreamingContextStates.All), new WriteSurrogate());
	}

	static public void RegSerializator(SurrogateSelector selector)
	{
		selector.AddSurrogate(typeof(WRITE_DATA), new StreamingContext(StreamingContextStates.All), new ReadSurrogate());
	}
	
	static public void RegReadSerializatorEmpty(SurrogateSelector selector)
	{
		selector.AddSurrogate(typeof(WRITE_DATA), new StreamingContext(StreamingContextStates.All), new EmptyReadSurrogate());
	}
}