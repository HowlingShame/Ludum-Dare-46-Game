using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Base class
[Serializable]
public abstract class DataContainerBase
{
	[Serializable]
	public class DataElementBase
	{
		[NonSerialized]
		public object		m_Data;
		
		//////////////////////////////////////////////////////////////////////////
		public int GetInt()
		{
			return (int)m_Data;
		}

		public float GetFloat()
		{
			return (float)m_Data;
		}

		public bool GetBool()
		{
			return (bool)m_Data;
		}
		
		public string GetString()
		{
			return (string)m_Data;
		}

		public Vector2 GetVector2()
		{
			return (Vector2)m_Data;
		}

		public Vector3 GetVector3()
		{
			return (Vector3)m_Data;
		}

		public Vector2Int GetVector2Int()
		{
			return (Vector2Int)m_Data;
		}

		public Vector3Int GetVector3Int()
		{
			return (Vector3Int)m_Data;
		}

		public object GetData()
		{
			return m_Data;
		}
		
		//////////////////////////////////////////////////////////////////////////
		public void SetInt(int value)
		{
			m_Data = value;
		}
		
		public void SetFloat(float value)
		{
			m_Data = value;
		}

		public void SetBool(bool value)
		{
			m_Data = value;
		}

		public void SetString(string value)
		{
			m_Data = value;
		}
		
		public void SetVector2(Vector2 value)
		{
			m_Data = value;
		}
		
		public void SetVector3(Vector3 value)
		{
			m_Data = value;
		}
		
		public void SetVector2Int(Vector2Int value)
		{
			m_Data = value;
		}

		public void SetVector3Int(Vector3Int value)
		{
			m_Data = value;
		}

		public void SetData(object data)
		{
			m_Data = data;
		}
	}

	[Serializable]
	public class DataElement : DataElementBase, ISerializationCallbackReceiver
	{
		[SerializeField]
		private byte[]		m_DataBuffer;

		[SerializeField]
		public DataType		m_DataType;


		//////////////////////////////////////////////////////////////////////////
		public void OnBeforeSerialize()
		{
			try
			{
				switch (m_DataType)		// fill buffer from data
				{
					case DataType.Null:				m_DataBuffer = new byte[0];															break;
					case DataType.Int:				m_DataBuffer = BitConverter.GetBytes(m_Data == null ? 0 : (int)m_Data);				break;
					case DataType.Float:			m_DataBuffer = BitConverter.GetBytes(m_Data == null ? 0.0f : (float)m_Data);		break;
					case DataType.Boolean:			m_DataBuffer = BitConverter.GetBytes(m_Data == null ? false : (bool)m_Data);		break;
					case DataType.String:
					{
						var str = m_Data == null ? "" : (string)m_Data;
						m_DataBuffer = System.Text.Encoding.UTF8.GetBytes(str.ToCharArray(), 0, str.Length);
					}	break;
					case DataType.Vector2:
					{
						if (m_Data == null)
							m_DataBuffer = new byte[8];
						else
						{
							var vec = (Vector2)m_Data;
							var data = new List<byte>(8);
							data.AddRange(BitConverter.GetBytes(vec.x));
							data.AddRange(BitConverter.GetBytes(vec.y));
							m_DataBuffer = data.ToArray();
						}
					}	break;
					case DataType.Vector3:
					{
						if (m_Data == null)
							m_DataBuffer = new byte[12];
						else
						{
							var vec = (Vector3)m_Data;
							var data = new List<byte>(12);
							data.AddRange(BitConverter.GetBytes(vec.x));
							data.AddRange(BitConverter.GetBytes(vec.y));
							data.AddRange(BitConverter.GetBytes(vec.z));
							m_DataBuffer = data.ToArray();
						}
					}	break;
					case DataType.Vector2Int:
					{
						if (m_Data == null)
							m_DataBuffer = new byte[8];
						else
						{
							var vec = (Vector2Int)m_Data;
							var data = new List<byte>(8);
							data.AddRange(BitConverter.GetBytes(vec.x));
							data.AddRange(BitConverter.GetBytes(vec.y));
							m_DataBuffer = data.ToArray();
						}
					}	break;
					case DataType.Vector3Int:
					{
						if (m_Data == null)
							m_DataBuffer = new byte[12];
						else
						{
							var vec = (Vector3Int)m_Data;
							var data = new List<byte>(12);
							data.AddRange(BitConverter.GetBytes(vec.x));
							data.AddRange(BitConverter.GetBytes(vec.y));
							data.AddRange(BitConverter.GetBytes(vec.z));
							m_DataBuffer = data.ToArray();
						}
					}	break;
					case DataType.Object:
					{
						var formatter = new BinaryFormatter();
						using (var stream = new MemoryStream())
						{
							formatter.Serialize(stream, m_Data == null ? new object() : m_Data);
							m_DataBuffer = stream.ToArray();
						}
					}	break;


				}
			}
			catch (System.Exception ex)
			{
				if (m_Data != null)
				{
					if (m_DataType == DataType.Object)
						Debug.LogWarning(ex);

					m_Data = null;
					OnBeforeSerialize();
				}
				else
					Debug.LogError(ex); 

			}
			

		}

