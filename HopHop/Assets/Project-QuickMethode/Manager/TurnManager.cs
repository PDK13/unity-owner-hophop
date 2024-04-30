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
    #region Varible: Action

    //Main events for every unit(s) and other progess(s)!

    public Action<int> onTurn;              //<Turn>
    public Action<string> onStepStart;      //<Step>
    public Action<string> onStepEnd;        //<Step>

    //Optionals event for every unit(s) and other progess(s)!

    public Action<string> onStepAdd;        //<Step>
    public Action<string> onStepRemove;     //<Step>

    #endregion

    #region Varible: Setting

    [SerializeField][Min(0)] private float m_delayTurn = 1f;
    [SerializeField][Min(0)] private float m_delayStep = 1f;

    #endregion

    //One TURN got many STEP, one STEP got many GameObject that all need to be complete their Move for next STEP or next TURN!!

    #region Varible: Turn & Step

    private int m_turnPass = 0;

    [Serializable]
    private class StepSingle
    {
        public int Start = 0;

        public string Step = "None";

        public List<ITurnManager> Unit;
        public List<ITurnManager> UnitEndStep;
        public List<ITurnManager> UnitEndMove;

        //IMFORTANCE: New unit(s) not add into main queue unless current STEP ended, to avoid bug in check old unit(s) end their MOVE!

        public List<ITurnManager> UnitWaitAdd; //Unit WAIT for add into main queue!

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

        //

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

        //

        /// <summary>
        /// Add unit to WAIT!
        /// </summary>
        /// <param name="Unit"></param>
        public void SetAdd(ITurnManager Unit)
        {
            UnitWaitAdd.Add(Unit);
        }

        /// <summary>
        /// Add WAIT unit(s) to main queue!
        /// </summary>
        public void SetWaitAdd()
        {
            Unit.AddRange(UnitWaitAdd);
            UnitWaitAdd.Clear();
        }
    }

    [SerializeField] private StepSingle m_stepCurrent;
    [SerializeField] private List<StepSingle> m_stepQueue = new List<StepSingle>();

    public (string Step, int Count) StepCurrent => (m_stepCurrent.Step, m_stepCurrent.Unit.Count);

    public List<(string Step, int Count)> StepQueueCurrent
    {
        get
        {
            List<(string Step, int Count)> Queue = new List<(string Step, int Count)>();
            foreach (var StepCheck in m_stepQueue)
                Queue.Add((StepCheck.Step, StepCheck.Unit.Count));
            return Queue;
        }
    }

    //

    public List<string> StepRemove = new List<string>()
    {
        "None"
    }; //When Step add to this list, they will be auto remove out of Queue when their Move complete!

    #endregion

    private bool m_stop = false;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += SetPlayModeStateChange;
#endif
    }

#if UNITY_EDITOR
    private static void SetPlayModeStateChange(PlayModeStateChange State)
    {
        //NOTE: This used for stop Current Step coroutine called by ended Playing on Editor Mode!!

        if (State == PlayModeStateChange.ExitingPlayMode)
            Instance.m_stop = true;
    }
