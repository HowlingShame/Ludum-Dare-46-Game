using UnityEngine;

[DefaultExecutionOrder(1)]
public class AimObject : MonoBehaviour
{
    private static int  c_AnimationTimeHash = Animator.StringToHash("Scale");

    private float       m_Duration;
    private float       m_CurrentTime;
    public Animator     m_Animator;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        var threat = GetComponentInParent<Threat>();
        m_Duration = threat.TimeLeft;
        m_CurrentTime = 0.0f;
        
        transform.SetParent(null, false);
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        m_CurrentTime += Time.deltaTime;
        var scale = m_CurrentTime / m_Duration;
        m_Animator.SetFloat(c_AnimationTimeHash, scale);
        if (scale > 1.0f)
            Destroy(gameObject);
    }
}