		public void OnAfterDeserialize()
		{
			try
			{
				switch (m_DataType)		// set data from buffer
				{
					case DataType.Null:			m_Data = null;																																					break;
					case DataType.Int:			m_Data = BitConverter.ToInt32(m_DataBuffer, 0);																													break;
					case DataType.Float:		m_Data = BitConverter.ToSingle(m_DataBuffer, 0);																												break;
					case DataType.Boolean:		m_Data = BitConverter.ToBoolean(m_DataBuffer, 0);																												break;
					case DataType.String:		m_Data = System.Text.Encoding.UTF8.GetString(m_DataBuffer, 0, m_DataBuffer.Length);																				break;
					case DataType.Vector2:		m_Data = new Vector2(BitConverter.ToSingle(m_DataBuffer, 0), BitConverter.ToSingle(m_DataBuffer, 4));															break;
					case DataType.Vector3:		m_Data = new Vector3(BitConverter.ToSingle(m_DataBuffer, 0), BitConverter.ToSingle(m_DataBuffer, 4), BitConverter.ToSingle(m_DataBuffer, 8));					break;
					case DataType.Vector2Int:	m_Data = new Vector2Int(BitConverter.ToInt32(m_DataBuffer, 0), BitConverter.ToInt32(m_DataBuffer, 4));															break;
					case DataType.Vector3Int:	m_Data = new Vector3Int(BitConverter.ToInt32(m_DataBuffer, 0), BitConverter.ToInt32(m_DataBuffer, 4), BitConverter.ToInt32(m_DataBuffer, 8));					break;
					case DataType.Object:
					{
						var formatter = new BinaryFormatter();
						using (var stream = new MemoryStream(m_DataBuffer))
							m_Data = formatter.Deserialize(stream);
					}	break;
				}
			}
			catch (System.Exception ex)	
			{
				switch (m_DataType)
				{
					case DataType.Null:				m_Data = null;												break;
					case DataType.Int:				m_Data = 0;													break;
					case DataType.Float:			m_Data = 0.0f;												break;
					case DataType.Boolean:			m_Data = false;												break;
					case DataType.String:			m_Data = "";												break;
					case DataType.Vector2:			m_Data = default(Vector2);									break;
					case DataType.Vector3:			m_Data = default(Vector3);									break;
					case DataType.Vector2Int:		m_Data = default(Vector2Int);								break;
					case DataType.Vector3Int:		m_Data = default(Vector3Int);								break;
					case DataType.Object:			if (implIsNativeType(m_Data))
							m_Data = new object();		break;
					default:
						Debug.LogError(ex);		break;
				}
				OnBeforeSerialize();		// refill buffer to fix property drawler error
			}
		}
		
		private bool implIsNativeType(object data)
		{
			if (m_Data == null)								return true;
			var dataType = m_Data.GetType();
			if (dataType == typeof(int))					return true;
			else if (dataType == typeof(float))				return true;
			else if (dataType == typeof(bool))				return true;
			else if (dataType == typeof(string))			return true;
			else if (dataType == typeof(Vector2))			return true;
			else if (dataType == typeof(Vector3))			return true;
			else if (dataType == typeof(Vector2Int))		return true;
			else if (dataType == typeof(Vector3Int))		return true;

			return false;
		}

