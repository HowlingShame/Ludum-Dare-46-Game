using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Action = System.Action;
using Random = UnityEngine.Random;


[DisallowMultipleComponent, DefaultExecutionOrder(Core.c_ManagerDefaultExecutionOrder)]
public class Core : GLMonoBehaviour
{
	public static Core			Instance;
	public Camera				m_Camera;
	public Level				m_LoggingLevel;
	private const string		c_DefaultDelimiterObjectName = "----------//----------";


	[NonSerialized]	
	private DebugText	                m_DebugText;

	public const int					c_ManagerDefaultExecutionOrder = -10;
	
	public static readonly Vector2		c_CellSize = new Vector2(1.0f, 1.0f);
	public static readonly Vector2		c_CellCenter = new Vector2(0.5f, 0.5f);
										
	public static readonly Plane		c_GroundPlaneXY = new Plane(Vector3.forward, 0.0f);
	public static readonly Plane		c_GroundPlaneXZ = new Plane(Vector3.up, 0.0f);
										
	public static readonly Vector2		c_Vector2LeftTop = new Vector2(-0.707106769084930419921875f, 0.707106769084930419921875f);
	public static readonly Vector2		c_Vector2RightTop = new Vector2(0.707106769084930419921875f, 0.707106769084930419921875f);
	public static readonly Vector2		c_Vector2LeftBottom = new Vector2(-0.707106769084930419921875f, -0.707106769084930419921875f);
	public static readonly Vector2		c_Vector2RightBottom = new Vector2(0.707106769084930419921875f, -0.707106769084930419921875f);
										
	public static readonly Vector3		c_Vector3LeftTop = new Vector3(-0.707106769084930419921875f, 0.0f, 0.707106769084930419921875f);
	public static readonly Vector3		c_Vector3RightTop = new Vector3(0.707106769084930419921875f, 0.0f, 0.707106769084930419921875f);
	public static readonly Vector3		c_Vector3LeftBottom = new Vector3(-0.707106769084930419921875f, 0.0f, -0.707106769084930419921875f);
	public static readonly Vector3		c_Vector3RightBottom = new Vector3(0.707106769084930419921875f, 0.0f, -0.707106769084930419921875f);

	public ProjectSpace				m_ProjectSpace;
	public bool						m_DebugCameraControll = true;
	public MouseButton				m_DebugCameraButtonKey = MouseButton.Middle;
	public bool						m_DoNotDestroyOnLoad = true;
	public TaskManagerTaskCount		m_TaskManagerTaskCount = TaskManagerTaskCount.ProcessorN2;
	
	private MousePosition			m_MousePosition = new MousePosition();
	public MousePosition			MouseWorldPosition => m_MousePosition;
	public SerializationManager		m_SerializationManager;
	
	public TaskManager				m_TaskManager = new TaskManager();

	public bool						m_MouseCollider;
	public bool						m_MouseGameObject;
	
	public bool						m_MouseSpawnPrefab;
	[DrawIf("m_MouseSpawnPrefab", true)]
	public GameObject				m_MousePrefab;

	public bool						m_MouseGravitation;
	[DrawIf("m_MouseGravitation", true)]
	public float					m_MouseGravitationForce = 1.0f;
	[DrawIf("m_MouseGravitation", true)]
	public float					m_MouseGravitationRadius = 1.0f;
	[DrawIf("m_MouseGravitation", true)]
	public LayerMask				m_MouseGravitationLayerMask;

	public string					m_LocalizationPath = "Localization";
	public string					m_LocalizationLanguage = "Auto";//"en-US";
	

	[SerializeField] 
	private bool					m_ControlTime;
	[SerializeField] 
	[DrawIf("m_ControlTime", true)]
	private bool					m_ControlTimeKeys;
	[SerializeField] 
	[DrawIf("m_ControlTime", true)]
	private bool					m_ApplyTimeScaleToPhysics = false;

	[Range(0.001f, 10.0f)]
	[DrawIf("m_ControlTime", true)]
	public float					m_GameSpeed = 1.0f;
	private float					m_InitialFixedDeltaTime;


	//////////////////////////////////////////////////////////////////////////
	public class TaskManager
	{
		private int						m_ProcessorCount = 4;
		private LinkedList<Task>		m_TaskList = new LinkedList<Task>();
		private List<Task>				m_TaskQueue = new List<Task>();

