using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TurnManager : SingletonManager<TurnManager>
{
    #region Varible: Setting

    [SerializeField][Min(0)] private float m_delayTurn = 1f;
    [SerializeField][Min(0)] private float m_delayStep = 1f;

    #endregion

    #region Varible: Debug

    private enum DebugType { None = 0, Primary = 1, Full = int.MaxValue, }

    [Space]
    [SerializeField] private DebugType m_debug = DebugType.None;

    #endregion

    //One TURN got many STEP, one STEP got many GameObject that all need to be complete their Move for next STEP or next TURN!!

    #region Event

    public Action<int> onTurn;          //<Turn>
    public Action<string> onStepStart;  //<Name>
    public Action<string> onStepEnd;    //<Name>

    #endregion

    #region Varible: Step Manager

    private int m_turnPass = 0;

    [Serializable]
    private class StepSingle
    {
        public int Start = 0;

        public string Step = "None";

        public List<ITurnManager> Unit;
        public List<ITurnManager> UnitEndStep;
        public List<ITurnManager> UnitEndMove;
        public List<ITurnManager> UnitWaitAdd;

        public bool EndMove => UnitEndMove.Count == Unit.Count - UnitEndStep.Count;
        public bool EndStep => UnitEndStep.Count == Unit.Count;

        public bool EndStepRemove = false;

        public StepSingle(string Step, int Start, ITurnManager Unit)
        {
            this.Start = Start;
            //
            this.Step = Step;
            //
            this.Unit = new List<ITurnManager>();
            UnitEndStep = new List<ITurnManager>();
            UnitEndMove = new List<ITurnManager>();
            UnitWaitAdd = new List<ITurnManager>
            {
                //
                Unit
            };
        }

        public bool GetEnd(ITurnManager UnitCheck)
        {
            if (!Unit.Contains(UnitCheck))
                return false;
            //
            if (!UnitEndStep.Contains(UnitCheck) && !UnitEndMove.Contains(UnitCheck))
                return false;
            //
            return true;
        }

        public void SetAdd(ITurnManager Unit)
        {
            UnitWaitAdd.Add(Unit);
        }

        public void SetWaitAdd()
        {
            Unit.AddRange(UnitWaitAdd);
            UnitWaitAdd.Clear();
        }
    }

    [SerializeField] private StepSingle m_stepCurrent;
    [SerializeField] private List<StepSingle> m_stepQueue = new List<StepSingle>();

    public List<string> StepRemove = new List<string>()
    {
        "None"
    }; //When Step add to this list, they will be auto remove out of Queue when their Move complete!

    #endregion

    private bool m_stop = false;

    protected override void Awake()
    {
        base.Awake();
        //
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += SetPlayModeStateChange;
#endif
    }

#if UNITY_EDITOR
    private static void SetPlayModeStateChange(PlayModeStateChange State)
    {
        //This used for stop Current Step coroutine called by ended Playing on Editor Mode!!
        //
        if (State == PlayModeStateChange.ExitingPlayMode)
            Instance.m_stop = true;
    }
#endif

    private void OnDestroy()
    {
        StopAllCoroutines();
        Instance.StopAllCoroutines();
        //
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= SetPlayModeStateChange;
#endif
    }

    #region Main

    public static void SetStart()
    {
        if ((int)Instance.m_debug >= (int)DebugType.None)
            Debug.LogFormat("[Turn] START!!");
        //
        Instance.m_turnPass = 0;
        Instance.m_stepQueue.RemoveAll(t => t.Step == "");
        Instance.m_stepQueue = Instance.m_stepQueue.OrderBy(t => t.Start).ToList();
        Instance.m_stepQueue.Insert(0, new StepSingle("", int.MaxValue, null));
        //
        Instance.SetCurrent();
    } //Start!!

    //

    private void SetCurrent()
    {
#if UNITY_EDITOR
        if (Instance.m_stop)
            return;
#endif
        //
        if (Instance != null)
            Instance.StartCoroutine(Instance.ISetCurrent());
    } //Force Next!!

    private IEnumerator ISetCurrent()
    {
        //Delay an Frame to wait for any Object complete Create and Init!!
        //
        yield return null;
        //
        m_stepCurrent = m_stepQueue[0];
        //
        bool DelayNewStep = true;
        //
        if (m_stepCurrent.Step == "")
        {
            DelayNewStep = false;
            //
            m_turnPass++;
            //
            if ((int)Instance.m_debug >= (int)DebugType.None)
                Debug.LogFormat("[Turn] <TURN {0} START>", m_turnPass);
            //
            onTurn?.Invoke(m_turnPass);
            //
            Instance.SetWait();
            //
            yield return null;
            //
            SetEndSwap(m_stepCurrent.Step);
            //
            if (m_delayTurn > 0)
                yield return new WaitForSeconds(m_delayTurn); //Delay before start new Turn!!
        }
        //
        //Fine to Start new Turn!!
        //
        m_stepCurrent = m_stepQueue[0];
        //
        if (m_stepCurrent.Unit.Count == 0)
        {
            m_stepCurrent.SetWaitAdd();
        }
        //
        if (m_stepCurrent != null)
        {
            if ((int)Instance.m_debug >= (int)DebugType.Full)
                Debug.LogFormat("[Turn] <TURN {1} START> {2} / {3}", m_turnPass, m_stepCurrent.Step, m_stepCurrent.UnitEndStep.Count, m_stepCurrent.Unit.Count);
        }
        //
        if (DelayNewStep && m_delayStep > 0)
            yield return new WaitForSeconds(m_delayStep); //Delay before start new Step in new Turn!!
        //
        onStepStart?.Invoke(m_stepCurrent.Step);
        //
        //Complete!!
    }

    private void SetWait()
    {
        foreach (StepSingle TurnCheck in Instance.m_stepQueue) TurnCheck.SetWaitAdd();
    }

    //

    public static void SetAutoRemove(string Step, bool Add = true)
    {
        if (string.IsNullOrEmpty(Step))
        {
            Debug.LogWarning("[Turn] Step name not valid!");
            return;
        }
        //
        if (Add && !Instance.StepRemove.Contains(Step))
            Instance.StepRemove.Add(Step);
        else
        if (!Add && Instance.StepRemove.Contains(Step))
            Instance.StepRemove.Remove(Step);
    }

    public static void SetAutoRemove<T>(T Step) where T : Enum
    {
        SetAutoRemove(Step.ToString());
    }

    #endregion

    #region Step ~ Int & String

    public static void SetInit(string Step, int Start, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
        {
            Debug.LogWarning("[Turn] Step name not valid!");
            return;
        }
        //
        if (Unit == null)
        {
            Debug.LogWarning("[Turn] Step object is null!");
            return;
        }
        //
        for (int i = 0; i < Instance.m_stepQueue.Count; i++)
        {
            if (Instance.m_stepQueue[i].Step != Step)
                continue;
            //
            if (Instance.m_stepQueue[i].Unit.Contains(Unit))
                return;
            //
            if ((int)Instance.m_debug >= (int)DebugType.Full)
                Debug.LogFormat("[Turn] <Init> {0}", Step.ToString());
            //
            Instance.m_stepQueue[i].UnitWaitAdd.Add(Unit);
            return;
        }
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            Debug.LogFormat("[Turn] <Init> {0}", Step.ToString());
        //
        Instance.m_stepQueue.Add(new StepSingle(Step, Start, Unit));
    } //Init on Start!!

    public static void SetRemove(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
        {
            Debug.LogWarning("[Turn] Step name not valid!");
            return;
        }
        //
        if (Unit == null)
        {
            Debug.LogWarning("[Turn] Step object is null!");
            return;
        }
        //
        for (int i = 0; i < Instance.m_stepQueue.Count; i++)
        {
            if (Instance.m_stepQueue[i].Step != Step)
                continue;
            //
            if (!Instance.m_stepQueue[i].Unit.Contains(Unit))
                return;
            //
            if (Instance.m_stepQueue[i] == Instance.m_stepCurrent)
            {
                Instance.m_stepQueue[i].Unit.Remove(Unit);
                Instance.m_stepQueue[i].UnitEndMove.Remove(Unit);
                Instance.m_stepQueue[i].UnitEndStep.Remove(Unit);
                Instance.m_stepQueue[i].UnitWaitAdd.Remove(Unit);
                //
                if ((int)Instance.m_debug >= (int)DebugType.Full)
                    SetDebug(Step, "Remove Same");
                //
                SetEndCheck(Step);
            }
            else
            {
                Instance.m_stepQueue[i].Unit.Remove(Unit);
                Instance.m_stepQueue[i].UnitEndMove.Remove(Unit);
                Instance.m_stepQueue[i].UnitEndStep.Remove(Unit);
                Instance.m_stepQueue[i].UnitWaitAdd.Remove(Unit);
                //
                if ((int)Instance.m_debug >= (int)DebugType.Full)
                    SetDebug(Step, "Remove Un-Same");
                //
            }
            //
            break;
        }
    } //Remove on Destroy!!

    public static void SetEndMove(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
        {
            Debug.LogWarning("[Turn] Step name not valid!");
            return;
        }
        //
        if (Unit == null)
        {
            Debug.LogWarning("[Turn] Step object is null!");
            return;
        }
        //
        if (Instance.m_stepCurrent.Step != Step)
            return;
        //
        if (Instance.m_stepCurrent.GetEnd(Unit))
            return;
        //
        Instance.m_stepCurrent.UnitEndMove.Add(Unit);
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            SetDebug(Step, "End Move");
        //
        SetEndCheck(Step);
    } //End!!

    public static void SetEndTurn(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
        {
            Debug.LogWarning("[Turn] Step name not valid!");
            return;
        }
        //
        if (Unit == null)
        {
            Debug.LogWarning("[Turn] Step object is null!");
            return;
        }
        //
        if (Instance.m_stepCurrent.Step != Step)
            return;
        //
        if (Instance.m_stepCurrent.GetEnd(Unit))
            return;
        //
        Instance.m_stepCurrent.UnitEndStep.Add(Unit);
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
            SetDebug(Step, "End Turn");
        //
        SetEndCheck(Step);
    } //End!!

    private static void SetEndCheck(string Step)
    {
        if (Instance.m_stepCurrent.EndStep)
        {
            if ((int)Instance.m_debug >= (int)DebugType.Primary)
                SetDebug(Step, "Next Turn");
            //
            Instance.m_stepCurrent.UnitEndMove.Clear();
            Instance.m_stepCurrent.UnitEndStep.Clear();
            //
            Instance.onStepEnd?.Invoke(Step.ToString());
            //
            SetEndSwap(Step);
            //
            Instance.SetCurrent();
        }
        else
        if (Instance.m_stepCurrent.EndMove)
        {
            if ((int)Instance.m_debug >= (int)DebugType.Primary)
                SetDebug(Step, "Next Step by Move");
            //
            Instance.m_stepCurrent.UnitEndMove.Clear();
            //
            Instance.SetCurrent();
        }
    } //Check End Step or End Move!!

    private static void SetEndSwap(string Step)
    {
        Instance.m_stepQueue.RemoveAt(Instance.m_stepQueue.FindIndex(t => t.Step == Step.ToString()));
        //
        if (!Instance.m_stepCurrent.EndStepRemove)
        {
            Instance.m_stepQueue.Add(Instance.m_stepCurrent);
        }
    } //Swap Current Step to Last!!

    public static void SetAdd(string Step, int Start, ITurnManager Unit, int After = 0)
    {
        if (string.IsNullOrEmpty(Step))
        {
            Debug.LogWarning("[Turn] Step name not valid!");
            return;
        }
        //
        if (Unit == null)
        {
            Debug.LogWarning("[Turn] Step object is null!");
            return;
        }
        //
        if (After < 0)
        {
            return;
        }
        //
        if (After > Instance.m_stepQueue.Count - 1)
        {
            Instance.m_stepQueue.Add(new StepSingle(Step, Start, Unit));
            Instance.m_stepQueue[Instance.m_stepQueue.Count - 1].EndStepRemove = Instance.StepRemove.Contains(Step);
        }
        else
        if (Instance.m_stepQueue[After].Step == Step)
        {
            if (Instance.m_stepQueue[After].Unit.Contains(Unit))
            {
                return;
            }
            //
            Instance.m_stepQueue[After].SetAdd(Unit);
        }
        else
        {
            Instance.m_stepQueue.Insert(After, new StepSingle(Step, Start, Unit));
            Instance.m_stepQueue[After].EndStepRemove = Instance.StepRemove.Contains(Step);
        }
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
        {
            SetDebug(Step, string.Format("Add [{0}]", After));
        }
    } //Add Step Special!!

    public static void SetAdd(string Step, int Start, ITurnManager Unit, string After)
    {
        if (string.IsNullOrEmpty(Step))
        {
            Debug.LogWarning("[Turn] Step name not valid!");
            return;
        }
        //
        if (Unit == null)
        {
            Debug.LogWarning("[Turn] Step object is null!");
            return;
        }
        //
        for (int i = 0; i < Instance.m_stepQueue.Count; i++)
        {
            if (Instance.m_stepQueue[i].Step != After)
                continue;
            //
            if (Instance.m_stepQueue[i].Step == Step)
            {
                if (Instance.m_stepQueue[i].Unit.Contains(Unit))
                    return;
                //
                Instance.m_stepQueue[i].SetAdd(Unit);
            }
            else
            {
                Instance.m_stepQueue.Insert(i, new StepSingle(Step.ToString(), Start, Unit));
                Instance.m_stepQueue[i].EndStepRemove = Instance.StepRemove.Contains(Step);
            }
            //
            return;
        }
        //
        if ((int)Instance.m_debug >= (int)DebugType.Full)
        {
            SetDebug(Step, string.Format("Add [{0}]", After));
        }
    } //Add Step Special!!

    #endregion

    #region Step ~ Enum

    public static void SetInit<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetInit(Step.ToString(), QEnum.GetChoice(Step), Unit);
    } //Init on Start!!

    public static void SetRemove<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetRemove(Step.ToString(), Unit);
    } //Remove on Destroy!!

    public static void SetEndMove<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetEndMove(Step.ToString(), Unit);
    } //End!!

    public static void SetEndStep<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetEndTurn(Step.ToString(), Unit);
    } //End!!

    public static void SetAdd<T>(T Step, ITurnManager Unit, int After = 0) where T : Enum
    {
        SetAdd(Step.ToString(), QEnum.GetChoice(Step), Unit, After);
    } //Add Step Special!!

    public static void SetAdd<T>(T Step, ITurnManager Unit, string After) where T : Enum
    {
        SetAdd(Step.ToString(), QEnum.GetChoice(Step), Unit, After);
    } //Add Step Special!!

    #endregion

    private static void SetDebug(string Step, string Message)
    {
        Debug.LogFormat("[Turn] <{0} : {1}> [End Turn: {2}] + [End Move: {3}] == {4} ?",
            Message,
            Step.ToString(),
            Instance.m_stepCurrent.UnitEndStep.Count,
            Instance.m_stepCurrent.UnitEndMove.Count,
            Instance.m_stepCurrent.Unit.Count);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TurnManager))]
