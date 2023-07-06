using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjBody : MonoBehaviour
{
    public Action<bool> onMove;
    public Action<bool> onGravity;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    #region Move

    public void SetMoveForce(IsoVector Dir, int Length)
    {
        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * Length;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnStart(()=> 
            {
                onMove?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(()=> 
            {
                onMove?.Invoke(false);
            });
    }

    public void SetMovePush(IsoVector Dir, int Length)
    {
        SetMoveForce(Dir, GetCheckPush(Dir, Length));
    }

    #endregion

    #region Gravity

    public void SetGravity()
    {
        if (GetCheckBot())
        {
            onGravity?.Invoke(false);
            return;
        }

        Vector3 MoveDir = IsoVector.GetVector(IsoVector.Bot);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameData.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(()=> 
            {
                onGravity?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                SetGravity();
            });
    }

    #endregion

    #region Check

    public bool GetCheckTop()
    {
        return m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.Top);
    }

    public bool GetCheckBot()
    {
        return m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.Bot);
    }

    public bool GetCheckPush(IsoVector Dir)
    {
        return GetCheckPush(Dir, 1) == 1;
    }

    public int GetCheckPush(IsoVector Dir, int Length)
    {
        for (int LengthCheck = 1; LengthCheck <= Length; LengthCheck++)
        {
            IsometricBlock Block = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + Dir * LengthCheck);

            if (Block == null)
                continue;

            Length = LengthCheck - 1;
        }
        return Length;
    }

    #endregion
}