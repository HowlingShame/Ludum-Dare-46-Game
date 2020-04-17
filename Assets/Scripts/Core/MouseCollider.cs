using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCollider : MonoBehaviour
{
	public CircleCollider2D		m_Collider;
	public Rigidbody2D			m_RigidBody;
	public const int			m_MouseColliderLayer = 31;
	public const float			m_ColliderRadius = 0.01f;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		gameObject.layer = m_MouseColliderLayer;

		m_Collider = gameObject.AddComponent<CircleCollider2D>();
		m_Collider.radius = m_ColliderRadius;

		m_RigidBody = gameObject.AddComponent<Rigidbody2D>();
		m_RigidBody.isKinematic = true;
	}

	private void LateUpdate()
	{
		m_RigidBody.MovePosition(Core.Instance.MouseWorldPosition.WorldPosition);
	}
}