public class GameTurnEditor : Editor
{
    private TurnManager Target;

    private SerializedProperty m_delayTurn;
    private SerializedProperty m_delayStep;

    private SerializedProperty m_debug;
    private SerializedProperty m_stepCurrent;
    private SerializedProperty m_stepQueue;

    private void OnEnable()
    {
        Target = target as TurnManager;
        //
        m_delayTurn = QUnityEditorCustom.GetField(this, "m_delayTurn");
        m_delayStep = QUnityEditorCustom.GetField(this, "m_delayStep");
        //
        m_debug = QUnityEditorCustom.GetField(this, "m_debug");
        m_stepCurrent = QUnityEditorCustom.GetField(this, "m_stepCurrent");
        m_stepQueue = QUnityEditorCustom.GetField(this, "m_stepQueue");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_delayTurn);
        QUnityEditorCustom.SetField(m_delayStep);
        //
        QUnityEditorCustom.SetField(m_debug);
        //
        QUnityEditor.SetDisableGroupBegin();
        //
        QUnityEditorCustom.SetField(m_stepCurrent);
        QUnityEditorCustom.SetField(m_stepQueue);
        //
        QUnityEditor.SetDisableGroupEnd();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif

//

public interface ITurnManager
{
    void ITurn(int Turn);

    void IStepStart(string Name);

    void IStepEnd(string Name);
}