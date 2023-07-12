using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBody : MonoBehaviour
{
    private bool m_turnControl = false;

    [SerializeField] private bool m_dynamic = false;

    public bool Dynamic => m_dynamic;

    public Action<bool> onMove;
    public Action<bool> onGravity;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    #region Gravity

    private void SetControlGravity(TypeTurn Turn)
    {
        if (Turn != TypeTurn.Gravity)
        {
            m_turnControl = false;
            return;
        }
        //
        m_turnControl = true;
        //
        SetControlGravity();
    }

    public void SetCheckGravity(IsoVector Dir)
    {
        if (GetCheckDir(Dir, IsoVector.Bot) == null)
        {
            GameTurn.SetAdd(TypeTurn.Gravity);
            GameTurn.onTurn += SetControlGravity;
        }
    }

    private void SetControlGravity()
    {
        if (GetCheckDir(IsoVector.Bot) != null)
        {
            //End Animation!!
            //
            GameTurn.SetEndTurn(TypeTurn.Gravity); //Follow Object (!)
            GameTurn.onTurn -= SetControlGravity;
            //
            onGravity?.Invoke(false);
            //
            m_turnControl = false;
            return;
        }

        Vector3 MoveDir = IsoVector.GetVector(IsoVector.Bot);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
                //
                onGravity?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                //GameTurn.SetEndMove(TypeTurn.Object); //Follow Object (!)
                SetControlGravity();
            });
    }

    #endregion

    #region Push

    public void SetControlPush(IsoVector Dir, IsoVector From)
    {
        SetCheckGravity(Dir);
        //
        IsometricBlock BlockNext = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + Dir);
        if (BlockNext != null)
        {
            if (From == IsoVector.Bot)
                return;
            else
                Debug.LogError("[Debug] Push to Wall!!");
        }
        //
        Vector3 MoveDir = IsoVector.GetVector(Dir);
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
            });
        //
    }

    #endregion

    #region Force

    public void SetControlForce(IsoVector Dir)
    {
        //
        Vector3 MoveDir = IsoVector.GetVector(Dir);
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
            });
        //
    }

    #endregion

    #region Check

    public IsometricBlock GetCheckDir(IsoVector Dir)
    {
        return m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + Dir);
    }

    public IsometricBlock GetCheckDir(IsoVector Dir, IsoVector DirNext)
    {
        return m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + Dir + DirNext);
    }

    #endregion
}