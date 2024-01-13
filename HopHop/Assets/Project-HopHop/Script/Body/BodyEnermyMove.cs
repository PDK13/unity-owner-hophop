using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BodyEnermyMove : BodyEnermy, IBodyMove
{
    [SerializeField] private IsoDir m_moveDir = IsoDir.None;

    protected override void Start()
    {
        if (m_block.Data.Init.Data.Exists(t => t.Contains(GameConfigInit.Move)))
        {
            string Data = m_block.Data.Init.Data.Find(t => t.Contains(GameConfigInit.Move));
            List<string> Command = QEncypt.GetDencyptString('-', Data);
            m_moveDir = IsometricVector.GetDir(IsometricVector.GetDirDeEncypt(Command[1]));
            m_checkPlayerHit = Command[2] == "1" ? true : false;
            m_checkStopBot = Command[3] == "1" ? true : false;
            m_checkStopAhead = Command[4] == "1" ? true : false;
        }
        //
        m_body.onMove += IMoveForce;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMoveForce;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
    }

    protected override void OnDestroy()
    {
        m_body.onMove -= IMoveForce;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMoveForce;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;
    }

    //

    public override void IOnStep(string Name)
    {
        if (m_turnActive)
        {
            if (Name == TurnType.Enermy.ToString())
            {
                if (!m_body.SetControlMoveForce())
                {
                    if (!IMove(IsometricVector.GetDir(m_moveDir)))
                    {
                        m_moveDir = IsometricVector.GetDir(IsometricVector.GetDir(m_moveDir) * -1);
                        if (!IMove(IsometricVector.GetDir(m_moveDir)))
                        {
                            m_moveDir = IsometricVector.GetDir(IsometricVector.GetDir(m_moveDir) * -1);
                            //
                            m_turnActive = false;
                            TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Enermy (!)
                        }
                    }
                }
                else
                {
                    m_turnActive = false;
                }
            }
        }
    }

    public override void IOnTurn(int Turn)
    {
        m_turnActive = true;
    }

    //

    public void IMoveForce(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndTurn(TurnType.Enermy, gameObject);
        }
    }

    public void IForce(bool State, IsometricVector Dir)
    {
        //...
    }

    public bool IMove(IsometricVector Dir)
    {
        if (m_moveDir == IsoDir.None)
        {
            m_turnActive = false;
            TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Enermy (!)
            return true;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameConfigTag.Player))
            {
                if (m_checkPlayerHit)
                {
                    Debug.Log("[Debug] Enermy hit Player!!");
                    //
                    m_turnActive = false;
                    TurnManager.SetEndTurn(TurnType.Enermy, gameObject); //Follow Enermy (!)
                    //
                    return true;
                }
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
            //else
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
                //Fine to continue push this Block ahead!!
            }
        }
        else
        if (m_checkStopBot)
        {
            //Continue check move Ahead Bot!!
            //
            IsometricBlock BlockBot = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length + IsometricVector.Bot);
            if (BlockBot == null)
                //Stop Ahead because no block ahead bot!!
                return false;
        }
        //Fine to continue move to pos ahead!!
        //
        m_turnActive = false;
        //
        m_body.SetControlMove(Dir);
        //
        return true;
    }

    public void IGravity(bool State)
    {
        //...
    }

    public void IPush(bool State, IsometricVector Dir)
    {
        //...
    }

    //**Editor**

    public void SetEditorMove()
    {
        IsometricBlock Block = GetComponent<IsometricBlock>();
        string Data = string.Format("{0}-{1}-{2}-{3}-{4}",
            GameConfigInit.Move,
            IsometricVector.GetDirEncypt(m_moveDir),
            m_checkPlayerHit ? 1 : 0,
            m_checkStopAhead ? 1 : 0,
            m_checkStopBot ? 1 : 0);
        Block.Data.Init.Data.Add(Data);
    }

    //**Editor**
}