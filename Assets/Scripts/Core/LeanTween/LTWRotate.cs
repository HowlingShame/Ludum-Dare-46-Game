using System;
using UnityEngine;

[Serializable]
public class LTWRotate : LeanTweenWrapper
{
    [SerializeField] 
    protected GameObject        m_Source;

    [SerializeField] 
    protected LeanTweenType     m_Ease = LeanTweenType.linear;
    
    [SerializeField]
    private float               m_Time = 1.0f;

    [SerializeField]
    private Vector3             m_Rotation;
    
    public Vector3              Rotation
    {
        set
        {
            m_Rotation = value;
        }
    }

    public override LTDescr Descriptor
    {
        get
        {
            // initialize if null
            if (m_Descriptor == null)
            {
                // instantiate descriptor
                m_Descriptor = LeanTween.rotate(m_Source, m_Rotation, m_Time);
                
                // set ease & onComplete event (set descriptor to null)
                m_Descriptor
                    .setEase(m_Ease)
                    .setOnComplete(() =>
                    {
                        m_Descriptor = null;
                    });

                // pause tween
                m_Descriptor.pause();
            }

            return m_Descriptor;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    public override LTDescr Start()
    {
        return Descriptor.resume();
    }

    public override LTDescr Pause()
    {
        return Descriptor.pause();
    }

    public override void Cancel()
    {
        if (m_Descriptor != null)
            LeanTween.cancel(m_Descriptor.uniqueId);
    }

    public void AddToSequence(LTSeq sequence)
    {
        sequence.append(Descriptor);
    }
}