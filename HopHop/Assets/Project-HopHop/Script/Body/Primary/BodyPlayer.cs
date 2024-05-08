using System.Collections.Generic;
using UnityEngine;

public class BodyPlayer : MonoBehaviour, ITurnManager, IBodyPhysic, IBodyInteractive, IBodyCommand
{
    #region Move

    private int m_moveStepCurrent = 0;

    private int m_fallStep = 0;

    #endregion

    #region Interactive

    private bool m_interacteActive = false;

    #endregion

    #region Command

    private List<IsometricVector> m_commandMove = new List<IsometricVector>();
    private int m_commandMoveIndex = 0;

    #endregion

    #region Get

    public StepType Step => StepType.Player;

    private bool StepEnd => m_moveStepCurrent >= m_character.MoveStep && m_character.MoveStep > 0;

    private bool StepCommandEnd => m_commandMoveIndex >= m_commandMove.Count;

    private bool StepGravity => StepEnd || !m_character.MoveFloat ? m_body.SetGravityControl() : false;

    private bool StepForce => m_body.SetBottomControl();

    #endregion

    #region Componenet

    private IsometricBlock m_block;
    private BodyPhysic m_body;
    private BodyCharacter m_character;

    #endregion

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();
        m_character = GetComponent<BodyCharacter>();
    }

    private void Start()
    {
        WorldManager.Instance.Player = m_block;

        CharacterManager.Instance.onCharacter += OnCharacter;

        TurnManager.Instance.SetInit(Step, this);
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

        CharacterManager.Instance.onCharacter -= OnCharacter;

        TurnManager.Instance.SetRemove(Step, this);
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

    #region Control

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
        if (m_interacteActive)
            IInteractive(IsometricVector.Up);
        else
            IControl(IsometricVector.Up);
    }

    private void OnControlDown()
    {
        if (m_interacteActive)
            IInteractive(IsometricVector.Down);
        else
            IControl(IsometricVector.Down);
    }

    private void OnControlLeft()
    {
        if (m_interacteActive)
            IInteractive(IsometricVector.Left);
        else
            IControl(IsometricVector.Left);
    }

    private void OnControlRight()
    {
        if (m_interacteActive)
            IInteractive(IsometricVector.Right);
        else
            IControl(IsometricVector.Right);
    }

    private void OnControlStand()
    {
        if (m_interacteActive)
            IInteractive(IsometricVector.None);
        else
            IControl(IsometricVector.None);
    }

    private void OnControlInteracte()
    {
        m_interacteActive = !m_interacteActive;
        SetInteractiveCheck(m_interacteActive);
    }

    #endregion

    #region Character

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

    #endregion

    #region ITurnManager

    public void ISetTurn(int Turn) { }

    public void ISetStepStart(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            if (!StepCommandEnd)
            {
                IControl(m_commandMove[m_commandMoveIndex]);
                m_commandMoveIndex++;
            }
        }
        else
        if (Step == this.Step.ToString())
        {
            if (!m_body.SetMoveControlForce())
                IControl();
        }
    }

    public void ISetStepEnd(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            m_commandMove.Clear();
            m_commandMoveIndex = 0;
        }
        else
        if (Step == this.Step.ToString())
        {
            m_moveStepCurrent = 0;
        }
    }

    #endregion

    #region IBodyPhysic

    public bool IControl()
    {
        SetControlStage(true);
        return true;
    }

    public bool IControl(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
        {
            SetControlStage(false);
            m_body.SetMoveControlReset();
            TurnManager.Instance.SetEndStep(Step, this);
            return false;
        }

        //NOTE: Check Move before excute Move
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
                BodyPhysic BodyPhysic = Block.GetComponent<BodyPhysic>();

                if (BodyPhysic == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return false;
                }
            }
        }
        //NOTE: Fine Move to excute Move

        SetControlStage(false);

        m_body.SetMoveControl(Dir);

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step == StepType.EventCommand.ToString() && !StepCommandEnd)
        {
            if (State)
            {
                //...
            }
            else
            {
                if (StepCommandEnd)
                    TurnManager.Instance.SetEndStep(StepType.EventCommand, this);
                else
                    TurnManager.Instance.SetEndMove(StepType.EventCommand, this);
            }
        }
        else
        if (TurnManager.Instance.StepCurrent.Step == this.Step.ToString() && !StepEnd)
        {
            if (State)
            {

            }
            else
            {
                m_moveStepCurrent++;

                bool End = StepEnd;
                bool Gravity = StepGravity;
                bool Force = StepForce;
                if (End || Gravity || Force)
                {
                    SetControlStage(false);
                    TurnManager.Instance.SetEndStep(Step, this);
                }
                else
                {
                    TurnManager.Instance.SetEndMove(Step, this);

                    if (m_character.MoveLock)
                    {
                        SetControlStage(false);
                        IControl(m_body.MoveLastXY);
                    }
                    else
                    {
                        SetControlStage(true);
                    }
                }
            }
        }
        else
        {
            if (!State)
            {
                //NOTE: Check end Step or Move of this when this Move out of owner Step
                SetControlStage(false);
                TurnManager.Instance.SetEndStep(Step, this);
            }
        }
    }

    public void IForce(bool State, IsometricVector Dir, IsometricVector From)
    {
        if (State)
        {

        }
        else
        {
            m_body.SetGravityControl();
            m_body.SetBottomControl();
        }
    }

    public void IGravity(bool State)
    {
        if (State)
        {
            m_fallStep++;
        }
        else
        {
            if (m_fallStep >= 10)
            {
                //...
            }

            m_body.SetBottomControl();

            m_fallStep = 0;
        }
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        if (State)
        {

        }
        else
        {
            m_body.SetGravityControl();
            m_body.SetBottomControl();
        }
    }

    #endregion

    #region IBodyInteractiveActive

    public bool IInteractive() { return false; }

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
                    m_interacteActive = false;
                    SetInteractiveCheck(false);

                    SetControlStage(false);

                    TurnManager.Instance.SetEndStep(Step, this);

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

    #endregion

    #region Interactive

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

    #endregion

    #region IBodyCommand

    public void ISetCommandMove(IsometricVector Dir)
    {
        TurnManager.Instance.SetAdd(StepType.EventCommand, this);
        m_commandMove.Add(Dir);
    }

    #endregion
}