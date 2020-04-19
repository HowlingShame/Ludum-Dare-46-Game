using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(Core.c_ManagerDefaultExecutionOrder + 1)]
public class PlayerEntity : MonoBehaviour
{
    public static PlayerEntity      Instance;
    
    public int                      m_VisitorsCount;
    public AnimationCurve           m_VisitorsCurve;
    public RollList                 m_VisitorsList;

    public int                      m_DangersCount;
    public AnimationCurve           m_DangersCurve;
    public RollList                 m_DangersList;
    
    public int                      m_CurrentTurn;
    public int                      m_TurnsLimit;

    [Space]
    [ReadOnly]
    public int                      m_RequireSceneVisitors;
    [ReadOnly]
    public int                      m_CurrentVisitors;
    [ReadOnly]
    public int                      m_RequireSceneDangers;
    [ReadOnly]
    public int                      m_CurrentDangers;
    
    [ReadOnly]
    public List<Actor>              m_BoardActors;
    [ReadOnly]
    public List<Actor>              m_ReserveActors;
    [SerializeField]
    private CoroutineWrapper        m_Round;

    public float                    m_MoveDuration;
    public float                    m_TurnDuration;

    public Bounds                   m_ZoneDetaindLeft;
    public Bounds                   m_ZoneDetaindRight;
    public Bounds                   m_ZoneLeave;
    public Bounds                   m_ZoneSpawn;
    public Vector2                  m_PositionRandomization;

    public float                    m_BodyguardDeadRestore = 1.0f;
    public float                    m_BodyguardDetainRestore = 1.0f;

    public bool                     m_AutoStart;
    public bool                     m_AutoPause;

    private List<BodyguardSlot>     m_BodyguardSlots;
    public bool                     m_Pause;

    public Threat                   m_ThrowThreat;
    public Threat                   m_SniperThreat;

    public GameObject               m_PhotoEffect;
    public GameObject               m_ScreamEffect;
    public GameObject               m_AplodismentEffect;

    [NonSerialized]
    public Threat                   m_CurrentThreat;
    public List<Actor.ActionBase>   m_ActionSequence = new List<Actor.ActionBase>();
    public GameObject               m_RescurePosition;
    public int                      m_EventLock;
    public bool                     m_DoubleClickToDetain;
    public float                    m_RevielTime = 0.6f;
    public Color                    m_DangerColor;
    public Color                    m_CivilianColor;
    public Color                    m_NoizeColor;
    public RevealTimer              m_RevealTimer;
    public GameObject               m_PresidentModel;
    public bool                     m_ReserveNextTurn = false;
    public Image                    m_SpeechProgress;
    public GameObject               m_SpeechProgressRoot;
    public GameObject               m_WinMenu;
    public GameObject               m_LooseMenu;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        Instance = this;
        m_CurrentVisitors = 0;
        m_CurrentDangers = 0;

        m_BodyguardSlots = new List<BodyguardSlot>();
        m_BodyguardSlots.AddRange(FindObjectsOfType<BodyguardSlot>());

        m_SpeechProgressRoot.SetActive(false);
        m_WinMenu.SetActive(false);
        m_LooseMenu.SetActive(false);

        if (m_AutoStart)
            StartRound();

