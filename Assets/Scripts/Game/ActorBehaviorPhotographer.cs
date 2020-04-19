using UnityEngine;

public class ActorBehaviorPhotographer : Actor.BehaviorBase
{
    [Range(0.0f, 1.0f)]
    public float    m_PhotoChanse;
    public Vector2  m_PhotoDelay;

    //////////////////////////////////////////////////////////////////////////
    public override void iProceedTurn()
    {
        if (UnityRandom.Bool(m_PhotoChanse))
            PlayerEntity.Instance.m_ActionSequence.Add(new Actor.ActionPhoto()
            {
                m_Delay  = Random.Range(m_PhotoDelay.x, m_PhotoDelay.y),
                Master = Master
            });
    }
}