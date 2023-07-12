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

    #region Gravity

    private void SetControlGravity()
    {
        if (GetCheckDir(IsoVector.Bot) != null)
        {
            //GameEvent.SetDelay(TypeDelay.Gravtiy, false);
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
                //SetControlGravity();
            });
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