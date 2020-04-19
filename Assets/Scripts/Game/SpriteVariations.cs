using System;
using System.Linq;
using Malee;
using UnityEngine;

[CreateAssetMenu]
public class SpriteVariations : ScriptableObject
{
    [Serializable]
    public class RollDataList : ReorderableArray<RollData> {}

    [Serializable]
    public class RollData
    {
        public float            m_Chanse = 1.0f;
        public Sprite           m_Sprite;
    }
    //////////////////////////////////////////////////////////////////////////
    public RollDataList                                 m_Data;
    private URandom.WeightedRandomCollection<Sprite>    m_WeightedBag;

    //////////////////////////////////////////////////////////////////////////
    public Sprite Roll()
    {
        if (m_WeightedBag == null)
            m_WeightedBag = UnityRandom.CreateWeightedBag(m_Data.Select(n => n.m_Sprite), m_Data.Select(n => n.m_Chanse));

        return m_WeightedBag.Next();
    }
}