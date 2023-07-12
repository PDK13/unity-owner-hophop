using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerObject : MonoBehaviour
{
    private bool m_turnControl = false;

    private IsoDataBlockMove m_dataMove;
    private string m_dataFollow;

    private ControllerBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<ControllerBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_dataMove = m_block.Data.MoveData;
        m_dataFollow = m_block.Data.EventData.DataExist ? m_block.Data.EventData.Data.Find(t => t.Name == ConstGameKey.EVENT_FOLLOW).Value : null;

        if (m_dataMove != null)
        {
            if (m_dataMove.DataExist)
            {
                GameTurn.SetInit(TypeTurn.Object);
                GameTurn.onTurn += SetControlTurn;
            }
            else
            if (m_dataFollow != null)
            {
                GameEvent.onFollow += SetControlFollow;
            }
        }
    }

    private void OnDestroy()
    {
        if (m_dataMove != null)
        {
            if (m_dataMove.DataExist)
            {
                GameTurn.SetRemove(TypeTurn.Object);
                GameTurn.onTurn -= SetControlTurn;
            }
            else
            if (m_dataFollow != null)
            {
                GameEvent.onFollow -= SetControlFollow;
            }
        }
    }

    private void SetControlTurn(TypeTurn Turn)
    {
        if (Turn != TypeTurn.Object)
        {
            m_turnControl = false;
            return;
        }
        //
        m_turnControl = true;
        //
        SetControlMove();
    }

    private void SetControlMove()
    {
        IsoVector Dir = IsoVector.GetDir(m_dataMove.Dir[m_dataMove.Index]) * m_dataMove.Quantity;
        int Length = m_dataMove.Length[m_dataMove.Index];
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
                //GameTurn.SetEndMove(TypeTurn.Object); //Follow Object (!)
                GameTurn.SetEndTurn(TypeTurn.Object); //Follow Object (!)
            });
        //
        GameEvent.SetFollow(m_dataFollow, Dir);
        //
        m_dataMove.Index += m_dataMove.Quantity;
        if (m_dataMove.Loop && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.DataCount - 1))
        {
            m_dataMove.Quantity *= -1;
            m_dataMove.Index += m_dataMove.Quantity;
        }
    }

    private void SetControlFollow(string KeyFollow, IsoVector Dir)
    {
        if (KeyFollow != m_dataFollow)
            return;
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
            });
        //
    }
}