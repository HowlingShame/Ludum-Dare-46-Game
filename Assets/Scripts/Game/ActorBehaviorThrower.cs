public class ActorBehaviorThrower : Actor.BehaviorBase, Actor.IBehavior
{
    public bool m_RunAfterThrow = true;

    //////////////////////////////////////////////////////////////////////////
    public override void iSwapExecuted()
    {
    }

    public override void iProceedTurn()
    {
        if (Master.IsInTargetRow)
        {
            // execute throw
            PlayerEntity.Instance.m_ActionSequence.Add(new Actor.ActionThreatThrow(){Master = Master});

            // set weight to zero, leave
            Master.m_Weight = 0;
            if (m_RunAfterThrow)
            {
                Master.m_TargetRow = -1;
                Master.m_MaxMoves = 1;
                Master.m_MovesLeft = Master.m_MaxMoves;
            }
        }
    }
}