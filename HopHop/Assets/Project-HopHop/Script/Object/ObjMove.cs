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
            SetMove(m_dataMove.Data[m_dataMove.Index].Dir, m_dataMove.Data[m_dataMove.Index].Value, m_dataMove.Dir == -1);
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
            SetMove(MoveCheck.Data[MoveCheck.Index].Dir, MoveCheck.Data[MoveCheck.Index].Value, MoveCheck.Dir == -1);
            MoveCheck.Index += m_dataMove.Dir;
            if (MoveCheck.Loop && (MoveCheck.Index < 0 || MoveCheck.Index > MoveCheck.Data.Count - 1))
            {
                MoveCheck.Dir *= -1;
                MoveCheck.Index += m_dataMove.Dir;
            }
        }
    }

    private void SetMove(IsoDir Dir, int Length, bool Revert)
    {
        Vector3 PosMove = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => PosMove, x => PosMove = x, PosMove + IsoVector.GetVectorDir(Dir, Revert) * Length, GameData.TimeMove)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                GameEvent.SetForceMove(m_block.Pos + IsoVector.Top, Dir, Length, Revert);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(PosMove);
            })
            .OnComplete(() =>
            {
                GameEvent.SetKeyEnd(GameKey.OBJECT);
            });
    } //Move!!
}