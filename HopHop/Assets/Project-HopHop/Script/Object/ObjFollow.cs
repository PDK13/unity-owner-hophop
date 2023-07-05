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

    private void SetForceMove(IsoVector Pos, IsoDir Dir, int Length, bool Revert)
    {
        if (m_block.Pos != Pos)
            return;

        Vector3 PosMove = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => PosMove, x => PosMove = x, PosMove + IsoVector.GetVectorDir(Dir, Revert) * Length, GameData.TimeMove)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                if (m_topForce)
                    GameEvent.SetForceMove(m_block.Pos + IsoVector.Top, Dir, Length, Revert);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(PosMove);
            });
    } //Move!!
}