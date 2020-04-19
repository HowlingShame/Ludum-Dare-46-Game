using System;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodyguardSlot : MonoBehaviour, IPointerClickHandler
{
    public bool         HasCharge => m_BodyguardModel != null;
    public GameObject   m_BodyguardPrefab;
    public GameObject   m_RescureEffectPrefab;
    [NonSerialized]
    public GameObject   m_BodyguardModel;
    [ReadOnly]
    public float        m_RestoreTime;
    public LTWMove      m_MoveTween;
    public LTWMove      m_RescureTween;
    public GameObject   m_BodyguardPosition;
    public GameObject   m_BodyguardSpawn;
    public Image        m_RestoreImage;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        m_BodyguardModel = Instantiate(m_BodyguardPrefab, transform);
        m_BodyguardModel.transform.position = m_BodyguardPosition.transform.position;
        m_RestoreImage.enabled = false;
    }

    public float Detain(Actor actor, out GameObject model)
    {
        // 
        m_MoveTween.Cancel();
        m_MoveTween.Source = m_BodyguardModel;
        m_MoveTween.MovePosition = actor.transform.position;

        model = m_BodyguardModel;
        m_BodyguardModel = null;

        // start restore coroutine
        m_RestoreTime = PlayerEntity.Instance.m_BodyguardDetainRestore;
        StartCoroutine(implRestore());
        
        // disable model colliders
        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        return m_MoveTween.Instance(model).time;
    }

    //////////////////////////////////////////////////////////////////////////
    private IEnumerator implRestore()
    {
        yield return null;
        var initialTime = m_RestoreTime;
        if (m_RestoreImage != null)
            m_RestoreImage.enabled = true;

        while (m_RestoreTime > 0.0f)
        {
            if (m_RestoreImage != null)
                m_RestoreImage.fillAmount = 1.0f - m_RestoreTime / initialTime;
            if (PlayerEntity.Instance.m_Pause == false && PlayerEntity.Instance.m_WinLooseLock == false)
                m_RestoreTime -= Time.deltaTime;
            yield return null;
        }
        if (m_RestoreImage != null)
            m_RestoreImage.enabled = false;

        m_RestoreTime = 0.0f;

        // instantiate model
        m_BodyguardModel = Instantiate(m_BodyguardPrefab, transform);
        m_BodyguardModel.transform.position = m_BodyguardSpawn.transform.position + UnityRandom.Vector2(PlayerEntity.Instance.m_PositionRandomization).To3DXY();
        // move to position
        m_MoveTween.Cancel();
        m_MoveTween.MovePosition = m_BodyguardPosition.transform.position;
        m_MoveTween.Source = m_BodyguardModel;
        m_MoveTween.Start();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Protect();
    }

    public void Protect()
    {
        if (HasCharge)
        {
            if (PlayerEntity.Instance.m_CurrentThreat != null
                && PlayerEntity.Instance.m_CurrentThreat.Failed == false)
            {
                // eject bodyguard
                var model = m_BodyguardModel;
                m_BodyguardModel = null;

                m_RescureTween.MovePosition = PlayerEntity.Instance.m_RescurePosition.transform.position;
                m_RescureTween
                    .Instance(model)
                    .setTime(PlayerEntity.Instance.m_CurrentThreat.TimeLeft)
                    .setOnComplete(() =>
                    {
                        // spawn effect
                        var effect = Instantiate(m_RescureEffectPrefab);
                        effect.transform.position = model.transform.position;

                        // destroy model
                        Destroy(model);
                    });

                // destroy colliders
                foreach (var collider in model.GetComponentsInChildren<Collider2D>())
                    Destroy(collider);

                // threat failed now
                PlayerEntity.Instance.m_CurrentThreat.Failed = true;


                // start restore coroutine
                m_RestoreTime = PlayerEntity.Instance.m_BodyguardDeadRestore;
                StartCoroutine(implRestore());
            }
            else
            {
                SoundManager.Instance.PlaySound("Ban");
            }
        }
    }
}