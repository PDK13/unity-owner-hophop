using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjFollow : MonoBehaviour
{
    [SerializeField] private bool m_topForce = false;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onForceMove += SetForceMove;
    }

    private void OnDestroy()
    {
        GameEvent.onForceMove -= SetForceMove;
    }

    private void SetForceMove(IsoVector Pos, Vector3Int Dir, int Length)
    {
        if (m_block.Pos != Pos)
            return;

        if (m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.GetDir(Dir) * Length) != null)
            return;

        Vector3 PosStart = IsoVector.GetVector(m_block.Pos);
        Vector3 PosEnd = IsoVector.GetVector(m_block.Pos) + Dir * Length;
        DOTween.To(() => PosStart, x => PosEnd = x, PosEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                if (m_topForce)
                    GameEvent.SetForceMove(m_block.Pos + IsoVector.Top, Dir, Length);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(PosEnd);
            });
    } //Move!!
}