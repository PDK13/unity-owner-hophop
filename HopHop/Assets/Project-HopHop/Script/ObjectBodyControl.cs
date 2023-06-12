using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectBodyControl : MonoBehaviour
{
    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SetMove(IsoDir.Up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            SetMove(IsoDir.Down);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetMove(IsoDir.Left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SetMove(IsoDir.Right);
    }

    private void SetMove(IsoDir Dir)
    {
        Vector3 Pos = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => Pos, x => Pos = x, Pos + IsoVector.GetVectorDir(Dir), 1f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            m_block.Pos = new IsoVector(Pos);
        });
    }
}
