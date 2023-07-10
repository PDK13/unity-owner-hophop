using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControllerPlayer : MonoBehaviour
{
    private bool m_turnControl = false;
    private bool m_turnDelay = false;

    private int m_fallCount = 0;

    private ControllerBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<ControllerBody>();
        m_block = GetComponent<IsometricBlock>();

        m_body.onGravity += SetGravity;

        GameEvent.onTurn += SetTurn;
    }

    private void OnDestroy()
    {
        m_body.onGravity -= SetGravity;

        GameEvent.onTurn -= SetTurn;
    }

    private void Update()
    {
        if (!m_turnControl)
            return;

        if (Input.GetKey(KeyCode.UpArrow))
            SetMoveForceTurn(IsoDir.Up);
        if (Input.GetKey(KeyCode.DownArrow))
            SetMoveForceTurn(IsoDir.Down);
        if (Input.GetKey(KeyCode.LeftArrow))
            SetMoveForceTurn(IsoDir.Left);
        if (Input.GetKey(KeyCode.RightArrow))
            SetMoveForceTurn(IsoDir.Right);
        if (Input.GetKeyDown(KeyCode.Space))
            SetMoveForceTurn(IsoDir.None);
    }

    #region Turn

    private void SetTurn(TypeTurn Turn, bool State)
    {
        if (Turn == TypeTurn.Player && State)
            SetKeyTurn();
    }

    private void SetKeyTurn()
    {
        m_turnControl = true;
    }

    #endregion

    #region Move This

    private void SetMoveForceTurn(IsoDir Dir)
    {
        if (Dir == IsoDir.None)
        {
            m_turnControl = false;
            GameEvent.SetTurn(TypeTurn.Player, false);
            return;
        }

        int Length = 1; //Follow Character

        //Check if there is a Block ahead?!
        IsometricBlock BlockNext = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + IsoVector.GetDir(Dir) * Length);
        if (BlockNext != null)
        {
            //There is a Block ahead!!
            //Check if can push this Block?!
            //
            ControllerBody BlockBody = BlockNext.GetComponent<ControllerBody>();
            if (BlockBody == null)
                //Surely can't continue move to this Pos, because this Block can't be push!!
                return;
            if (!BlockBody.GetCheckPush(IsoVector.GetDir(Dir)))
                //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                return;
            //
            //Fine to continue push this Block ahead!!
            BlockBody.SetMoveForcePush(IsoVector.GetDir(Dir), Length);
        }
        //Fine to continue move to pos ahead!!

        m_turnControl = false;

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
                m_body.SetGravity();
            });
    } //Move!!

    #endregion

    #region Gravity

    private void SetGravity(bool State)
    {
        if (State)
            m_fallCount++;
        else
        {
            m_fallCount = 0;
            GameEvent.SetTurn(TypeTurn.Player, false);
        }
    }

    #endregion
}