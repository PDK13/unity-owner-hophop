using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBody : MonoBehaviour
{
    [SerializeField] private bool m_dynamic = false;

    public bool Dynamic => m_dynamic;

    public Action<bool> onMove;
    public Action<bool> onGravity;

    private bool m_checkGravity = false;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        if (m_dynamic)
            GameManager.SetObjectDelay(true);
    }

    private void OnDestroy()
    {
        if (m_dynamic)
            GameManager.SetObjectDelay(false);
    }

    #region Move

    public void SetMove(IsoVector Dir)
    {
        if (m_dynamic)
        {
            if (GetCheckDir(Dir) == null)
            {
                SetMovePush(Dir);
                SetMoveTop(Dir);
                SetMoveSide(Dir);
            }
            else
            {
                SetMovePush(IsoVector.None);
            }
        }
        else
        {
            SetMovePush(Dir);
            SetMoveTop(Dir);
            SetMoveSide(Dir);
        }
    } //Move!!

    private void SetMovePush(IsoVector Dir)
    {
        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onMove?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMove?.Invoke(false);
                if (m_dynamic)
                    SetMoveGravity();
            });
    }

    private void SetMoveTop(IsoVector Dir)
    {
        IsometricBlock BlockTop = GetCheckDir(IsoVector.Top);

        if (BlockTop == null)
            return;

        ControllerBody BlockTopBody = BlockTop.GetComponent<ControllerBody>();

        if (BlockTopBody == null)
            return;

        BlockTopBody.SetMove(Dir);
    }

    private void SetMoveSide(IsoVector Dir)
    {
        if (Dir == IsoVector.Top && Dir == IsoVector.Bot)
            return;

        IsometricBlock BlockTop = GetCheckDir(Dir);

        if (BlockTop == null)
            return;

        ControllerBody BlockTopBody = BlockTop.GetComponent<ControllerBody>();

        if (BlockTopBody == null)
            return;

        BlockTopBody.SetMove(Dir);
    }

    private void SetMoveGravity()
    {
        if (GetCheckDir(IsoVector.Bot) != null)
        {
            GameEvent.SetDelay(TypeDelay.Gravtiy, false);
            onGravity?.Invoke(false);
            return;
        }

        Vector3 MoveDir = IsoVector.GetVector(IsoVector.Bot);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onGravity?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                SetMoveGravity();
            });

        SetMoveTop(IsoVector.Bot);
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