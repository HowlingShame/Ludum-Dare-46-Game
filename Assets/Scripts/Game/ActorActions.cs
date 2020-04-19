using System;
using System.Collections;
using UnityEngine;

public partial class Actor
{
    [Serializable]
    public abstract class ActionBase
    {
        public Actor    Master;

        public abstract IEnumerator Run();
    }

    [Serializable]
    public class ActionThreatKiller : ActionBase
    {
        public override IEnumerator Run()
        {
            yield return null;
        }
    }

    [Serializable]
    public class ActionThreatSniper : ActionBase
    {
        public override IEnumerator Run()
        {
            PlayerEntity.Instance.m_EventLock ++;

            while ( PlayerEntity.Instance.m_CurrentThreat != null)
                yield return null;

            if (Master != null)
            {
                // spawn threat
                var go = Instantiate(PlayerEntity.Instance.m_SniperThreat);
                go.transform.position = Master.m_EffectSpawnPoint.transform.position;
                go.Run();

                while (go.TimeLeft > 0)
                {
                    go.TimeLeft -= Time.deltaTime;
                    yield return null;
                }

                go.Execute();
                Destroy(go.gameObject);
            }

            PlayerEntity.Instance.m_EventLock --;
        }
    }

    [Serializable]
    public class ActionThreatThrow : ActionBase
    {
        public override IEnumerator Run()
        {
            PlayerEntity.Instance.m_EventLock ++;

            while ( PlayerEntity.Instance.m_CurrentThreat != null)
                yield return null;

            if (Master != null)
            {
                // spawn threat
                var go = Instantiate(PlayerEntity.Instance.m_ThrowThreat);
                go.transform.position = Master.m_EffectSpawnPoint.transform.position;
                go.Run();

                while (go.TimeLeft > 0)
                {
                    go.TimeLeft -= Time.deltaTime;
                    yield return null;
                }

                go.Execute();
                Destroy(go.gameObject);
            }
            PlayerEntity.Instance.m_EventLock --;
        }
    }

    [Serializable]
    public class ActionScream : ActionBase
    {
        public float m_Delay;

        public override IEnumerator Run()
        {
            // same as action photo instead prefab
            yield return new WaitForSeconds(m_Delay);
            if (Master == null)
                yield break;
            // spawn effect
            var go = Instantiate(PlayerEntity.Instance.m_ScreamEffect);
            go.transform.position = Master.m_EffectSpawnPoint.transform.position;
            // play camera animation
            Core.Instance.m_Camera.GetComponent<Animator>().SetTrigger("Scream");
        }
    }

    [Serializable]
    public class ActionPhoto : ActionBase
    {
        public float        m_Delay;

        //////////////////////////////////////////////////////////////////////////
        public override IEnumerator Run()
        {
            yield return new WaitForSeconds(m_Delay);
            if (Master == null)
                yield break;
            // spawn effect
            var go = Instantiate(PlayerEntity.Instance.m_PhotoEffect);
            go.transform.SetParent(Master.transform);
            go.transform.position = Master.m_EffectSpawnPoint.transform.position;
        }
    }
}