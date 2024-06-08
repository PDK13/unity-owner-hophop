using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMoveStatic : MonoBehaviour, ITurnManager, IBodyStatic, IBodyMove, IBodyCommand
{
    #region Action

    public static Action<string, IsometricVector> onMove;

    #endregion

    #region Move

    private IsometricDataMove m_move;
    private int m_moveDurationCurrent = 0;

    private string m_moveIdentityBase;
    private string m_moveIdentityCheck;

    #endregion

    #region Command

    private List<IsometricVector> m_commandMove = new List<IsometricVector>();
    private int m_commandMoveCurrent = 0;

    #endregion

    #region Get

    public StepType Step => StepType.MoveStatic;

    public bool State => m_switch != null ? m_switch.State : true;

    public bool StepEnd => m_moveDurationCurrent >= m_move.DurationCurrent;

    public bool CommandEnd => m_commandMoveCurrent >= m_commandMove.Count - 1;

    #endregion

    #region Component

    private IsometricBlock m_block;
    private BodyStatic m_body;
    private BodyInteractiveSwitch m_switch;

    #endregion

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyStatic>();
        m_switch = GetComponent<BodyInteractiveSwitch>();
    }

    protected void Start()
    {
        m_move = GetComponent<IsometricDataMove>();
        m_moveIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.FollowIdentityBase, false);
        m_moveIdentityCheck = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.FollowIdentityCheck, false);

        if (!string.IsNullOrEmpty(m_moveIdentityCheck))
            onMove += IMoveIdentity;

        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                m_body.onMove += IMove;

                TurnManager.Instance.SetInit(Step, this);
                TurnManager.Instance.onTurnStart += ISetTurnStart;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
                TurnManager.Instance.onTurnEnd += ISetTurnEnd;
            }
        }
    }

    protected void OnDestroy()
    {
        onMove -= IMoveIdentity;

        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                m_body.onMove -= IMove;

                TurnManager.Instance.SetRemove(Step, this);
                TurnManager.Instance.onTurnStart -= ISetTurnStart;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
                TurnManager.Instance.onTurnEnd -= ISetTurnEnd;
            }
        }
    }

    #region ITurnManager

    public void ISetTurnStart(int Turn) { }

    public void ISetStepStart(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            if (!CommandEnd)
                IControl(m_commandMove[m_commandMoveCurrent]);
        }
        else
        if (Step == this.Step.ToString())
        {
            if (StepEnd)
                return;

            if (!State)
            {
                m_moveDurationCurrent = int.MaxValue;
                TurnManager.Instance.SetEndStep(this.Step, this);
            }
            else
                IControl();
        }
    }

    public void ISetStepEnd(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            m_commandMove.Clear();
            m_commandMoveCurrent = 0;
        }
    }

    public void ISetTurnEnd(int Turn)
    {
        if (State)
        {
            m_move.SetDirNext();
            m_moveDurationCurrent = 0;
        }
    }

    #endregion

    #region IBodyStatic

    public bool IControl()
    {
        IControl(m_move.DirCurrent);

        return true;
    }

    public bool IControl(IsometricVector Dir)
    {
        m_body.SetMoveControl(Dir);

        IMove(Dir);

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step == StepType.EventCommand.ToString())
        {
            if (State)
            {
                m_commandMoveCurrent++;
            }
            else
            {
                if (CommandEnd)
                    TurnManager.Instance.SetEndStep(StepType.EventCommand, this);
                else
                    TurnManager.Instance.SetEndMove(StepType.EventCommand, this);
            }
        }
        else
        if (TurnManager.Instance.StepCurrent.Step == this.Step.ToString())
        {
            if (State)
            {
                m_moveDurationCurrent++;
            }
            else
            {
                if (StepEnd)
                    TurnManager.Instance.SetEndStep(Step, this);
                else
                    TurnManager.Instance.SetEndMove(Step, this);
            }
        }
    }

    #endregion

    #region IBodyMove

    public void IMove(IsometricVector Dir)
    {
        if (string.IsNullOrEmpty(m_moveIdentityBase) || Dir == IsometricVector.None)
            return;

        onMove?.Invoke(m_moveIdentityBase, Dir);
    }

    public void IMoveIdentity(string Identity, IsometricVector Dir)
    {
        if (string.IsNullOrEmpty(m_moveIdentityCheck) || Identity != m_moveIdentityCheck)
            return;

        m_body.SetMoveControl(Dir);
    }

    #endregion

    #region IBodyCommand

    public void ISetCommandMove(IsometricVector Dir)
    {
        TurnManager.Instance.SetAdd(StepType.EventCommand, this);
        m_commandMove.Add(Dir);
    }

    #endregion

#if UNITY_EDITOR

    public void SetEditorMoveIdentityBase()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string Identity = !string.IsNullOrEmpty(m_block.Identity) ? m_block.Identity : m_block.Pos.ToString();
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase) + Identity);
    }

    public void SetEditorMoveIdentityBaseRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase)));
    }

    public bool GetEditorMoveIdentityBase()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase)));
    }

    //

    public void SetEditorMoveIdentityCheck(IsometricBlock BlockFollow)
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string Identity = !string.IsNullOrEmpty(BlockFollow.Identity) ? BlockFollow.Identity : BlockFollow.Pos.ToString();
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck) + Identity);
    }

    public void SetEditorMoveIdentityCheckRemove(IsometricBlock BlockFollow)
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string BlockCheckIdentity = !string.IsNullOrEmpty(BlockFollow.Identity) ? BlockFollow.Identity : BlockFollow.Pos.ToString();
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck) + BlockCheckIdentity));
    }

    public bool GetEditorMoveIdentityCheck(IsometricBlock BlockFollow)
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string BlockCheckIdentity = !string.IsNullOrEmpty(BlockFollow.Identity) ? BlockFollow.Identity : BlockFollow.Pos.ToString();
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck) + BlockCheckIdentity));
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyMoveStatic))]
[CanEditMultipleObjects]
public class BodyMoveStaticEditor : Editor
{
    private BodyMoveStatic m_target;

    private void OnEnable()
    {
        m_target = target as BodyMoveStatic;
        //
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //QUnityEditorCustom.SetUpdate(this);
        //
        //...
        //
        //QUnityEditorCustom.SetApply(this);
    }
}

#endif