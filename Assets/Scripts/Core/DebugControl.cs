using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugControl : GLMonoBehaviour 
{
	public float m_MouseScrollScale = 1.0f;
	public float m_MouseMoveScale = 0.06f;

	public float m_KeyboardMoveScale = 10.0f;

	protected Vector3 m_MousePosLast;

	private Vector3		m_ForvardVector;
	private Vector3		m_RightVector;
	private Vector3		m_UpVector;

	public int			m_DragMouseButton;
	public bool			m_EnableArrowKeysMovement = true;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		hideFlags = HideFlags.HideInInspector;
		switch (Core.Instance.m_ProjectSpace)
		{
			case Core.ProjectSpace.XY:
				m_ForvardVector = Vector3.up;
				m_RightVector = Vector3.right;
				break;
			case Core.ProjectSpace.XZ:
				m_ForvardVector = Vector3.forward;
				m_RightVector = Vector3.right;
				break;
			default:
				m_ForvardVector = Vector3.up;
				m_RightVector = Vector3.right;
				break;
		}

		m_UpVector = Vector3.Cross(m_ForvardVector, m_RightVector);
	}

	private void Update()
	{
		if (m_EnableArrowKeysMovement)
		{
			Vector3 translateVector = Vector3.zero;

			if (Input.GetKey(KeyCode.UpArrow))
				translateVector += m_ForvardVector;
			if (Input.GetKey(KeyCode.DownArrow))
				translateVector -= m_ForvardVector;
			if (Input.GetKey(KeyCode.LeftArrow))
				translateVector -= m_RightVector;
			if (Input.GetKey(KeyCode.RightArrow))
				translateVector += m_RightVector;
			
			if (Input.GetKey(KeyCode.RightShift))
				translateVector += m_UpVector;
			if (Input.GetKey(KeyCode.RightControl))
				translateVector -= m_UpVector;

			if (translateVector != Vector3.zero)
			{
				translateVector.Normalize();
				Core.Instance.m_Camera.gameObject.transform.position += (translateVector * m_KeyboardMoveScale * Time.deltaTime);
			}
		}

		if (m_DragMouseButton != -1)
		{
			var view = Core.Instance.m_Camera.ScreenToViewportPoint(Input.mousePosition);

			if ((view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1) == false)
			{
				if (Input.GetMouseButton(m_DragMouseButton))
				{
					Vector3 offset = m_MousePosLast - Input.mousePosition;
					if (offset.magnitude < 40.0f)
						switch (Core.Instance.m_ProjectSpace)
						{
							case Core.ProjectSpace.XY:
								Core.Instance.m_Camera.transform.position += ((offset * m_MouseMoveScale).WithZ(0.0f));
								break;
							case Core.ProjectSpace.XZ:
								Core.Instance.m_Camera.transform.position += ((offset * m_MouseMoveScale).WithZ(0.0f)).XZY();
								break;
						}
				}


				{   // scroll
					var scrollImpact = Input.mouseScrollDelta.y * m_MouseScrollScale;

					if (Core.Instance.m_Camera.orthographic)
						Core.Instance.m_Camera.orthographicSize = Mathf.Clamp(Core.Instance.m_Camera.orthographicSize - scrollImpact, 1.0f, int.MaxValue);
					else
					{
						switch (Core.Instance.m_ProjectSpace)
						{
							case Core.ProjectSpace.XY:
								Core.Instance.m_Camera.transform.TranslateZ(scrollImpact);
								break;
							case Core.ProjectSpace.XZ:
								Core.Instance.m_Camera.transform.TranslateY(scrollImpact);
								break;
						}
					}
				}
			}

			m_MousePosLast = Input.mousePosition;
		}
	}

	//
	private void OnGUI()
	{
	}
}
