using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectPlayerControl : MonoBehaviour
{
    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onPlayerMove += SetMove;
    }

    private void OnDestroy()
    {
        GameEvent.onPlayerMove -= SetMove;
    }

    private void SetMove(IsoDir Dir)
    {
        int Length = 1; //Follow Character

        if (m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.GetDir(Dir) * Length) != null)
            return;

        GameEvent.SetOnPlayerMoveSuccess(true);

        Vector3 Pos = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => Pos, x => Pos = x, Pos + IsoVector.GetVectorDir(Dir) * Length, GameData.m_timeMove).SetEase(Ease.Linear).OnUpdate(() =>
        {
            m_block.Pos = new IsoVector(Pos);
        }).OnComplete(() =>
        {
            GameEvent.SetOnControlDone(GameKey.PLAYER);
        });
    } //Move!!
}