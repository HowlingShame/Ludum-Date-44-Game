using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using Action = System.Action;

public class Core : GLMonoBehaviour
{
	public static Core			Instance;
	public Camera				m_Camera;
	public Level				m_LoggingLevel;

	[NonSerialized]	private DebugText			m_DebugText;
	[NonSerialized]	public Register				m_Registry = new Register();
	
	static public readonly Vector2		CellSize = new Vector2(1.0f, 1.0f);
	static public readonly Vector2		CellCenter = new Vector2(0.5f, 0.5f);
	
	static public readonly Plane		GroundPlane = new Plane(Vector3.forward, 0.0f);

	static public readonly Vector2		VectorLeftTop = new Vector2(-0.707106769084930419921875f, 0.707106769084930419921875f);
	static public readonly Vector2		VectorRightTop = new Vector2(0.707106769084930419921875f, 0.707106769084930419921875f);
	static public readonly Vector2		VectorLeftBottom = new Vector2(-0.707106769084930419921875f, -0.707106769084930419921875f);
	static public readonly Vector2		VectorRightBottom = new Vector2(0.707106769084930419921875f, -0.707106769084930419921875f);

	public AnimationCurve			m_MoveUpdateTimeCurve;
	public bool						m_DebugCameraControll = true;
	public bool						m_DoNotDestroyOnLoad = true;
	public object					m_DragAndDropObject;
	
	public MouseWorldPos			m_MouseWorldPos = new MouseWorldPos();
	public PrefabManager			m_SerializationManager = new PrefabManager();
	
	public bool						m_MouseCollider;
	
	//////////////////////////////////////////////////////////////////////////
	public class MouseWorldPos : IWorldPosition
	{
		public Vector3			m_Position;

		public void Update()
		{
			var ray = Core.Instance.m_Camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Core.Instance.m_Camera.farClipPlane ));

			float d = 0.0f;
			GroundPlane.Raycast(ray, out d);
		
			m_Position = ray.GetPoint(d);
		}

		public Vector3 iGetWorldPosition()
		{
			return m_Position;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		if(Instance != null)
			Debug.LogWarning("Core have more then one instance");

		Instance = this;
		if(m_DoNotDestroyOnLoad)
			DontDestroyOnLoad(gameObject);

		if(m_LoggingLevel >= Level.Medium)
			m_DebugText = gameObject.AddComponent<DebugText>();

		if(m_DebugCameraControll)
			gameObject.AddComponent<DebugControl>();

		if(m_MouseCollider)
			new GameObject("MouseCollider").AddComponent<MouseCollider>();
	}

	private void Update() 
	{
		m_MouseWorldPos.Update();
	}

	//////////////////////////////////////////////////////////////////////////
	public void Log(string text)
	{
		Debug.Log(text);
	}

	//////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
	[InspectorButton]
	public void UpdateResources()
	{
		//Resources.Load(
//		foreach(var n in Resources.FindObjectsOfTypeAll(typeof(ILoadableResource))/*GameObject.FindObjectsOfType(typeof(ILoadableResource))*/)
//		{
//			(n as ILoadableResource).iSetResourcePath(PrefabManager.GetPrefabPath((n as ILoadableResource).iGetObject()));
//PrefabManager.GetPrefabPath(res)
//		}
		foreach(var n in GameObject.FindObjectsOfType<GameObject>())
		{
			foreach(var c in n.GetComponents<ILoadableResource>())
			{
				c.iSetResourcePath(PrefabManager.GetPrefabPath(c.iGetObject()));
				EditorUtility.SetDirty(n);
				EditorUtility.SetDirty(c as Component);
			}
		}
		
		var levelDirectoryPath = new DirectoryInfo(Application.dataPath);
		var fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
				 
		foreach(FileInfo file in fileInfo) 
		{
			if((file.Extension == ".prefab" || file.Extension == ".asset") && file.FullName.Contains("Resources\\"))
			{
				var indexStart = file.FullName.IndexOf("Resources\\") + 10;
				var path = file.FullName.Substring(indexStart, file.FullName.Length - indexStart - file.Extension.Length ).Replace('\\', '/');

				var res = Resources.Load(path);
				var ilr = res as ILoadableResource;
				if(ilr != null)
				{
					ilr.iSetResourcePath(path);
					EditorUtility.SetDirty(res);
					Debug.Log(path);
				}
			}
		}

		Resources.UnloadUnusedAssets();
	}
	
