using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerObject : MonoBehaviour
{
    private bool m_turnControl = false;

    private IsometricDataBlockMove m_dataMove;
    private string m_dataFollow;

    private IsometricVector m_turnDir;
    private int m_turnLength = 0;
    private int m_turnLengthCurrent = 0;

    private bool TurnEnd => m_turnLengthCurrent == m_turnLength && m_turnLength != 0;

    private ControllerBody m_body;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<ControllerBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_dataMove = m_block.Data.Move;
        //
        if (m_block.Data.Follow.KeySet == GameManager.GameConfig.Event.Follow)
            m_dataFollow = m_block.Data.Follow.KeyGet;
        //
        if (m_dataMove != null)
        {
            if (m_dataMove.DataExist)
            {
                GameTurn.SetInit(TurnType.Object, this.gameObject);
                GameTurn.Instance.onTurn += SetControlTurn;
                GameTurn.Instance.onStepStart += SetControlStep;
            }
            else
            if (m_dataFollow != null)
            {
                GameEvent.onFollow += SetControlFollow;
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        //
        if (m_dataMove != null)
        {
            if (m_dataMove.DataExist)
            {
                GameTurn.SetRemove(TurnType.Object, this.gameObject);
                GameTurn.Instance.onTurn -= SetControlTurn;
                GameTurn.Instance.onStepStart -= SetControlStep;
            }
            else
            if (m_dataFollow != null)
            {
                GameEvent.onFollow -= SetControlFollow;
            }
        }
    }

    private void SetControlTurn(int Turn)
    {
        //Reset!!
        m_turnLength = 0;
        m_turnLengthCurrent = 0;
        //
        m_turnControl = true;
    }

    private void SetControlStep(string Name)
    {
        if (m_turnControl)
        {
            if (Name == TurnType.Object.ToString())
            {
                SetControlMove();
            }
        }
    }

    private void SetControlMove()
    {
        if (m_turnLength == 0)
        {
            m_turnDir = IsometricVector.GetDir(m_dataMove.DirList[m_dataMove.Index]) * m_dataMove.Quantity;
            m_turnLength = m_dataMove.LengthList[m_dataMove.Index];
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
                    m_turnControl = false;
                    GameTurn.SetEndTurn(TurnType.Object, this.gameObject); //Follow Object (!)
                    //
                    m_turnDir = IsometricVector.None;
                }
                else
                    GameTurn.SetEndMove(TurnType.Object, this.gameObject); //Follow Object (!)
            });
        //
        GameEvent.SetFollow(m_dataFollow, m_turnDir);
        //
        SetMovePush(m_turnDir);
        //
        SetMoveTop(m_turnDir);
        //
        if (TurnEnd)
        {
            m_dataMove.Index += m_dataMove.Quantity;
            //
            if (m_dataMove.TypeList == DataBlockType.Forward && m_dataMove.Index > m_dataMove.DataCount - 1)
            {
                //End Here!!
            }
            else
            if (m_dataMove.TypeList == DataBlockType.Loop && m_dataMove.Index > m_dataMove.DataCount - 1)
            {
                m_dataMove.Index = 0;
            }
            else
            if (m_dataMove.TypeList == DataBlockType.Revert && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.DataCount - 1))
            {
                m_dataMove.Quantity *= -1;
                m_dataMove.Index += m_dataMove.Quantity;
            }
        } 
    }

    private void SetControlFollow(string KeyFollow, IsometricVector Dir)
    {
        if (KeyFollow != m_dataFollow)
            return;
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
            return;
        //
        IsometricBlock BlockPush = m_block.WorldManager.World.GetBlockCurrent(m_block.Pos + Dir);
        if (BlockPush != null)
        {
            ControllerBody BodyPush = BlockPush.GetComponent<ControllerBody>();
            if (BodyPush != null)
            {
                BodyPush.SetControlPush(Dir, Dir * -1); //Push!!
            }
        }
    }

    private void SetMoveTop(IsometricVector Dir)
    {
        //Top!!
        IsometricBlock BlockTop = m_block.WorldManager.World.GetBlockCurrent(m_block.Pos + IsometricVector.Top);
        if (BlockTop != null)
        {
            ControllerBody BodyTop = BlockTop.GetComponent<ControllerBody>();
            if (BodyTop != null)
            {
                if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
                    BodyTop.SetControlForce(Dir); //Force!!
                else
                    BodyTop.SetControlPush(Dir, IsometricVector.Bot); //Push!!
            }
        }
    }
}