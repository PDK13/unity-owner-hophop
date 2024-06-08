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
    #region Action

    //Main events for every unit(s) and other progess(s)!

    public Action<int> onTurnStart;         //Turn
    public Action<string> onStepStart;      //Step
    public Action<string> onStepEnd;        //Step
    public Action<int> onTurnEnd;         //Turn

    //Optionals event for every unit(s) and other progess(s)!

    public Action<bool> onTurnPause;        //Pause
    public Action<string> onStepAdd;        //Step
    public Action<string> onStepRemove;     //Step

    #endregion

    #region Setting

    [SerializeField][Min(0)] private float m_delayTurn = 1f;
    [SerializeField][Min(0)] private float m_delayStep = 1f;

    #endregion

    //One TURN got many STEP, one STEP got many GameObject that all need to be complete their Move for next STEP or next TURN!!

    #region Turn & Step

    private int m_turnPass = 0;
    private bool m_turnPause = false;
    private Coroutine m_iSetCurrent;

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
            if (UnitWaitAdd.Contains(Unit))
                return;

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

    [Space]
    [SerializeField] private StepSingle m_stepCurrent;
    [SerializeField] private List<StepSingle> m_stepQueue = new List<StepSingle>();

    private List<string> m_stepRemove = new List<string>()
    {
        "None"
    }; //When Step add to this list, they will be auto remove out of Queue when their Steo or Move completed!

    //

    public bool TurnPause
    {
        get => m_turnPause;
        set
        {
            m_turnPause = value;
            onTurnPause?.Invoke(value);
        }
    } //Pause current Turn (if end it's Move or Step) before next Turn activated!

    public (string Step, int Count) StepCurrent => (m_stepCurrent.Step, m_stepCurrent.Unit.Count);

    public (string Step, int Count)[] StepQueue
    {
        get
        {
            List<(string Step, int Count)> Queue = new List<(string Step, int Count)>();
            foreach (var StepCheck in m_stepQueue)
                Queue.Add((StepCheck.Step, StepCheck.Unit.Count));
            return Queue.ToArray();
        }
    }

    public string[] StepRemove => m_stepRemove.ToArray();

    #endregion

    private bool m_stop = false;

    //

    private void Awake()
    {
        SetInstance();

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += SetPlayModeStateChange;
#endif
    }

#if UNITY_EDITOR
    private void SetPlayModeStateChange(PlayModeStateChange State)
    {
        //NOTE: This used for stop Current Step coroutine called by ended Playing on Editor Mode!!

        if (State == PlayModeStateChange.ExitingPlayMode)
            m_stop = true;
    }
