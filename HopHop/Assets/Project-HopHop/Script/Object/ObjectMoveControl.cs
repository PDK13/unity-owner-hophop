using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoveControl : MonoBehaviour
{
    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onObjectTurn += SetMove;
    }

    private void OnDestroy()
    {
        GameEvent.onObjectTurn -= SetMove;
    }

    private void SetMove(string Key)
    {
        foreach(var MoveCheck in m_block.Data.MoveData)
        {
            if (Key != MoveCheck.KeyStart)
                continue;

            SetMove(MoveCheck.Data[MoveCheck.Index].Dir, MoveCheck.Data[MoveCheck.Index].Length);
        }
    } //Move!!

    private void SetMove(IsoDir Dir, int Length)
    {
        Vector3 Pos = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => Pos, x => Pos = x, Pos + IsoVector.GetVectorDir(Dir) * Length, GameData.m_timeMove).SetEase(Ease.Linear).OnUpdate(() =>
        {
            m_block.Pos = new IsoVector(Pos);
        }).OnComplete(() =>
        {
            GameEvent.SetOnControlDone(GameKey.OBJECT);
        });
    }
}