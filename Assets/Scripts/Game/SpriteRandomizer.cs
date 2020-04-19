using System;
using Malee;
using UnityEngine;

[Serializable]
public class SpriteRandomizer : MonoBehaviour
{
    [Serializable]
    public class RandomList : ReorderableArray<RandomElement> {}

    [Serializable]
    public class RandomElement
    {
        public Sprite   m_Sprite;
        //public float    m_Weight;
    }

    //////////////////////////////////////////////////////////////////////////
    [Reorderable(elementNameProperty = "m_Sprite")]
    public RandomList       m_RandomList;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = UnityRandom.RandomFromList(m_RandomList, null)?.m_Sprite;
    }

}