#endif
	//////////////////////////////////////////////////////////////////////////
	static public void ShowText(object obj)
	{
		if(Instance.m_DebugText != null)
			Instance.m_DebugText.ShowString(obj);
	}
	static public void ShowText(string tile, object obj)
	{
		if(Instance.m_DebugText != null)
			Instance.m_DebugText.ShowString(tile, obj);
	}

	static public T DeepCopy<T>(T other)
	{
		using(var ms = new MemoryStream())
		{
			var formatter = new BinaryFormatter();
			formatter.Serialize(ms, other);
			ms.Position = 0;
			return (T)formatter.Deserialize(ms);
		}
	}
	static public T ShallowCopy<T>(T other)
	{
		return (T)Utilities.Reflection.MakeShallowCopy(other, false);
	}

	static public Coroutine StartWaitTimeAndDo(float time, Action action)
	{
		return Instance.StartCoroutine(WaitAndDo(time, action));
	}
	static public IEnumerator WaitAndDo(float time, Action action) 
	{
		yield return new WaitForSeconds(time);
		action();
	}
	
	static public Coroutine Start(IEnumerator coroutine)
	{
		return Instance.StartCoroutine(coroutine);
	}

	static public Coroutine StartDo(Action action)
	{
		return Instance.StartCoroutine(Do(action));
	}
	static public IEnumerator Do(Action action) 
	{
		yield return null;
		action();
	}
	
	static public Coroutine StartDoRepeating(int count, float startInteval, float timeInterval, Action action) 
	{
		return Instance.StartCoroutine(DoRepeating(count, startInteval, timeInterval, action));
	}
	static public IEnumerator DoRepeating(int count, float startInteval, float timeInterval, Action action) 
	{
		yield return new WaitForSeconds(startInteval);
		
		if(count <= 0)	yield break;

		do
		{
			action();
			yield return new WaitForSeconds(timeInterval);
		}
		while(count-- >= 0);
	}
	
	static public Coroutine StartDoRepeating(int count, float startInteval, float timeInterval, Action action, Action finish) 
	{
		return Instance.StartCoroutine(DoRepeating(count, startInteval, timeInterval, action, finish));
	}
	static public IEnumerator DoRepeating(int count, float startInteval, float timeInterval, Action action, Action finish) 
	{
		yield return new WaitForSeconds(startInteval);

		if(count <= 0)	yield break;

		do
		{
			action();
			yield return new WaitForSeconds(timeInterval);
		}
		while(count-- >= 0);
		
		finish();
	}
	
	static public Coroutine StartWaitFrameAndDo(int frameCount, Action action)
	{
		return Instance.StartCoroutine(WaitFrameAndDo(frameCount, action));
	}
	static public IEnumerator WaitFrameAndDo(int frameCount, Action action) 
	{
		//yield return null;

		while(frameCount-- > 0)
			yield return null;

		action();
	}
}


public class ResultDescriptor
{
	public string		m_Description;
	public State		m_Result;

	public bool	IsOk{ get{ return m_Result.HasFlag(State.Succeeded); } }

	public ResultDescriptor(string desc, State state)
	{
		m_Description = desc;
		m_Result = state;

		if(Core.Instance.m_LoggingLevel >= Level.Medium)
		{
			switch(m_Result)
			{
				case State.None:			Debug.Log("state None: " + desc);			break;
				case State.Interrupted:		Debug.Log("state Interrupted: " + desc);	break;
				case State.Failed:
				{
					Debug.Log("state Failed: " + desc);
					if(Core.Instance.m_LoggingLevel >= Level.Hight)		
						Debug.Break();		
				}
				break;
			}
		}
	}
}
public class ErrorResultDescriptor : ResultDescriptor
{
	public ErrorResultDescriptor(string desc) : base(desc, State.Failed)
	{
		if(Core.Instance.m_LoggingLevel >= Level.Hight)
			throw new Exception(desc);
	}
	public ErrorResultDescriptor() : base("Error", State.Failed)
	{
		if(Core.Instance.m_LoggingLevel >= Level.Hight)
			throw new Exception("Error");
	}
}
public class BadResultDescriptor : ResultDescriptor
{
	public BadResultDescriptor(string desc) : base(desc, State.Interrupted)
	{
	}
	public BadResultDescriptor() : base("Bad", State.Interrupted)
	{
	}
}

public enum State
{
	None = 0,
	Succeeded	= 1<<0,
	Failed		= 1<<1,
	Running		= 1<<2,
	Interrupted	= Failed | 1<<3
}

public enum Level
{
	None,
	Low,
	Medium,
	Hight,
}

[Serializable]
public enum DataIndex : int
{
	// Type Flags
	None	= 0,
	Int		= 1 << 31,
	Float	= 1 << 30,
	String	= 1 << 29
}

public enum Сomparison
{
	Equal,
	NotEqual
}

public enum Speed
{
	Slow,
	Fast,
	Normal
}

[Serializable]
public enum CharacterRole
{
	ProjectLead,
	FreeTime,
	Prisioner
}

[Serializable]
public enum RestructionFlag
{
	No,
	Soft,
	Hard,
	DoorHorizontal,
	DoorVertical,
}


public struct DifferentialFloat
{
	private float m_CurrentValue;

	public float		CurrentValue		{ get{ return m_CurrentValue; } set{ LastValue = m_CurrentValue; m_CurrentValue = value; } }
	public float		LastValue			{ get; private set; }
	public float		Change				{ get{ return CurrentValue - LastValue; } }

}

public class CoroutineWrapper
{
	MonoBehaviour		m_pGameObject = null;
	Coroutine			m_pCoroutine = null;
	Func<IEnumerator>	m_pFunction;

	//////////////////////////////////////////////////////////////////////////
	public void Start()
	{
		Stop();
		m_pCoroutine = m_pGameObject.StartCoroutine(m_pFunction());
	}

	public void Stop()
	{
		if(m_pCoroutine != null)
		{
			m_pGameObject.StopCoroutine(m_pCoroutine);
			m_pCoroutine = null;
		}
	}

	public void Restart()
	{
		Stop();
		Start();
	}

	public void Ended()	// helper for optimization purpouse
	{
		m_pCoroutine = null;
	}
	
	public CoroutineWrapper(MonoBehaviour gameObject, Action func)
	{
		m_pGameObject = gameObject;
		m_pFunction = () => Core.Do(func);
	}

	public CoroutineWrapper(MonoBehaviour gameObject, Func<IEnumerator> func)
	{
		m_pGameObject = gameObject;
		m_pFunction = func;
	}

	public CoroutineWrapper(MonoBehaviour gameObject, IEnumerator func)
	{
		m_pGameObject = gameObject;
		m_pCoroutine = gameObject.StartCoroutine(func);
	}
}