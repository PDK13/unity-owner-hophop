using DG.Tweening;
using QuickMethode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerShoot : MonoBehaviour
{
    [SerializeField] private ControllerBullet m_bullet;

    private bool m_turnControl = false;
    private string m_turnCommand;
    private int m_turnTime = 0;
    private int m_turnTimeCurrent = 0;

    private bool TurnLock => m_turnTimeCurrent == m_turnTime && m_turnTime != 0;

    private IsoDataBlockAction m_dataAction;

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

        if (m_dataAction != null)
        {
            if (m_dataAction.DataExist)
            {
                GameTurn.SetInit(TypeTurn.Object);
                GameTurn.onTurn += SetControlTurn;
                GameTurn.onEnd += SetControlEnd;
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
        List<string> Command = QEncypt.GetDencyptString('-', m_turnCommand);
        switch (Command[0])
        {
            case ConstGameKey.COMMAND_WAIT:
                //"wait"
                break;
            case ConstGameKey.COMMAND_SHOOT:
                //"shoot-[1]-[2]-[3]"
                IsoVector DirSpawm = IsoVector.GetDirValue(Command[1]);
                IsoVector DirMove = IsoVector.GetDirValue(Command[2]);
                int Speed = int.Parse(Command[3]);
                SetShoot(DirSpawm, DirMove, Speed);
                break;
        }
        //
        StartCoroutine(ISetDelay());
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

    private IEnumerator ISetDelay()
    {
        yield return new WaitForSeconds(GameManager.TimeMove * 1);

        GameTurn.SetEndTurn(TypeTurn.Object); //Follow Object (!)
    }

    private void SetShoot(IsoVector DirSpawm, IsoVector DirMove, int Speed)
    {
        GameObject Bullet = QGameObject.SetCreate(m_bullet.gameObject, this.transform.parent);
        IsometricBlock BulletBlock = Bullet.GetComponent<IsometricBlock>();
        BulletBlock.WorldManager = m_block.WorldManager;
        BulletBlock.Pos = m_block.Pos + DirSpawm;
        Bullet.GetComponent<ControllerBullet>().SetInit(DirMove, Speed);
    } //Shoot Bullet!!
}