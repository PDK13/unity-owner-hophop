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

    private static int m_stepPass = 0;

    private static int m_turnPass = 0;
    private static string m_turnBase = "";

    public static int TurnPass => m_turnPass;

    public class TurnSingle
    {
        public int Start = 0;

        public string Turn = "None";

        public List<GameObject> Unit;
        public List<GameObject> UnitEndTurn;
        public List<GameObject> UnitEndMove;

        public bool EndMove => UnitEndMove.Count == Unit.Count - UnitEndTurn.Count;
        public bool EndTurn => UnitEndTurn.Count == Unit.Count;

        public bool EndTurnRemove = false;

        public TurnSingle (int Start, string Turn, GameObject UnitFirst)
        {
            this.Start = Start;
            //
            this.Turn = Turn;
            //
            this.Unit = new List<GameObject>();
            this.Unit.Add(UnitFirst);
            //
            this.UnitEndTurn = new List<GameObject>();
            this.UnitEndMove = new List<GameObject>();
        }

        public bool GetEnd(GameObject UnitCheck)
        {
            if (!Unit.Contains(UnitCheck))
                return false;
            //
            if (!UnitEndTurn.Contains(UnitCheck) && !UnitEndMove.Contains(UnitCheck))
                return false;
            //
            return true;
        }
    }

    private static TurnSingle m_turnCurrent;
    private static List<TurnSingle> m_turnQueue = new List<TurnSingle>();

    public static List<string> TurnRemove = new List<string>()
    {
        "None",
        "Gravity",
    };

    public static void SetStart(string TurnBase = "")
    {
        Debug.Log("[Turn] Turn Start!!");
        //
        if (TurnBase != "")
        {
            m_turnBase = TurnBase;
        }
        //
        m_turnPass = 0;
        m_stepPass = 0;
        m_turnQueue = m_turnQueue.OrderBy(t => t.Start).ToList();
        SetCurrent();
    } //Start!!

    private static void SetCurrent()
    {
        m_turnCurrent = m_turnQueue[0];
        //
        if (m_turnCurrent.Turn == m_turnBase)
            m_turnPass++;
        //
        m_stepPass++;
        //
        if (m_turnCurrent.Turn == m_turnBase)
            Debug.LogWarningFormat("[Turn] <TURN {0} START>", m_turnPass);
        //
        if (m_turnCurrent.Turn != m_turnBase)
            Debug.LogWarningFormat("[Turn] <TURN {1} START> {2} / {3}", m_turnPass, m_turnCurrent.Turn, m_turnCurrent.UnitEndTurn.Count, m_turnCurrent.Unit.Count);
        //
        onTurn?.Invoke(m_turnCurrent.Turn);
        //
        SetEndNext(m_turnCurrent.Turn.ToString());
        //
    } //Force Turn Next!!

    #region Enum

    public static void SetInit<EnumType>(EnumType Turn, GameObject Unit)
    {
        SetInit(QEnum.GetChoice(Turn), Turn.ToString(), Unit);
    } //Init on Start!!

    public static void SetRemove<EnumType>(EnumType Turn, GameObject Unit)
    {
        SetRemove(Turn.ToString(), Unit);
    } //Remove on Destroy!!

    public static void SetEndMove<EnumType>(EnumType Turn, GameObject Unit)
    {
        SetEndMove(Turn.ToString(), Unit);
    } //End!!

    public static void SetEndTurn<EnumType>(EnumType Turn, GameObject Unit)
    {
        SetEndTurn(Turn.ToString(), Unit);
    } //End!!

    public static void SetAdd<EnumType>(EnumType Turn, GameObject Unit, int After = 0)
    {
        SetAdd(QEnum.GetChoice(Turn), Turn.ToString(), Unit, After);
    } //Add Turn Special!!

    public static void SetAdd<EnumType>(EnumType Turn, GameObject Unit, string After)
    {
        SetAdd(Turn.ToString(), Unit, After);
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
            Debug.LogFormat("[Turn] <Init> {0}", Turn.ToString());
            //
            m_turnQueue[i].Unit.Add(Unit);
            return;
        }
        //
        Debug.LogFormat("[Turn] <Init> {0}", Turn.ToString());
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
                m_turnQueue[i].Unit.Remove(Unit);
                m_turnQueue[i].UnitEndMove.Remove(Unit);
                m_turnQueue[i].UnitEndTurn.Remove(Unit);
                //
                SetDebug(Turn, "Remove Same");
                //
                SetEndNext(Turn);
            }
            else
            {
                m_turnQueue[i].Unit.Remove(Unit);
                m_turnQueue[i].UnitEndMove.Remove(Unit);
                m_turnQueue[i].UnitEndTurn.Remove(Unit);
                //
                SetDebug(Turn, "Remove Un-Same");
                //
            }
            //
            break;
        }
    } //Remove on Destroy!!

    public static void SetEndMove(string Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn)
            return;
        //
        if (m_turnCurrent.GetEnd(Unit))
            return;
        //
        m_turnCurrent.UnitEndMove.Add(Unit);
        //
        SetDebug(Turn, "End Move");
        //
        SetEndNext(Turn);
    } //End!!

    public static void SetEndTurn(string Turn, GameObject Unit)
    {
        if (m_turnCurrent.Turn != Turn)
            return;
        //
        if (m_turnCurrent.GetEnd(Unit))
            return;
        //
        m_turnCurrent.UnitEndTurn.Add(Unit);
        //
        SetDebug(Turn, "End Turn");
        //
        SetEndNext(Turn);
    } //End!!

    private static void SetEndNext(string Turn)
    {
        if (m_turnCurrent.EndTurn)
        {
            SetDebug(Turn, "Next Turn");
            //
            m_turnCurrent.UnitEndMove.Clear();
            m_turnCurrent.UnitEndTurn.Clear();
            //
            onEnd?.Invoke(Turn.ToString());
            //
            SetEndSwap(Turn);
            //
            SetCurrent();
        }
        else
        if (m_turnCurrent.EndMove)
        {
            SetDebug(Turn, "Next Turn by Move");
            //
            m_turnCurrent.UnitEndMove.Clear();
            //
            SetCurrent();
        }
    } //Check End Turn or End Move!!

    private static void SetEndSwap(string Turn)
    {
        m_turnQueue.RemoveAt(m_turnQueue.FindIndex(t => t.Turn == Turn.ToString()));
        //
        if (!m_turnCurrent.EndTurnRemove)
            m_turnQueue.Add(m_turnCurrent);
    } //Swap Current Turn to Last!!

    public static void SetAdd(int Start, string Turn, GameObject Unit, int After = 0)
    {
        if (After < 0)
            return;
        //
        if (After > m_turnQueue.Count - 1)
        {
            m_turnQueue.Add(new TurnSingle(Start, Turn, Unit));
            m_turnQueue[m_turnQueue.Count - 1].EndTurnRemove = TurnRemove.Contains(Turn);
        }
        else
        if (m_turnQueue[After].Turn == Turn)
        {
            if (m_turnQueue[After].Unit.Contains(Unit))
                return;
            //
            m_turnQueue[After].Unit.Add(Unit);
        }
        else
        {
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
                m_turnQueue[i].Unit.Add(Unit);
            }
            else
            {
                m_turnQueue.Insert(i, new TurnSingle(Start, Turn.ToString(), Unit));
                m_turnQueue[i].EndTurnRemove = TurnRemove.Contains(Turn);
            }
            //
            return;
        }
    } //Add Turn Special!!

    #endregion

    private static void SetDebug(string Turn, string Message)
    {
        if (Turn == m_turnBase)
            return;
        //
        Debug.LogFormat("[Turn] <{0} : {1}> [End Turn: {2}] + [End Move: {3}] == {4} ?",
            Message,
            Turn.ToString(), 
            m_turnCurrent.UnitEndTurn.Count,
            m_turnCurrent.UnitEndMove.Count,
            m_turnCurrent.Unit.Count);
    }
}