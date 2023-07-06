using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjMove : MonoBehaviour
{
    private IsoDataBlockMove m_dataMove;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onKeyStart += SetKeyStart;
    }

    private void Start()
    {
        foreach (var MoveCheck in m_block.Data.MoveData)
        {
            if (MoveCheck.Name != GameKey.OBJECT)
                continue;
            m_dataMove = MoveCheck;
            GameData.m_objectControlCount++;
            return;
        }
    }

    private void OnDestroy()
    {
        GameEvent.onKeyStart -= SetKeyStart;

        if (m_dataMove != null)
            GameData.m_objectControlCount--;
    }

    private void SetKeyStart(string Key)
    {
        if (Key == GameKey.OBJECT && m_dataMove != null)
        {
            SetMoveForce(IsoVector.GetDir(m_dataMove.Data[m_dataMove.Index].Dir) * m_dataMove.Quantity, m_dataMove.Data[m_dataMove.Index].Value);
            SetMoveTop(IsoVector.GetDir(m_dataMove.Data[m_dataMove.Index].Dir) * m_dataMove.Quantity, m_dataMove.Data[m_dataMove.Index].Value);
            m_dataMove.Index += m_dataMove.Quantity;
            if (m_dataMove.Loop && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.Data.Count - 1))
            {
                m_dataMove.Quantity *= -1;
                m_dataMove.Index += m_dataMove.Quantity;
            }
            return;
        }

        foreach (var MoveCheck in m_block.Data.MoveData)
        {
            if (Key != MoveCheck.Name)
                continue;
            SetMoveForce(IsoVector.GetDir(MoveCheck.Data[MoveCheck.Index].Dir) * MoveCheck.Quantity, MoveCheck.Data[MoveCheck.Index].Value);
            SetMoveTop(IsoVector.GetDir(MoveCheck.Data[MoveCheck.Index].Dir) * MoveCheck.Quantity, MoveCheck.Data[MoveCheck.Index].Value);
            MoveCheck.Index += m_dataMove.Quantity;
            if (MoveCheck.Loop && (MoveCheck.Index < 0 || MoveCheck.Index > MoveCheck.Data.Count - 1))
            {
                MoveCheck.Quantity *= -1;
                MoveCheck.Index += m_dataMove.Quantity;
            }
        }
    }

    #region Move

    private void SetMoveForce(IsoVector Dir, int Length)
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
                GameEvent.SetKeyEnd(GameKey.OBJECT);
            });
    }

    private void SetMoveTop(IsoVector Dir, int Length)
    {
        List<IsometricBlock> Blocks = m_block.WorldManager.GetWorldBlockCurrentAll(m_block.Pos + IsoVector.Top);

        foreach(IsometricBlock Block in Blocks)
        {
            ObjBody BlockBody = Block.GetComponent<ObjBody>();

            if (BlockBody == null)
                continue;

            BlockBody.SetMovePush(Dir, Length);
        }
    }

    #endregion
}