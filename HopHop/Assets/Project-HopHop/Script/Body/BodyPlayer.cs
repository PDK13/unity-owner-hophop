using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BodyPlayer : MonoBehaviour, IBodyTurn, IBodyPhysic
{
    private bool m_turnActive = false;

    private BaseBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<BaseBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        TurnManager.SetInit(TurnType.Player, gameObject);
        TurnManager.Instance.onTurn += IOnTurn;
        TurnManager.Instance.onStepStart += IOnStep;
        //
        m_body.onMove += IMoveForce;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMoveForce;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
        //
        //Camera:
        if (GameManager.Instance != null)
            GameManager.Instance.SetCameraFollow(this.transform);
    }

    private void OnDestroy()
    {
        TurnManager.SetRemove(TurnType.Player, gameObject);
        TurnManager.Instance.onTurn -= IOnTurn;
        TurnManager.Instance.onStepStart -= IOnStep;
        //
        m_body.onMove -= IMoveForce;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMoveForce;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;
        //
        //Camera:
        if (GameManager.Instance != null)
            GameManager.Instance.SetCameraFollow(null);
    }

    private void Update()
    {
        if (!m_turnActive)
            return;
        //
        if (Input.GetKey(KeyCode.UpArrow))
            IMove(IsometricVector.Up);
        //
        if (Input.GetKey(KeyCode.DownArrow))
            IMove(IsometricVector.Down);
        //
        if (Input.GetKey(KeyCode.LeftArrow))
            IMove(IsometricVector.Left);
        //
        if (Input.GetKey(KeyCode.RightArrow))
            IMove(IsometricVector.Right);
        //
        if (Input.GetKeyDown(KeyCode.Space))
            IMove(IsometricVector.None);
    }

    //

    public bool ITurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public void IOnTurn(int Turn)
    {
        //Reset!!
        //
        //...
    }

    public void IOnStep(string Name)
    {
        if (Name == TurnType.Player.ToString())
        {
            if (!m_body.SetControlMoveForce())
            {
                m_turnActive = true;
            }
        }
    }

    //

    public void IMoveForce(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndTurn(TurnType.Player, gameObject);
        }
    }

    public void IForce(bool State, IsometricVector Dir)
    {
        //...
    }

    public bool IMove(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
        {
            m_turnActive = false;
            TurnManager.SetEndTurn(TurnType.Player, gameObject); //Follow Player (!)
            return false;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameConfigTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                //
                Block.GetComponent<BaseBullet>().SetHit();
            }
            else
            {
                BaseBody BlockBody = Block.GetComponent<BaseBody>();
                //
                if (BlockBody == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return false;
                }
            }
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
}