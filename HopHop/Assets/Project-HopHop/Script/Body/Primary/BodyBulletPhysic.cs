using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BodyBulletPhysic : MonoBehaviour, ITurnManager, IBodyBullet, IBodyPhysic
{
    #region Const

    private const string ANIM_BLOW = "Blow";

    private const float DESTROY_DELAY = 0.3f;

    #endregion

    #region Move

    private IsometricVector m_turnDir;
    private int m_turnLength = 1;
    private int m_moveStep = 0;
    private int m_moveStepCurrent = 0;

    private int m_fallStep = 0;

    #endregion

    #region Get

    public StepType Step => StepType.Bullet;

    private bool StepEnd => m_moveStepCurrent == m_moveStep && m_moveStep > 0;

    private bool StepGravity => m_body.SetGravityControl();

    private bool StepForce => m_body.SetBottomControl();

    #endregion

    #region Component

    private Animator m_animator;
    private IsometricBlock m_block;
    private BodyPhysic m_body;

    #endregion

    private void OnDestroy()
    {
        TurnManager.Instance.SetRemove(Step, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;

        m_body.onGravity -= IGravity;
    }

    #region ITurnManager

    public void ISetTurn(int Turn)
    {
        m_moveStep = 0;
        m_moveStepCurrent = 0;
    }

    public void ISetStepStart(string Step)
    {
        if (Step != this.Step.ToString())
            return;

        IControl();
    }

    public void ISetStepEnd(string Step) { }

    #endregion

    #region IBodyBullet

    public void IInit(IsometricVector Dir, int Speed)
    {
        m_animator = GetComponent<Animator>();
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();

        TurnManager.Instance.SetInit(Step, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;

        m_body.onGravity += IGravity;

        m_turnDir = Dir;
        m_turnLength = Speed;
    }

    public bool IHit(IsometricBlock Target)
    {
        if (Target == null)
            return false;

        if (m_fallStep >= 10)
        {
            //...
        }

        if (Target.GetTag(KeyTag.Block))
        {
            //...
        }

        if (Target.GetTag(KeyTag.Player))
        {
            Debug.Log("[Debug] Bullet hit Player!!");
        }

        IHit();

        return true;
    } //Destroy when Hit after check Target

    public void IHit()
    {
        TurnManager.Instance.SetEndStep(Step, this);
        TurnManager.Instance.SetRemove(Step, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;

        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.World.Current.SetBlockRemoveInstant(m_block, DESTROY_DELAY);
    } //Destroy when Hit

    #endregion

    #region IBodyPhysic

    public bool IControl()
    {
        if (m_moveStep == 0)
        {
            m_moveStep = m_turnLength;
            m_moveStepCurrent = 0;
        }

        m_moveStepCurrent++;

        //NOTE: Check Move before excute Move
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + m_turnDir);
        if (IHit(Block))
            return false;
        //NOTE: Fine Move to excute Move

        IControl(m_turnDir);

        return true;
    }

    public bool IControl(IsometricVector Dir)
    {
        m_body.SetMoveControl(Dir);

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step == this.Step.ToString())
        {
            if (State)
            {
                //...
            }
            else
            {
                if (State)
                {
                    //Start Move!
                    m_moveStepCurrent++;
                }
                else
                {
                    if (StepEnd || StepGravity || StepForce)
                    {
                        //End Step!
                        TurnManager.Instance.SetEndStep(Step, this);

                        m_turnDir = IsometricVector.None;
                    }
                    else
                    {
                        //End Move!
                        TurnManager.Instance.SetEndMove(Step, this);

                        IControl();
                    }
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
            IsometricBlock Block = m_block.GetBlock(IsometricVector.Bot)[0];
            if (Block.GetTag(KeyTag.Bullet))
            {
                Block.GetComponent<IBodyBullet>().IHit(m_block);
                m_body.SetGravityControl();
                return;
            }

            IHit(Block);
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

    #region Animation

    private void SetControlAnimation(string Name)
    {
        m_animator.SetTrigger(Name);
        //m_animator.ResetTrigger(Name);
    }

    #endregion
}