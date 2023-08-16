using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBody : MonoBehaviour
{
    private bool m_turnControl = false;

    public Action<bool> onGravity;              //State
    public Action<bool, IsoVector> onPush;      //State, From, Dir
    public Action<bool> onForce;                //State

    [HideInInspector] public IsoVector MoveLastXY;
    [HideInInspector] public IsoVector? MoveForceXY;

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

    public IsometricBlock SetCheckGravity(IsoVector Dir)
    {
        IsometricBlock Block = GetCheckDir(Dir, IsoVector.Bot);
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
        IsometricBlock Block = GetCheckDir(IsoVector.Bot);
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
        Vector3 MoveDir = IsoVector.GetVector(IsoVector.Bot);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
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
                SetControlGravity();
            });
        //
    }

    #endregion

    #region Push

    public void SetControlPush(IsoVector Dir, IsoVector From)
    {
        if (From == IsoVector.Bot)
        {
            IsometricBlock BlockNext = m_block.WorldManager.WorldData.GetWorldBlockCurrent(m_block.Pos.Fixed + Dir);
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
            IsometricBlock BlockNext = m_block.WorldManager.WorldData.GetWorldBlockCurrent(m_block.Pos.Fixed + Dir);
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
        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onPush?.Invoke(true, Dir);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onPush?.Invoke(false, Dir);
            });
        //
    }

    #endregion

    #region Force

    public void SetControlForce(IsoVector Dir)
    {
        if (Dir != IsoVector.Top && Dir != IsoVector.Bot)
            MoveLastXY = Dir;
        //
        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onForce?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
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
        if (GetCheckDir(IsoVector.Bot) == null)
            return;
        //
        if (GetCheckDir(IsoVector.Bot).Tag.Contains(GameManager.GameConfig.Tag.Slow))
            MoveForceXY = IsoVector.None;
        else
        if (GetCheckDir(IsoVector.Bot).Tag.Contains(GameManager.GameConfig.Tag.Slip))
            MoveForceXY = MoveLastXY;
        else
            MoveForceXY = null;
    }

    #endregion

    #region Check

    public IsometricBlock GetCheckDir(IsoVector Dir)
    {
        return m_block.WorldManager.WorldData.GetWorldBlockCurrent(m_block.Pos.Fixed + Dir);
    }

    public IsometricBlock GetCheckDir(IsoVector Dir, IsoVector DirNext)
    {
        return m_block.WorldManager.WorldData.GetWorldBlockCurrent(m_block.Pos.Fixed + Dir + DirNext);
    }

    #endregion
}