using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using Malee;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class Slot : MonoBehaviour
{
    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class TransitionDictionary: SerializableDictionaryBase<Direction, Slot> {}

    [Serializable]
    public class TransitionList : ReorderableArray<Slot> {}

    //////////////////////////////////////////////////////////////////////////
    public Vector2Int               m_Position;
    public Actor                    m_Actor;

    public TransitionDictionary     m_TransitionDictionary;

    public bool                     IsExpanded => m_Position.y % 2 != 0;
    public int                      Row => m_Position.y;

    //////////////////////////////////////////////////////////////////////////
    public void AddTransition(Direction dir, Slot slot)
    {
        if (slot == null)
            return;

        m_TransitionDictionary.Add(dir, slot);
    }

    public List<Slot> GetTransitions(Direction requireFlags, bool randomize)
    {
        var result = m_TransitionDictionary
            .Where(n => n.Key.HasFlag(requireFlags))
            .Select(n => n.Value)
            .ToList();

        if (randomize)
            UnityRandom.RandomizeList(result);

        return result;
    }

    public bool Pretend(Actor pretender)
    {
        // can move to that place
        if (m_Actor == null)
            return true;

        return m_Actor.SwapRequest(pretender);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
    }
}
