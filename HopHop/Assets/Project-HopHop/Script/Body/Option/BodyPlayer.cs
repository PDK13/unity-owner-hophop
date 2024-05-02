using UnityEngine;

public class BodyPlayer : MonoBehaviour, ITurnManager, IBodyPhysic, IBodyInteractiveActive
{
    private int m_moveStepCurrent = 0;

    private bool m_control = false;
    private bool m_interacte = false;

    private IsometricBlock m_block;
    private BodyPhysic m_body;
    private BodyCharacter m_character;

    //

    public TurnType Turn => TurnType.Player;

    private bool TurnEnd => m_moveStepCurrent >= m_character.MoveStep && m_character.MoveStep > 0;

    private bool TurnLast => m_moveStepCurrent >= m_character.MoveStep - 1 && m_character.MoveStep > 0;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();
        m_character = GetComponent<BodyCharacter>();
    }

    private void Start()
    {
        EventManager.Instance.onEvent += OnEvent;

        CharacterManager.Instance.onCharacter += OnCharacter;

        TurnManager.SetInit(Turn, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;

        m_body.onMove += IMove;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMove;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;

        //Camera:
        GameManager.Instance.SetCameraFollow(this.transform);
    }

    private void OnDestroy()
    {
        SetControlStage(false);

        EventManager.Instance.onEvent -= OnEvent;

        CharacterManager.Instance.onCharacter -= OnCharacter;

        TurnManager.SetRemove(Turn, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;

        m_body.onMove -= IMove;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMove;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;

        //Camera
        GameManager.Instance.SetCameraFollow(null);
    }

    //Event

    private void OnEvent(bool Stage)
    {
        //SetControlStage(!Stage);
    }

    //Control

    private void SetControlStage(bool Stage)
    {
        if (Stage)
        {
            SetControlStage(false);

            InputManager.Instance.onUp += OnControlUp;
            InputManager.Instance.onDown += OnControlDown;
            InputManager.Instance.onLeft += OnControlLeft;
            InputManager.Instance.onRight += OnControlRight;
            InputManager.Instance.onStand += OnControlStand;

            InputManager.Instance.onInteracte += OnControlInteracte;

            InputManager.Instance.onCharacterNext += OnCharacterNext;
            InputManager.Instance.onCharacterPrev += OnCharacterPrev;
        }
        else
        {
            InputManager.Instance.onUp -= OnControlUp;
            InputManager.Instance.onDown -= OnControlDown;
            InputManager.Instance.onLeft -= OnControlLeft;
            InputManager.Instance.onRight -= OnControlRight;
            InputManager.Instance.onStand -= OnControlStand;

            InputManager.Instance.onInteracte -= OnControlInteracte;

            InputManager.Instance.onCharacterNext -= OnCharacterNext;
            InputManager.Instance.onCharacterPrev -= OnCharacterPrev;
        }
    }

    private void OnControlUp() 
    {
        if (m_interacte)
            IInteractive(IsometricVector.Up);
        else
            IControl(IsometricVector.Up);
    }

    private void OnControlDown()
    {
        if (m_interacte)
            IInteractive(IsometricVector.Down);
        else
            IControl(IsometricVector.Down);
    }

    private void OnControlLeft()
    {
        if (m_interacte)
            IInteractive(IsometricVector.Left);
        else
            IControl(IsometricVector.Left);
    }

    private void OnControlRight()
    {
        if (m_interacte)
            IInteractive(IsometricVector.Right);
        else
            IControl(IsometricVector.Right);
    }

    private void OnControlStand() 
    {
        if (m_interacte)
            IInteractive(IsometricVector.None);
        else
            IControl(IsometricVector.None);
    }

    private void OnControlInteracte() 
    {
        m_interacte = !m_interacte;
        SetInteractiveCheck(m_interacte);
    }

    //Character

    private void OnCharacter()
    {
        m_character.SetCharacter(CharacterManager.Instance.CharacterCurrent);
    }

    private void OnCharacterNext()
    {
        CharacterManager.Instance.SetCharacterPrev();
    }

    private void OnCharacterPrev()
    {
        CharacterManager.Instance.SetCharacterNext();
    }

    //Turn

    public void ISetTurn(int Turn)
    {
        //Reset!!

        m_moveStepCurrent = 0;
    }

    public void ISetStepStart(string Step)
    {
        if (Step != Turn.ToString())
            return;

        if (m_body.SetControlMoveForce())
            return;

        SetControlStage(true);
    }

    public void ISetStepEnd(string Step) 
    {
        if (Step != Turn.ToString())
            return;
    }

    //Move

    public bool IControl(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
        {
            SetControlStage(false);

            m_body.SetControlMoveReset();

            TurnManager.SetEndStep(Turn, this);

            return false;
        }

        //Check if there is a Block ahead?!

        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * 1);
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");

                Block.GetComponent<IBodyBullet>().IHit();
            }
            else
            {
                BodyPhysic BlockBody = Block.GetComponent<BodyPhysic>();

                if (BlockBody == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return false;
                }
            }
        }

        //Fine to continue move to pos ahead!!

        SetControlStage(false);

        m_body.SetControlMove(Dir, TurnLast || !m_character.MoveFloat);

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step != Turn.ToString() && !State)
        {
            SetControlStage(false);

            TurnManager.SetEndStep(Turn, this);

            return;
        }

        if (State)
        {
            //Start Move!
            m_moveStepCurrent++;
        }
        else
        {
            //End Move!

            if (TurnEnd)
            {
                //End Turn!

                SetControlStage(false);

                TurnManager.SetEndStep(Turn, this);
            }
            else
            {
                //End Move!
                TurnManager.SetEndMove(Turn, this);

                //Still Turn!

                if (m_character.MoveLock)
                {
                    //Lock Move!

                    SetControlStage(false);

                    IControl(m_body.MoveLastXY);
                }
                else
                {
                    //Freely Move!

                    SetControlStage(true);
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
        if (Dir == IsometricVector.None)
            return false;

        //Check if there is a Block / Object ahead?!

        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * 1);
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Switch))
            {
                if (Block.GetComponent<IBodyInteractive>().IInteractive())
                {
                    m_interacte = false;
                    SetInteractiveCheck(false);

                    SetControlStage(false);

                    TurnManager.SetEndStep(Turn, this);

                    return true;
                }
            }
            else
            {
                //...
            }
        }

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

        if (Block == null)
            return;

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
}