		public DataElementBase ExtractData()
		{
			return new DataElementBase(){ m_Data = m_Data };
		}
	}

	[Serializable]
	public enum DataType : int
	{
		Null,
		Int,
		Float,
		Boolean,
		String,
		Object,
		Vector2,
		Vector3,
		Vector2Int,
		Vector3Int,
	}
}

// DataContainerTemplate
[Serializable]
public class DataContainer<TKey, TDic> : DataContainerBase, ISerializationCallbackReceiver
	where TDic : SerializableDictionaryBase<TKey, DataContainerBase.DataElement>
{
	[SerializeField]
	private TDic								m_Data;
	private Dictionary<TKey, DataElementBase>	m_DataBase = new Dictionary<TKey, DataElementBase>();

	//////////////////////////////////////////////////////////////////////////
	public void OnBeforeSerialize()
	{
		if (m_Data == null || m_DataBase == null)
			return;

		if (implCheckSynchronization() == false)
		{
			m_Data.Clear();

			foreach (var n in m_DataBase)
			{
				var type = DataType.Null;
				if (n.Value.m_Data != null)
				{
					var dataType = n.Value.m_Data.GetType();
					if (dataType == typeof(int))
						type = DataType.Int;
					else if (dataType == typeof(float))
						type = DataType.Float;
					else if (dataType == typeof(bool))
						type = DataType.Boolean;
					else if (dataType == typeof(string))
						type = DataType.String;
					else if (dataType == typeof(Vector2))
						type = DataType.Vector2;
					else if (dataType == typeof(Vector3))
						type = DataType.Vector3;
					else if (dataType == typeof(Vector2Int))
						type = DataType.Vector2Int;
					else if (dataType == typeof(Vector3Int))
						type = DataType.Vector3Int;
					else
						type = DataType.Object;
				}

				var dataElement = new DataElement() { m_DataType = type, m_Data = n.Value.m_Data };
				m_Data.Add(n.Key, dataElement);
			}
		}
	}

	private bool implCheckSynchronization()
	{
		if (m_Data.Count != m_DataBase.Count)
			return false;

		foreach (var n in m_DataBase)
			if (m_Data.ContainsKey(n.Key))
			{
				var data = m_Data[n.Key];
				switch (data.m_DataType)
				{
					case DataType.Null:
						break;
					case DataType.Int:
						if (data.GetInt() != n.Value.GetInt())
							return false;
						break;
					case DataType.Float:
						if (data.GetFloat() != n.Value.GetFloat())
							return false;
						break;
					case DataType.Boolean:
						if (data.GetBool() != n.Value.GetBool())
							return false;
						break;
					case DataType.String:
						if (data.GetString() != n.Value.GetString())
							return false;
						break;
					case DataType.Vector2:
						if (data.GetVector2() != n.Value.GetVector2())
							return false;
						break;
					case DataType.Vector3:
						if (data.GetVector3() != n.Value.GetVector3())
							return false;
						break;
					case DataType.Vector2Int:
						if (data.GetVector2Int() != n.Value.GetVector2Int())
							return false;
						break;
					case DataType.Vector3Int:
						if (data.GetVector3Int() != n.Value.GetVector3Int())
							return false;
						break;
				}
			}
			else
				return false;

		return true;
	}

	public void OnAfterDeserialize()
	{
		if (m_Data == null)
			return;

		if (m_DataBase == null)
		{
			m_DataBase = new Dictionary<TKey, DataElementBase>();
		}
		else
		{
			m_DataBase.Clear();
		}
		foreach (var n in m_Data)
			m_DataBase.Add(n.Key, n.Value.ExtractData());

#if !UNITY_EDITOR
		m_Data = null;
#endif
	}

	//////////////////////////////////////////////////////////////////////////
	public DataContainer<TKey,TDic> Copy()
	{
		var result = new DataContainer<TKey,TDic>();
		result.m_Data = null;
		foreach (var n in m_DataBase)
		{
			result.AddKey(n.Key, n.Value.m_Data);
		}

		return result;
	}
	
	public void CopyFrom(Dictionary<TKey, DataElement> data)
	{
		foreach (var n in data)
			AddKey(n.Key, n.Value.m_Data);
	}

	public bool HasKey(TKey dataIndex)
	{
		return m_DataBase.ContainsKey(dataIndex);
	}

	public void RemoveKey(TKey index)
	{
		m_DataBase.Remove(index);
	}

	public void AddKey(TKey index, object value)
	{
		if (m_DataBase.ContainsKey(index))
			return;

		var dataElement = new DataElementBase(){ m_Data = value };
		m_DataBase.Add(index, dataElement);
	}


	////////////////////////////////////////////////////////////////////////
	public int GetInt(TKey index)
	{
		DataElementBase result;
		if (m_DataBase.TryGetValue(index, out result))
			return result.GetInt();

		return default;
	}

	public float GetFloat(TKey index)
	{
		DataElementBase result;
		if (m_DataBase.TryGetValue(index, out result))
			return result.GetFloat();

		return default;
	}
	
	public bool GetBool(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out var result))
			return result.GetBool();

		return default;
	}

	public bool GetBool(TKey index, bool valueMissing)
	{
		if (m_DataBase.TryGetValue(index, out var result))
			return result.GetBool();

		return valueMissing;
	}
	
	public string GetString(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out var result))
			return result.GetString();

		return "";
	}
	
	public Vector2 GetVector2(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out var result))
			return result.GetVector2();

		return default;
	}
	
	public Vector3 GetVector3(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out DataElementBase result))
			return result.GetVector3();

		return default;
	}
	
	public Vector2Int GetVector2Int(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out DataElementBase result))
			return result.GetVector2Int();

		return default;
	}

	public Vector3Int GetVector3Int(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out DataElementBase result))
			return result.GetVector3Int();

		return default;
	}
	
	public object GetKey(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out var result))
			return result.GetData();

		return null;
	}

	public T GetKey<T>(TKey index)
	{
		if (m_DataBase.TryGetValue(index, out DataElementBase result))
			return (T) result.GetData();

		return default;
	}

	public bool TryGetValue<T>(TKey index, out T value)
	{
		if (m_DataBase.TryGetValue(index, out var dataContainer))
		{
			value = (T)dataContainer.GetData();
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public void SetInt(TKey index, int value)
	{
		m_DataBase[index].SetInt(value);
	}
	
	public void SetFloat(TKey index, float value)
	{
		m_DataBase[index].SetFloat(value);
	}
	
	public void SetBool(TKey index, bool value)
	{
		m_DataBase[index].SetBool(value);
	}

	public void SetString(TKey index, string value)
	{
		m_DataBase[index].SetString(value);
	}
	
	public void SetVector2(TKey index, Vector2 value)
	{
		m_DataBase[index].SetVector2(value);
	}

	public void SetVector3(TKey index, Vector2 value)
	{
		m_DataBase[index].SetVector3(value);
	}
	
	public void SetVector2Int(TKey index, Vector2Int value)
	{
		m_DataBase[index].SetVector2Int(value);
	}
	
	public void SetVector3Int(TKey index, Vector3Int value)
	{
		m_DataBase[index].SetVector3Int(value);
	}
	
	public void SetValue<T>(TKey index, T value)
	{
		m_DataBase[index].SetData(value);
	}
	
}


// DataIndexContainer
[Serializable]
public class DataContainerDataIndex : DataContainer<DataIndex, DataContainerDataIndexDicitonary> {};
[Serializable]
public class DataContainerDataIndexDicitonary : SerializableDictionaryBase<DataIndex, DataContainerBase.DataElement> {};

// DataStringContainer
[Serializable]
public class DataContainerString : DataContainer<string, DataContainerStringDicitonary> {};
[Serializable]
public class DataContainerStringDicitonary : SerializableDictionaryBase<string, DataContainerBase.DataElement> {};

[Serializable]
public enum DataIndex
{
	None
}