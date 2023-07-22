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
        GameTurn.SetInit(TypeTurn.Phase, this.gameObject);
        GameTurn.SetInit(TypeTurn.Object, this.gameObject);
        GameTurn.onTurn += SetControlTurn;
        //
        m_speed = Speed;
        m_turnDir = Dir;
        //
        m_turnControl = false;
        GameTurn.SetEndTurn(TypeTurn.Object, this.gameObject); //Follow Object (!)
    }

    private void OnDestroy()
    {
        GameTurn.SetRemove(TypeTurn.Phase, this.gameObject);
        GameTurn.SetRemove(TypeTurn.Object, this.gameObject);
        GameTurn.onTurn -= SetControlTurn;
    }

    private void SetControlTurn(string Turn)
    {
        if (Turn == TypeTurn.Phase.ToString())
        {
            //Reset!!
            m_turnLength = 0;
            m_turnLengthCurrent = 0;
            //
            m_turnControl = true;
            GameTurn.SetEndTurn(TypeTurn.Phase, this.gameObject);
        }
        else
        if (m_turnControl)
        {
            if (Turn == TypeTurn.Object.ToString())
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
        IsometricBlock Block = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + m_turnDir);
        if (Block != null)
        {
            m_turnControl = false;
            GameTurn.SetEndTurn(TypeTurn.Object, this.gameObject); //Follow Object (!)
            //
            GameTurn.SetRemove(TypeTurn.Phase, this.gameObject);
            GameTurn.SetRemove(TypeTurn.Object, this.gameObject);
            GameTurn.onTurn -= SetControlTurn;
            //
            //Can't not continue move ahead because of burden, so destroy this!!
            //
            if (Block.Tag.Contains(GameManager.GameConfig.Tag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //else
            //if (Block.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
            //{
            //    Debug.Log("[Debug] Bullet hit Player!!");
            //    //
            //    Block.GetComponent<ControllerBullet>().SetHit();
            //}
            //
            SetControlAnimation(ANIM_BLOW);
            m_block.WorldManager.SetWorldBlockRemoveInstant(m_block, DESTROY_DELAY);
            //
            return;
            //Destroy this, instead of continue move Wahead!!
        }
        //
        if (m_body != null)
            //If this got Body, then check if it will Fall ahead!!
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
                if (TurnEnd)
                {
                    m_turnControl = false;
                    GameTurn.SetEndTurn(TypeTurn.Object, this.gameObject); //Follow Object (!)
                }
                else
                    GameTurn.SetEndMove(TypeTurn.Object, this.gameObject); //Follow Object (!)
            });
    }

    public void SetHit()
    {
        m_turnControl = false;
        GameTurn.SetEndTurn(TypeTurn.Object, this.gameObject); //Follow Object (!)
        //
        GameTurn.SetRemove(TypeTurn.Phase, this.gameObject);
        GameTurn.SetRemove(TypeTurn.Object, this.gameObject);
        GameTurn.onTurn -= SetControlTurn;
        //
        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.SetWorldBlockRemoveInstant(m_block, DESTROY_DELAY);
    } //This is touched by other object!!

    #region Animation

    private void SetControlAnimation(string Name)
    {
        m_animator.SetTrigger(Name);
        //m_animator.ResetTrigger(Name);
    }

    #endregion
}