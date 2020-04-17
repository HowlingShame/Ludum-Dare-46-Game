using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class NormalAttribute : PropertyAttribute
{
	public Vector4	m_DefaultValue;
	
	public NormalAttribute(float x = 0.0f, float y = 0.0f, float z = 0.0f, float w = 0.0f)
	{
		this.m_DefaultValue = new Vector4(x, y, z, w);
	}
}
