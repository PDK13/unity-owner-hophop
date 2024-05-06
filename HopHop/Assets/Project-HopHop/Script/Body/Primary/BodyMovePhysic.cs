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

    private IsometricDataMove m_dataMove;

    #endregion

    #region Command

    private List<IsometricVector> m_commandMove;

    #endregion

    #region Get

    public bool State => m_switch != null ? m_switch.State : true;

    public StepType Step => StepType.MovePhysic;

    #endregion

    #region Component

    private IsometricBlock m_block;
    private BodyPhysic m_body;
    private BodyInteractiveSwitch m_switch;

    #endregion

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();
        m_switch = GetComponent<BodyInteractiveSwitch>();
    }

    private void Start()
    {
        m_dataMove = GetComponent<IsometricDataMove>();
        //
        if (m_dataMove != null)
        {
            if (m_dataMove.Data.Count > 0)
            {
                TurnManager.Instance.SetInit(Step, this);
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }
        //
        m_moveCheckAhead = KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.MoveCheckAheadSide);
        m_moveCheckAheadBot = KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.MoveCheckAheadBot);
        //
        m_body.onMove += IMove;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMove;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
    }

    private void OnDestroy()
    {
        if (m_dataMove != null)
        {
            if (m_dataMove.Data.Count > 0)
            {
                TurnManager.Instance.SetRemove(Step, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
            }
        }
        //
        m_body.onMove -= IMove;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMove;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;
    }

    #region ITurnManager

    public void ISetTurn(int Turn) { }

    public void ISetStepStart(string Step)
    {
        if (Step != this.Step.ToString())
            return;
        //
        if (!State)
        {
            TurnManager.Instance.SetEndStep(this.Step, this);
            return;
        }
        //
        if (!m_body.SetControlMoveForce())
        {
            if (!IControl(m_dataMove.DirCombineCurrent))
            {
                m_dataMove.SetDirRevert();
                m_dataMove.SetDirNext();
                if (!IControl(m_dataMove.DirCombineCurrent))
                {
                    m_dataMove.SetDirRevert();
                    m_dataMove.SetDirNext();

                    TurnManager.Instance.SetEndStep(this.Step, this);
                }
            }
        }
    }

    public void ISetStepEnd(string Step) { }

    #endregion

    #region IBodyPhysic

    public bool IControl(IsometricVector Dir)
    {
        if (m_dataMove.DirCombineCurrent == IsometricVector.None)
        {
            TurnManager.Instance.SetEndStep(Step, this); //Follow Enermy (!)
            return true;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length);
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");
                //
                Block.GetComponent<IBodyBullet>().IHit();
            }
            else
            if (m_moveCheckAhead)
                //Stop Ahead when there is an burden ahead!!
                return false;
            //else
            {
                //None Stop Ahead and continue check move ahead!!
                BodyPhysic BlockBody = Block.GetComponent<BodyPhysic>();
                if (BlockBody == null)
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
            IsometricBlock BlockBot = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length + IsometricVector.Bot);
            if (BlockBot == null)
                //Stop Ahead because no block ahead bot!!
                return false;
        }
        //Fine to continue move to pos ahead!!

        m_body.SetControlMove(Dir, true);

        m_dataMove.SetDirNext();

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            TurnManager.Instance.SetEndStep(Step, this);
        }
    }

    public void IForce(bool State, IsometricVector Dir)
    {
        //...
    }

    public void IGravity(bool State)
    {
        //...
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        //...
    }

    #endregion

    #region IBodyCommand

    public void ISetCommandMove(IsometricVector Dir)
    {
        //m_commandMove.Add(Dir);

        //if (!m_commandtActive)
        //{
        //    m_commandtActive = true;
        //    TurnManager.Instance.SetAdd(StepType.Event, this);
        //    m_body.SetControlMove(Dir, Gravity);
        //}
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