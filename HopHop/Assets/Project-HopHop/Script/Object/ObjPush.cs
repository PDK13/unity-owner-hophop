using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPush : MonoBehaviour
{
    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onMovePush += SetForceMove;
    }

    private void OnDestroy()
    {
        GameEvent.onMovePush -= SetForceMove;
    }

    private void SetForceMove(IsoVector Pos, Vector3Int Dir, int Length)
    {
        if (m_block.Pos != Pos)
            return;

        if (m_block.WorldManager.GetWorldBlockCurrentAll(m_block.Pos + IsoVector.GetDir(Dir) * Length) != null)
            return;

        Vector3 PosStart = IsoVector.GetVector(m_block.Pos);
        Vector3 PosEnd = IsoVector.GetVector(m_block.Pos) + Dir * Length;
        DOTween.To(() => PosStart, x => PosEnd = x, PosEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(PosEnd);
            });
    } //Move!!
}
