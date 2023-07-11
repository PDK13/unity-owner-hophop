using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTurn
{
    public static Action<TypeTurn> onTurn;
    public static Action<TypeTurn, int> onAdd;

    public class TurnSingle
    {
        public TypeTurn Turn;
        public int Count = 0;
        public int End = 0;

        public TurnSingle (TypeTurn Turn)
        {
            this.Turn = Turn;
            Count = 1;
        }
    }

    private static List<TurnSingle> m_turn = new List<TurnSingle>();

    public static TypeTurn TurnCurrent => m_turn[0].Turn;
    public static bool TurnFinish => m_turn[0].Count == m_turn[0].End;

    public static List<TypeTurn> TurnRemove = new List<TypeTurn>()
    {
        TypeTurn.None,
        TypeTurn.Gravity,
    };

    private static int m_objectTurnCount = 0;
    private static int m_objectTurnEnd = 0;
    private static int m_objectDelayCount = 0;
    private static int m_objectDelayEnd = 0;

    public static bool ObjectTurnDone => m_objectTurnCount == m_objectTurnEnd;
    public static bool ObjectDelayDone => m_objectDelayCount == m_objectDelayEnd;

    public static void SetInit(TypeTurn Turn)
    {
        if (TurnRemove.Contains(TurnCurrent))
            return;
        //
        for (int i = 0; i < m_turn.Count; i++)
        {
            if (m_turn[i].Turn != Turn)
                continue;
            m_turn[i].Count++;
            return;
        }
        //
        m_turn.Add(new TurnSingle(Turn));
        m_turn = m_turn.OrderBy(t => (int)t.Turn).ToList();
    }

    public static void SetStart()
    {
        onTurn?.Invoke(TurnCurrent);
    }

    public static void SetNext()
    {
        if (TurnRemove.Contains(TurnCurrent))
        {
            m_turn.RemoveAt(0);
        }
        else
        {
            TurnSingle TurnCurrent = m_turn[0];
            m_turn.RemoveAt(0);
            m_turn.Add(TurnCurrent);
        }
        //
        onTurn?.Invoke(TurnCurrent);
    }

    public static void SetNextInit(TypeTurn Turn, int After = 1)
    {
        if (After == 0)
        {
            onTurn?.Invoke(Turn);
        }
        else
        if (m_turn[After].Turn == Turn)
        {
            m_turn[After].Count++;
        }
        else
        {
            m_turn.Insert(After, new TurnSingle(Turn));
        }
        //
        onAdd?.Invoke(Turn, After);
    }
}