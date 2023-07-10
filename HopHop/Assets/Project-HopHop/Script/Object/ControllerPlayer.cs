using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControllerPlayer : MonoBehaviour
{
    private bool m_turnControl = false;

    private int m_fallCount = 0;

    private ControllerBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<ControllerBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_body.onGravity += SetGravity;
        m_body.onMove += SetMove;

        GameEvent.onTurn += SetTurn;
        GameEvent.onDelay += SetDelay;
    }

    private void OnDestroy()
    {
        m_body.onGravity -= SetGravity;
        m_body.onMove -= SetMove;

        GameEvent.onTurn -= SetTurn;
        GameEvent.onDelay -= SetDelay;
    }

    private void Update()
    {
        if (!m_turnControl)
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
        {
            m_turnControl = false;
            m_body.SetMove(IsoVector.GetDir(IsoDir.None));
        }
    }

    #region Turn

    private void SetTurn(TypeTurn Turn, bool State)
    {
        if (Turn != TypeTurn.Player)
            return;

        if (!State)
            return;

        m_turnControl = true;
    }

    private void SetDelay(TypeDelay Delay, bool State)
    {

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
            ControllerBody BlockBody = BlockNext.GetComponent<ControllerBody>();
            if (BlockBody == null)
                //Surely can't continue move to this Pos, because this Block can't be push!!
                return;
            if (!BlockBody.Dynamic)
                //Surely can't continue move to this Pos, because this Block can't be push!!
                return;
            if (!BlockBody.GetCheckDir(IsoVector.GetDir(Dir), IsoVector.GetDir(Dir)))
                //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                return;
            //
            //Fine to continue push this Block ahead!!
            BlockBody.SetMove(IsoVector.GetDir(Dir));
        }
        //Fine to continue move to pos ahead!!

        m_turnControl = false;

        m_body.SetMove(IsoVector.GetDir(Dir));
    } //Move!!

    private void SetGravity(bool State)
    {
        if (State)
        {
            m_fallCount++;
        }
        else
        {
            m_fallCount = 0;
            GameEvent.SetTurn(TypeTurn.Player, false);
        }
    }

    private void SetMove(bool State)
    {
        if (State)
        {

        }
        else
        {
            GameEvent.SetTurn(TypeTurn.Player, false);
        }
    }

    #endregion
}