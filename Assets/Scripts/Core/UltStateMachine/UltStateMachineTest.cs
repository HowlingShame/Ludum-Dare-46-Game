using System;
using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UltEvents;
using UnityEngine;

public class UltStateMachineTest : MonoBehaviour
{
	public UltStateMachineString		m_StateMachine;
	public TestDic	m_TestDic;

	[Serializable]
	public class UltStateMachineString : UltStateMachine<string, StringStateDictionary>{}
	[Serializable]
	public class StringStateDictionary : SerializableDictionaryBase<string, UltStateMachineState> {};

	[Serializable]
	public class TestDic : SerializableDictionaryBase<string, UltEvent> {};
}
