using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Grid : MonoBehaviour
{
    public static Grid      Instance;
    public float            m_RowSpacing;
    public float            m_ColumnSpacing;

    public int              m_Row;
    public int              m_Column;

    public Slot             m_SlotPrefab;
    public List<List<Slot>> m_SlotList;

    //////////////////////////////////////////////////////////////////////////
    public void Awake()
    {
        Instance = this;
        Rebuild();
    }

    public Slot GetSlot(Vector2Int pos)
    {
        // check bounds
        if (pos.y < 0 || pos.y >= m_SlotList.Count)
            return null;
        if (pos.x < 0 || pos.x >= m_SlotList[pos.y].Count)
            return null;

        return m_SlotList[pos.y][pos.x];
    }

    public List<Slot> GetRow(int index, bool randomize)
    {
        if (index >= m_SlotList.Count)
            return null;

        var result = m_SlotList[index];
        if (randomize)
        {
            result = result.ToList();
            UnityRandom.RandomizeList(result);
        }

        return result;
    }

    //////////////////////////////////////////////////////////////////////////
    [Button]
    public void Rebuild()
    {
        // clear data
        foreach (var slot in GetComponentsInChildren<Slot>())
            DestroyImmediate(slot.gameObject);
        m_SlotList = new List<List<Slot>>();

        // instantiate slots
        for (var c = 0; c < m_Column; c++)
        {
            var row = new List<Slot>();
            m_SlotList.Add(row);
            for (var r = 0; r < (c % 2 == 0 ? m_Row : m_Row + 1); r++)
            {
                var slot = Instantiate<Slot>(m_SlotPrefab, transform);
                // set index
                slot.m_Position.Set(r, c);
                // set location
                slot.transform.localPosition = new Vector3(
                    slot.IsExpanded ? slot.m_Position.x * m_RowSpacing - m_RowSpacing * 0.5f : slot.m_Position.x * m_RowSpacing,
                    slot.m_Position.y * m_ColumnSpacing);

                row.Add(slot);
            }
        }
        
        // init slots
        for (var c = 0; c < m_Column; c++)
        for (var r = 0; r < (c % 2 == 0 ? m_Row : m_Row + 1); r++)
        {
            var slot = m_SlotList[c][r];

            // add transitions
            var topBottomOffset = slot.IsExpanded ? 0 : 1;

            var left = GetSlot(new Vector2Int(slot.m_Position.x - 1, slot.m_Position.y));
            var right = GetSlot(new Vector2Int(slot.m_Position.x + 1, slot.m_Position.y));

            var leftTop = GetSlot(new Vector2Int(slot.m_Position.x - 1 + topBottomOffset, slot.m_Position.y + 1));
            var rightTop = GetSlot(new Vector2Int(slot.m_Position.x + topBottomOffset, slot.m_Position.y + 1));

            var leftBottom = GetSlot(new Vector2Int(slot.m_Position.x - 1 + topBottomOffset, slot.m_Position.y - 1));
            var rightBottom = GetSlot(new Vector2Int(slot.m_Position.x + topBottomOffset, slot.m_Position.y - 1));

            slot.AddTransition(Direction.Left, left);
            slot.AddTransition(Direction.Right, right);

            slot.AddTransition(Direction.LeftTop, leftTop);
            slot.AddTransition(Direction.RightTop, rightTop);

            slot.AddTransition(Direction.LeftBottom, leftBottom);
            slot.AddTransition(Direction.RightBottom, rightBottom);
        }
    }

}
