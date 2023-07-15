using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBullet : MonoBehaviour
{
    private int m_speed = 1;

    private bool m_turnControl = false;
    private IsoVector m_turnDir;
    private int m_turnLength = 0;
    private int m_turnLengthCurrent = 0;

    private bool TurnLock => m_turnLengthCurrent == m_turnLength && m_turnLength != 0;

    private ControllerBody m_body;
    private IsometricBlock m_block;

    public void SetInit(IsoVector Dir, int Speed)
    {
        m_body = GetComponent<ControllerBody>();
        m_block = GetComponent<IsometricBlock>();
        //
        GameTurn.SetInit(TypeTurn.Object);
        GameTurn.onTurn += SetControlTurn;
        GameTurn.onEnd += SetControlEnd;
        //
        m_speed = Speed;
        m_turnDir = Dir;
        //
        StartCoroutine(ISetDelay());
    }

    private void OnDestroy()
    {
        GameTurn.SetRemove(TypeTurn.Object);
        GameTurn.onTurn -= SetControlTurn;
        GameTurn.onEnd -= SetControlEnd;
    }

    private void SetControlTurn(TypeTurn Turn)
    {
        if (Turn != TypeTurn.Object)
        {
            m_turnControl = false;
            return;
        }
        //
        if (TurnLock)
            return;
        //
        m_turnControl = true;
        //
        SetControlMove();
    }

    private void SetControlEnd(TypeTurn Turn)
    {
        if (Turn != TypeTurn.Object)
            return;
        //
        m_turnLength = 0;
        m_turnLengthCurrent = 0;
        m_turnControl = false;
    }

    private void SetControlMove()
    {
        if (m_turnLength == 0)
        {
            m_turnLength = m_speed;
            m_turnLengthCurrent = 0;
        }
        //
        m_turnControl = false;
        //
        m_turnLengthCurrent++;
        //
        IsometricBlock Block = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + m_turnDir);
        if (Block != null)
        {
            //Can't not continue move ahead because of burden, so destroy this!!
            if (Block.GetComponent<ControllerPlayer>())
            {
                //If hit is Player!!
            }
            //
            GameTurn.SetEndTurn(TypeTurn.Object); //Follow Object (!)
            //
            StartCoroutine(ISetDestroy()); //Destroy this!!
            //
            return;
            //Destroy this, instead of continue move ahead!!
        }
        //
        StartCoroutine(ISetDelay());
        //
        Vector3 MoveDir = IsoVector.GetVector(m_turnDir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                //End Animation!!
                if (TurnLock)
                    GameTurn.SetEndTurn(TypeTurn.Object); //Follow Object (!)
                else
                    GameTurn.SetEndMove(TypeTurn.Object); //Follow Object (!)
            });
    }

    private IEnumerator ISetDelay()
    {
        yield return new WaitForSeconds(GameManager.TimeMove * 1);

        GameTurn.SetEndTurn(TypeTurn.Object); //Follow Object (!)
    }

    private IEnumerator ISetDestroy()
    {
        //Do animation destroy!!

        yield return new WaitForSeconds(GameManager.TimeMove * 1);

        Destroy(this.gameObject);
    } //Destroy this!!
}