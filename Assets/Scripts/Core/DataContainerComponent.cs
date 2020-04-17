using Gamelogic.Extensions;
using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class DataContainerComponent : GLMonoBehaviour
{
#pragma warning disable 649
	public DataContainerString			m_DataString;
	[SerializeField]
	private DataContainerDataIndex		m_Data;
	/* 
	[DrawKeyAsProperty]
	public TestCustom					m_TestCustom;*/
	public TypeReference				m_Value;
#pragma warning restore 649

	public DataContainerDataIndex		Data => m_Data;
	
	[Serializable]
	public class Foo
	{
		string g = "123";
		string a;
	}
	[InspectorButton]
	public void Add()
	{
		m_Data.AddKey(DataIndex.None, new Foo());
	}
	[InspectorButton]
	public void Show()
	{
		var foo = m_Data.GetKey(DataIndex.None);
		Debug.Log(foo.GetType());
	}

	[Serializable]
	public class TestCustom : SerializableDictionaryBase<AdvancedGenericClass, string> {};
	[System.Serializable]
	public class AdvancedGenericClass
	{
		public TypeReference		m_Value;

		public bool Equals(AdvancedGenericClass other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.m_Value.Type == m_Value.Type;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(AdvancedGenericClass)) return false;
			return Equals((AdvancedGenericClass)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return m_Value.GetHashCode();
			}
		}
	}
}