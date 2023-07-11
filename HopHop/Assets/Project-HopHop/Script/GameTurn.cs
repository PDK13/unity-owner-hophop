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
        public TypeTurn Turn = TypeTurn.None;

        public int Count = 0;
        public int EndMoveCount = 0;
        public int EndTurnCount = 0;

        public bool EndTurnRemove = false;

        public bool EndMove => EndMoveCount == Count;
        public bool EndTurn => EndTurnCount == Count;

        public TurnSingle (TypeTurn Turn)
        {
            this.Turn = Turn;
        }
    }

    private static TurnSingle m_turnCurrent;
    private static List<TurnSingle> m_turnQueue = new List<TurnSingle>();



    public static List<TypeTurn> TurnRemove = new List<TypeTurn>()
    {
        TypeTurn.None,
        TypeTurn.Gravity,
    };

    public static void SetInit(TypeTurn Turn)
    {
        //
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn)
                continue;
            m_turnQueue[i].Count++;
            return;
        }
        //
        m_turnQueue.Add(new TurnSingle(Turn));
        m_turnQueue = m_turnQueue.OrderBy(t => (int)t.Turn).ToList();
    }

    public static void SetEndMove()
    {
        m_turnCurrent.EndMoveCount++;
        //
        if (m_turnCurrent.EndMove)
            SetCurrent();
    }

    public static void SetEndTurn()
    {
        m_turnCurrent.EndTurnCount++;
        //
        if (m_turnCurrent.EndTurn)
        {
            if (m_turnCurrent.EndTurnRemove)
                m_turnQueue.Remove(m_turnCurrent);
            SetCurrent();
        }
    }

    private static void SetCurrent()
    {
        m_turnCurrent = m_turnQueue[0];
    }

    public static void SetAdd(TypeTurn Turn)
    {
        if (m_turnQueue[0].Turn == Turn)
            m_turnQueue[0].Count++;
        else
            m_turnQueue.Insert(0, new TurnSingle(Turn));
    }
}