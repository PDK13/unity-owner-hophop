using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjPlayer : MonoBehaviour
{
    private bool m_controlInput = false;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onKeyStart += SetKeyStart;
    }

    private void OnDestroy()
    {
        GameEvent.onKeyStart -= SetKeyStart;
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
        if (Input.GetKeyDown(KeyCode.Space))
            GameEvent.SetKeyEnd(GameKey.PLAYER);
    }

    private void SetKeyStart(string Key)
    {
        if (Key != GameKey.PLAYER)
            return;

        m_controlInput = true;
    }

    private void SetMove(IsoDir Dir)
    {
        int Length = 1; //Follow Character

        //Check if there is a Block ahead?!
        IsometricBlock BlockNext = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.GetDir(Dir) * Length);
        if (BlockNext != null)
        {
            //There is a Block ahead!!
            //Check if can push this Block?!
            //
            ObjPush BlockPush = BlockNext.GetComponent<ObjPush>();
            if (BlockPush == null)
                //Surely can't continue move to this Pos, because this Block can't be push!!
                return;
            IsometricBlock BlockNext2 = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.GetDir(Dir) * (Length + 1));
            if (BlockNext2 != null)
                //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                return;
            //
            //Fine to continue push this Block ahead!!
        }
        //Fine to continue move to pos ahead!!

        m_controlInput = false;

        Vector3 PosStart = IsoVector.GetVector(m_block.Pos);
        Vector3 PosEnd = IsoVector.GetVector(m_block.Pos) + IsoVector.GetVector(Dir) * Length;
        DOTween.To(() => PosStart, x => PosEnd = x, PosEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(PosEnd);
            })
            .OnComplete(() =>
            {
                GameEvent.SetKeyEnd(GameKey.PLAYER);
            });
    } //Move!!
}