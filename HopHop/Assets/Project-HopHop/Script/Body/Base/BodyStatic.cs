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

    public void SetControlMove(IsometricVector Dir)
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
        SetPushNext(Dir);
        SetTopNext(Dir);
    } //Move Invoke!

    #endregion

    #region Push

    private void SetPushNext(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None || Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
            return;

        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir);
        if (Block != null)
        {
            BodyPhysic BodyPhysic = Block.GetComponent<BodyPhysic>();
            if (BodyPhysic != null)
            {
                BodyPhysic.SetPushControl(Dir, Dir * -1); //Push!!
            }
        }
    }

    #endregion

    #region Top

    private void SetTopNext(IsometricVector Dir)
    {
        //Top!!
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + IsometricVector.Top);
        if (Block != null)
        {
            BodyPhysic BodyPhysic = Block.GetComponent<BodyPhysic>();
            if (BodyPhysic != null)
            {
                if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
                    BodyPhysic.SetForceControl(Dir); //Force!!
                else
                    BodyPhysic.SetPushControl(Dir, IsometricVector.Bot); //Push!!
            }
        }
    }

    #endregion
}