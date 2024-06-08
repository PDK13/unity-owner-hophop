using DG.Tweening;
using System;
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

    public void ISetTurnStart(int Turn) { }

    public void ISetStepStart(string Step) { }

    public void ISetStepEnd(string Step) { }

    public void ISetTurnEnd(int Turn) { }

    #endregion

    #region Move

    public void SetMoveControl(IsometricVector Dir)
    {
        Vector3 MoveDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMove?.Invoke(false, Dir);
            });
        onMove?.Invoke(true, Dir);

        SetPush(Dir);
        SetForceTop(Dir);
    } //Move Invoke!

    #endregion

    #region Push (Side)

    private void SetPush(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None || Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
            return;

        var Block = m_block.GetBlockAll(Dir);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            BodyPhysic BodyPhysic = BlockCheck.GetComponent<BodyPhysic>();
            if (BodyPhysic != null)
                BodyPhysic.SetPushControl(Dir, Dir * -1);
        }
    }

    #endregion

    #region Force (Top & Bot)

    private void SetForceTop(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;

        var Block = m_block.GetBlockAll(IsometricVector.Top);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            BodyPhysic BodyPhysic = BlockCheck.GetComponent<BodyPhysic>();
            if (BodyPhysic != null ? BodyPhysic.Gravity : false)
                BodyPhysic.SetForceControl(Dir, IsometricVector.Bot, Dir != IsometricVector.Bot);
        }
    }

    private void SetForceBot(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;

        var Block = m_block.GetBlockAll(IsometricVector.Bot);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            BodyPhysic BodyPhysic = BlockCheck.GetComponent<BodyPhysic>();
            if (BodyPhysic != null ? BodyPhysic.Gravity : false)
                BodyPhysic.SetForceControl(Dir, IsometricVector.Top, Dir != IsometricVector.Top);
        }
    }

    #endregion
}