		//////////////////////////////////////////////////////////////////////////
		public void Init()
		{
			switch (Core.Instance.m_TaskManagerTaskCount)
			{
				case TaskManagerTaskCount.Processor:
					m_ProcessorCount = SystemInfo.processorCount;
					break;
				case TaskManagerTaskCount.ProcessorX2:
					m_ProcessorCount = SystemInfo.processorCount * 2;
					break;
				case TaskManagerTaskCount.ProcessorN2:
					m_ProcessorCount = SystemInfo.processorCount + 2;
					break;
				case TaskManagerTaskCount.ProcessorN4:
					m_ProcessorCount = SystemInfo.processorCount + 4;
					break;
				case TaskManagerTaskCount.ProcessorN8:
					m_ProcessorCount = SystemInfo.processorCount + 8;
					break;
				case TaskManagerTaskCount.N8:
					m_ProcessorCount = 8;
					break;
				case TaskManagerTaskCount.N16:
					m_ProcessorCount = 16;
					break;
				case TaskManagerTaskCount.Unlimited:
					m_ProcessorCount = int.MaxValue;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			m_TaskList.AddLast(new LinkedListNode<Task>(null));
		}

		public void Update()
		{
			// clear completed tasks
			var current = m_TaskList.First;
			
			while (current.Next != null)
			{
				if (current.Next.Value.IsCompleted)
				{
					m_TaskList.Remove(current.Next);
				}
				
				current = current.Next;
				if (current == null)
					break;
			}

			// get new tasks
			while (m_TaskList.Count <= m_ProcessorCount && m_TaskQueue.Count != 0)
			{
				var task = m_TaskQueue[0];
				m_TaskList.AddLast(new LinkedListNode<Task>(task));
				task.Start();
				m_TaskQueue.RemoveAt(0);
			}
		}

		public void AddTask(Task task)
		{
			m_TaskQueue.Add(task);
		}

		public void RemoveTask(Task task)
		{
			if (m_TaskQueue.Remove(task) == false)
				m_TaskList.Remove(task);
		}

		public void RemoveTask(int taskID)
		{
			var task = m_TaskQueue.Find((t) => t.Id == taskID);
			if (task != null)
				m_TaskQueue.Remove(task);
			else
			{
				task = m_TaskList.First(t => t.Id == taskID);
				if (task != null)
					m_TaskList.Remove(task);
			}
		}
	}

	[Serializable]
	public enum ProjectSpace
	{
		XY,
		XZ
	}

	[Serializable]
	public enum MouseButton : int
	{
		None = -1,
		Left = 0,
		Right = 1,
		Middle = 2
	}

	[Serializable]
	public enum TaskManagerTaskCount
	{
		/// <summary>Cores count</summary>
		Processor,
		/// <summary>Cores count * 2</summary>
		ProcessorX2,
		/// <summary>Cores count + 2</summary>
		ProcessorN2,
		/// <summary>Cores count + 4</summary>
		ProcessorN4,
		/// <summary>Cores count + 8</summary>
		ProcessorN8,
		/// <summary>8 tasks limit</summary>
		N8,
		/// <summary>16 tasks limit</summary>
		N16,
		/// <summary>Unlimited</summary>
		Unlimited,
	}

	public class MousePosition : IWorldPosition
	{
		public Plane			m_GroundPlane;
		private Vector3			m_ScreenPosition;
		private Vector3			m_WorldPosition;

		public Vector3			ScreenPosition => m_ScreenPosition;
		public Vector3			WorldPosition => m_WorldPosition;

        public Ray              CameraRay;

		//////////////////////////////////////////////////////////////////////////
		public void Update()
		{
			m_ScreenPosition = Input.mousePosition;
			m_WorldPosition = GetWordPosition(m_GroundPlane);
		}

		public Vector3 GetWordPosition(Plane plane)
		{
            CameraRay = Core.Instance.m_Camera.ScreenPointToRay(new Vector3(m_ScreenPosition.x, m_ScreenPosition.y, Core.Instance.m_Camera.farClipPlane));

            plane.Raycast(CameraRay, out var d);

            return CameraRay.GetPoint(d);
		}

        public Vector3 GetWordPosition(float distance)
        {
            var ray = Core.Instance.m_Camera.ScreenPointToRay(new Vector3(m_ScreenPosition.x, m_ScreenPosition.y, Core.Instance.m_Camera.farClipPlane));

            return ray.GetPoint(distance);
        }

		public Vector3 iGetWorldPosition()
		{
			return m_WorldPosition;
		}

