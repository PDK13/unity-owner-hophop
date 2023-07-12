using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControllerPlayer : MonoBehaviour
{
    private bool m_turnControl = false;

    private ControllerBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<ControllerBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        GameTurn.SetInit(TypeTurn.Player);
        GameTurn.onTurn += SetControlTurn;
    }

    private void OnDestroy()
    {
        GameTurn.SetRemove(TypeTurn.Player);
        GameTurn.onTurn -= SetControlTurn;
    }

    private void Update()
    {
        if (!m_turnControl)
            return;

        if (Input.GetKey(KeyCode.UpArrow))
            SetControlMove(IsoVector.Up);

        if (Input.GetKey(KeyCode.DownArrow))
            SetControlMove(IsoVector.Down);

        if (Input.GetKey(KeyCode.LeftArrow))
            SetControlMove(IsoVector.Left);

        if (Input.GetKey(KeyCode.RightArrow))
            SetControlMove(IsoVector.Right);

        if (Input.GetKeyDown(KeyCode.Space))
            SetControlMove(IsoVector.None);
    }

    private void SetControlTurn(TypeTurn Turn)
    {
        if (Turn != TypeTurn.Player)
        {
            m_turnControl = false;
            return;
        }
        //
        m_turnControl = true;
        //
    }

    private void SetControlMove(IsoVector Dir)
    {
        if (Dir == IsoVector.None)
        {
            m_turnControl = false;
            GameTurn.SetEndTurn(TypeTurn.Player); //Follow Player (!)
            return;
        }

        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock BlockNext = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + Dir * Length);
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
            if (!BlockBody.GetCheckDir(Dir, Dir))
                //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                return;
            //
            //Fine to continue push this Block ahead!!
            //BlockBody.SetMove(IsoVector.GetDir(Dir));
        }
        //Fine to continue move to pos ahead!!
        //
        m_turnControl = false;
        //
        m_body.SetCheckGravity(Dir);
        //
        Vector3 MoveDir = IsoVector.GetVector(Dir);
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
                //GameTurn.SetEndMove(TypeTurn.Player); //Follow Player (!)
                GameTurn.SetEndTurn(TypeTurn.Player); //Follow Player (!)
            });
        //
    }
}