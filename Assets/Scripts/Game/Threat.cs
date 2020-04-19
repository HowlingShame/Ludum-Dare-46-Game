using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class Threat : MonoBehaviour
{
    [ReadOnly]
    public float            TimeLeft;
    [ReadOnly]
    public bool             Failed;

    public MinMaxCurveAsset m_DurationCurve;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        TimeLeft = m_DurationCurve.m_Curve.Evaluate(Random.value, Random.value);
        Failed = false;
    }

    public void Run()
    {
        PlayerEntity.Instance.m_CurrentThreat = this;
    }

    public void Execute()
    {
        if (PlayerEntity.Instance.m_CurrentThreat == this)
            PlayerEntity.Instance.m_CurrentThreat = null;

        if (Failed)
            return;

        PlayerEntity.Instance.Loose();
    }
}
