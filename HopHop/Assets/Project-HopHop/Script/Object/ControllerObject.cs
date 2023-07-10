using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerObject : MonoBehaviour
{
    private bool m_turnControl = false;
    private bool m_turnDelay = false;

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
            GameEvent.onTurn += SetTurn;
        }
        else
        if (m_dataFollow != null)
        {
            GameEvent.onKeyFollow += SetKeyFollow;
        }
    }

    private void OnDestroy()
    {
        if (m_dataMove != null)
        {
            if (m_dataMove.DataExist)
            {
                GameData.m_objectTurnCount--;
                GameEvent.onTurn -= SetTurn;
            }
            else
            if (m_dataFollow != null)
            {
                GameEvent.onKeyFollow -= SetKeyFollow;
            }
        }
    }

    #region Turn

    private void SetTurn(TypeTurn Turn, bool State)
    {
        if (Turn == TypeTurn.Player && State)
            SetKeyTurn();
    }

    private void SetKeyTurn()
    {
        if (m_dataMove == null)
            return;

        IsoVector Dir = IsoVector.GetDir(m_dataMove.Data[m_dataMove.Index].Dir) * m_dataMove.Quantity;
        int Length = m_dataMove.Data[m_dataMove.Index].Length;
        SetMoveForceTurn(Dir, Length);
        SetMovePushTop(Dir, Length);
        SetMovePushSide(Dir, Length);
        SetMoveFollow(Dir, Length);

        m_dataMove.Index += m_dataMove.Quantity;
        if (m_dataMove.Loop && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.Data.Count - 1))
        {
            m_dataMove.Quantity *= -1;
            m_dataMove.Index += m_dataMove.Quantity;
        }
    }

    #endregion

    #region Key

    private void SetKeyFollow(string Value, IsoVector Dir, int Length)
    {
        if (Value != m_dataFollow)
            return;

        SetMoveForceFollow(Dir, Length);
        SetMovePushTop(Dir, Length);
        SetMovePushSide(Dir, Length);
    }

    #endregion

    #region Move This

    private void SetMoveForceTurn(IsoVector Dir, int Length)
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
                GameEvent.SetTurn(TypeTurn.Object, false);
            });
    }

    private void SetMoveForceFollow(IsoVector Dir, int Length)
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

    #endregion

    #region Move Other

    private void SetMovePushTop(IsoVector Dir, int Length)
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
                BlockBody.SetMoveForcePush(Dir, Length);
        }
    }

    private void SetMovePushSide(IsoVector Dir, int Length)
    {
        if (Dir == IsoVector.Top || Dir == IsoVector.Bot)
            return;

        List<IsometricBlock> Blocks = m_block.WorldManager.GetWorldBlockCurrentAll(m_block.Pos + Dir);

        foreach (IsometricBlock Block in Blocks)
        {
            ControllerBody BlockBody = Block.GetComponent<ControllerBody>();

            if (BlockBody == null)
                continue;

            BlockBody.SetMoveForcePush(Dir, Length);
        }
    }

    private void SetMoveFollow(IsoVector Dir, int Length)
    {
        if (m_dataFollow == null)
            return;

        GameEvent.SetKeyFollow(m_dataFollow, Dir, Length);
    }

    #endregion
}