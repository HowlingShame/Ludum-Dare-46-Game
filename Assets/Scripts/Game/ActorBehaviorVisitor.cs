using System;
using UnityEngine;

public class ActorBehaviorVisitor : Actor.BehaviorBase, Actor.IBehavior
{
    public int m_MoveWeightExcange;

    //////////////////////////////////////////////////////////////////////////
    public override void iSwapExecuted()
    {
        // increment weight
        Master.m_Weight += m_MoveWeightExcange;
    }

    public override bool iSwapRequest(Actor pretender)
    {
        return true;
    }

    public override void iProceedTurn()
    {
    }
}