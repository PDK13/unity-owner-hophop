using DG.Tweening;
using UnityEngine;

public class ControllerPlayer : MonoBehaviour
{
    private bool m_playerControl = false;

    private ControllerBody m_body;
    private AnimationCharacter m_animation;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<ControllerBody>();
        m_animation = GetComponent<AnimationCharacter>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_body.onGravity += SetGravity;
        m_body.onPush += SetPush;
        m_body.onForce += SetForce;
        //
        TurnManager.SetInit(TurnType.Player, gameObject);
        TurnManager.Instance.onTurn += SetControlTurn;
        TurnManager.Instance.onStepStart += SetControlStep;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        //
        m_body.onGravity -= SetGravity;
        m_body.onPush -= SetPush;
        m_body.onForce -= SetForce;
        //
        TurnManager.SetRemove(TurnType.Player, gameObject);
        TurnManager.Instance.onTurn -= SetControlTurn;
        TurnManager.Instance.onStepStart -= SetControlStep;
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
            if (m_body.MoveForceXY.HasValue)
            {
                SetControlMove(m_body.MoveForceXY.Value);
                m_body.MoveForceXY = null;
                return;
            }
            //
            m_playerControl = true;
        }
    }

    private void SetControlMove(IsometricVector Dir)
    {
        m_body.MoveLastXY = Dir;
        //
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
            if (Block.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                //
                Block.GetComponent<ControllerBullet>().SetHit();
            }
            else
            {
                ControllerBody BlockBody = Block.GetComponent<ControllerBody>();
                //
                if (BlockBody == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return;
                }
                //
                if (!BlockBody.GetCheckDir(Dir, Dir))
                {
                    //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                    return;
                }
                //
                //Fine to continue push this Block ahead!!
            }
        }
        //Fine to continue move to pos ahead!!
        //
        m_playerControl = false;
        //
        m_body.SetCheckGravity(Dir);
        m_animation.SetMove(m_body.GetCheckDir(IsometricVector.Bot), m_body.GetCheckDir(IsometricVector.Bot, Dir));
        //
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //...
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                m_body.SetStandOnForce();
                m_animation.SetStand(m_body.GetCheckDir(IsometricVector.Bot));
                //
                TurnManager.SetEndTurn(TurnType.Player, gameObject); //Follow Player (!)
            });
        //
    }

    #endregion

    #region Body

    private void SetGravity(bool State)
    {
        if (!State)
        {
            m_body.SetStandOnForce();
            m_animation.SetStand(m_body.GetCheckDir(IsometricVector.Bot));
        }
    }

    private void SetPush(bool State, IsometricVector Dir)
    {
        if (State)
        {
            m_animation.SetMove(m_body.GetCheckDir(IsometricVector.Bot), m_body.GetCheckDir(IsometricVector.Bot, Dir));
        }
        else
        {
            m_body.SetStandOnForce();
            m_animation.SetStand(m_body.GetCheckDir(IsometricVector.Bot));
        }
    }

    public void SetForce(bool State)
    {
        if (!State)
        {
            m_body.SetStandOnForce();
            m_animation.SetStand(m_body.GetCheckDir(IsometricVector.Bot));
        }
    }

    #endregion
}