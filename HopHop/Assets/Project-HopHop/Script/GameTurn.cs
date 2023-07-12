using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTurn
{
    public static Action<TypeTurn> onTurn;

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
        m_turnQueue[m_turnQueue.Count - 1].Count++;
    } //Init on Start!!

    public static void SetStart()
    {
        m_turnQueue = m_turnQueue.OrderBy(t => (int)t.Turn).ToList();
        SetCurrent();
    }

    public static void SetRemove(TypeTurn Turn)
    {
        int Index = -1;
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn)
                continue;
            Index = i;
            m_turnQueue[i].Count--;
            break;
        }
        //
        if (Index > m_turnQueue.Count - 1)
            //Avoid null exception after close game on editor!!
            return;
        //
        if (m_turnQueue[Index].Count == 0)
            m_turnQueue.RemoveAt(Index);
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
            m_turnCurrent.EndTurnCount = 0;
            //
            m_turnQueue.Remove(m_turnCurrent);
            //
            if (!m_turnCurrent.EndTurnRemove)
                m_turnQueue.Add(m_turnCurrent);
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
            m_turnQueue.Add(new TurnSingle(Turn));
        else
        if (m_turnQueue[After].Turn == Turn)
            m_turnQueue[After].Count++;
        else
            m_turnQueue.Insert(After, new TurnSingle(Turn));
    } //Add Turn Special!!
}