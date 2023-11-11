using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    private bool m_playerControl = false;
    //
    [HideInInspector] public bool CharacterFollow = false;
    //
    private BaseBody m_body;
    private BaseCharacter m_character;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<BaseBody>();
        m_character = GetComponent<BaseCharacter>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        TurnManager.SetInit(TurnType.Player, gameObject);
        TurnManager.Instance.onTurn += SetControlTurn;
        TurnManager.Instance.onStepStart += SetControlStep;
        //
        m_body.onMove += SetMove;
        m_body.onMoveForce += SetMove;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        //
        TurnManager.SetRemove(TurnType.Player, gameObject);
        TurnManager.Instance.onTurn -= SetControlTurn;
        TurnManager.Instance.onStepStart -= SetControlStep;
        //
        m_body.onMove -= SetMove;
        m_body.onMoveForce -= SetMove;
    }

    private void Update()
    {
        if (!m_playerControl)
        {
            return;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            SetControlMove(IsometricVector.Up);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            SetControlMove(IsometricVector.Down);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            SetControlMove(IsometricVector.Left);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            SetControlMove(IsometricVector.Right);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetControlMove(IsometricVector.None);
        }
    }

    #region Move

    private void SetControlTurn(int Turn)
    {
        //Reset!!
        //
        //...
    }

    private void SetControlStep(string Name)
    {
        if (Name == TurnType.Player.ToString())
        {
            if (!m_body.SetControlMoveForce())
            {
                m_playerControl = true;
            }
        }
    }

    private void SetControlMove(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
        {
            m_playerControl = false;
            TurnManager.SetEndTurn(TurnType.Player, gameObject); //Follow Player (!)
            return;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.World.GetBlockCurrent(m_block.Pos + Dir * Length);
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
                    return;
                }
                //
                if (BlockBody.CharacterPush)
                {
                    if (BlockBody.GetCheckDir(Dir))
                    {
                        //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                        return;
                    }
                    BlockBody.SetControlPush(Dir, m_block.Pos);
                }
                else
                    //Surely can't continue move to this Pos, because this Block can't be push by character!!
                    return;
                //
                //Fine to continue push this Block ahead!!
            }
        }
        //Fine to continue move to pos ahead!!
        //
        m_playerControl = false;
        //
        m_body.SetControlMove(Dir);
    }

    private void SetMove(bool State, IsometricVector Dir)
    {
        if (!State)
            TurnManager.SetEndTurn(TurnType.Player, gameObject); //Follow Player (!)
    }

    #endregion
}