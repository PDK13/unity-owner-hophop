using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectPlayerControl : MonoBehaviour
{
    private bool m_controlInput = false;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onTriggerStart += SetTriggerStart;
    }

    private void OnDestroy()
    {
        GameEvent.onTriggerStart -= SetTriggerStart;
    }

    private void Update()
    {
        if (!m_controlInput)
            return;

        if (Input.GetKey(KeyCode.UpArrow))
            SetMove(IsoDir.Up);
        if (Input.GetKey(KeyCode.DownArrow))
            SetMove(IsoDir.Down);
        if (Input.GetKey(KeyCode.LeftArrow))
            SetMove(IsoDir.Left);
        if (Input.GetKey(KeyCode.RightArrow))
            SetMove(IsoDir.Right);
    }

    private void SetTriggerStart(string Key)
    {
        if (Key != GameKey.PLAYER)
            return;

        m_controlInput = true;
    }

    private void SetMove(IsoDir Dir)
    {
        int Length = 1; //Follow Character

        if (m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.GetDir(Dir) * Length) != null)
            return;

        m_controlInput = false;

        Vector3 Pos = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => Pos, x => Pos = x, Pos + IsoVector.GetVectorDir(Dir) * Length, GameData.TimeMove).SetEase(Ease.Linear).OnUpdate(() =>
        {
            m_block.Pos = new IsoVector(Pos);
        }).OnComplete(() =>
        {
            GameEvent.SetTriggerEnd(GameKey.PLAYER);
        });
    } //Move!!
}