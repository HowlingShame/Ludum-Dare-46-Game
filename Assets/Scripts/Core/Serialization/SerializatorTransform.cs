using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializatorTransform : MonoBehaviour, SerializationManager.IComponent
{
	private const string c_KeyPosition		= "pos";
	private const string c_KeyRotation		= "rot";
	private const string c_KeyScale			= "scale";
	
	//////////////////////////////////////////////////////////////////////////
	public void iSave(SerializationManager.SerializationInfoWrapper info)
	{
		info.AddValue(c_KeyPosition, gameObject.transform.localPosition);
		info.AddValue(c_KeyRotation, gameObject.transform.localRotation);
		info.AddValue(c_KeyScale, gameObject.transform.localScale);
	}

	public void iLoad(SerializationManager.SerializationInfoWrapper info)
	{
		gameObject.transform.localPosition	= info.GetValue<Vector3>(c_KeyPosition);
		gameObject.transform.localRotation	= info.GetValue<Quaternion>(c_KeyRotation);
		gameObject.transform.localScale		= info.GetValue<Vector3>(c_KeyScale);
	}
}