using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class OrientedSprite : GLMonoBehaviour 
{
    public const string     c_DirectionX = "DirectionX"; 
    public const string     c_DirectionY = "DirectionY";

    [SerializeField]
    private OrientationMode m_Mode = OrientationMode.MainCamera;
    
    [DrawIf("m_Mode", OrientationMode.Target)]
    public Transform        m_Target;
    [NonSerialized]
    public Transform        m_CurrentTarget;

	[Space]
    public Animator         m_Animator;
    [DrawIf("m_Animator")]
	public float            m_AnimatorOrientation;	// in radians
	public float            m_OrientationDegree;
	
	[HideInInspector, NonSerialized]
	public float            m_Atan;
	[HideInInspector, NonSerialized]
	public Vector3          m_ToTarget;

    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public enum OrientationMode
    {
        None,
        MainCamera,
        Target,
    }

    public OrientationMode Mode
    {
        get => m_Mode;
        set
        { 
            if (m_Mode == value)
                return;

            // apply mode
            m_Mode = value;
            implApplyMode();
        }
    }

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        implApplyMode();
    }

    public void Update()
	{
        if (m_CurrentTarget == null)
            return;

        // update values
		m_ToTarget = (m_CurrentTarget.position - transform.position);
		m_Atan = Mathf.Atan2(m_ToTarget.z, m_ToTarget.x);
		
		m_OrientationDegree = (m_Atan + Mathf.PI * 0.5f) * Mathf.Rad2Deg;

        // set rotation to camera
		transform.rotation = Quaternion.AngleAxis(m_OrientationDegree, Vector3.down);
		
        // apply to animator
        if (m_Animator != null)
        {
            m_Animator.SetFloat(c_DirectionX, Mathf.Cos(m_Atan - m_AnimatorOrientation));
            m_Animator.SetFloat(c_DirectionY, Mathf.Sin(m_Atan - m_AnimatorOrientation));
        }
	}

    public void LieDown(Vector2 direction)
	{
		enabled = false;
		
		var atan = Mathf.Atan2(-direction.y, direction.x);
		transform.rotation = Quaternion.AngleAxis((atan + Mathf.PI * 0.5f) * Mathf.Rad2Deg, Vector3.back);

        // look forward
		m_Animator.SetFloat(c_DirectionX, 1.0f);
		m_Animator.SetFloat(c_DirectionY, 0.0f);
	}
	
	public void GetUp()
	{
		enabled = true;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(-m_AnimatorOrientation), 0.0f, Mathf.Sin(m_AnimatorOrientation)));
	}
    
    //////////////////////////////////////////////////////////////////////////
    private void implApplyMode()
    {
        switch (m_Mode)
        {
            case OrientationMode.None:
                m_CurrentTarget = null;
                break;
            case OrientationMode.MainCamera:
                m_CurrentTarget = Core.Instance.m_Camera.transform;
                break;
            case OrientationMode.Target:
                m_CurrentTarget = m_Target;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(m_Mode), m_Mode, null);
        }
    }
}
