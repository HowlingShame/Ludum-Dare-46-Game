using Gamelogic.Extensions;
using UnityEngine;
// ReSharper disable SimplifyConditionalTernaryExpression

public class ModelModifier : MonoBehaviour
{
    private float                   m_Offset;
    private Vector3                 m_InitialScale;
    public AnimationCurveAsset      m_ScaleCurve;
    public bool                     m_RandomFlipX;
    public Vector2                  m_ScaleRoll;
    [SerializeField]
    private Color                   m_Color = UnityEngine.Color.white;

    public Color Color
    {
        get => m_Color;
        set
        {
            m_Color = value;

            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
                spriteRenderer.color = m_Color;

        }
    }

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        m_InitialScale = transform.localScale;
        m_Offset = Random.Range(m_ScaleCurve.m_AnimationCurve.keys[0].time,
            m_ScaleCurve.m_AnimationCurve.keys[m_ScaleCurve.m_AnimationCurve.length - 1].time);

        if (m_RandomFlipX)
        {
            var flipRoll = UnityRandom.Bool();
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
                spriteRenderer.flipX = flipRoll;
        }

        m_InitialScale += UnityRandom.Vector2(m_ScaleRoll).To3DXY();

        Color = m_Color;
    }

    private void Update()
    {
        transform.localScale = new Vector3(m_InitialScale.x, m_InitialScale.y * m_ScaleCurve.m_AnimationCurve.Evaluate(Time.time + m_Offset), m_InitialScale.z);
    }
}