		public static implicit operator Vector3(MousePosition mouseWorldPos)
		{
			return mouseWorldPos.m_WorldPosition;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		if (Instance != null)
		{
			gameObject.SetActive(false);
			return;
		}

		Instance = this;

		LocalizationManager.Read(m_LocalizationPath, true);
        LocalizationManager.Language =
            (string.IsNullOrEmpty(m_LocalizationLanguage) || m_LocalizationLanguage == "Auto") ? CultureInfo.CurrentUICulture.Name : m_LocalizationLanguage;

		m_SerializationManager = SerializationManager.Instance;
		m_TaskManager.Init();

		switch (m_ProjectSpace)
		{
			case ProjectSpace.XY:
				m_MousePosition.m_GroundPlane = c_GroundPlaneXY;
				break;
			case ProjectSpace.XZ:
				m_MousePosition.m_GroundPlane = c_GroundPlaneXZ;
				break;
		}

		if (m_DoNotDestroyOnLoad)
			DontDestroyOnLoad(gameObject);

		if (m_LoggingLevel >= Level.Medium)
			m_DebugText = gameObject.AddComponent<DebugText>();

		if (m_DebugCameraControll)
		{
			var debugControl = gameObject.AddComponent<DebugControl>();
			debugControl.m_DragMouseButton = (int)m_DebugCameraButtonKey;
		}

		if (m_MouseGameObject)
		{
			var mouseObject = transform.FindChild("MouseObject", false);
			if(mouseObject == null)
				new GameObject("MouseObject").AddComponent<MouseTracker>();
			else
				if(mouseObject.GetComponent<MouseTracker>() == null)
					mouseObject.AddComponent<MouseTracker>();

		}
		if(m_MouseCollider)
			new GameObject("MouseCollider").AddComponent<MouseCollider>();

		if (m_MouseGravitation)
			gameObject.AddComponent<RunnerFixedUpdate>().Executable = new ExecutableAction(() =>
			{
				if (Input.GetKey(KeyCode.Mouse0))
				{
					var colliderResult = new List<Collider2D>();
					Physics2D.OverlapCircle(m_MousePosition.WorldPosition, m_MouseGravitationRadius,
						new ContactFilter2D(){ layerMask = m_MouseGravitationLayerMask, useTriggers = false, useDepth = false, useLayerMask = true, useNormalAngle = false },
						colliderResult);
				
					
					foreach (var n in colliderResult)
					{
						var forceNormal = (m_MousePosition.WorldPosition.To2DXY() - n.attachedRigidbody.position).normalized;
						n.attachedRigidbody.AddForce(forceNormal * m_MouseGravitationForce * Time.fixedDeltaTime);
					}
				}
			});


        m_InitialFixedDeltaTime = Time.fixedDeltaTime;
		if (m_ControlTime)
		{
			gameObject.AddComponent<RunnerUpdate>().Executable = new ExecutableAction(() =>
            {
                if (m_ControlTimeKeys)
				{
					if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
						m_GameSpeed = Mathf.Clamp(m_GameSpeed * 1.1f, 0.0f, 10.0f);

					if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
						m_GameSpeed = Mathf.Clamp(m_GameSpeed * 0.9f, 0.0f, 10.0f);
				}

                ApplyTimeScale(m_GameSpeed, m_ApplyTimeScaleToPhysics);
            });
		}
	}

    private void Update() 
	{
		m_MousePosition.Update();

		if (m_MouseSpawnPrefab && Input.GetKeyDown(KeyCode.Mouse0))
			Instantiate(m_MousePrefab, m_MousePosition.WorldPosition, Quaternion.identity);

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Pause))
			Debug.Break();
#endif
		m_TaskManager.Update();
	}

    public void ApplyTimeScale(float gameSpeed, bool applyTimeScaleToPhysics)
    {
        if (Time.timeScale == gameSpeed) 
            return;

        Time.timeScale = gameSpeed;
        if (applyTimeScaleToPhysics && gameSpeed > 0.0f)
            Time.fixedDeltaTime = m_InitialFixedDeltaTime * gameSpeed;
    }

	public void Log(string text)
	{
		Debug.Log(text);
	}

	//////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
	/*[InspectorButton]
	public void UpdateResources()
	{
//		foreach(var n in Resources.FindObjectsOfTypeAll(typeof(ILoadableResource)))
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
	}*/

