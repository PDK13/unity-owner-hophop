using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBody : MonoBehaviour
{
    private bool m_turnControl = false;

    public Action<bool> onGravity;              //State
    public Action<bool, IsometricVector> onPush;      //State, From, Dir
    public Action<bool> onForce;                //State

    [HideInInspector] public IsometricVector MoveLastXY;
    [HideInInspector] public IsometricVector? MoveForceXY;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    #region Gravity

    private void SetControlGravity(string Turn)
    {
        if (Turn != TurnType.Gravity.ToString())
        {
            m_turnControl = false;
            return;
        }
        //
        m_turnControl = true;
        //
        SetControlGravity();
    }

    public IsometricBlock SetCheckGravity(IsometricVector Dir)
    {
        IsometricBlock Block = GetCheckDir(Dir, IsometricVector.Bot);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
            {
                //Will touch OBJECT BULLET later!!
            }
            else
                //Can't not Fall ahead!!
                return Block;
        }
        //
        SetForceGravity();
        //
        return null;
    }

    private void SetForceGravity()
    {
        GameTurn.SetAdd(TurnType.Gravity, this.gameObject);
        GameTurn.Instance.onStepStart += SetControlGravity;
    }

    private void SetControlGravity()
    {
        IsometricBlock Block = GetCheckDir(IsometricVector.Bot);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                //
                Block.GetComponent<ControllerBullet>().SetHit();
            }
            else
            {
                GameTurn.SetEndTurn(TurnType.Gravity, this.gameObject); //Follow Object (!)
                GameTurn.Instance.onStepStart -= SetControlGravity;
                //
                onGravity?.Invoke(false);
                //
                m_turnControl = false;
                return;
            }
        }
        //
        Vector3 MoveDir = IsometricVector.GetVector(IsometricVector.Bot);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onGravity?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                SetControlGravity();
            });
        //
    }

    #endregion

    #region Push

    public void SetControlPush(IsometricVector Dir, IsometricVector From)
    {
        if (From == IsometricVector.Bot)
        {
            IsometricBlock BlockNext = m_block.WorldManager.WorldData.GetBlockCurrent(m_block.Pos.Fixed + Dir);
            if (BlockNext != null)
            {
                //When Block Bot end move, surely Bot of this will be emty!!
                SetForceGravity();
                return;
            } 
        }
        else
        {
            MoveLastXY = Dir;
            //
            IsometricBlock BlockNext = m_block.WorldManager.WorldData.GetBlockCurrent(m_block.Pos.Fixed + Dir);
            if (BlockNext != null)
            {
                Debug.LogError("[Debug] Push to Wall!!");
                return;
            }
            else
                //Can continue move, so check next pos if it emty at Bot?!
                SetCheckGravity(Dir);
        }
        //
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onPush?.Invoke(true, Dir);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onPush?.Invoke(false, Dir);
            });
        //
    }

    #endregion

    #region Force

    public void SetControlForce(IsometricVector Dir)
    {
        if (Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
            MoveLastXY = Dir;
        //
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onForce?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onForce?.Invoke(false);
            });
        //
    }

    #endregion

    #region Stand On Force

    public void SetStandOnForce()
    {
        if (GetCheckDir(IsometricVector.Bot) == null)
            return;
        //
        if (GetCheckDir(IsometricVector.Bot).Tag.Contains(GameManager.GameConfig.Tag.Slow))
            MoveForceXY = IsometricVector.None;
        else
        if (GetCheckDir(IsometricVector.Bot).Tag.Contains(GameManager.GameConfig.Tag.Slip))
            MoveForceXY = MoveLastXY;
        else
            MoveForceXY = null;
    }

    #endregion

    #region Check

    public IsometricBlock GetCheckDir(IsometricVector Dir)
    {
        return m_block.WorldManager.WorldData.GetBlockCurrent(m_block.Pos.Fixed + Dir);
    }

    public IsometricBlock GetCheckDir(IsometricVector Dir, IsometricVector DirNext)
    {
        return m_block.WorldManager.WorldData.GetBlockCurrent(m_block.Pos.Fixed + Dir + DirNext);
    }

    #endregion
}