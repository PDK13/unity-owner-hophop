using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BodyStatic : MonoBehaviour, ITurnManager
{
    //NOTE: Base Static controller for all Block(s)

    #region Action

    public Action<bool, IsometricVector> onMove;

    #endregion

    private IsometricBlock m_block;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        
    }

    #region Turn

    public void ISetTurn(int Turn) { }

    public void ISetStepStart(string Step) { }

    public void ISetStepEnd(string Step) { }

    #endregion

    #region Move

    private void SetControlMove(IsometricVector Dir)
    {
        Vector3 MoveDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onMove?.Invoke(true, Dir);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMove?.Invoke(false, Dir);
            });

        SetControlPush(Dir);

        SetControlTop(Dir);
    } //Move Invoke!

    #endregion

    #region Push

    private void SetControlPush(IsometricVector Dir)
    {
        if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
        {
            return;
        }
        //
        IsometricBlock BlockPush = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir);
        if (BlockPush != null)
        {
            BodyPhysic BodyPush = BlockPush.GetComponent<BodyPhysic>();
            if (BodyPush != null)
            {
                BodyPush.SetControlPush(Dir, Dir * -1); //Push!!
            }
        }
    }

    #endregion

    #region Top

    private void SetControlTop(IsometricVector Dir)
    {
        //Top!!
        IsometricBlock BlockTop = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + IsometricVector.Top);
        if (BlockTop != null)
        {
            BodyPhysic BodyTop = BlockTop.GetComponent<BodyPhysic>();
            if (BodyTop != null)
            {
                if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
                {
                    BodyTop.SetControlForce(Dir); //Force!!
                }
                else
                {
                    BodyTop.SetControlPush(Dir, IsometricVector.Bot); //Push!!
                }
            }
        }
    }

    #endregion
}