#endif

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (m_iSetCurrent != null)
            StopCoroutine(m_iSetCurrent);

        onTurnStart = null;
        onStepStart = null;
        onStepEnd = null;
        onStepAdd = null;
        onStepRemove = null;

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= SetPlayModeStateChange;
#endif
    }

    //

    #region Main

    /// <summary>
    /// Start main progess!
    /// </summary>
    public void SetStart()
    {
        m_turnPass = 0;
        m_stepQueue.RemoveAll(t => t.Step == "");
        m_stepQueue = m_stepQueue.OrderBy(t => t.Start).ToList();

        //NOTE: Create a STEP of EMTY to guild Manager know when end of TURN!
        m_stepQueue.Insert(0, new StepSingle("", int.MaxValue, null));

        SetCurrent();
    } //Start!!

    //

    private void SetCurrent()
    {
        if (Instance == null)
            return;

#if UNITY_EDITOR
        if (m_stop)
            return;
#endif

        if (m_iSetCurrent != null)
            StopCoroutine(m_iSetCurrent);
        m_iSetCurrent = StartCoroutine(ISetCurrent());
    } //Force Next!!

    private IEnumerator ISetCurrent()
    {
        yield return new WaitUntil(() => !m_turnPause);

        //NOTE: Delay to wait for any Object complete Create and Init!!

        yield return null;

        m_stepCurrent = m_stepQueue[0];

        bool DelayNewStep = true;

        //NOTE: Check if current STEP is EMTY (Created at start for manager know when end of TURN)!

        if (m_stepCurrent.Step == "")
        {
            //NOTE: New TURN occured!

            DelayNewStep = false;

            if (m_turnPass > 0)
                onTurnEnd?.Invoke(m_turnPass);

            yield return null;

            m_turnPass++;

            onTurnStart?.Invoke(m_turnPass);

            SetWait();

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

        m_iSetCurrent = null;
    }

    private void SetWait()
    {
        //NOTE: Add WAIT unit(s) to main queue!
        //
        foreach (StepSingle StepCheck in m_stepQueue)
            StepCheck.SetWaitAdd();
    }

    //

    /// <summary>
    /// Add or remove STEP that to check for remove when STEP end!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Add"></param>
    public void SetAutoRemove(string Step, bool Add = true)
    {
        if (string.IsNullOrEmpty(Step))
            return;
        //
        if (Add && !m_stepRemove.Contains(Step))
            m_stepRemove.Add(Step);
        else
        if (!Add && m_stepRemove.Contains(Step))
            m_stepRemove.Remove(Step);
    }

    /// <summary>
    /// Add or remove STEP that to check for remove when STEP end!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Step"></param>
    /// <param name="Add"></param>
    public void SetAutoRemove<T>(T Step, bool Add = true) where T : Enum
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
    public void SetInit(string Step, int Start, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        for (int i = 0; i < m_stepQueue.Count; i++)
        {
            if (m_stepQueue[i].Step != Step)
                continue;

            if (m_stepQueue[i].Unit.Contains(Unit))
                return;

            m_stepQueue[i].UnitWaitAdd.Add(Unit);

            return;
        }

        m_stepQueue.Add(new StepSingle(Step, Start, Unit));

        onStepAdd?.Invoke(Step);
    } //Init on Start!!

    /// <summary>
    /// Remove a unit from main queue!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public void SetRemove(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        var StepFind = m_stepQueue.Find(t => t.Step == Step);
        if (StepFind == null)
            return;

        var UnitFind = StepFind.Unit.Find(t => t == Unit);
        if (UnitFind == null)
            return;

        if (StepFind == m_stepCurrent)
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

        onStepRemove?.Invoke(Step);
    } //Remove on Destroy!!

    /// <summary>
    /// Accept a unit that completed it's MOVE, but not it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public void SetEndMove(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        if (m_stepCurrent.Step != Step)
            return;

        if (m_stepCurrent.GetEnd(Unit))
            return;

        m_stepCurrent.UnitEndMove.Add(Unit);

        SetEndCheck(Step);
    } //End!!

    /// <summary>
    /// Accept a unit that completed it's MOVE and it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public void SetEndStep(string Step, ITurnManager Unit)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        if (m_stepCurrent.Step != Step)
            return;

        if (m_stepCurrent.GetEnd(Unit))
            return;

        m_stepCurrent.UnitEndStep.Add(Unit);

        SetEndCheck(Step);
    } //End!!

    private void SetEndCheck(string Step)
    {
        if (m_stepCurrent.EndStep)
        {
            m_stepCurrent.UnitEndMove.Clear();
            m_stepCurrent.UnitEndStep.Clear();

            onStepEnd?.Invoke(Step.ToString());

            SetEndSwap(Step);

            SetCurrent();
        }
        else
        if (m_stepCurrent.EndMove)
        {
            m_stepCurrent.UnitEndMove.Clear();

            SetCurrent();
        }
    } //Check End Step or End Move!!

    private void SetEndSwap(string Step)
    {
        //NOTE: Check to remove STEP in check!

        var StepFind = m_stepQueue.Find(t => t.Step == Step);
        if (StepFind == null)
            return;

        if (StepFind.EndStepRemove)
            onStepRemove?.Invoke(Step);

        m_stepQueue.Remove(StepFind);

        //NOTE: STEP in check maybe is the current STEP in queue!

        if (!m_stepCurrent.EndStepRemove)
            //NOTE: If not remove STEP at the end current STEP, move current STEP to last in queue!
            m_stepQueue.Add(m_stepCurrent);
    } //Swap Current Step to Last!!

    /// <summary>
    /// Check a unit end stage in current
    /// </summary>
    /// <param name="Unit"></param>
    /// <returns></returns>
    public bool GetEndCurrent(ITurnManager Unit)
    {
        return m_stepCurrent.GetEnd(Unit);
    }

    /// <summary>
    /// Add a unit to main queue after start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    /// <param name="After"></param>
    public void SetAdd(string Step, int Start, ITurnManager Unit, int After = 0)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        if (After < 0)
            return;

        if (After > m_stepQueue.Count - 1)
        {
            m_stepQueue.Add(new StepSingle(Step, Start, Unit));
            m_stepQueue[m_stepQueue.Count - 1].EndStepRemove = m_stepRemove.Contains(Step);
        }
        else
        if (m_stepQueue[After].Step == Step)
        {
            if (m_stepQueue[After].Unit.Contains(Unit))
                return;

            m_stepQueue[After].SetAdd(Unit);
        }
        else
        {
            m_stepQueue.Insert(After, new StepSingle(Step, Start, Unit));
            m_stepQueue[After].EndStepRemove = m_stepRemove.Contains(Step);
        }

        onStepAdd?.Invoke(Step);
    } //Add Step Special!!

    /// <summary>
    /// Add a unit to main queue after start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    /// <param name="After"></param>
    public void SetAdd(string Step, int Start, ITurnManager Unit, string After)
    {
        if (string.IsNullOrEmpty(Step))
            return;

        if (Unit == null)
            return;

        for (int i = 0; i < m_stepQueue.Count; i++)
        {
            if (m_stepQueue[i].Step != After)
                continue;

            if (m_stepQueue[i].Step == Step)
            {
                if (m_stepQueue[i].Unit.Contains(Unit))
                    return;

                m_stepQueue[i].SetAdd(Unit);
            }
            else
            {
                m_stepQueue.Insert(i, new StepSingle(Step.ToString(), Start, Unit));
                m_stepQueue[i].EndStepRemove = m_stepRemove.Contains(Step);
            }

            return;
        }

        onStepAdd?.Invoke(Step);
    } //Add Step Special!!

    #endregion

    #region Step ~ Enum

    /// <summary>
    /// Add a unit to main queue before start!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Start"></param>
    /// <param name="Unit"></param>
    public void SetInit<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetInit(Step.ToString(), Mathf.Clamp(QEnum.GetChoice(Step), 0, int.MaxValue), Unit);
    } //Init on Start!!

    /// <summary>
    /// Remove a unit from main queue!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public void SetRemove<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetRemove(Step.ToString(), Unit);
    } //Remove on Destroy!!

    /// <summary>
    /// Accept a unit that completed it's MOVE, but not it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public void SetEndMove<T>(T Step, ITurnManager Unit) where T : Enum
    {
        SetEndMove(Step.ToString(), Unit);
    } //End!!

    /// <summary>
    /// Accept a unit that completed it's MOVE and it's STEP!
    /// </summary>
    /// <param name="Step"></param>
    /// <param name="Unit"></param>
    public void SetEndStep<T>(T Step, ITurnManager Unit) where T : Enum
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
    public void SetAdd<T>(T Step, ITurnManager Unit, int After = 0) where T : Enum
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
    public void SetAdd<T>(T Step, ITurnManager Unit, string After) where T : Enum
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

    private SerializedProperty m_stepCurrent;
    private SerializedProperty m_stepQueue;

    private void OnEnable()
    {
        Target = target as TurnManager;

        m_delayTurn = QUnityEditorCustom.GetField(this, "m_delayTurn");
        m_delayStep = QUnityEditorCustom.GetField(this, "m_delayStep");

        m_stepCurrent = QUnityEditorCustom.GetField(this, "m_stepCurrent");
        m_stepQueue = QUnityEditorCustom.GetField(this, "m_stepQueue");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_delayTurn);
        QUnityEditorCustom.SetField(m_delayStep);

        QUnityEditor.SetDisableGroupBegin(true);
        QUnityEditorCustom.SetField(m_stepCurrent);
        QUnityEditorCustom.SetField(m_stepQueue);
        QUnityEditor.SetDisableGroupEnd();

        QUnityEditorCustom.SetApply(this);
    }
}

#endif

//

public interface ITurnManager
{
    void ISetTurnStart(int Step);

    void ISetStepStart(string Step);

    void ISetStepEnd(string Step);

    void ISetTurnEnd(int Step);
}