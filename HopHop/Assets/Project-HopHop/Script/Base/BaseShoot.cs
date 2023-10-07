using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShoot : MonoBehaviour
{
    [SerializeField] private BaseBullet m_bullet;

    private bool m_turnControl = false;

    private IsometricDataAction m_dataAction;

    private string m_turnCommand;
    private int m_turnTime = 0;
    private int m_turnTimeCurrent = 0;

    private bool TurnEnd => m_turnTimeCurrent == m_turnTime && m_turnTime != 0;

    private BaseBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<BaseBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_dataAction = m_block.Data.Action;

        if (m_dataAction != null)
        {
            if (m_dataAction.DataExist)
            {
                TurnManager.SetInit(TurnType.Object, gameObject);
                TurnManager.Instance.onTurn += SetControlTurn;
                TurnManager.Instance.onStepStart += SetControlStep;
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        //
        if (m_dataAction != null)
        {
            if (m_dataAction.DataExist)
            {
                TurnManager.SetRemove(TurnType.Object, gameObject);
                TurnManager.Instance.onTurn -= SetControlTurn;
                TurnManager.Instance.onStepStart -= SetControlStep;
            }
        }
    }

    private void SetControlTurn(int Turn)
    {
        //Reset!!
        m_turnTime = 0;
        m_turnTimeCurrent = 0;
        //
        m_turnControl = true;
    }

    private void SetControlStep(string Name)
    {
        if (m_turnControl)
        {
            if (Name == TurnType.Object.ToString())
            {
                SetControlAction();
            }
        }
    }

    private void SetControlAction()
    {
        if (m_turnTime == 0)
        {
            m_turnCommand = m_dataAction.Action[m_dataAction.Index];
            m_turnTime = m_dataAction.ActionDuration[m_dataAction.Index];
            m_turnTimeCurrent = 0;
        }
        //
        m_turnTimeCurrent++;
        //
        List<string> Command = QEncypt.GetDencyptString('-', m_turnCommand);
        //
        if (Command[0] == GameManager.GameConfig.Command.Wait)
        {
            //"wait"
        }
        else
        if (Command[0] == GameManager.GameConfig.Command.Shoot)
        {
            //"shoot-[1]-[2]-[3]"
            IsometricVector DirSpawm = IsometricVector.GetDirDeEncypt(Command[1]);
            IsometricVector DirMove = IsometricVector.GetDirDeEncypt(Command[2]);
            int Speed = int.Parse(Command[3]);
            SetShoot(DirSpawm, DirMove, Speed);
        }
        //
        StartCoroutine(ISetDelay());
        //
        if (TurnEnd)
        {
            m_dataAction.Index += m_dataAction.Quantity;
            if (m_dataAction.Type == DataBlockType.Forward && m_dataAction.Index > m_dataAction.DataCount - 1)
            {
                //End Here!!
            }
            else
            if (m_dataAction.Type == DataBlockType.Loop && m_dataAction.Index > m_dataAction.DataCount - 1)
            {
                m_dataAction.Index = 0;
            }
            else
            if (m_dataAction.Type == DataBlockType.Revert && (m_dataAction.Index < 0 || m_dataAction.Index > m_dataAction.DataCount - 1))
            {
                m_dataAction.Quantity *= -1;
                m_dataAction.Index += m_dataAction.Quantity;
            }
        }
    }

    private IEnumerator ISetDelay()
    {
        yield return new WaitForSeconds(GameManager.TimeMove * 1);
        //
        m_turnControl = false;
        TurnManager.SetEndTurn(TurnType.Object, gameObject); //Follow Object (!)
    }

    private void SetShoot(IsometricVector DirSpawm, IsometricVector DirMove, int Speed)
    {
        IsometricBlock Block = m_block.WorldManager.World.GetBlockCurrent(m_block.Pos + DirSpawm);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameManager.GameConfig.Tag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //
            //Surely can't spawm bullet here!!
            return;
        }
        //
        IsometricBlock Bullet = m_block.WorldManager.World.SetBlockCreate(m_block.Pos + DirSpawm, m_bullet.gameObject);
        Bullet.GetComponent<BaseBullet>().SetInit(DirMove, Speed);
    } //Shoot Bullet!!
}