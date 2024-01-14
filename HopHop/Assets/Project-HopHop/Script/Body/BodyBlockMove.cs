using DG.Tweening;
using UnityEngine;

public class BodyBlockMove : BodyBlock
{
    private IsometricDataMove m_dataMove;
    private IsometricDataFollow m_dataFollow;
    //
    private IsometricVector m_turnDir;
    private int m_turnLength = 0;
    private int m_turnLengthCurrent = 0;

    private bool TurnEnd => m_turnLengthCurrent == m_turnLength && m_turnLength != 0;
    //
    private IsometricBlock m_block;
    //
    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    protected override void Start()
    {
        m_dataMove = m_block.Data.Move;
        m_dataFollow = m_block.Data.Follow;
        //
        if (m_dataMove.DataExist)
        {
            TurnManager.SetInit(TurnType.Block, gameObject);
            TurnManager.Instance.onTurn += IOnTurn;
            TurnManager.Instance.onStepStart += IOnStep;
        }
        //
        if (m_dataFollow.IdentityGet != "")
        {
            GameEvent.onFollow += SetControlFollow;
        }
    }

    protected override void OnDestroy()
    {
        if (m_dataMove.DataExist)
        {
            TurnManager.SetRemove(TurnType.Block, gameObject);
            TurnManager.Instance.onTurn -= IOnTurn;
            TurnManager.Instance.onStepStart -= IOnStep;
        }
        //
        if (m_dataFollow.IdentityGet != "")
        {
            GameEvent.onFollow -= SetControlFollow;
        }
    }

    //

    public override void IOnTurn(int Turn)
    {
        //Reset!!
        m_turnLength = 0;
        m_turnLengthCurrent = 0;
        //
        m_turnActive = true;
    }

    public override void IOnStep(string Name)
    {
        if (m_turnActive)
        {
            if (Name == TurnType.Block.ToString())
            {
                SetControlMove();
            }
        }
    }

    //

    private void SetControlMove()
    {
        if (m_turnLength == 0)
        {
            m_turnDir = IsometricVector.GetDir(m_dataMove.Dir[m_dataMove.Index]) * m_dataMove.Quantity;
            m_turnLength = m_dataMove.Duration[m_dataMove.Index];
            m_turnLengthCurrent = 0;
        }
        //
        m_turnLengthCurrent++;
        //
        Vector3 MoveDir = IsometricVector.GetVector(m_turnDir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                //End Animation!!
                if (TurnEnd)
                {
                    m_turnActive = false;
                    TurnManager.SetEndTurn(TurnType.Block, gameObject); //Follow Object (!)
                    //
                    m_turnDir = IsometricVector.None;
                }
                else
                {
                    TurnManager.SetEndMove(TurnType.Block, gameObject); //Follow Object (!)
                }
            });
        //
        if (m_dataFollow.Identity != "")
        {
            GameEvent.SetFollow(m_dataFollow.Identity, m_turnDir);
        }
        //
        SetMovePush(m_turnDir);
        //
        SetMoveTop(m_turnDir);
        //
        if (TurnEnd)
        {
            m_dataMove.Index += m_dataMove.Quantity;
            //
            if (m_dataMove.Type == DataBlockType.Forward && m_dataMove.Index > m_dataMove.DataCount - 1)
            {
                //End Here!!
            }
            else
            if (m_dataMove.Type == DataBlockType.Loop && m_dataMove.Index > m_dataMove.DataCount - 1)
            {
                m_dataMove.Index = 0;
            }
            else
            if (m_dataMove.Type == DataBlockType.Revert && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.DataCount - 1))
            {
                m_dataMove.Quantity *= -1;
                m_dataMove.Index += m_dataMove.Quantity;
            }
        }
    }

    private void SetControlFollow(string Identity, IsometricVector Dir)
    {
        if (Identity != m_dataFollow.IdentityGet)
        {
            return;
        }
        //
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                //End Animation!!
            });
        //
        SetMovePush(Dir);
        //
        SetMoveTop(Dir);
        //
    }

    private void SetMovePush(IsometricVector Dir)
    {
        if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
        {
            return;
        }
        //
        IsometricBlock BlockPush = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir);
        if (BlockPush != null)
        {
            BodyPhysic BodyPush = BlockPush.GetComponent<BodyPhysic>();
            if (BodyPush != null)
            {
                BodyPush.SetControlPush(Dir, Dir * -1); //Push!!
            }
        }
    }

    private void SetMoveTop(IsometricVector Dir)
    {
        //Top!!
        IsometricBlock BlockTop = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + IsometricVector.Top);
        if (BlockTop != null)
        {
            BodyPhysic BodyTop = BlockTop.GetComponent<BodyPhysic>();
            if (BodyTop != null)
            {
                if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
                {
                    BodyTop.SetControlForce(Dir); //Force!!
                }
                else
                {
                    BodyTop.SetControlPush(Dir, IsometricVector.Bot); //Push!!
                }
            }
        }
    }
}