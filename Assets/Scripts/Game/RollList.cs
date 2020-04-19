using System;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using Malee;
using UnityEngine;

[CreateAssetMenu]
public class RollList : ScriptableObject
{
    [Serializable]
    public class RollDataList : ReorderableArray<RollData> {}

    [Serializable]
    public class RollData
    {
        public AnimationCurve   m_Chanse;
        public GameObject       m_Prefab;
    }

    //////////////////////////////////////////////////////////////////////////
    [Reorderable(elementNameProperty = "m_Prefab")]
    public RollDataList     m_Data;
    private URandom.WeightedRandomCollection<GameObject> m_WeightedBag;

    //////////////////////////////////////////////////////////////////////////
    public GameObject Roll(float scale)
    {
        if (m_WeightedBag == null)
            m_WeightedBag = UnityRandom.CreateWeightedBag(m_Data.Select(n => n.m_Prefab), m_Data.Select(n => n.m_Chanse.Evaluate(scale)));

        var result = m_WeightedBag.Next();
        return result;
    }

}