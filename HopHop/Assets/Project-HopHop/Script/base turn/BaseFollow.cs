using UnityEngine;

public class BaseFollow : MonoBehaviour
{
    private bool m_turnControl = false;
    //
    private BasePlayer m_player;
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
        TurnManager.SetInit(TurnType.Follow, gameObject);
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
        TurnManager.SetRemove(TurnType.Follow, gameObject);
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
            if (Name == TurnType.Follow.ToString())
            {
                if (!m_body.SetControlMoveForce())
                {
                    SetControlMove();
                }
                else
                {
                    m_turnControl = false;
                }
            }
        }
    }

    private void SetControlMove()
    {
        if (m_player == null)
        {
            m_turnControl = false;
            TurnManager.SetEndTurn(TurnType.Follow, gameObject); //Follow Object (!)
            //
            return;
        }
        //
    }

    //Player Found!!

    public void SetFollow(BasePlayer Player)
    {
        m_player = Player;
    }

    //Player Forget!!

    private void SetMove(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnControl = false;
            TurnManager.SetEndTurn(TurnType.Follow, gameObject); //Follow Object (!)
        }
    }

    private void SetMoveForce(bool State, IsometricVector Dir)
    {
        if (State)
            m_player = null;
        else
        {
            m_turnControl = false;
            TurnManager.SetEndTurn(TurnType.Follow, gameObject); //Follow Object (!)
        }
    }

    private void SetGravity(bool State)
    {
        if (State)
            m_player = null;
    }

    private void SetPush(bool State, IsometricVector Dir)
    {
        if (State)
            m_player = null;
    }

    public void SetForce(bool State, IsometricVector Dir)
    {
        if (State)
            m_player = null;
    }
}