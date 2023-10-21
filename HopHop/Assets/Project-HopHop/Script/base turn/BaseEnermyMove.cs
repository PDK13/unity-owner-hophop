using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnermyMove : MonoBehaviour
{
    private bool m_turnControl = false;
    //
    [SerializeField] private IsoDir m_moveDir = IsoDir.None;
    [SerializeField] private bool m_checkStopBot = false;
    [SerializeField] private bool m_checkStopAhead = false;
    //
    private BaseCharacter m_character;
    private BaseBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_character = GetComponent<BaseCharacter>();
        m_body = GetComponent<BaseBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        TurnManager.SetInit(TurnType.Enermy, gameObject);
        TurnManager.Instance.onTurn += SetControlTurn;
        TurnManager.Instance.onStepStart += SetControlStep;
        //
        m_body.onMove += SetMove;
        m_body.onMoveForce += SetMoveForce;
        m_body.onGravity += SetGravity;
        m_body.onPush += SetPush;
        m_body.onForce += SetForce;
    }

    private void OnDestroy()
    {
        TurnManager.SetRemove(TurnType.Enermy, gameObject);
        TurnManager.Instance.onTurn -= SetControlTurn;
        TurnManager.Instance.onStepStart -= SetControlStep;
        //
        m_body.onMove -= SetMove;
        m_body.onMoveForce -= SetMoveForce;
        m_body.onGravity -= SetGravity;
        m_body.onPush -= SetPush;
        m_body.onForce -= SetForce;
    }

    private void SetControlTurn(int Turn)
    {
        m_turnControl = true;
    }

    private void SetControlStep(string Name)
    {
        if (m_turnControl)
        {
            if (Name == TurnType.Enermy.ToString())
            {
                if (!m_body.SetControlMoveForce())
                {
                    if (!SetControlMove(IsometricVector.GetDir(m_moveDir)))
                    {
                        m_moveDir = IsometricVector.GetDir(IsometricVector.GetDir(m_moveDir) * -1);
                        if (!SetControlMove(IsometricVector.GetDir(m_moveDir)))
                        {
                            m_moveDir = IsometricVector.GetDir(IsometricVector.GetDir(m_moveDir) * -1);
                            //
                            m_turnControl = false;
                            TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Enermy (!)
                        }
                    }
                }
                else
                {
                    m_turnControl = false;
                }
            }
        }
    }

    private bool SetControlMove(IsometricVector Dir)
    {
        if (m_moveDir == IsoDir.None)
        {
            m_turnControl = false;
            TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Enermy (!)
            return true;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.World.GetBlockCurrent(m_block.Pos + Dir * Length);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Enermy hit Player!!");
                //
                m_turnControl = false;
                TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Enermy (!)
                //
                return true;
            }
            else
            if (Block.Tag.Contains(GameConfigTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");
                //
                Block.GetComponent<BaseBullet>().SetHit();
            }
            else
            if (m_checkStopAhead)
                //Stop Ahead when there is an burden ahead!!
                return false;
            else
            {
                //None Stop Ahead and continue check move ahead!!
                //
                BaseBody BlockBody = Block.GetComponent<BaseBody>();
                //
                if (BlockBody == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return false;
                }
                //
                if (BlockBody.CharacterPush)
                {
                    if (BlockBody.GetCheckDir(Dir))
                    {
                        //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                        return false;
                    }
                    BlockBody.SetControlPush(Dir, m_block.Pos);
                }
                else
                    //Surely can't continue move to this Pos, because this Block can't be push by character!!
                    return false;
                //
                //Fine to continue push this Block ahead!!
            }
        }
        else
        if (m_checkStopBot)
        {
            //Continue check move Ahead Bot!!
            //
            IsometricBlock BlockBot = m_block.WorldManager.World.GetBlockCurrent(m_block.Pos + Dir * Length + IsometricVector.Bot);
            if (BlockBot == null)
                //Stop Ahead because no block ahead bot!!
                return false;
        }
        //Fine to continue move to pos ahead!!
        //
        m_turnControl = false;
        //
        m_body.SetControlMove(Dir);
        //
        return true;
    }

    //Player Forget!!

    private void SetMove(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnControl = false;
            TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Object (!)
        }
    }

    private void SetMoveForce(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnControl = false;
            TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Object (!)
        }
    }

    private void SetGravity(bool State)
    {
        //...
    }

    private void SetPush(bool State, IsometricVector Dir)
    {
        //...
    }

    public void SetForce(bool State, IsometricVector Dir)
    {
        //...
    }
}