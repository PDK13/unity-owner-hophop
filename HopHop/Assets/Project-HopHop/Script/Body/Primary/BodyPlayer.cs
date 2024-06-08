using System.Collections.Generic;
using UnityEngine;

public class BodyPlayer : MonoBehaviour, ITurnManager, IBodyPhysic, IBodyInteractive, IBodyCommand
{
    public const string IDENTITY = "player";

    #region Move

    private int m_moveDurationCurrent = 0;

    #endregion

    #region Interactive

    private bool m_interacteActive = false;

    #endregion

    #region Command

    private List<IsometricVector> m_commandMove = new List<IsometricVector>();
    private int m_commandMoveCurrent = 0;

    #endregion

    #region Get

    public StepType Step => StepType.Player;

    public bool StepEnd => m_moveDurationCurrent >= m_character.MoveStep;

    private bool StepGravity => StepEnd || !m_character.MoveFloat ? m_body.SetGravityBottom() : false;

    private bool StepForce => m_body.SetBottomControl();

    public bool CommandEnd => m_commandMoveCurrent >= m_commandMove.Count - 1;

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
        CharacterManager.Instance.onCharacter += OnCharacter;

        TurnManager.Instance.SetInit(Step, this);
        TurnManager.Instance.onTurnStart += ISetTurnStart;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
        TurnManager.Instance.onTurnEnd += ISetTurnEnd;

        m_body.onMove += IMove;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMoveForce;
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
        TurnManager.Instance.onTurnStart -= ISetTurnStart;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
        TurnManager.Instance.onTurnEnd -= ISetTurnEnd;

        m_body.onMove -= IMove;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMoveForce;
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

    public void ISetTurnStart(int Turn) { }

    public void ISetStepStart(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            if (!CommandEnd)
                IControl(m_commandMove[m_commandMoveCurrent]);
        }
        else
        if (Step == this.Step.ToString())
        {
            if (!m_body.SetMoveControlForce())
                IControl();
            else
                m_moveDurationCurrent = int.MaxValue;
        }
    }

    public void ISetStepEnd(string Step)
    {
        if (Step == StepType.EventCommand.ToString())
        {
            m_commandMove.Clear();
            m_commandMoveCurrent = 0;
        }
    }

    public void ISetTurnEnd(int Turn)
    {
        m_moveDurationCurrent = 0;
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

        if (m_body.SetMoveControl(Dir, !m_character.MoveFloat))
        {
            ICollide(Dir);
            SetControlStage(false);
        }

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step == StepType.EventCommand.ToString())
        {
            if (State)
            {
                m_commandMoveCurrent++;
            }
            else
            {
                if (CommandEnd)
                    TurnManager.Instance.SetEndStep(StepType.EventCommand, this);
                else
                    TurnManager.Instance.SetEndMove(StepType.EventCommand, this);
            }
        }
        else
        if (TurnManager.Instance.StepCurrent.Step == this.Step.ToString())
        {
            if (State)
            {
                m_moveDurationCurrent++;
            }
            else
            {
                bool End = StepEnd;
                bool Gravity = StepGravity;
                bool Force = End || !m_character.MoveFloat ? StepForce : false;
                if (End || Gravity || Force)
                    TurnManager.Instance.SetEndStep(Step, this);
                else
                {
                    TurnManager.Instance.SetEndMove(Step, this);
                    if (m_character.MoveLock)
                        IControl(m_body.MoveLastXY);
                    else
                        SetControlStage(true);
                }
            }
        }
    }

    public void IMoveForce(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step == this.Step.ToString())
        {
            if (State)
            {
                m_moveDurationCurrent++;
            }
            else
            {
                bool Gravity = StepGravity;
                bool Force = StepForce;
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
            m_body.SetGravityBottom();
            m_body.SetBottomControl();
        }
    }

    public void IGravity(bool State, int Duration)
    {
        if (State)
        {
            //...
        }
        else
        {
            if (Duration >= 10)
            {
                //...
            }

            m_body.SetBottomControl();
        }
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        if (State)
        {

        }
        else
        {
            m_body.SetGravityBottom();
            m_body.SetBottomControl();
        }
    }

    public void ICollide(IsometricVector Dir)
    {
        var Block = m_block.GetBlockAll(Dir);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            if (BlockCheck.Tag.Contains(KeyTag.Bullet))
            {
                Debug.Log("Player hit Bullet!");
                BlockCheck.GetComponent<IBodyBullet>().IHit();
            }
            if (BlockCheck.Tag.Contains(KeyTag.Dark))
            {
                Debug.Log("Player hit Enermy");
            }
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