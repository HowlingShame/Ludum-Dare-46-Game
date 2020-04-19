using UnityEngine;

public class ActorBehaviorScreamer : Actor.BehaviorBase, Actor.IBehavior
{
    [Range(0.0f, 1.0f)]
    public float    m_Chanse;
    public Vector2  m_ScreamDelay;

    //////////////////////////////////////////////////////////////////////////
    public override void iProceedTurn()
    {
        if (UnityRandom.Bool(m_Chanse))
            PlayerEntity.Instance.m_ActionSequence.Add(new Actor.ActionScream()
            {
                m_Delay = Random.Range(m_ScreamDelay.x, m_ScreamDelay.y),
                Master = Master
            });
    }
}