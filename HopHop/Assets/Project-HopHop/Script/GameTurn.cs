using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTurn
{
    public static Action<TypeTurn> onTurn;
    public static Action<TypeTurn> onEnd;

    private static int m_turnPass = 0;

    public static int TurnPass => m_turnPass;

    public class TurnSingle
    {
        public TypeTurn Turn = TypeTurn.None;

        public List<GameObject> Unit;

        public int Count = 0;
        public int EndMoveCount = 0;
        public int EndTurnCount = 0;
        public int EndRemoveCount = 0; //Remove from this Turn, caculate this after End Turn!!

        public bool EndTurnRemove = false; //Remove this Turn after End Turn!!

        public bool EndMoveCheck => EndMoveCount == Count - EndTurnCount; //End Turn also mean End Move!!
        public bool EndTurnCheck => EndTurnCount == Count;

        public TurnSingle (TypeTurn Turn, GameObject UnitFirst)
        {
            this.Turn = Turn;
            this.Count = 1;
            //
            this.Unit = new List<GameObject>();
            this.Unit.Add(UnitFirst);
        }
    }

    private static TurnSingle m_turnCurrent;
    private static List<TurnSingle> m_turnQueue = new List<TurnSingle>();

    public static List<TypeTurn> TurnRemove = new List<TypeTurn>()
    {
        TypeTurn.None,
        TypeTurn.Gravity,
    };

    public static void SetInit(TypeTurn Turn, GameObject Unit)
    {
        Debug.LogFormat("[Debug] {0}: Init: {1}", m_turnPass, Turn.ToString());
        //
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn)
                continue;
            //
            if (m_turnQueue[i].Unit.Contains(Unit))
                return;
            //
            m_turnQueue[i].Count++;
            m_turnQueue[i].Unit.Add(Unit);
            return;
        }
        //
        m_turnQueue.Add(new TurnSingle(Turn, Unit));
    } //Init on Start!!

    public static void SetStart()
    {
        m_turnPass = 0;
        m_turnQueue = m_turnQueue.OrderBy(t => (int)t.Turn).ToList();
        SetCurrent();
    }

    public static void SetRemove(TypeTurn Turn, GameObject Unit)
    {
        Debug.LogFormat("{0}: Remove: {1}", m_turnPass, Turn);
        //
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn)
                continue;
            //
            if (!m_turnQueue[i].Unit.Contains(Unit))
                return;
            //
            m_turnQueue[i].EndRemoveCount++;
            break;
        }
    } //Remove on Destroy!!

    public static void SetEndMove(TypeTurn Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn)
            return;
        //
        if (!m_turnCurrent.Unit.Contains(Unit))
            return;
        //
        Debug.LogFormat("[Debug] {0}: End Move: {1}", m_turnPass, Turn.ToString());
        //
        m_turnCurrent.EndMoveCount++;
        //
        if (m_turnCurrent.EndMoveCheck)
        {
            m_turnCurrent.EndMoveCount = 0;
            //
            if (!m_turnCurrent.EndTurnCheck)
                SetCurrent();
        }
    } //End!!

    public static void SetEndTurn(TypeTurn Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn)
            return;
        //
        if (!m_turnCurrent.Unit.Contains(Unit))
            return;
        //
        Debug.LogFormat("[Debug] {0}: End Turn: {1}", m_turnPass, Turn.ToString());
        //
        m_turnCurrent.EndTurnCount++;
        //
        if (m_turnCurrent.EndTurnCheck)
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
            if (m_turnCurrent.EndRemoveCount > 0)
            {
                Debug.LogFormat("{0}: Remove Final: {1}", m_turnPass, Turn);
                m_turnCurrent.Count -= m_turnCurrent.EndRemoveCount;
                m_turnCurrent.EndRemoveCount = 0;
            }
            //
            SetCurrent();
        }
    } //End!!

    private static void SetCurrent()
    {
        m_turnCurrent = m_turnQueue[0];
        //
        Debug.LogWarning("");
        Debug.LogFormat("[Debug] {0}: Current: {1} | {2} | Moved: {3} | Ended: {4}",
            m_turnPass,
            m_turnCurrent.Turn,
            m_turnCurrent.Count,
            m_turnCurrent.EndMoveCount,
            m_turnCurrent.EndTurnCount);
        //
        onTurn?.Invoke(m_turnCurrent.Turn);
        //
        
    } //Force Turn Next!!

    public static void SetAdd(TypeTurn Turn, GameObject Unit, int After = 0)
    {
        if (After < 0)
            return;
        //
        Debug.LogFormat("[Debug] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
        //
        if (After > m_turnQueue.Count - 1)
        {
            m_turnQueue.Add(new TurnSingle(Turn, Unit));
            m_turnQueue[m_turnQueue.Count - 1].EndTurnRemove = TurnRemove.Contains(Turn);
        }
        else
        if (m_turnQueue[After].Turn == Turn)
        {
            if (m_turnQueue[After].Unit.Contains(Unit))
                return;
            //
            m_turnQueue[After].Unit.Add(Unit);
            m_turnQueue[After].Count++;
        }
        else
        {
            m_turnQueue.Insert(After, new TurnSingle(Turn, Unit));
            m_turnQueue[After].EndTurnRemove = TurnRemove.Contains(Turn);
        }
    } //Add Turn Special!!
}