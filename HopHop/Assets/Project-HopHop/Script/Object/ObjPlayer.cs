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
            ObjBody BlockBody = BlockNext.GetComponent<ObjBody>();
            if (BlockBody == null)
                //Surely can't continue move to this Pos, because this Block can't be push!!
                return;
            if (!BlockBody.GetCheckPush(IsoVector.GetDir(Dir)))
                //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                return;
            //
            //Fine to continue push this Block ahead!!
            BlockBody.SetMovePush(IsoVector.GetDir(Dir), Length);
        }
        //Fine to continue move to pos ahead!!

        m_controlInput = false;

        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * Length;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameData.TimeMove * Length)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                GameEvent.SetKeyEnd(GameKey.PLAYER);
            });
    } //Move!!
}