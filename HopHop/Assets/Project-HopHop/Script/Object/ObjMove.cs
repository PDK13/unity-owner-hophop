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
            Vector3Int MoveDir = IsoVector.GetVector(m_dataMove.Data[m_dataMove.Index].Dir);
            SetMove(MoveDir * m_dataMove.Dir, m_dataMove.Data[m_dataMove.Index].Value);
            m_dataMove.Index += m_dataMove.Dir;
            if (m_dataMove.Loop && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.Data.Count - 1))
            {
                m_dataMove.Dir *= -1;
                m_dataMove.Index += m_dataMove.Dir;
            }
            return;
        }

        foreach (var MoveCheck in m_block.Data.MoveData)
        {
            if (Key != MoveCheck.Name)
                continue;
            Vector3Int MoveDir = IsoVector.GetVector(MoveCheck.Data[MoveCheck.Index].Dir);
            SetMove(MoveDir * MoveCheck.Dir, MoveCheck.Data[MoveCheck.Index].Value);
            MoveCheck.Index += m_dataMove.Dir;
            if (MoveCheck.Loop && (MoveCheck.Index < 0 || MoveCheck.Index > MoveCheck.Data.Count - 1))
            {
                MoveCheck.Dir *= -1;
                MoveCheck.Index += m_dataMove.Dir;
            }
        }
    }

    private void SetMove(Vector3Int Dir, int Length)
    {
        Vector3 PosStart = IsoVector.GetVector(m_block.Pos);
        Vector3 PosEnd = IsoVector.GetVector(m_block.Pos) + Dir * Length;
        DOTween.To(() => PosStart, x => PosEnd = x, PosEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                GameEvent.SetMoveFollow(m_block.Pos + IsoVector.Top, Dir, Length);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(PosEnd);
            })
            .OnComplete(() =>
            {
                GameEvent.SetKeyEnd(GameKey.OBJECT);
            });
    } //Move!!
}