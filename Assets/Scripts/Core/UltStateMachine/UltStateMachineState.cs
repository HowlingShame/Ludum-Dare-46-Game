using System;
using UltEvents;

[Serializable]
public class UltStateMachineState
{
	public UltEvent		onStart;
	public UltEvent		onStop;
	public UltEvent		onUpdate;
	public UltEvent		onReEnter;
}