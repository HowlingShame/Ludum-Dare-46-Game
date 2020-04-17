using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public class Registry
{
	[SerializeField]
	protected RegistryDic		m_Data = new RegistryDic();

	//////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class RegistryDic: SerializableDictionaryBase<UniqueID, DataContainerString> {};

	/*[Serializable]
	public class RegistryUniqueID : UniqueID
	{
	}*/

	//////////////////////////////////////////////////////////////////////////
	public void SetValue<T>(UniqueID id, string key, T value)
	{
		if (m_Data.TryGetValue(id, out var data))
		{	// set value
			data.SetValue(key, value);
		}
		else
		{	// create container & add value
			var dataContainer = new DataContainerString();
			dataContainer.AddKey(key, value);
			m_Data.Add(id, dataContainer);
		}
	}

	public T GetValue<T>(UniqueID id, string key)
	{
		// get existing value or create new default
		if (m_Data.TryGetValue(id, out var data) == false)
		{
			data = new DataContainerString();
			m_Data.Add(id, data);
		}

		if (data.TryGetValue<T>(key, out var value) == false)
		{
			value = default;
			data.AddKey(key, value);
		}

		return value;
	}
	
	public bool TryGetValue<T>(UniqueID id, string key, out T value)
	{
		if (m_Data.TryGetValue(id, out var data))
			if (data.TryGetValue<T>(key, out var val))
			{
				value = val;
				return true;
			}

		value = default;
		return false;
	}
}
