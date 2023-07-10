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
                GameManager.SetObjectTurn(true);

                GameEvent.onTurn += SetTurn;
                GameEvent.onDelay += SetDelay;

                m_body.onMove += SetMove;
            }
            else
            if (m_dataFollow != null)
            {
                GameEvent.onFollow += SetFollow;
            }
        }
    }

    private void OnDestroy()
    {
        if (m_dataMove != null)
        {
            if (m_dataMove.DataExist)
            {
                GameManager.SetObjectTurn(false);

                GameEvent.onTurn -= SetTurn;
                GameEvent.onDelay -= SetDelay;

                m_body.onMove -= SetMove;
            }
            else
            if (m_dataFollow != null)
            {
                GameEvent.onFollow -= SetFollow;
            }
        }
    }

    #region Turn

    private void SetTurn(TypeTurn Turn, bool State)
    {
        if (m_dataMove == null)
            return;

        if (Turn != TypeTurn.Object)
            return;

        if (!State)
            return;

        SetMove();
    }

    private void SetDelay(TypeDelay Delay, bool State)
    {

    }

    private void SetMove()
    {
        IsoVector Dir = IsoVector.GetDir(m_dataMove.Dir[m_dataMove.Index]) * m_dataMove.Quantity;
        int Length = m_dataMove.Length[m_dataMove.Index];

        m_body.SetMove(Dir);
        GameEvent.SetFollow(m_dataFollow, Dir);

        m_dataMove.Index += m_dataMove.Quantity;
        if (m_dataMove.Loop && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.DataCount - 1))
        {
            m_dataMove.Quantity *= -1;
            m_dataMove.Index += m_dataMove.Quantity;
        }
    }

    private void SetFollow(string KeyFollow, IsoVector Dir)
    {
        if (KeyFollow != m_dataFollow)
            return;

        m_body.SetMove(Dir);
    } //Move!!

    private void SetMove(bool State)
    {
        if (State)
            return;

        GameEvent.SetTurn(TypeTurn.Object, false);
    }

    #endregion
}