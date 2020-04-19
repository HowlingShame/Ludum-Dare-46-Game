using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class Actor : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    public enum MovePatter
    {
        MoveAtRow,
    }

    public interface IBehavior
    {
        Actor Master { get; set; }

        void iInit();
        void iSwapExecuted();
        bool iSwapRequest(Actor pretender);
        void iProceedTurn();
    }
    
    [Serializable]
    public abstract class BehaviorBase : MonoBehaviour, IBehavior
    {
        public Actor Master { get; set; }

        public virtual void iInit() {}
        public virtual void iSwapExecuted() {}
        public virtual bool iSwapRequest(Actor pretender) { return true; }
        public virtual void iProceedTurn() {}
    }

    //////////////////////////////////////////////////////////////////////////
    [NaughtyAttributes.ReadOnly]
    public Slot             m_Slot;

    public IBehavior        m_Behavior;

    public MovePatter       m_MovePatter;
    [DrawIf("m_MovePatter", MovePatter.MoveAtRow)]
    public int              m_TargetRow;

    public int              m_Weight;
    
    public int              m_MaxMoves;
    [NaughtyAttributes.ReadOnly]
    public int              m_MovesLeft;

    public int              m_DangerLevel;

    public bool             IsInTargetRow => m_Slot != null && m_Slot.m_Position.y == m_TargetRow;

    public LTWMove          m_MoveTween;
    public LTWMove          m_LeaveTween;
    public LTWMove          m_DetainedTween;

    public bool             IsThrower => m_Behavior is ActorBehaviorThrower;
    public bool             IsSniper => m_Behavior is ActorBehaviorSniper;

    public bool             IsVisitor => m_Behavior is ActorBehaviorVisitor;
    public bool             IsPhotographer => m_Behavior is ActorBehaviorPhotographer;
    public bool             IsScreamer => m_Behavior is ActorBehaviorScreamer;


    private bool            m_PretendLock;
    private bool            m_SwapLock;
    public GameObject       m_EffectSpawnPoint;
    private ModelModifier   m_ModelModifier;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        m_ModelModifier = GetComponentInChildren<ModelModifier>();
        m_TargetRow = Mathf.Clamp(m_TargetRow, 0, Grid.Instance.m_Column - 1);
        m_Behavior = GetComponent<BehaviorBase>();
        m_Behavior.Master = this;
        m_Behavior.iInit();

        transform.position = UnityRandom.BoundPoint(PlayerEntity.Instance.m_ZoneSpawn);

        // restore moves
        m_MovesLeft = m_MaxMoves;

        // add to reserve 
        PlayerEntity.Instance.m_ReserveActors.Add(this);
        // increase danger level
        PlayerEntity.Instance.m_CurrentDangers += m_DangerLevel;
        // increase visitors count
        PlayerEntity.Instance.m_CurrentVisitors ++;
    }

    private void OnDestroy()
    {
        // decrease danger level
        PlayerEntity.Instance.m_CurrentDangers -= m_DangerLevel;
        // decrease visitors count
        PlayerEntity.Instance.m_CurrentVisitors --;
        
        PlayerEntity.Instance.m_BoardActors.Remove(this);
        PlayerEntity.Instance.m_ReserveActors.Remove(this);

        if (PlayerEntity.Instance.m_RevealTimer.m_Target == this)
            PlayerEntity.Instance.m_RevealTimer.StopReveal();
    }

    public void Pretend()
    {
        // call once per turn
        if (m_PretendLock)
            return;

        m_PretendLock = true;
        m_SwapLock = false;

        if (m_Slot != null)
        {
            List<Slot> options = null;
            switch (m_MovePatter)
            {
                case MovePatter.MoveAtRow:
                {
                    // move in grid
                    if (m_TargetRow > m_Slot.m_Position.y)
                    {
                        // move top
                        options = m_Slot.GetTransitions(Direction.Top, true);
                    }

                    if (m_TargetRow < m_Slot.m_Position.y)
                    {
                        if (m_TargetRow < 0 && m_Slot.m_Position.y == 0)
                        {
                            // leave
                            Leave();
                        }
                        else
                        {
                            // move bottom
                            options = m_Slot.GetTransitions(Direction.Bottom, true);
                        }
                    }
                }   break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // pretend if has options
            if (options != null)
            {
                var slot = options.FirstOrDefault(n => n.Pretend(this));

                if (slot != null)
                {
                    // swap
                    m_Slot.m_Actor = slot.m_Actor;
                    if (slot.m_Actor != null)
                    {
                        slot.m_Actor.m_Slot = m_Slot;
                        slot.m_Actor.SwapExecuted();
                    }

                    // assign slot
                    slot.m_Actor = this;
                    m_Slot = slot;
                    SwapExecuted();
                }
            }
        }
        else
        {
            if (m_TargetRow < 0)
            {
                // leave scene
                Leave();
            }
            else
            {
                // pretend at any position in start row
                var options = Grid.Instance.GetRow(0, true);

                var slot = options.FirstOrDefault(n => n.Pretend(this));
                if (slot != null)
                {
                    // enter
                    slot.m_Actor?.Leave();
                    m_Slot = slot;
                    m_Slot.m_Actor = this;
                    Enter();
                }
            }
            
        }
    }

    public bool SwapRequest(Actor pretender)
    {
        // swap lock check
        if (m_SwapLock)
            return false;

        // weight check
        if (pretender.m_Weight <= m_Weight)
            return false;

        // move limit check
        if (m_MovesLeft <= 0)
            return false;
            
        // behavior check
        if (m_Behavior.iSwapRequest(pretender) == false)
            return false;

        return true;
    }

    public void SwapExecuted()
    {
        if (m_ModelModifier.Color != Color.white)
            m_ModelModifier.Color = Color.Lerp(m_ModelModifier.Color, Color.white, 0.3f);

        m_MovesLeft --;
        m_Behavior.iSwapExecuted();
    }

    public void Enter()
    {
        m_SwapLock = true;

        ImplementMove();
        // move to board list from reserve list
        PlayerEntity.Instance.m_BoardActors.Add(this);
        PlayerEntity.Instance.m_ReserveActors.Remove(this);
    }

    public void Leave()
    {
        if (m_Slot != null)
        {
            m_Slot.m_Actor = null;
            m_Slot = null;
        }

        PlayerEntity.Instance.m_BoardActors.Remove(this);
        PlayerEntity.Instance.m_ReserveActors.Remove(this);

        m_LeaveTween.MovePosition = UnityRandom.BoundPoint(PlayerEntity.Instance.m_ZoneLeave);
        m_LeaveTween
            .Start()
            .setOnComplete(() =>
            {
                Destroy(gameObject);
            });
    }

    public void Detained()
    {
        var bodyguard = PlayerEntity.Instance.GetBodyguard();

        if (bodyguard == null)
        {
            SoundManager.Instance.PlaySound("Ban");
            return;
        }

        if (m_Slot != null)
        {
            m_Slot.m_Actor = null;
            m_Slot = null;
        }

        // play sound
        SoundManager.Instance.PlaySound("Detain");

        // disable colliders
        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        PlayerEntity.Instance.m_BoardActors.Remove(this);
        PlayerEntity.Instance.m_ReserveActors.Remove(this);

        var closestDetainedBounds = 
            Vector3.Distance(PlayerEntity.Instance.m_ZoneDetaindLeft.ClosestPoint(transform.position), transform.position) 
            < Vector3.Distance(PlayerEntity.Instance.m_ZoneDetaindRight.ClosestPoint(transform.position), transform.position)
                ? PlayerEntity.Instance.m_ZoneDetaindLeft
                : PlayerEntity.Instance.m_ZoneDetaindRight;

        // play move animation
        m_DetainedTween.MovePosition = UnityRandom.BoundPoint(closestDetainedBounds);
        m_DetainedTween
            .Start()
            .setDelay(bodyguard.Detain(this, out var model))
            .setOnStart(() =>
            {
                // link model
                model.transform.SetParent(transform, false);
                model.transform.localPosition = UnityRandom.Vector2(PlayerEntity.Instance.m_PositionRandomization).WithY(-0.1f);
            })
            .setOnComplete(() =>
            {
                Destroy(model);
                Destroy(gameObject);
            });

    }

    public void ImplementTurn()
    {
        // restore pretend
        m_PretendLock = false;
        if (m_Slot != null)
        {
            // restore moves
            m_MovesLeft = m_MaxMoves;
            m_Behavior.iProceedTurn();
        }
    }
    
    [Button]
    public void ImplementMove()
    {
        if (m_Slot != null)
        {
            m_MoveTween.Cancel();
            m_MoveTween.MovePosition = m_Slot.transform.position + UnityRandom.Vector2(PlayerEntity.Instance.m_PositionRandomization).To3DXY();
            m_MoveTween.Start();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((PlayerEntity.Instance.m_DoubleClickToDetain ? eventData.clickCount == 2 : eventData.clickCount == 1)
            && eventData.button == 0)
        {
            // double click event, detain
            Detained();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerEntity.Instance.m_RevealTimer.StartReveal(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerEntity.Instance.m_RevealTimer.StopReveal();
    }
}