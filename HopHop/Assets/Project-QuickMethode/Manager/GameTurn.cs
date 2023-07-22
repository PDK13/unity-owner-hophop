using QuickMethode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTurn
{
    public static Action<string> onTurn;
    public static Action<string> onEnd;

    private static int m_turnPass = 0;

    public static int TurnPass => m_turnPass;

    public class TurnSingle
    {
        public int Start = 0;

        public string Turn = "None";

        public List<GameObject> Unit;

        public int Count = 0;
        public int EndMoveCount = 0;
        public int EndTurnCount = 0;
        public int EndRemoveCount = 0; //Remove from this Turn, caculate this after End Turn!!

        public bool EndTurnRemove = false; //Remove this Turn after End Turn!!

        public bool EndMoveCheck => EndMoveCount == Count - EndTurnCount; //End Turn also mean End Move!!
        public bool EndTurnCheck => EndTurnCount == Count;

        public TurnSingle (int Start, string Turn, GameObject UnitFirst)
        {
            this.Start = Start;
            //
            this.Turn = Turn;
            this.Count = 1;
            //
            this.Unit = new List<GameObject>();
            this.Unit.Add(UnitFirst);
        }
    }

    private static TurnSingle m_turnCurrent;
    private static List<TurnSingle> m_turnQueue = new List<TurnSingle>();

    public static List<string> TurnRemove = new List<string>()
    {
        "None",
        "Gravity",
    };

    public static void SetStart()
    {
        Debug.Log("[Turn] Turn Start!!");
        //
        m_turnPass = 0;
        m_turnQueue = m_turnQueue.OrderBy(t => t.Start).ToList();
        SetCurrent();
    } //Start!!

    private static void SetCurrent()
    {
        m_turnCurrent = m_turnQueue[0];
        //
        m_turnPass++;
        //
        Debug.LogFormat("[Turn] {0}: Current: {1} | {2} | Moved: {3} | Ended: {4}",
            m_turnPass,
            m_turnCurrent.Turn,
            m_turnCurrent.Count,
            m_turnCurrent.EndMoveCount,
            m_turnCurrent.EndTurnCount);
        //
        onTurn?.Invoke(m_turnCurrent.Turn);
        //

    } //Force Turn Next!!

    #region Enum

    public static void SetInit<EnumType>(EnumType Turn, GameObject Unit)
    {
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn.ToString())
                continue;
            //
            if (m_turnQueue[i].Unit.Contains(Unit))
                return;
            //
            Debug.LogFormat("[Turn] {0}: Init: {1}", m_turnPass, Turn.ToString());
            //
            m_turnQueue[i].Count++;
            m_turnQueue[i].Unit.Add(Unit);
            return;
        }
        //
        Debug.LogFormat("[Turn] {0}: Init: {1}", m_turnPass, Turn.ToString());
        //
        m_turnQueue.Add(new TurnSingle(QEnum.GetChoice(Turn), Turn.ToString(), Unit));
    } //Init on Start!!

    public static void SetRemove<EnumType>(EnumType Turn, GameObject Unit)
    {
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn.ToString())
                continue;
            //
            if (!m_turnQueue[i].Unit.Contains(Unit))
                return;
            //

            if (m_turnQueue[i] == m_turnCurrent)
            {
                Debug.LogFormat("[Turn] {0}: Remove Same: {1}", m_turnPass, Turn);
                //
                m_turnQueue[i].Unit.Remove(Unit);
                m_turnQueue[i].EndRemoveCount++;
            }
            else
            {
                Debug.LogFormat("[Turn] {0}: Remove Un-same: {1}", m_turnPass, Turn);
                //
                m_turnQueue[i].Unit.Remove(Unit);
                m_turnQueue[i].Count--;
            }
            //
            break;
        }
    } //Remove on Destroy!!

    public static void SetEndMove<EnumType>(EnumType Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn.ToString())
            return;
        //
        if (!m_turnCurrent.Unit.Contains(Unit))
            return;
        //
        Debug.LogFormat("[Turn] {0}: End Move: {1}", m_turnPass, Turn.ToString());
        //
        m_turnCurrent.EndMoveCount++;
        //
        if (m_turnCurrent.EndMoveCheck)
        {
            //All End Move or End Turn!!
            //
            m_turnCurrent.EndMoveCount = 0;
            //
            SetCurrent();
        }
    } //End!!

    public static void SetEndTurn<EnumType>(EnumType Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn.ToString())
            return;
        //
        if (!m_turnCurrent.Unit.Contains(Unit))
            return;
        //
        m_turnCurrent.EndTurnCount++;
        //
        Debug.LogFormat("[Turn] {0}: End Turn: {1} | Left: {2}", m_turnPass, Turn.ToString(), m_turnCurrent.Count - m_turnCurrent.EndTurnCount);
        //
        if (m_turnCurrent.EndMoveCheck)
        {
            //All End Move or End Turn!!
            //
            m_turnCurrent.EndMoveCount = 0;
            //
            if (m_turnCurrent.EndTurnCheck)
            {
                //All End Turn, then Swap this Turn to Last!!
                //
                onEnd?.Invoke(Turn.ToString());
                //
                m_turnCurrent.EndTurnCount = 0;
                //
                m_turnQueue.RemoveAt(m_turnQueue.FindIndex(t => t.Turn == Turn.ToString()));
                //
                if (!m_turnCurrent.EndTurnRemove)
                    m_turnQueue.Add(m_turnCurrent);
                //
                if (m_turnCurrent.EndRemoveCount > 0)
                {
                    Debug.LogFormat("[Turn] {0}: Remove Same Final: {1}", m_turnPass, Turn);
                    //
                    m_turnCurrent.Count -= m_turnCurrent.EndRemoveCount;
                    m_turnCurrent.EndRemoveCount = 0;
                }
            }
            //
            SetCurrent();
        }
    } //End!!

    public static void SetAdd<EnumType>(EnumType Turn, GameObject Unit, int After = 0)
    {
        if (After < 0)
            return;
        //
        if (After > m_turnQueue.Count - 1)
        {
            Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
            //
            m_turnQueue.Add(new TurnSingle(QEnum.GetChoice(Turn), Turn.ToString(), Unit));
            m_turnQueue[m_turnQueue.Count - 1].EndTurnRemove = TurnRemove.Contains(Turn.ToString());
        }
        else
        if (m_turnQueue[After].Turn == Turn.ToString())
        {
            if (m_turnQueue[After].Unit.Contains(Unit))
                return;
            //
            Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
            //
            m_turnQueue[After].Unit.Add(Unit);
            m_turnQueue[After].Count++;
        }
        else
        {
            Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
            //
            m_turnQueue.Insert(After, new TurnSingle(QEnum.GetChoice(Turn), Turn.ToString(), Unit));
            m_turnQueue[After].EndTurnRemove = TurnRemove.Contains(Turn.ToString());
        }
    } //Add Turn Special!!

    public static void SetAdd<EnumType>(EnumType Turn, GameObject Unit, string After)
    {
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != After)
                continue;
            //
            if (m_turnQueue[i].Turn == Turn.ToString())
            {
                if (m_turnQueue[i].Unit.Contains(Unit))
                    return;
                //
                Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
                //
                m_turnQueue[i].Unit.Add(Unit);
                m_turnQueue[i].Count++;
            }
            else
            {
                Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
                //
                m_turnQueue.Insert(i, new TurnSingle(QEnum.GetChoice(Turn), Turn.ToString(), Unit));
                m_turnQueue[i].EndTurnRemove = TurnRemove.Contains(Turn.ToString());
            }
            //
            return;
        }
    } //Add Turn Special!!

    #endregion

    #region Int & String

    public static void SetInit(int Start, string Turn, GameObject Unit)
    {
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn)
                continue;
            //
            if (m_turnQueue[i].Unit.Contains(Unit))
                return;
            //
            Debug.LogFormat("[Turn] {0}: Init: {1}", m_turnPass, Turn.ToString());
            //
            m_turnQueue[i].Count++;
            m_turnQueue[i].Unit.Add(Unit);
            return;
        }
        //
        Debug.LogFormat("[Turn] {0}: Init: {1}", m_turnPass, Turn.ToString());
        //
        m_turnQueue.Add(new TurnSingle(Start, Turn, Unit));
    } //Init on Start!!

    public static void SetRemove(string Turn, GameObject Unit)
    {
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != Turn)
                continue;
            //
            if (!m_turnQueue[i].Unit.Contains(Unit))
                return;
            //

            if (m_turnQueue[i] == m_turnCurrent)
            {
                Debug.LogFormat("[Turn] {0}: Remove Same: {1}", m_turnPass, Turn);
                //
                m_turnQueue[i].Unit.Remove(Unit);
                m_turnQueue[i].EndRemoveCount++;
            }
            else
            {
                Debug.LogFormat("[Turn] {0}: Remove Un-same: {1}", m_turnPass, Turn);
                //
                m_turnQueue[i].Unit.Remove(Unit);
                m_turnQueue[i].Count--;
            }
            //
            break;
        }
    } //Remove on Destroy!!

    public static void SetEndMove(string Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn.ToString())
            return;
        //
        if (!m_turnCurrent.Unit.Contains(Unit))
            return;
        //
        Debug.LogFormat("[Turn] {0}: End Move: {1}", m_turnPass, Turn.ToString());
        //
        m_turnCurrent.EndMoveCount++;
        //
        if (m_turnCurrent.EndMoveCheck)
        {
            //All End Move or End Turn!!
            //
            m_turnCurrent.EndMoveCount = 0;
            //
            SetCurrent();
        }
    } //End!!

    public static void SetEndTurn(string Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn.ToString())
            return;
        //
        if (!m_turnCurrent.Unit.Contains(Unit))
            return;
        //
        m_turnCurrent.EndTurnCount++;
        //
        Debug.LogFormat("[Turn] {0}: End Turn: {1} | Left: {2}", m_turnPass, Turn.ToString(), m_turnCurrent.Count - m_turnCurrent.EndTurnCount);
        //
        if (m_turnCurrent.EndMoveCheck)
        {
            //All End Move or End Turn!!
            //
            m_turnCurrent.EndMoveCount = 0;
            //
            if (m_turnCurrent.EndTurnCheck)
            {
                //All End Turn, then Swap this Turn to Last!!
                //
                onEnd?.Invoke(Turn.ToString());
                //
                m_turnCurrent.EndTurnCount = 0;
                //
                m_turnQueue.RemoveAt(m_turnQueue.FindIndex(t => t.Turn == Turn.ToString()));
                //
                if (!m_turnCurrent.EndTurnRemove)
                    m_turnQueue.Add(m_turnCurrent);
                //
                if (m_turnCurrent.EndRemoveCount > 0)
                {
                    Debug.LogFormat("[Turn] {0}: Remove Same Final: {1}", m_turnPass, Turn);
                    //
                    m_turnCurrent.Count -= m_turnCurrent.EndRemoveCount;
                    m_turnCurrent.EndRemoveCount = 0;
                }
            }
            //
            SetCurrent();
        }
    } //End!!

    public static void SetAdd(int Start, string Turn, GameObject Unit, int After = 0)
    {
        if (After < 0)
            return;
        //
        if (After > m_turnQueue.Count - 1)
        {
            Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
            //
            m_turnQueue.Add(new TurnSingle(Start, Turn, Unit));
            m_turnQueue[m_turnQueue.Count - 1].EndTurnRemove = TurnRemove.Contains(Turn);
        }
        else
        if (m_turnQueue[After].Turn == Turn)
        {
            if (m_turnQueue[After].Unit.Contains(Unit))
                return;
            //
            Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
            //
            m_turnQueue[After].Unit.Add(Unit);
            m_turnQueue[After].Count++;
        }
        else
        {
            Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
            //
            m_turnQueue.Insert(After, new TurnSingle(Start, Turn, Unit));
            m_turnQueue[After].EndTurnRemove = TurnRemove.Contains(Turn);
        }
    } //Add Turn Special!!

    public static void SetAdd(int Start, string Turn, GameObject Unit, string After)
    {
        for (int i = 0; i < m_turnQueue.Count; i++)
        {
            if (m_turnQueue[i].Turn != After)
                continue;
            //
            if (m_turnQueue[i].Turn == Turn)
            {
                if (m_turnQueue[i].Unit.Contains(Unit))
                    return;
                //
                Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
                //
                m_turnQueue[i].Unit.Add(Unit);
                m_turnQueue[i].Count++;
            }
            else
            {
                Debug.LogFormat("[Turn] {0}: Add {1} | {2}", m_turnPass, Turn.ToString(), After);
                //
                m_turnQueue.Insert(i, new TurnSingle(Start, Turn.ToString(), Unit));
                m_turnQueue[i].EndTurnRemove = TurnRemove.Contains(Turn);
            }
            //
            return;
        }
    } //Add Turn Special!!

    #endregion
}