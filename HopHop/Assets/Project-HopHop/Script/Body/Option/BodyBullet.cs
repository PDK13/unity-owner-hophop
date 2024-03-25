using DG.Tweening;
using UnityEngine;
using System;

public class BodyBullet : MonoBehaviour, ITurnManager
{
    private const string ANIM_BLOW = "Blow";

    private const float DESTROY_DELAY = 0.3f;

    private int m_speed = 1;

    private bool m_turnActive = false;

    private IsometricVector m_turnDir;
    private int m_turnLength = 0;
    private int m_turnLengthCurrent = 0;

    private bool TurnEnd => m_turnLengthCurrent == m_turnLength && m_turnLength != 0;

    private Animator m_animator;
    private BodyPhysic m_body;
    private IsometricBlock m_block;

    public void SetInit(IsometricVector Dir, int Speed)
    {
        m_animator = GetComponent<Animator>();
        m_body = GetComponent<BodyPhysic>();
        m_block = GetComponent<IsometricBlock>();
        //
        TurnManager.SetInit(TurnType.Bullet, this);
        TurnManager.Instance.onTurn += ITurn;
        TurnManager.Instance.onStepStart += IStepStart;
        TurnManager.Instance.onStepEnd += IStepEnd;
        //
        if (m_body != null)
            m_body.onGravity += SetGravity;
        //
        m_speed = Speed;
        m_turnDir = Dir;
        //
        m_turnActive = false;
    }

    private void OnDestroy()
    {
        TurnManager.SetRemove(TurnType.Bullet, this);
        TurnManager.Instance.onTurn -= ITurn;
        TurnManager.Instance.onStepStart -= IStepStart;
        TurnManager.Instance.onStepEnd -= IStepEnd;
        //
        if (m_body != null)
            m_body.onGravity -= SetGravity;
    }

    #region Turn

    public bool TurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public void ITurn(int Turn)
    {
        //Reset!!
        m_turnLength = 0;
        m_turnLengthCurrent = 0;
        //
        m_turnActive = true;
    }

    public void IStepStart(string Step)
    {
        if (!m_turnActive)
            return;
        //
        if (Step != TurnType.Bullet.ToString())
            return;
        //
        SetControlMove();
    }

    public void IStepEnd(string Step) { }

    #endregion

    #region Move

    private void SetControlMove()
    {
        if (m_turnLength == 0)
        {
            m_turnLength = m_speed;
            m_turnLengthCurrent = 0;
        }
        //
        m_turnLengthCurrent++;
        //
        IsometricBlock BlockAhead = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + m_turnDir);
        if (BlockAhead != null)
        {
            if (BlockAhead.Tag.Contains(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            else
            if (BlockAhead.Tag.Contains(GameConfigTag.Enermy))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");
            }
            //
            SetHit();
            //
            return;
        }
        //
        if (m_body != null)
        {
            m_body.SetCheckGravity(m_turnDir);
        }
        //
        Vector3 MoveDir = IsometricVector.GetVector(m_turnDir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                //End Animation!!
                //
                if (TurnEnd)
                {
                    m_turnActive = false;
                    TurnManager.SetEndStep(TurnType.Bullet, this);
                }
                else
                {
                    TurnManager.SetEndMove(TurnType.Bullet, this);
                }
                //
                //Check if Bot can't stand on!!
                //
                SetStandOn();
                //
            });
    }

    public void SetHit()
    {
        m_turnActive = false;
        TurnManager.SetEndStep(TurnType.Bullet, this);
        TurnManager.SetRemove(TurnType.Bullet, this);
        TurnManager.Instance.onTurn -= ITurn;
        TurnManager.Instance.onStepStart -= IStepStart;
        TurnManager.Instance.onStepEnd -= IStepEnd;
        //
        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.World.Current.SetBlockRemoveInstant(m_block, DESTROY_DELAY);
    } //This is touched by other object!!

    public void SetStandOn()
    {
        if (m_body != null)
        {
            IsometricBlock BlockBot = m_body.GetCheckDir(IsometricVector.Bot);
            if (BlockBot == null)
            {
                return;
            }
            //
            if (BlockBot.Tag.Contains(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //
            if (!BlockBot.Tag.Contains(GameConfigTag.Block))
            {
                SetHit();
            }
        }
    }

    #endregion

    #region Body

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