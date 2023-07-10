using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerObject : MonoBehaviour
{
    private IsoDataBlockMove m_dataMove;
    private string m_dataFollow;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_dataMove = m_block.Data.MoveData;
        m_dataFollow = m_block.Data.EventData.DataExist ? m_block.Data.EventData.Data.Find(t => t.Name == ConstGameKey.EVENT_FOLLOW).Value : null;

        if (m_dataMove.DataExist)
        {
            GameData.m_objectTurnCount++;
            GameEvent.onKey += SetKey;
        }
        else
        if (m_dataFollow != null)
        {
            GameEvent.onKeyFollow += SetKeyFollow;
        }
    }

    private void OnDestroy()
    {
        GameEvent.onKey -= SetKey;

        if (m_dataMove.DataExist)
        {
            GameData.m_objectTurnCount--;
            GameEvent.onKey -= SetKey;
        }
        else
        if (m_dataFollow != null)
        {
            GameEvent.onKeyFollow -= SetKeyFollow;
        }
    }

    private void SetKey(string Key, bool State)
    {
        if (!State)
            return;

        if (Key == ConstGameKey.TURN_OBJECT)
            SetKeyTurn();
    }

    private void SetKeyTurn()
    {
        if (m_dataMove == null)
            return;

        IsoVector Dir = IsoVector.GetDir(m_dataMove.Data[m_dataMove.Index].Dir) * m_dataMove.Quantity;
        int Length = m_dataMove.Data[m_dataMove.Index].Length;
        SetMoveTurn(Dir, Length);
        SetMoveForceTop(Dir, Length);
        SetMoveForceSide(Dir, Length);
        SetMoveForceFollow(Dir, Length);

        m_dataMove.Index += m_dataMove.Quantity;
        if (m_dataMove.Loop && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.Data.Count - 1))
        {
            m_dataMove.Quantity *= -1;
            m_dataMove.Index += m_dataMove.Quantity;
        }
    }

    private void SetKeyFollow(string Value, IsoVector Dir, int Length)
    {
        if (Value != m_dataFollow)
            return;

        SetMoveFollow(Dir, Length);
        SetMoveForceTop(Dir, Length);
        SetMoveForceSide(Dir, Length);
    }

    #region Move

    private void SetMoveTurn(IsoVector Dir, int Length)
    {
        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * Length;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                GameEvent.SetKey(ConstGameKey.TURN_OBJECT, false);
            });
    }

    private void SetMoveFollow(IsoVector Dir, int Length)
    {
        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * Length;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            });
    }

    private void SetMoveForceTop(IsoVector Dir, int Length)
    {
        List<IsometricBlock> Blocks = m_block.WorldManager.GetWorldBlockCurrentAll(m_block.Pos + IsoVector.Top);

        foreach(IsometricBlock Block in Blocks)
        {
            ControllerBody BlockBody = Block.GetComponent<ControllerBody>();

            if (BlockBody == null)
                continue;

            if (Dir == IsoVector.Bot || Dir == IsoVector.Top)
                BlockBody.SetMoveForce(Dir, Length);
            else
                BlockBody.SetMovePush(Dir, Length);
        }
    }

    private void SetMoveForceSide(IsoVector Dir, int Length)
    {
        if (Dir == IsoVector.Top || Dir == IsoVector.Bot)
            return;

        List<IsometricBlock> Blocks = m_block.WorldManager.GetWorldBlockCurrentAll(m_block.Pos + Dir);

        foreach (IsometricBlock Block in Blocks)
        {
            ControllerBody BlockBody = Block.GetComponent<ControllerBody>();

            if (BlockBody == null)
                continue;

            BlockBody.SetMovePush(Dir, Length);
        }
    }

    private void SetMoveForceFollow(IsoVector Dir, int Length)
    {
        if (m_dataFollow == null)
            return;

        GameEvent.SetKeyFollow(m_dataFollow, Dir, Length);
    }

    #endregion
}