using UnityEngine;

public class BodyPlayer : MonoBehaviour, ITurnManager, IBodyPhysic, IBodyInteractiveActive
{
    private bool m_turnActive = false;

    private int m_moveStepCurrent = 0;

    private bool m_interactive = false;

    private IsometricBlock m_block;
    private BodyTurn m_turn;
    private BodyPhysic m_body;
    private BodyCharacter m_character;

    //

    public TurnType Turn => m_turn != null ? m_turn.Turn : TurnType.Player;

    private bool TurnEnd => m_moveStepCurrent >= m_character.MoveStep && m_character.MoveStep > 0;

    private bool TurnLast => m_moveStepCurrent >= m_character.MoveStep - 1 && m_character.MoveStep > 0;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_turn = GetComponent<BodyTurn>();
        m_body = GetComponent<BodyPhysic>();
        m_character = GetComponent<BodyCharacter>();
    }

    private void Start()
    {
        CharacterManager.Instance.onCharacter += SetCharacter;
        //
        TurnManager.SetInit(Turn, this); //Player
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
        //
        m_body.onMove += IMove;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMove;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
        //
        //Camera:
        GameManager.Instance.SetCameraFollow(this.transform);
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.onCharacter -= SetCharacter;
        //
        TurnManager.SetRemove(Turn, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
        //
        m_body.onMove -= IMove;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMove;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;
        //
        //Camera:
        GameManager.Instance.SetCameraFollow(null);
    }

    private void Update()
    {
        if (!m_turnActive)
            return;
        //
        if (Input.GetKeyDown(KeyCode.Q))
            CharacterManager.Instance.SetCharacterPrev();
        //
        if (Input.GetKeyDown(KeyCode.E))
            CharacterManager.Instance.SetCharacterNext();
        //
        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_interactive = !m_interactive;
            SetInteractiveCheck(m_interactive);
        }
        else
        if (m_interactive)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                IInteractive(IsometricVector.Up);
            //
            if (Input.GetKeyDown(KeyCode.DownArrow))
                IInteractive(IsometricVector.Down);
            //
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                IInteractive(IsometricVector.Left);
            //
            if (Input.GetKeyDown(KeyCode.RightArrow))
                IInteractive(IsometricVector.Right);
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
                IControl(IsometricVector.Up);
            //
            if (Input.GetKey(KeyCode.DownArrow))
                IControl(IsometricVector.Down);
            //
            if (Input.GetKey(KeyCode.LeftArrow))
                IControl(IsometricVector.Left);
            //
            if (Input.GetKey(KeyCode.RightArrow))
                IControl(IsometricVector.Right);
            //
            if (Input.GetKeyDown(KeyCode.Space))
                IControl(IsometricVector.None);
        }
    }

    //Character

    private void SetCharacter()
    {
        m_character.SetCharacter(CharacterManager.Instance.CharacterCurrent);
    }

    //Turn

    public void ISetTurn(int Turn)
    {
        //Reset!!
        //
        m_moveStepCurrent = 0;
    }

    public void ISetStepStart(string Step)
    {
        if (Step != Turn.ToString())
            return;
        //
        if (m_body.SetControlMoveForce())
            return;
        //
        m_turnActive = true;
    }

    public void ISetStepEnd(string Step) { }

    //Move

    public bool IControl(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
        {
            m_turnActive = false;
            m_body.SetControlMoveReset();
            TurnManager.SetEndStep(Turn, this);
            return false;
        }
        //
        //Check if there is a Block ahead?!
        //
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * 1);
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                //
                Block.GetComponent<IBodyBullet>().IHit();
            }
            else
            {
                BodyPhysic BlockBody = Block.GetComponent<BodyPhysic>();
                //
                if (BlockBody == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return false;
                }
            }
        }
        //
        //Fine to continue move to pos ahead!!
        //
        m_turnActive = false;
        //
        m_body.SetControlMove(Dir, TurnLast || !m_character.MoveFloat);
        //
        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step != Turn.ToString() && !State)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(Turn, this);
            return;
        }
        //
        if (State)
        {
            //Start Move!
            m_moveStepCurrent++;
        }
        else
        {
            //End Move!
            //
            if (TurnEnd)
            {
                //End Turn!
                //
                m_turnActive = false;
                TurnManager.SetEndStep(Turn, this);
            }
            else
            {
                //End Move!
                TurnManager.SetEndMove(Turn, this);
                //
                //Still Turn!
                //
                if (m_character.MoveLock)
                {
                    //Lock Move!
                    //
                    m_turnActive = false;
                    IControl(m_body.MoveLastXY);
                }
                else
                {
                    //Freely Move!
                    //
                    m_turnActive = true;
                }
            }
        }
    }

    public void IForce(bool State, IsometricVector Dir)
    {
        //...
    }

    public void IGravity(bool State)
    {
        //...
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        //...
    }

    //Interactive

    public bool IInteractive(IsometricVector Dir)
    {
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * 1);
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Switch))
            {
                if (Block.GetComponent<IBodyInteractive>().IInteractive())
                {
                    m_interactive = false;
                    SetInteractiveCheck(false);
                    //
                    m_turnActive = false;
                    TurnManager.SetEndStep(Turn, this);
                    //
                    return true;
                }
            }
            else
            {
                //...
            }
        }
        //
        return false;
    }

    private void SetInteractiveCheck(bool State)
    {
        SetInteractiveCheck(State, IsometricVector.Up);
        SetInteractiveCheck(State, IsometricVector.Down);
        SetInteractiveCheck(State, IsometricVector.Left);
        SetInteractiveCheck(State, IsometricVector.Right);
    }

    private void SetInteractiveCheck(bool State, IsometricVector Dir)
    {
        IsometricBlock Block = IsometricManager.Instance.World.Current.GetBlockCurrent(this.m_block.Pos + Dir);
        //
        if (Block == null)
            return;
        //
        if (State)
        {
            if (Block.GetTag(KeyTag.Interactive))
                Block.GetComponent<BodyChild>().Square.SetBlue();
            else
                Block.GetComponent<BodyChild>().Square.SetGrid();
        }
        else
            Block.GetComponent<BodyChild>().Square.SetNone();
    }

    //
}