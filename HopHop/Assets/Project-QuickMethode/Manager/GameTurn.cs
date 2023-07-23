using QuickMethode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameTurn : MonoBehaviour
{
    public static GameTurn Instance;

    #region Varible: Debug

    private enum DebugType { None = 0, Primary = 1, Full = int.MaxValue, }

    [SerializeField] private DebugType m_debug = DebugType.None;

    #endregion

    public Action<string> onTurn;
    public Action<string> onEnd;

    private int m_turnPass = 0;

    [Serializable]
    private class TurnSingle
    {
        public int Start = 0;

        public string Turn = "None";

        public List<GameObject> Unit;
        public List<GameObject> UnitEndTurn;
        public List<GameObject> UnitEndMove;
        public List<GameObject> UnitWaitAdd;

        public bool EndMove => UnitEndMove.Count == Unit.Count - UnitEndTurn.Count;
        public bool EndTurn => UnitEndTurn.Count == Unit.Count;

        public bool EndTurnRemove = false;

        public TurnSingle (int Start, string Turn, GameObject Unit)
        {
            this.Start = Start;
            //
            this.Turn = Turn;
            //
            this.Unit = new List<GameObject>();
            this.UnitEndTurn = new List<GameObject>();
            this.UnitEndMove = new List<GameObject>();
            this.UnitWaitAdd = new List<GameObject>();
            //
            this.UnitWaitAdd.Add(Unit);
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

        public void SetAdd(GameObject Unit)
        {
            this.UnitWaitAdd.Add(Unit);
        }

        public void SetWaitAdd()
        {
            Unit.AddRange(UnitWaitAdd);
            UnitWaitAdd.Clear();
        }
    }

    [SerializeField] private TurnSingle m_turnCurrent;
    [SerializeField] private List<TurnSingle> m_turnQueue = new List<TurnSingle>();

    public List<string> TurnRemove = new List<string>()
    {
        "None",
        "Gravity",
    };

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        //
        Instance = this;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public static void SetStart(string TurnBase = "")
    {
        if ((int)Instance.m_debug >= (int)DebugType.None)
            Debug.LogWarning("[Turn] START!!");
        //
        Instance.m_turnPass = 0;
        Instance.m_turnQueue.RemoveAll(t => t.Turn == "");
        Instance.m_turnQueue = Instance.m_turnQueue.OrderBy(t => t.Start).ToList();
        Instance.m_turnQueue.Insert(0, new TurnSingle(int.MaxValue, "", Instance.gameObject));
        //
        Instance.SetCurrent();
    } //Start!!

    private void SetCurrent()
    {
        Instance.StartCoroutine(Instance.ISetCurrent());
    } //Force Turn Next!!

    private IEnumerator ISetCurrent()
    {
        //Delay an Frame to wait for any Object complete Create and Init!!
        //
        yield return null;
        //
        m_turnCurrent = m_turnQueue[0];
        //
        if (m_turnCurrent.Turn == "")
        {
            m_turnPass++;
            //
            if ((int)Instance.m_debug >= (int)DebugType.None)
                Debug.LogWarningFormat("[Turn] <TURN {0} START>", m_turnPass);
            //
            Instance.SetAdd();
            //
            SetEndSwap(m_turnCurrent.Turn);
        }
        //
        m_turnCurrent = m_turnQueue[0];
        //
        if (m_turnCurrent.Unit.Count == 0)
            m_turnCurrent.SetWaitAdd();
        //
        if (m_turnCurrent != null)
        {
            if ((int)Instance.m_debug >= (int)DebugType.Full)
                Debug.LogWarningFormat("[Turn] <TURN {1} START> {2} / {3}", m_turnPass, m_turnCurrent.Turn, m_turnCurrent.UnitEndTurn.Count, m_turnCurrent.Unit.Count);
        }
        //
        onTurn?.Invoke(m_turnCurrent.Turn);
        //
        SetEndCheck(m_turnCurrent.Turn.ToString());
    }

    private void SetAdd()
    {
        foreach (TurnSingle TurnCheck in Instance.m_turnQueue)
            TurnCheck.SetWaitAdd();
    }

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
        for (int i = 0; i < Instance.m_turnQueue.Count; i++)
        {
            if (Instance.m_turnQueue[i].Turn != Turn)
                continue;
            //
            if (Instance.m_turnQueue[i].Unit.Contains(Unit))
                return;
            //
            if ((int)Instance.m_debug >= (int)DebugType.Full)
                Debug.LogFormat("[Turn] <Init> {0}", Turn.ToString());
            //
            Instance.m_turnQueue[i].UnitWaitAdd.Add(Unit);
            return;
        }
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            Debug.LogFormat("[Turn] <Init> {0}", Turn.ToString());
        //
        Instance.m_turnQueue.Add(new TurnSingle(Start, Turn, Unit));
    } //Init on Start!!

    public static void SetRemove(string Turn, GameObject Unit)
    {
        for (int i = 0; i < Instance.m_turnQueue.Count; i++)
        {
            if (Instance.m_turnQueue[i].Turn != Turn)
                continue;
            //
            if (!Instance.m_turnQueue[i].Unit.Contains(Unit))
                return;
            //
            if (Instance.m_turnQueue[i] == Instance.m_turnCurrent)
            {
                Instance.m_turnQueue[i].Unit.Remove(Unit);
                Instance.m_turnQueue[i].UnitEndMove.Remove(Unit);
                Instance.m_turnQueue[i].UnitEndTurn.Remove(Unit);
                Instance.m_turnQueue[i].UnitWaitAdd.Remove(Unit);
                //
                if ((int)Instance.m_debug >= (int)DebugType.Full)
                    SetDebug(Turn, "Remove Same");
                //
                SetEndCheck(Turn);
            }
            else
            {
                Instance.m_turnQueue[i].Unit.Remove(Unit);
                Instance.m_turnQueue[i].UnitEndMove.Remove(Unit);
                Instance.m_turnQueue[i].UnitEndTurn.Remove(Unit);
                Instance.m_turnQueue[i].UnitWaitAdd.Remove(Unit);
                //
                if ((int)Instance.m_debug >= (int)DebugType.Full)
                    SetDebug(Turn, "Remove Un-Same");
                //
            }
            //
            break;
        }
    } //Remove on Destroy!!

    public static void SetEndMove(string Turn, GameObject Unit)
    {
        if (Instance.m_turnCurrent.Turn != Turn)
            return;
        //
        if (Instance.m_turnCurrent.GetEnd(Unit))
            return;
        //
        Instance.m_turnCurrent.UnitEndMove.Add(Unit);
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            SetDebug(Turn, "End Move");
        //
        SetEndCheck(Turn);
    } //End!!

    public static void SetEndTurn(string Turn, GameObject Unit)
    {
        if (Instance.m_turnCurrent.Turn != Turn)
            return;
        //
        if (Instance.m_turnCurrent.GetEnd(Unit))
            return;
        //
        Instance.m_turnCurrent.UnitEndTurn.Add(Unit);
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            SetDebug(Turn, "End Turn");
        //
        SetEndCheck(Turn);
    } //End!!

    private static void SetEndCheck(string Turn)
    {
        if (Instance.m_turnCurrent.EndTurn)
        {
            if ((int)Instance.m_debug >= (int)DebugType.Primary)
                SetDebug(Turn, "Next Turn");
            //
            Instance.m_turnCurrent.UnitEndMove.Clear();
            Instance.m_turnCurrent.UnitEndTurn.Clear();
            //
            Instance.onEnd?.Invoke(Turn.ToString());
            //
            SetEndSwap(Turn);
            //
            Instance.SetCurrent();
        }
        else
        if (Instance.m_turnCurrent.EndMove)
        {
            if ((int)Instance.m_debug >= (int)DebugType.Primary)
                SetDebug(Turn, "Next Turn by Move");
            //
            Instance.m_turnCurrent.UnitEndMove.Clear();
            //
            Instance.SetCurrent();
        }
    } //Check End Turn or End Move!!

    private static void SetEndSwap(string Turn)
    {
        Instance.m_turnQueue.RemoveAt(Instance.m_turnQueue.FindIndex(t => t.Turn == Turn.ToString()));
        //
        if (!Instance.m_turnCurrent.EndTurnRemove)
            Instance.m_turnQueue.Add(Instance.m_turnCurrent);
    } //Swap Current Turn to Last!!

    public static void SetAdd(int Start, string Turn, GameObject Unit, int After = 0)
    {
        if (After < 0)
            return;
        //
        if (After > Instance.m_turnQueue.Count - 1)
        {
            Instance.m_turnQueue.Add(new TurnSingle(Start, Turn, Unit));
            Instance.m_turnQueue[Instance.m_turnQueue.Count - 1].EndTurnRemove = Instance.TurnRemove.Contains(Turn);
        }
        else
        if (Instance.m_turnQueue[After].Turn == Turn)
        {
            if (Instance.m_turnQueue[After].Unit.Contains(Unit))
                return;
            //
            Instance.m_turnQueue[After].SetAdd(Unit);
        }
        else
        {
            Instance.m_turnQueue.Insert(After, new TurnSingle(Start, Turn, Unit));
            Instance.m_turnQueue[After].EndTurnRemove = Instance.TurnRemove.Contains(Turn);
        }
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            SetDebug(Turn, string.Format("Add [{0}]", After));
    } //Add Turn Special!!

    public static void SetAdd(int Start, string Turn, GameObject Unit, string After)
    {
        for (int i = 0; i < Instance.m_turnQueue.Count; i++)
        {
            if (Instance.m_turnQueue[i].Turn != After)
                continue;
            //
            if (Instance.m_turnQueue[i].Turn == Turn)
            {
                if (Instance.m_turnQueue[i].Unit.Contains(Unit))
                    return;
                //
                Instance.m_turnQueue[i].SetAdd(Unit);
            }
            else
            {
                Instance.m_turnQueue.Insert(i, new TurnSingle(Start, Turn.ToString(), Unit));
                Instance.m_turnQueue[i].EndTurnRemove = Instance.TurnRemove.Contains(Turn);
            }
            //
            return;
        }
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            SetDebug(Turn, string.Format("Add [{0}]", After));
    } //Add Turn Special!!

    #endregion

    private static void SetDebug(string Turn, string Message)
    {
        Debug.LogFormat("[Turn] <{0} : {1}> [End Turn: {2}] + [End Move: {3}] == {4} ?",
            Message,
            Turn.ToString(),
            Instance.m_turnCurrent.UnitEndTurn.Count,
            Instance.m_turnCurrent.UnitEndMove.Count,
            Instance.m_turnCurrent.Unit.Count);
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(GameTurn))]
    public class GameTurnEditor : Editor
    {
        private GameTurn Target;

        private SerializedProperty m_debug;
        private SerializedProperty m_turnCurrent;
        private SerializedProperty m_turnQueue;

        private void OnEnable()
        {
            Target = target as GameTurn;

            m_debug = QEditorCustom.GetField(this, "m_debug");
            m_turnCurrent = QEditorCustom.GetField(this, "m_turnCurrent");
            m_turnQueue = QEditorCustom.GetField(this, "m_turnQueue");
        }

        public override void OnInspectorGUI()
        {
            QEditorCustom.SetUpdate(this);
            //
            QEditorCustom.SetField(m_debug);
            //
            QEditor.SetDisableGroupBegin();
            //
            QEditorCustom.SetField(m_turnCurrent);
            QEditorCustom.SetField(m_turnQueue);
            //
            QEditor.SetDisableGroupEnd();
            //
            QEditorCustom.SetApply(this);
        }
    }

#endif

}