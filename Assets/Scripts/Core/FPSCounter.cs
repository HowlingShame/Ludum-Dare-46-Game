using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltEvents;
using UnityEngine;

[Serializable]
public class FPSCounter : MonoBehaviour
{
    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class UpdateEvent : UltEvent<float> {}

    //////////////////////////////////////////////////////////////////////////
    public float                    c_FPSRange = 1.0f;

    public float                    m_FPS;
    private LinkedList<float>       m_FPSHistory = new LinkedList<float>();

    public float                    FPS => m_FPS;
    public TMP_Text                 m_OutpuText;

    //////////////////////////////////////////////////////////////////////////
    public void Update()
    {
        // add frame time
        m_FPSHistory.AddLast(Time.deltaTime);
        // get sum
        var sum = m_FPSHistory.Sum(n => n);
        // cut sum to observed range
        while (m_FPSHistory.Count != 0 && sum > c_FPSRange)
        {
            sum -= m_FPSHistory.First.Value;
            m_FPSHistory.RemoveFirst();
        }

        // count * ratio / frame diapason
        m_FPS = (m_FPSHistory.Count * (c_FPSRange / sum)) / c_FPSRange;

        // set text
        if (m_OutpuText != null)
            m_OutpuText.text = m_FPS.ToString("##.00");
    }
}