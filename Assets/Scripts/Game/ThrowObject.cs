using UnityEngine;


// awake after threat, because we need thread time
[DefaultExecutionOrder(1)]
public class ThrowObject : MonoBehaviour
{
    public LTWMove  m_MoveTween;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        // move to position
        var threat = GetComponentInParent<Threat>();
        m_MoveTween.m_Time = threat.TimeLeft;
        m_MoveTween.MovePosition = PlayerEntity.Instance.m_RescurePosition.transform.position;
        m_MoveTween.Start();
    }
}

// awake after threat, because we need thread time