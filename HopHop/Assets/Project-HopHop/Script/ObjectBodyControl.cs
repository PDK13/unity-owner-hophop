using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectBodyControl : MonoBehaviour
{
    [SerializeField] private string m_key;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameManager.onMove += SetMove;
    }

    private void OnDestroy()
    {
        GameManager.onMove -= SetMove;
    }

    private void SetMove(string Key, IsoDir Dir)
    {
        if (Key != m_key)
            return;

        if (m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.GetDir(Dir)).Count > 0)
            return;

        GameManager.SetOnPlayerMoveSuccess(true);

        Vector3 Pos = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => Pos, x => Pos = x, Pos + IsoVector.GetVectorDir(Dir), 1f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            m_block.Pos = new IsoVector(Pos);
        }).OnComplete(() =>
        {
            GameManager.SetOnMoveDone(Key);
        });
    }
}
