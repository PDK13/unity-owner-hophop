using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerShoot : MonoBehaviour
{
    private bool m_turnControl = false;
    private string m_turnCommand;
    private int m_turnTime = 0;
    private int m_turnTimeCurrent = 0;

    private bool TurnLock => m_turnTimeCurrent == m_turnTime && m_turnTime != 0;

    private IsoDataBlockAction m_dataAction;
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
        m_dataAction = m_block.Data.ActionData;
        m_dataFollow = m_block.Data.EventData.DataExist ? m_block.Data.EventData.Data.Find(t => t.Name == ConstGameKey.EVENT_FOLLOW).Value : null;

        if (m_dataAction != null)
        {
            if (m_dataAction.DataExist)
            {
                GameTurn.SetInit(TypeTurn.Object);
                GameTurn.onTurn += SetControlTurn;
                GameTurn.onEnd += SetControlEnd;
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
        if (m_dataAction != null)
        {
            if (m_dataAction.DataExist)
            {
                GameTurn.SetRemove(TypeTurn.Object);
                GameTurn.onTurn -= SetControlTurn;
                GameTurn.onEnd -= SetControlEnd;
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
        if (TurnLock)
            return;
        //
        m_turnControl = true;
        //
        SetControlAction();
    }

    private void SetControlEnd(TypeTurn Turn)
    {
        if (Turn != TypeTurn.Object)
            return;
        //
        m_turnTime = 0;
        m_turnTimeCurrent = 0;
        m_turnControl = false;
    }

    private void SetControlAction()
    {
        if (m_turnTime == 0)
        {
            m_turnCommand = m_dataAction.Action[m_dataAction.Index];
            m_turnTime = m_dataAction.Time[m_dataAction.Index];
            m_turnTimeCurrent = 0;
        }
        //
        m_turnControl = false;
        //
        m_turnTimeCurrent++;
        //
        //???
        //
        if (TurnLock)
        {
            m_dataAction.Index += m_dataAction.Quantity;
            if (m_dataAction.Loop && (m_dataAction.Index < 0 || m_dataAction.Index > m_dataAction.DataCount - 1))
            {
                m_dataAction.Quantity *= -1;
                m_dataAction.Index += m_dataAction.Quantity;
            }
        }
    }

    private void SetControlFollow(string KeyFollow, IsoVector Dir)
    {
        if (KeyFollow != m_dataFollow)
            return;
        //
        //???
        //
    }
}