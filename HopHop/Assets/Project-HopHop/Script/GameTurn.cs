using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTurn
{
    public static Action<TypeTurn> onTurn;
    public static Action<TypeTurn> onEnd;

    public class TurnSingle
    {
        public TypeTurn Turn = TypeTurn.None;

        public int Count = 0;
        public int EndMoveCount = 0;
        public int EndTurnCount = 0;
        public int EndRemoveCount = 0; //Remove from this Turn, caculate this after End Turn!!

        public bool EndTurnRemove = false; //Remove this Turn after End Turn!!

        public bool EndMove => EndMoveCount == Count - EndTurnCount; //End Turn also mean End Move!!
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
        m_turnQueue[m_turnQueue.Count - 1].Count++;
    } //Init on Start!!

    public static void SetStart()
    {
        m_turnQueue = m_turnQueue.OrderBy(t => (int)t.Turn).ToList();
        SetCurrent();
    }

    public static void SetRemove(TypeTurn Turn)
    {
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn)
                continue;
            m_turnQueue[i].EndRemoveCount++;
            break;
        }
    } //Remove on Destroy!!

    public static void SetEndMove(TypeTurn Turn)
    {
        if (m_turnCurrent.Turn != Turn)
            return;
        //
        m_turnCurrent.EndMoveCount++;
        //
        if (m_turnCurrent.EndMove)
        {
            m_turnCurrent.EndMoveCount = 0;
            //
            if (!m_turnCurrent.EndTurn)
                SetCurrent();
        }
    }

    public static void SetEndTurn(TypeTurn Turn)
    {
        if (m_turnCurrent.Turn != Turn)
            return;
        //
        m_turnCurrent.EndTurnCount++;
        //
        if (m_turnCurrent.EndTurn)
        {
            onEnd?.Invoke(Turn);
            //
            m_turnCurrent.EndTurnCount = 0;
            //
            m_turnQueue.RemoveAt(m_turnQueue.FindIndex(t => t.Turn == Turn));
            //
            if (!m_turnCurrent.EndTurnRemove)
                m_turnQueue.Add(m_turnCurrent);
            //
            m_turnCurrent.Count -= m_turnCurrent.EndRemoveCount;
            m_turnCurrent.EndRemoveCount = 0;
            //
            SetCurrent();
        }
    }

    private static void SetCurrent()
    {
        m_turnCurrent = m_turnQueue[0];
        onTurn?.Invoke(m_turnCurrent.Turn);

        Debug.Log("[Turn Current]");
        for (int i = 0; i < m_turnQueue.Count; i++)
            Debug.Log(m_turnQueue[i].Turn + "(" + m_turnQueue[i].Count + ")");
        Debug.Log("--------------");
    } //Force Turn Next!!

    public static void SetAdd(TypeTurn Turn, int After = 0)
    {
        if (After < 0)
            return;
        //
        if (After > m_turnQueue.Count - 1)
        {
            m_turnQueue.Add(new TurnSingle(Turn));
            m_turnQueue[m_turnQueue.Count - 1].Count++;
            m_turnQueue[m_turnQueue.Count - 1].EndTurnRemove = TurnRemove.Contains(Turn);
        }
        else
        if (m_turnQueue[After].Turn == Turn)
            m_turnQueue[After].Count++;
        else
        {
            m_turnQueue.Insert(After, new TurnSingle(Turn));
            m_turnQueue[After].Count++;
            m_turnQueue[After].EndTurnRemove = TurnRemove.Contains(Turn);
        }
    } //Add Turn Special!!
}