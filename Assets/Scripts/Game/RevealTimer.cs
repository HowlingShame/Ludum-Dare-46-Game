using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RevealTimer : MonoBehaviour
{
    public Image        m_Progress;
    public GameObject   m_Root;
    public float        m_Time;
    public Actor        m_Target;

    //////////////////////////////////////////////////////////////////////////
    private void Update()
    {
        if (m_Target == null)
            return;

        while (m_Time > 0.0f)
        {
            m_Root.transform.position = Input.mousePosition;

            var scale = m_Time / PlayerEntity.Instance.m_RevielTime;
            m_Progress.fillAmount = scale;

            m_Time -= Time.deltaTime;
            if (m_Time <= 0.0f)
            {
                // reveal
                if (m_Target.m_DangerLevel > 0)
                {
                    m_Target.GetComponentInChildren<ModelModifier>().Color = PlayerEntity.Instance.m_DangerColor;
                }
                else
                if (m_Target.IsVisitor)
                {
                    m_Target.GetComponentInChildren<ModelModifier>().Color = PlayerEntity.Instance.m_CivilianColor;
                }
                if (m_Target.IsPhotographer || m_Target.IsScreamer)
                {
                    m_Target.GetComponentInChildren<ModelModifier>().Color = PlayerEntity.Instance.m_NoizeColor;
                }

                
                SoundManager.Instance.PlaySound("Reveal");
                StopReveal();
            }
            return;
        }
    }

    public void StartReveal(Actor actor)
    {
        m_Target = actor;
        m_Time = PlayerEntity.Instance.m_RevielTime;
        m_Progress.enabled = true;
    }

    public void StopReveal()
    {
        m_Target = null;
        m_Time = 0.0f;
        m_Progress.enabled = false;
    }
}