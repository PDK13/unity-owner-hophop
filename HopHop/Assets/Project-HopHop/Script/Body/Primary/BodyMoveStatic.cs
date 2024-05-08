using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMoveStatic : MonoBehaviour, ITurnManager, IBodyStatic, IBodyFollow, IBodyCommand
{
    #region Action

    public static Action<string, IsometricVector> onFollow;

    #endregion

    #region Move

    private bool m_avaibleFollow = false; //This follow current on check identity!

    private IsometricDataMove m_move;
    private string m_followIdentityBase;
    private string m_followIdentityCheck;

    private IsometricVector m_turnDir; //Dir Combine in progess!
    private int m_moveStep = 0;
    private int m_moveStepCurrent = 0;

    #endregion

    #region Command

    private List<IsometricVector> m_commandMove = new List<IsometricVector>();
    private int m_commandMoveIndex = 0;

    #endregion

    #region Get

    public bool State => m_switch != null ? m_switch.State : true;

    public StepType Step => StepType.MoveStatic;

    public bool AvaibleSwitch => m_avaibleFollow;

    private bool StepEnd => m_moveStepCurrent >= m_moveStep;

    private bool StepCommandEnd => m_commandMoveIndex >= m_commandMove.Count;

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
        m_followIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.FollowIdentityBase, false);
        m_followIdentityCheck = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.FollowIdentityCheck, false);

        m_avaibleFollow = !string.IsNullOrEmpty(m_followIdentityCheck);

        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                m_body.onMove += IMove;

                TurnManager.Instance.SetInit(Step, this);
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }

        if (m_avaibleFollow)
            onFollow += IFollowIdentity;
    }

    protected void OnDestroy()
    {
        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                m_body.onMove -= IMove;

                TurnManager.Instance.SetRemove(Step, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
            }
        }
        //
        if (m_avaibleFollow)
            onFollow -= IFollowIdentity;
    }

    #region ITurnManager

    public void ISetTurn(int Turn)
    {
        m_turnDir = m_move.DirCombineCurrent;
        m_moveStep = m_move.Data[m_move.Index].Duration;
        m_moveStep = Mathf.Clamp(m_moveStep, 1, m_moveStep); //Avoid bug by duration 0 value!
        m_moveStepCurrent = 0;
    }

    public void ISetStepStart(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            if (!StepCommandEnd)
            {
                IControl(m_commandMove[m_commandMoveIndex]);
                m_commandMoveIndex++;
            }
        }
        else
        if (Step == this.Step.ToString())
        {
            if (!State)
            {
                TurnManager.Instance.SetEndStep(this.Step, this);
                return;
            }

            if (!StepEnd)
                IControl();
        }
    }

    public void ISetStepEnd(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            m_commandMove.Clear();
            m_commandMoveIndex = 0;
        }
    }

    #endregion

    #region IBodyStatic

    public bool IControl()
    {
        IControl(m_turnDir);

        return true;
    }

    public bool IControl(IsometricVector Dir)
    {
        m_body.SetMoveControl(Dir);

        IFollow(Dir);

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step == StepType.EventCommand.ToString() && !StepCommandEnd)
        {
            if (State)
            {
                //...
            }
            else
            {
                if (StepCommandEnd)
                    TurnManager.Instance.SetEndStep(StepType.EventCommand, this);
                else
                    TurnManager.Instance.SetEndMove(StepType.EventCommand, this);
            }
        }
        else
        if (TurnManager.Instance.StepCurrent.Step == this.Step.ToString() && !StepEnd)
        {
            if (State)
            {

            }
            else
            {
                m_moveStepCurrent++;

                if (StepEnd)
                {
                    TurnManager.Instance.SetEndStep(Step, this);
                    m_move.SetDirNext();
                }
                else
                {
                    TurnManager.Instance.SetEndMove(Step, this);
                    IControl();
                }
            }
        }
    }

    #endregion

    #region IBodyFollow

    public void IFollow(IsometricVector Dir)
    {
        if (string.IsNullOrEmpty(m_followIdentityBase) || Dir == IsometricVector.None)
            return;

        onFollow?.Invoke(m_followIdentityBase, Dir);
    }

    public void IFollowIdentity(string Identity, IsometricVector Dir)
    {
        if (string.IsNullOrEmpty(m_followIdentityCheck) || Identity != m_followIdentityCheck)
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

    public void SetEditorFollowIdentityBase()
    {
        SetEditorFollowIdentityRemove();
        //
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase) + m_block.Pos.ToString());
    }

    //

    public void SetEditorFollowIdentityCheck(IsometricBlock BlockFollow)
    {
        SetEditorFollowIdentityRemove();
        //
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck) + BlockFollow.Pos.ToString());
    }

    //

    public void SetEditorFollowIdentityRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase)) || t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck)));
    }

    public bool GetEditorFollowIdentity()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase)) || t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck)));
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