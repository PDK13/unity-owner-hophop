using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBullet : MonoBehaviour
{
    private const string ANIM_BLOW = "Blow";

    private const float DESTROY_DELAY = 0.3f;

    private int m_speed = 1;

    private bool m_turnControl = false;

    private IsoVector m_turnDir;
    private int m_turnLength = 0;
    private int m_turnLengthCurrent = 0;

    private bool TurnEnd => m_turnLengthCurrent == m_turnLength && m_turnLength != 0;

    private Animator m_animator;
    private ControllerBody m_body;
    private IsometricBlock m_block;

    public void SetInit(IsoVector Dir, int Speed)
    {
        m_animator = GetComponent<Animator>();
        m_body = GetComponent<ControllerBody>();
        m_block = GetComponent<IsometricBlock>();
        //
        GameTurn.SetInit(TurnType.Phase, this.gameObject);
        GameTurn.SetInit(TurnType.Object, this.gameObject);
        GameTurn.Instance.onStepStart += SetControlTurn;
        //
        if (m_body != null)
        {
            m_body.onGravity += SetGravity;
        }
        //
        m_speed = Speed;
        m_turnDir = Dir;
        //
        m_turnControl = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        //
        GameTurn.SetRemove(TurnType.Phase, this.gameObject);
        GameTurn.SetRemove(TurnType.Object, this.gameObject);
        GameTurn.Instance.onStepStart -= SetControlTurn;
        //
        if (m_body != null)
        {
            m_body.onGravity -= SetGravity;
        }
    }

    private void SetControlTurn(string Turn)
    {
        if (Turn == TurnType.Phase.ToString())
        {
            //Reset!!
            m_turnLength = 0;
            m_turnLengthCurrent = 0;
            //
            m_turnControl = true;
            GameTurn.SetEndTurn(TurnType.Phase, this.gameObject);
        }
        else
        if (m_turnControl)
        {
            if (Turn == TurnType.Object.ToString())
            {
                SetControlMove();
            }
        }
    }

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
        IsometricBlock BlockAhead = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + m_turnDir);
        if (BlockAhead != null)
        {
            if (BlockAhead.Tag.Contains(GameManager.GameConfig.Tag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //
            SetHit();
            //
            return;
        }
        //
        if (m_body != null)
            m_body.SetCheckGravity(m_turnDir);
        //
        Vector3 MoveDir = IsoVector.GetVector(m_turnDir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                //End Animation!!
                //
                if (TurnEnd)
                {
                    m_turnControl = false;
                    GameTurn.SetEndTurn(TurnType.Object, this.gameObject); //Follow Object (!)
                }
                else
                    GameTurn.SetEndMove(TurnType.Object, this.gameObject); //Follow Object (!)
                //
                //Check if Bot can't stand on!!
                //
                SetStandOn();
                //
            });
    }

    public void SetHit()
    {
        m_turnControl = false;
        GameTurn.SetEndTurn(TurnType.Object, this.gameObject); //Follow Object (!)
        GameTurn.SetRemove(TurnType.Phase, this.gameObject);
        GameTurn.SetRemove(TurnType.Object, this.gameObject);
        GameTurn.Instance.onStepStart -= SetControlTurn;
        //
        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.SetWorldBlockRemoveInstant(m_block, DESTROY_DELAY);
    } //This is touched by other object!!

    public void SetStandOn()
    {
        if (m_body != null)
        {
            IsometricBlock BlockBot = m_body.GetCheckDir(IsoVector.Bot);
            if (BlockBot == null)
                return;
            //
            if (BlockBot.Tag.Contains(GameManager.GameConfig.Tag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //
            if (!BlockBot.Tag.Contains(GameManager.GameConfig.Tag.Block))
                SetHit();
        }
    }

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