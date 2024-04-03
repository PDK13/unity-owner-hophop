using UnityEngine;

public class BodyPlayer : MonoBehaviour, ITurnManager, IBodyPhysic, IBodyInteractiveActive
{
    private bool m_turnActive = false;

    private bool m_interactive = false;

    private BodyPhysic m_body;
    private IsometricBlock m_block;

    //

    private void Awake()
    {
        m_body = GetComponent<BodyPhysic>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        TurnManager.SetInit(TurnType.Player, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
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
        TurnManager.SetRemove(TurnType.Player, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
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
        if (Input.GetKeyDown(KeyCode.E))
            m_interactive = !m_interactive;
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
    }

    //Turn

    public bool TurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public void ISetTurn(int Turn)
    {
        //Reset!!
        //
        //...
    }

    public void ISetStepStart(string Step)
    {
        if (Step != TurnType.Player.ToString())
            return;
        //
        if (m_body.SetControlMoveForce())
            return;
        //
        m_turnActive = true;
    }

    public void ISetStepEnd(string Step) { }

    //Move

    public void IMoveForce(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(TurnType.Player, this);
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
            TurnManager.SetEndStep(TurnType.Player, this); //Follow Player (!)
            return false;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length);
        if (Block != null)
        {
            if (Block.GetTag(GameConfigTag.Bullet))
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

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        //...
    }

    public bool IInteractive(IsometricVector Dir)
    {
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * 1);
        if (Block != null)
        {
            if (Block.GetTag(GameConfigTag.Switch))
            {
                if (Block.GetComponent<IBodyInteractive>().IInteractive())
                {
                    m_interactive = false;
                    //
                    m_turnActive = false;
                    TurnManager.SetEndStep(TurnType.Player, this);
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
}