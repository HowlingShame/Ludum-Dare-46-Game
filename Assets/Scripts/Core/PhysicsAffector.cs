using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gamelogic.Extensions;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsAffector : GLMonoBehaviour
{
	private Rigidbody2D			m_Rigidbody;
	
	public bool					m_MoveEnable;
	public Vector2				m_MovePosition;
	public float				m_MoveRotation;
	
	public bool					m_ForceEnable;
	public Vector2				m_Force;
	public ForceMode2D			m_PositionForceMode;
	public float				m_Tourque;
	public ForceMode2D			m_TourqueForceMode;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (m_MoveEnable)
		{
			m_Rigidbody.MovePosition(m_Rigidbody.position + m_MovePosition);
			m_Rigidbody.MoveRotation(m_Rigidbody.rotation + m_MoveRotation);
		}

		if (m_ForceEnable)
		{
			m_Rigidbody.AddForce(m_Force * Time.fixedDeltaTime, m_PositionForceMode);
			m_Rigidbody.AddTorque(m_Tourque * Time.fixedDeltaTime, m_TourqueForceMode);
		}
	}

	//////////////////////////////////////////////////////////////////////////
	[InspectorButton]
	void Zero()
	{
		ZeroVelocity();
		ZeroRotation();
	}
	[InspectorButton]
	void ZeroVelocity()
	{
		m_Rigidbody.velocity = Vector2.zero;
	}
	[InspectorButton]
	void ZeroRotation()
	{
		m_Rigidbody.angularVelocity = 0.0f;
	}

	[InspectorButton]
	void ForceOnce()
	{
		PosForceOnce();
		RotForceOnce();
	}
	[InspectorButton]
	void PosForceOnce()
	{
		m_Rigidbody.AddForce(m_Force, m_PositionForceMode);
	}
	[InspectorButton]
	void RotForceOnce()
	{
		m_Rigidbody.AddTorque(m_Tourque, m_TourqueForceMode);
	}
}
