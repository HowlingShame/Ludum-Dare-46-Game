public class ActorBehaviorSniper : Actor.BehaviorBase, Actor.IBehavior
{
    public int      m_Turns;
    public bool     m_RunAfterAction = true;

    //////////////////////////////////////////////////////////////////////////
    public override bool iSwapRequest(Actor pretender)
    {
        // do not swap if in position
        if (Master.IsInTargetRow)
            return false;

        return true;
    }

    public override void iProceedTurn()
    {
        if (Master.IsInTargetRow)
        {
            m_Turns --;
            if (m_Turns < 0)
            {
                // execute shot
                PlayerEntity.Instance.m_ActionSequence.Add(new Actor.ActionThreatSniper(){Master = Master});

                // set weight to zero, leave
                Master.m_Weight = 0;
                if (m_RunAfterAction)
                {
                    Master.m_TargetRow = -1;
                    Master.m_MaxMoves = 1;
                    Master.m_MovesLeft = Master.m_MaxMoves;
                }
            }
        }
    }
}