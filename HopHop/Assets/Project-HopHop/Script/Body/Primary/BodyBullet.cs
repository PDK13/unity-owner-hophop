using DG.Tweening;
using UnityEngine;

public class BodyBullet : MonoBehaviour, ITurnManager, IBodyBullet
{
    #region Const

    private const string ANIM_BLOW = "Blow";

    private const float DESTROY_DELAY = 0.3f;

    #endregion

    #region Move

    private IsometricVector m_moveDir;
    private int m_moveLength = 1;
    private int m_moveStep = 0;
    private int m_moveStepCurrent = 0;

    #endregion

    #region Get

    public StepType Step => StepType.Bullet;

    private bool StepEnd => m_moveStepCurrent == m_moveStep && m_moveStep > 0;

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
        //
        if (m_body != null)
            m_body.onGravity -= SetGravity;
    }

    #region ITurnManager

    public void ISetTurn(int Turn)
    {
        //Reset!!
        m_moveStep = 0;
        m_moveStepCurrent = 0;
    }

    public void ISetStepStart(string Step)
    {
        if (Step != this.Step.ToString())
            return;
        //
        SetControlMove();
    }

    public void ISetStepEnd(string Step) { }

    #endregion

    #region IBodyBullet

    public void IInit(IsometricVector Dir, int Speed)
    {
        m_animator = GetComponent<Animator>();
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();
        //
        TurnManager.Instance.SetInit(Step, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
        //
        if (m_body != null)
            m_body.onGravity += SetGravity;
        //
        m_moveLength = Speed;
        m_moveDir = Dir;
    }

    public void IHit()
    {
        TurnManager.Instance.SetEndStep(Step, this);
        TurnManager.Instance.SetRemove(Step, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
        //
        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.World.Current.SetBlockRemoveInstant(m_block, DESTROY_DELAY);
    }

    #endregion

    #region Move

    private void SetControlMove()
    {
        if (m_moveStep == 0)
        {
            m_moveStep = m_moveLength;
            m_moveStepCurrent = 0;
        }
        //
        m_moveStepCurrent++;
        //
        IsometricBlock BlockAhead = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + m_moveDir);
        if (BlockAhead != null)
        {
            if (BlockAhead.GetTag(KeyTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            else
            if (BlockAhead.GetTag(KeyTag.Enermy))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");
            }
            //
            IHit();
            //
            return;
        }
        //
        if (m_body != null)
        {
            m_body.SetGravityControl(m_moveDir);
        }
        //
        Vector3 MoveVectorDir = IsometricVector.GetDirVector(m_moveDir);
        Vector3 MoveVectorStart = IsometricVector.GetDirVector(m_block.Pos);
        Vector3 MoveVectorEnd = IsometricVector.GetDirVector(m_block.Pos) + MoveVectorDir * 1;
        DOTween.To(() => MoveVectorStart, x => MoveVectorEnd = x, MoveVectorEnd, GameManager.Instance.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveVectorEnd);
            })
            .OnComplete(() =>
            {
                //End Animation!!
                //
                if (StepEnd)
                {
                    TurnManager.Instance.SetEndStep(Step, this);
                }
                else
                {
                    TurnManager.Instance.SetEndMove(Step, this);
                }
                //
                //Check if Bot can't stand on!!
                //
                SetStandOn();
                //
            });
    }

    private void SetStandOn()
    {
        if (m_body != null)
        {
            IsometricBlock BlockBot = m_block.GetBlock(IsometricVector.Bot)[0];
            if (BlockBot == null)
            {
                return;
            }
            //
            if (BlockBot.GetTag(KeyTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                IHit();
            }
            //
            if (!BlockBot.GetTag(KeyTag.Block))
            {
                IHit();
            }
        }
    }

    private void SetGravity(bool State)
    {
        if (!State)
        {
            SetStandOn();
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