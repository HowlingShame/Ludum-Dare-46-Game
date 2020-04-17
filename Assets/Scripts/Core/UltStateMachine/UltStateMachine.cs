using System;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;
using RotaryHeart.Lib.SerializableDictionary;
using UltEvents;
using UnityEngine;

[Serializable]
public class UltStateMachine<TLabel, TDic> where TDic : IDictionary<TLabel, UltStateMachineState>
{
	private UltStateMachineState		currentState;
	
	[SerializeField]
	private TLabel		currentStateLabel;
	[SerializeField]
	private TDic		stateDictionary;

	//////////////////////////////////////////////////////////////////////////
	public TLabel CurrentState
	{
		get { return currentStateLabel; }
		set { ChangeState(value); }
	}
	
	public void Update()
	{
		currentState?.onUpdate?.Invoke();
	}

	
	public void AddState(TLabel label, UltEvent onStart = null, UltEvent onUpdate = null, UltEvent onStop = null, UltEvent onReEnter = null)
	{
		stateDictionary[label] = new UltStateMachineState()
		{
			onStart		= onStart,
			onUpdate	= onUpdate,
			onStop		= onStop, 
			onReEnter	= onReEnter
		};
	}

	public override string ToString()
	{
		return CurrentState.ToString();
	}

	private void ChangeState(TLabel newState)
	{
#if UNITY_EDITOR
		if (stateDictionary.ContainsKey(newState) == false)
			throw new ArgumentOutOfRangeException($"State \"{newState.ToString()}\" not presented in dictionary.");
#endif
		var dicState = stateDictionary[newState];

		if (currentState != null)
		{
			if (currentStateLabel.Equals(newState))
			{	// same state, activate ReEnter only if not null, else normal behaviour
				if (currentState.onReEnter != null)
				{	// activate ReEnter
					currentState.onReEnter.Invoke();
				}
				else
				{	// reactivate current state
					currentState.onStop?.Invoke();
					currentState.onStart?.Invoke();
				}
			}
			else
			{	// switch state
				currentState.onStop?.Invoke();
				currentState = dicState;
				currentState.onStart?.Invoke();
			}
		}
		else
		{	// set new state
			currentState = dicState;
			currentState.onStart?.Invoke();
		}

		currentStateLabel = newState;
	}
}