	[MenuItem("GameObject/Delimiter", false, 10)]
	static void CreateCustomGameObject(MenuCommand menuCommand)
	{
		GameObject go = new GameObject(c_DefaultDelimiterObjectName);
		go.tag = "EditorOnly";
		go.isStatic = true;
		go.SetActive(false);
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

    [MenuItem("GameObject/Remove Missing Scripts Recursively", false, 0)]
    private static void FindAndRemoveMissingInSelected()
    {
        var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
        int compCount = 0;
        int goCount = 0;
        foreach (var o in deepSelection)
        {
            if (o is GameObject go)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (count > 0)
                {
                    // Edit: use undo record object, since undo destroy wont work with missing
                    Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    compCount += count;
                    goCount++;
                }
            }
        }
        Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
    }
	
    [MenuItem("Edit/Reserialize Assets", false, 10)]
    static void ReserializeAssets(MenuCommand menuCommand)
    {
        AssetDatabase.ForceReserializeAssets();
    }


#endif
	//////////////////////////////////////////////////////////////////////////
	public static void ShowText(object obj)
	{
		if(Instance.m_DebugText != null)
			Instance.m_DebugText.ShowString(obj);
	}
	public static void ShowText(string tile, object obj)
	{
		if(Instance.m_DebugText != null)
			Instance.m_DebugText.ShowString(tile, obj);
	}

	public static float Fib(float baseFib, int iterations, float stepLimit = float.MaxValue)
	{
		var a = 0.0f;
		var b = baseFib;
		var c = 0.0f;

		for (var n = 0; n < iterations; n++)
		{
			c = Mathf.Min(a, stepLimit) + b;
			a = b;
			b = c;
		}

		return c;
	}

	public static void Fib(float baseFib, int iterations, List<float> values, float stepLimit = float.MaxValue)
	{
		var a = 0.0f;
		var b = baseFib;

		values.Add(a);
		values.Add(b);

		for (var n = 0; n < iterations; n++)
		{
			var c = Mathf.Min(a, stepLimit) + b;
			a = b;
			b = c;
			values.Add(c);
		}
	}
	
	public static IEnumerable<T> GetFlags<T>(T input) where T : Enum
	{
		foreach (T value in Enum.GetValues(typeof(T)))
			if (input.HasFlag(value))
				yield return value;
	}

	public static void DrawEllipse(Vector3 pos, float radius, Color color, int segments = 20, float duration = 0)
	{
		DrawEllipse(pos, Vector3.forward, Vector3.up, radius, radius, color, segments, duration);
	}

	public static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, Color color, int segments, float duration = 0)
	{
		float angle = 0f;
		Quaternion rot = Quaternion.LookRotation(forward, up);
		Vector3 lastPoint = Vector3.zero;
		Vector3 thisPoint = Vector3.zero;
 
		for (int i = 0; i < segments + 1; i++)
		{
			thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
			thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;
 
			if (i > 0)
			{
				Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
			}
 
			lastPoint = thisPoint;
			angle += 360f / segments;
		}
	}

