using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    public float        m_LileTime;

    //////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (m_LileTime <= 0.0f)
            Destroy(gameObject);
        m_LileTime -= Time.deltaTime;
    }
}
