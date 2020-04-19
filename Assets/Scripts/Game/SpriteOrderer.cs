using System;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

[Serializable]
public class SpriteOrderer : MonoBehaviour
{
    private const float             c_HeightMultiplyer = 1000.0f;
    private const float             c_ZMultiplyer = 0.001f;
    private List<SpriteRenderer>    m_RendererList;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        m_RendererList = new List<SpriteRenderer>();
        m_RendererList.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }

    private void Update()
    {
        foreach (var spriteRenderer in m_RendererList)
            spriteRenderer.sortingOrder = 10000000 - Mathf.RoundToInt((transform.position.y + 1000.0f) * c_HeightMultiplyer - 1000.0f);
        transform.position = transform.position.WithZ(transform.position.y * c_ZMultiplyer);
    }
}