        SoundManager.Instance.PlayMusic("Main");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // protect on space
            GetBodyguard()?.Protect();
        }
    }

    [Button]
    public void StartRound()
    {
        m_SpeechProgressRoot.SetActive(true);

        m_Round.Stop();

        // all actors leave
        foreach (var actor in FindObjectsOfType<Actor>())
            actor.Leave();

        m_CurrentTurn = 0;

        m_Round.Start();
    }
    public void Restart()
    {
        // just reload scene
        SceneManager.LoadScene(1);
    }

    public void Aplodisments()
    {
        SoundManager.Instance.PlaySound("Aplodisments");
        Instantiate(m_AplodismentEffect);
    }

    //////////////////////////////////////////////////////////////////////////
    public void Pretend()
    {
        // preform grid pretends
        for (var r = Grid.Instance.m_Column - 1; r >= 0; r--)
        {
            foreach (var slot in Grid.Instance.GetRow(r, true))
                slot.m_Actor?.Pretend();
        }
        // randomize reserve order
        UnityRandom.RandomizeList(m_ReserveActors);
        // preform reserve pretends
        foreach (var actor in m_ReserveActors.ToList())
            actor.Pretend();

        // implement moves
        foreach (var actor in m_ReserveActors)
            actor.ImplementMove();
        foreach (var actor in m_BoardActors)
            actor.ImplementMove();
    }

    public void Turn()
    {
        // next turn for board actors
        foreach (var actor in m_BoardActors)
            actor.ImplementTurn();

        // play move sound
        SoundManager.Instance.PlaySound("Turn");

        // instantiate actors
        var scale = m_CurrentTurn / (float)m_TurnsLimit;
        m_RequireSceneVisitors = Mathf.FloorToInt(m_VisitorsCurve.Evaluate(scale) * m_VisitorsCount);
        m_RequireSceneDangers = Mathf.FloorToInt(m_DangersCurve.Evaluate(scale) * m_DangersCount);
        m_SpeechProgress.fillAmount = Mathf.Clamp01(1.0f - scale);

        // spawn dangers
        while (m_CurrentDangers < m_RequireSceneDangers)
            Instantiate(m_DangersList.Roll(scale));

        // spawn visitors
        while (m_CurrentVisitors < m_RequireSceneVisitors)
            Instantiate(m_VisitorsList.Roll(scale));

        // next turn for reserve actors
        if (m_ReserveNextTurn)
            foreach (var actor in m_ReserveActors)
                actor.ImplementTurn();

        m_CurrentTurn ++;

    }

    //////////////////////////////////////////////////////////////////////////
    private IEnumerator impInstantiateActors()
    {
        yield return null;
    }

    private IEnumerator implRound()
    {
        yield return null;

        while (m_CurrentTurn != m_TurnsLimit)
        {
            // pretend phase
            Pretend();
            yield return new WaitForSeconds(m_MoveDuration);
            if (m_AutoPause)
                m_Pause = true;
            while (m_Pause)
                yield return null;
            
            // turn phase
            Turn();
            // start all coroutines
            foreach (var action in m_ActionSequence)
                StartCoroutine(action.Run());
            m_ActionSequence.Clear();

            var minWaitTime = Time.time + m_TurnDuration;
            // wait all coroutines locks
            yield return new WaitUntil(() => m_EventLock <= 0);
            // wait min time
            yield return new WaitForSeconds(Mathf.Max(0.0f, minWaitTime - Time.time));

            if (m_AutoPause)
                m_Pause = true;
            while (m_Pause)    
                yield return null;
        }

        Win();
    }

   /* private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawCube(m_ZoneDetaindLeft.center, m_ZoneDetaindLeft.size);
        Gizmos.DrawCube(m_ZoneDetaindRight.center, m_ZoneDetaindRight.size);
        Gizmos.DrawCube(m_ZoneLeave.center, m_ZoneLeave.size);
        Gizmos.color = Color.green * 0.5f;
        Gizmos.DrawCube(m_ZoneSpawn.center, m_ZoneSpawn.size);
    }*/

    public BodyguardSlot GetBodyguard()
    {
        return m_BodyguardSlots.FirstOrDefault(n => n.HasCharge);
    }
    [NonSerialized]
    public bool    m_WinLooseLock;

    public void Loose()
    {
        if (m_WinLooseLock)
            return;
        m_WinLooseLock = true;

        StopAllCoroutines();
        m_ActionSequence.Clear();
        foreach (var actor in FindObjectsOfType<Actor>())
            actor.Leave();
        m_PresidentModel.GetComponent<Animator>().SetTrigger("Dead");
        m_SpeechProgressRoot.SetActive(false);
        
        // play camera animation
        Core.Instance.m_Camera.GetComponent<Animator>().SetTrigger("GameOver");

        SoundManager.Instance.PlayMusic(null);

        // show restart menu
        m_LooseMenu.SetActive(true);
    }
    [Button]
    public void Win()
    {
        if (m_WinLooseLock)
            return;
        m_WinLooseLock = true;

        StopAllCoroutines();
        m_ActionSequence.Clear();
        foreach (var actor in FindObjectsOfType<Actor>())
            actor.Leave();
        m_SpeechProgressRoot.SetActive(false);
        
        // play camera animation
        Core.Instance.m_Camera.GetComponent<Animator>().SetTrigger("Win");

        SoundManager.Instance.PlayMusic(null);

        m_WinMenu.SetActive(true);
    }
}