#endif

    private void OnDestroy()
    {
        StopAllCoroutines();
        Instance.StopAllCoroutines();

        onTurn = null;
        onStepStart = null;
        onStepEnd = null;
        onStepAdd = null;
        onStepRemove = null;

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= SetPlayModeStateChange;
#endif
    }

    #region Main

    /// <summary>
    /// Start main progess!
    /// </summary>
    public static void SetStart()
    {
        Instance.m_turnPass = 0;
        Instance.m_stepQueue.RemoveAll(t => t.Step == "");
        Instance.m_stepQueue = Instance.m_stepQueue.OrderBy(t => t.Start).ToList();

        //NOTE: Create a STEP of EMTY to guild Manager know when end of TURN!
        Instance.m_stepQueue.Insert(0, new StepSingle("", int.MaxValue, null));

        Instance.SetCurrent();
    } //Start!!

    //

    private void SetCurrent()
    {
#if UNITY_EDITOR
        if (Instance.m_stop)
            return;
#endif

        if (Instance != null)
            Instance.StartCoroutine(Instance.ISetCurrent());
    } //Force Next!!

    private IEnumerator ISetCurrent()
    {
        //NOTE: Delay an Frame to wait for any Object complete Create and Init!!

        yield return null;

        m_stepCurrent = m_stepQueue[0];

        bool DelayNewStep = true;

        //NOTE: Check if current STEP is EMTY (Created at start for manager know when end of TURN)!

        if (m_stepCurrent.Step == "")
        {
            //NOTE: New TURN occured!

            DelayNewStep = false;

            m_turnPass++;

            onTurn?.Invoke(m_turnPass);

            Instance.SetWait();

            yield return null;

            SetEndSwap(m_stepCurrent.Step);

            //NOTE: Delay before start new Step!!

            if (m_delayTurn > 0)
                yield return new WaitForSeconds(m_delayTurn);
        }

        //NOTE: Fine to Start new Step!!

        m_stepCurrent = m_stepQueue[0];

        if (m_stepCurrent.Unit.Count == 0)
            //NOTE: At the start, no unit(s) in queue, so add WAIT unit into queue!
            m_stepCurrent.SetWaitAdd();

        //NOTE: Delay before start new Step in new Turn!!

        if (DelayNewStep && m_delayStep > 0)
            yield return new WaitForSeconds(m_delayStep);

        onStepStart?.Invoke(m_stepCurrent.Step);

        //NOTE: Complete!!
    }

    private void SetWait()
    {
        //NOTE: Add WAIT unit(s) to main queue!
        //
        foreach (StepSingle StepCheck in Instance.m_stepQueue)
            StepCheck.SetWaitAdd();
    }

    //

    /// <summary>
    /// Add or remove STEP that to check for remove when STEP end!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Add"></param>
    public static void SetAutoRemove(string Step, bool Add = true)
    {
        if (string.IsNullOrEmpty(Step))
            return;
        //
        if (Add && !Instance.StepRemove.Contains(Step))
            Instance.StepRemove.Add(Step);
        else
        if (!Add && Instance.StepRemove.Contains(Step))
            Instance.StepRemove.Remove(Step);
    }

    /// <summary>
    /// Add or remove STEP that to check for remove when STEP end!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Step"></param>
    /// <param name="Add"></param>
    public static void SetAutoRemove<T>(T Step, bool Add = true) where T : Enum
    {
        SetAutoRemove(Step.ToString(), Add);
    }

    #endregion

    #region Step ~ Int & String

    /// <summary>
    /// Add a unit to main queue before start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    public static void SetInit(string Step, int Start, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        for (int i = 0; i < Instance.m_stepQueue.Count; i++)
        {
            if (Instance.m_stepQueue[i].Step != Step)
                continue;

            if (Instance.m_stepQueue[i].Unit.Contains(Unit))
                return;

            Instance.m_stepQueue[i].UnitWaitAdd.Add(Unit);

            return;
        }

        Instance.m_stepQueue.Add(new StepSingle(Step, Start, Unit));

        Instance.onStepAdd?.Invoke(Step);
    } //Init on Start!!

    /// <summary>
    /// Remove a unit from main queue!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public static void SetRemove(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        var StepFind = Instance.m_stepQueue.Find(t => t.Step == Step);
        if (StepFind == null)
            return;

        var UnitFind = StepFind.Unit.Find(t => t == Unit);
        if (UnitFind == null)
            return;

        if (StepFind == Instance.m_stepCurrent)
        {
            StepFind.Unit.Remove(Unit);
            StepFind.UnitEndMove.Remove(Unit);
            StepFind.UnitEndStep.Remove(Unit);
            StepFind.UnitWaitAdd.Remove(Unit);

            SetEndCheck(Step);
        }
        else
        {
            StepFind.Unit.Remove(Unit);
            StepFind.UnitEndMove.Remove(Unit);
            StepFind.UnitEndStep.Remove(Unit);
            StepFind.UnitWaitAdd.Remove(Unit);
        }

        Instance.onStepRemove?.Invoke(Step);
    } //Remove on Destroy!!

    /// <summary>
    /// Accept a unit that completed it's MOVE, but not it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public static void SetEndMove(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        if (Instance.m_stepCurrent.Step != Step)
            return;

        if (Instance.m_stepCurrent.GetEnd(Unit))
            return;

        Instance.m_stepCurrent.UnitEndMove.Add(Unit);

        SetEndCheck(Step);
    } //End!!

    /// <summary>
    /// Accept a unit that completed it's MOVE and it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public static void SetEndStep(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        if (Instance.m_stepCurrent.Step != Step)
            return;

        if (Instance.m_stepCurrent.GetEnd(Unit))
            return;

        Instance.m_stepCurrent.UnitEndStep.Add(Unit);

        SetEndCheck(Step);
    } //End!!

    private static void SetEndCheck(string Step)
    {
        if (Instance.m_stepCurrent.EndStep)
        {
            Instance.m_stepCurrent.UnitEndMove.Clear();
            Instance.m_stepCurrent.UnitEndStep.Clear();

            Instance.onStepEnd?.Invoke(Step.ToString());

            SetEndSwap(Step);

            Instance.SetCurrent();
        }
        else
        if (Instance.m_stepCurrent.EndMove)
        {
            Instance.m_stepCurrent.UnitEndMove.Clear();

            Instance.SetCurrent();
        }
    } //Check End Step or End Move!!

    private static void SetEndSwap(string Step)
    {
        //NOTE: Check to remove STEP in check!

        var StepFind = Instance.m_stepQueue.Find(t => t.Step == Step);
        if (StepFind == null)
            return;

        if (StepFind.EndStepRemove)
            Instance.onStepRemove?.Invoke(Step);

        Instance.m_stepQueue.Remove(StepFind);

        //NOTE: STEP in check maybe is the current STEP in queue!

        if (!Instance.m_stepCurrent.EndStepRemove)
            //NOTE: If not remove STEP at the end current STEP, move current STEP to last in queue!
            Instance.m_stepQueue.Add(Instance.m_stepCurrent);
    } //Swap Current Step to Last!!

    /// <summary>
    /// Add a unit to main queue after start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    /// <param name="After"></param>
    public static void SetAdd(string Step, int Start, ITurnManager Unit, int After = 0)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        if (After < 0)
            return;

        if (After > Instance.m_stepQueue.Count - 1)
        {
            Instance.m_stepQueue.Add(new StepSingle(Step, Start, Unit));
            Instance.m_stepQueue[Instance.m_stepQueue.Count - 1].EndStepRemove = Instance.StepRemove.Contains(Step);
        }
        else
        if (Instance.m_stepQueue[After].Step == Step)
        {
            if (Instance.m_stepQueue[After].Unit.Contains(Unit))
                return;

            Instance.m_stepQueue[After].SetAdd(Unit);
        }
        else
        {
            Instance.m_stepQueue.Insert(After, new StepSingle(Step, Start, Unit));
            Instance.m_stepQueue[After].EndStepRemove = Instance.StepRemove.Contains(Step);
        }

        Instance.onStepAdd?.Invoke(Step);
    } //Add Step Special!!

    /// <summary>
    /// Add a unit to main queue after start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    /// <param name="After"></param>
    public static void SetAdd(string Step, int Start, ITurnManager Unit, string After)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        for (int i = 0; i < Instance.m_stepQueue.Count; i++)
        {
            if (Instance.m_stepQueue[i].Step != After)
                continue;

            if (Instance.m_stepQueue[i].Step == Step)
            {
                if (Instance.m_stepQueue[i].Unit.Contains(Unit))
                    return;

                Instance.m_stepQueue[i].SetAdd(Unit);
            }
            else
            {
                Instance.m_stepQueue.Insert(i, new StepSingle(Step.ToString(), Start, Unit));
                Instance.m_stepQueue[i].EndStepRemove = Instance.StepRemove.Contains(Step);
            }

            return;
        }

        Instance.onStepAdd?.Invoke(Step);
    } //Add Step Special!!

    #endregion

    #region Step ~ Enum

    /// <summary>
    /// Add a unit to main queue before start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    public static void SetInit<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetInit(Step.ToString(), Mathf.Clamp(QEnum.GetChoice(Step), 0, int.MaxValue), Unit);
    } //Init on Start!!

    /// <summary>
    /// Remove a unit from main queue!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public static void SetRemove<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetRemove(Step.ToString(), Unit);
    } //Remove on Destroy!!

    /// <summary>
    /// Accept a unit that completed it's MOVE, but not it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public static void SetEndMove<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetEndMove(Step.ToString(), Unit);
    } //End!!

    /// <summary>
    /// Accept a unit that completed it's MOVE and it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public static void SetEndStep<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetEndStep(Step.ToString(), Unit);
    } //End!!

    /// <summary>
    /// Add a unit to main queue after start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    /// <param name="After"></param>
    public static void SetAdd<T>(T Step, ITurnManager Unit, int After = 0) where T : Enum
    {
        SetAdd(Step.ToString(), Mathf.Clamp(QEnum.GetChoice(Step), 0, int.MaxValue), Unit, After);
    } //Add Step Special!!

    /// <summary>
    /// Add a unit to main queue after start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    /// <param name="After"></param>
    public static void SetAdd<T>(T Step, ITurnManager Unit, string After) where T : Enum
    {
        SetAdd(Step.ToString(), Mathf.Clamp(QEnum.GetChoice(Step), 0, int.MaxValue), Unit, After);
    } //Add Step Special!!

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(TurnManager))]
public class GameTurnEditor : Editor
{
    private TurnManager Target;

    private SerializedProperty m_delayTurn;
    private SerializedProperty m_delayStep;

    private void OnEnable()
    {
        Target = target as TurnManager;

        m_delayTurn = QUnityEditorCustom.GetField(this, "m_delayTurn");
        m_delayStep = QUnityEditorCustom.GetField(this, "m_delayStep");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_delayTurn);
        QUnityEditorCustom.SetField(m_delayStep);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif

//

public interface ITurnManager
{
    void ISetTurn(int Step);

    void ISetStepStart(string Step);

    void ISetStepEnd(string Step);
}

public interface ITurnManagerOptional
{
    void ISetStepAdd(string Step);

    void ISetStepRemove(string Step);
}