//	static public void DestroyObj(GameObject obj)
//	{
//		if (obj != null)
//			Destroy(obj);
//	}

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

	static public Coroutine StartDoNextFrame(Action action)
	{
		return Instance.StartCoroutine(DoNextFrame(action));
	}
	static public IEnumerator DoNextFrame(Action action) 
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
		
		if (count <= 0)	yield break;

		do
		{
			action();
			yield return new WaitForSeconds(timeInterval);
		}
		while (count-- >= 0);
	}


	static public Coroutine StartDoForever(float startInteval, float timeInterval, Action action) 
	{
		return Instance.StartCoroutine(DoForever(startInteval, timeInterval, action));
	}
	static public IEnumerator DoForever( float startInteval, float timeInterval, Action action) 
	{
		yield return new WaitForSeconds(startInteval);
		
		while(true)
		{
			action();
			yield return new WaitForSeconds(timeInterval);
		}
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

	//////////////////////////////////////////////////////////////////////////
	public ResultDescriptor(string desc, State state)
	{
		m_Description = desc;
		m_Result = state;

		if (Core.Instance.m_LoggingLevel >= Level.Medium)
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
		if (Core.Instance.m_LoggingLevel >= Level.Hight)
			throw new Exception(desc);
	}
	public ErrorResultDescriptor() : base("Error", State.Failed)
	{
		if (Core.Instance.m_LoggingLevel >= Level.Hight)
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

[Serializable, Flags]
public enum State
{
	None		= 0,
	Succeeded	= 1,
	Failed		= 1 << 1,
	Running		= 1 << 2,
	Interrupted	= 1 << 3,
	Locked		= 1 << 4
}

public static class StateMethods
{	
	public static bool IsLocked(this State state)
	{
		return state.HasFlag(State.Locked);
	}

	public static void Lock(this State state)
	{
		state |= State.Locked;
	}

	public static void Unlock(this State state)
	{
		state &= ~State.Locked;
	}
}

[Serializable]
public enum Level
{
	None,
	Low,
	Medium,
	Hight,
}

//[Serializable]
//public enum Speed
//{
//	Slow,
//	Fast,
//	Normal
//}

public struct DifferentialFloat
{
	private float m_CurrentValue;

	public float		CurrentValue		{ get{ return m_CurrentValue; } set{ LastValue = m_CurrentValue; m_CurrentValue = value; } }
	public float		LastValue			{ get; private set; }
	public float		Change				{ get{ return CurrentValue - LastValue; } }

}

public class GameObjectIWorldPositionWrapper : IWorldPosition
{
	private GameObject		m_GameObject;

	//////////////////////////////////////////////////////////////////////////
	public Vector3 iGetWorldPosition()
	{
		return m_GameObject.transform.position;
	}

	//////////////////////////////////////////////////////////////////////////
	public GameObjectIWorldPositionWrapper(GameObject m_GameObject)
	{
		this.m_GameObject = m_GameObject;
	}
}

public class GameObjectIWorldPositionWrapperEx : IWorldPosition
{
	private GameObject				m_GameObject;
	private Action<GameObject>		m_OnDestroyAction;

	//////////////////////////////////////////////////////////////////////////
	public void Release()
	{
		m_GameObject.GetComponent<OnDestroyCallback>().OnDestroyAction -= m_OnDestroyAction;
	}

	//////////////////////////////////////////////////////////////////////////
	public Vector3 iGetWorldPosition()
	{
		return m_GameObject == null ? Vector3.zero : m_GameObject.transform.position;
	}

	//////////////////////////////////////////////////////////////////////////
	public GameObjectIWorldPositionWrapperEx(GameObject m_GameObject, Action objectDestroyed)
	{
		m_OnDestroyAction = (obj) => { m_GameObject = null; };
		if(objectDestroyed != null)
			m_OnDestroyAction += (obj) =>  { objectDestroyed.Invoke(); };

		var destroyCallback = m_GameObject.GetComponent<OnDestroyCallback>();
		if(destroyCallback == null)
			destroyCallback = m_GameObject.AddComponent<OnDestroyCallback>();

		destroyCallback.OnDestroyAction += m_OnDestroyAction;


		this.m_GameObject = m_GameObject;
	}
}

public sealed class WorldPositionVectorConst : IWorldPosition
{
	public static readonly WorldPositionVectorConst	c_Zero = new WorldPositionVectorConst(Vector3.zero);

	private Vector3					m_Position;
	
	//////////////////////////////////////////////////////////////////////////
	public Vector3 iGetWorldPosition()
	{
		return m_Position;
	}

	//////////////////////////////////////////////////////////////////////////
	public WorldPositionVectorConst(Vector3 m_Position)
	{
		this.m_Position = m_Position;
	}
}
public class WorldPositionVector : IWorldPosition
{
	public Vector3				m_Position;

	//////////////////////////////////////////////////////////////////////////
	public Vector3 iGetWorldPosition()
	{
		return m_Position;
	}
	
	public WorldPositionVector()
	{
	}

	public WorldPositionVector(Vector3 m_Position)
	{
		this.m_Position = m_Position;
	}
}


public class ExecutableAction : IExecutable
{
	public Action m_Action;


	//////////////////////////////////////////////////////////////////////////
	public void iExecute()
	{
		m_Action?.Invoke();
	}

	public ExecutableAction(Action m_Action)
	{
		this.m_Action = m_Action;
	}
}

public static class Actions
{
	public static void Empty() { }
	public static void Empty<T>(T value) { }
	public static void Empty<T1, T2>(T1 value1, T2 value2) { }
}

public static class Functions
{
	public static T Identity<T>(T value) { return value; }

	public static T0 Default<T0>() { return default(T0); }
	public static T0 Default<T1, T0>(T1 value1) { return default(T0); }

	public static bool IsNull<T>(T entity) where T : class { return entity == null; }
	public static bool IsNonNull<T>(T entity) where T : class { return entity != null; }

	public static bool True<T>(T entity) { return true; }
	public static bool False<T>(T entity) { return false; }
}

public static class Helpers
{
	public static float Evaluate(this ParticleSystem.MinMaxCurve curve)
	{
		return curve.Evaluate(Random.value, Random.value);
	}
}