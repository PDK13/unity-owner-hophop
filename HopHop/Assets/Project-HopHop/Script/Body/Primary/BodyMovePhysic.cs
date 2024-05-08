using UnityEngine;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMovePhysic : MonoBehaviour, ITurnManager, IBodyPhysic, IBodyCommand
{
    #region Move

    private bool m_moveCheckAhead = false;
    private bool m_moveCheckAheadBot = false;

    private IsometricVector m_turnDir; //Dir Combine in progess!
    private int m_moveStep = 0;
    private int m_moveStepCurrent = 0;

    private int m_fallStep = 0;

    private IsometricDataMove m_move;

    #endregion

    #region Command

    private List<IsometricVector> m_commandMove = new List<IsometricVector>();
    private int m_commandMoveIndex = 0;

    #endregion

    #region Get

    public StepType Step => StepType.MovePhysic;

    public bool State => m_switch != null ? m_switch.State : true;

    private bool StepEnd => m_moveStepCurrent >= m_moveStep && m_moveStep > 0;

    private bool StepCommandEnd => m_commandMoveIndex >= m_commandMove.Count;

    private bool StepGravity => StepEnd || (m_character != null ? !m_character.MoveFloat : true) ? m_body.SetGravityControl() : false;

    private bool StepForce => m_body.SetBottomControl();

    #endregion

    #region Component

    private IsometricBlock m_block;
    private BodyPhysic m_body;
    private BodyInteractiveSwitch m_switch;
    private BodyCharacter m_character;

    #endregion

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();
        m_character = GetComponent<BodyCharacter>();
        m_switch = GetComponent<BodyInteractiveSwitch>();
    }

    private void Start()
    {
        m_move = GetComponent<IsometricDataMove>();

        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                TurnManager.Instance.SetInit(Step, this);
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }

        m_moveCheckAhead = KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.MoveCheckAheadSide);
        m_moveCheckAheadBot = KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.MoveCheckAheadBot);

        m_body.onMove += IMove;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMove;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
    }

    private void OnDestroy()
    {
        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                TurnManager.Instance.SetRemove(Step, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
            }
        }

        m_body.onMove -= IMove;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMove;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;
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
                IControl(m_commandMove[m_commandMoveIndex]);
        }
        else
        if (Step == this.Step.ToString())
        {
            if (!State)
            {
                TurnManager.Instance.SetEndStep(this.Step, this);
                return;
            }

            if (!m_body.SetMoveControlForce())
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

    #region IBodyPhysic

    public bool IControl()
    {
        if (IControl(m_turnDir))
            return true;

        m_move.SetDirRevert();
        m_move.SetDirNext();

        m_turnDir = m_move.DirCombineCurrent;

        if (IControl(m_turnDir))
            return true;

        m_move.SetDirRevert();
        m_move.SetDirNext();

        TurnManager.Instance.SetEndStep(this.Step, this);

        return false;
    }

    public bool IControl(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
        {
            m_body.SetMoveControlReset();
            TurnManager.Instance.SetEndStep(Step, this);
            return true;
        }

        //NOTE: Check Move before excute Move
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir);
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");

                Block.GetComponent<IBodyBullet>().IHit();
            }
            else
            if (m_moveCheckAhead)
                //Stop Ahead when there is an burden ahead!!
                return false;
            //else
            {
                //None Stop Ahead and continue check move ahead!!
                BodyPhysic BodyPhysic = Block.GetComponent<BodyPhysic>();
                if (BodyPhysic == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return false;
                }
                //Fine to continue push this Block ahead!!
            }
        }
        else
        if (m_moveCheckAheadBot)
        {
            //Continue check move Ahead Bot!!
            IsometricBlock BlockBot = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir + IsometricVector.Bot);
            if (BlockBot == null)
                //Stop Ahead because no block ahead bot!!
                return false;
        }
        //NOTE: Fine Move to excute Move

        m_body.SetMoveControl(Dir);

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
                m_commandMoveIndex++;

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
                //...
            }
            else
            {
                m_moveStepCurrent++;

                bool End = StepEnd;
                bool Gravity = StepGravity;
                bool Force = StepForce;
                if (End || Gravity || Force)
                {
                    TurnManager.Instance.SetEndStep(Step, this);
                    m_move.SetDirNext();
                }
                else
                {
                    TurnManager.Instance.SetEndMove(Step, this);
                    IControl(m_body.MoveLastXY);
                }
            }
        }
    }

    public void IForce(bool State, IsometricVector Dir, IsometricVector From)
    {
        if (State)
        {

        }
        else
        {
            m_body.SetGravityControl();
            m_body.SetBottomControl();
        }
    }

    public void IGravity(bool State)
    {
        if (State)
        {
            m_fallStep++;
        }
        else
        {
            if (m_fallStep >= 10)
            {
                //...
            }

            m_body.SetBottomControl();

            m_fallStep = 0;
        }
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        if (State)
        {

        }
        else
        {
            m_body.SetGravityControl();
            m_body.SetBottomControl();
        }
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

    public void SetEditorMoveCheckAheadSide()
    {
        m_block ??= QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadSide));
    }

    public void SetEditorMoveCheckAheadSideRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadSide)));
    }

    public bool GetEditorMoveCheckAheadSide()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadSide));
    }

    //

    public void SetEditorMoveCheckAheadBot()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadBot));
    }

    public void SetEditorMoveCheckAheadBotRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadBot)));
    }

    public bool GetEditorMoveCheckAheadBot()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadBot));
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyMovePhysic))]
[CanEditMultipleObjects]
public class BodyEnermyMoveEditor : Editor
{
    private BodyMovePhysic m_target;

    private void OnEnable()
    {
        m_target = target as BodyMovePhysic;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //
        //QUnityEditorCustom.SetUpdate(this);
        //
        //...
        //
        //QUnityEditorCustom.SetApply(this);
    }
}

#endif