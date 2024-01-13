using DG.Tweening;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    private const string ANIM_BLOW = "Blow";

    private const float DESTROY_DELAY = 0.3f;

    private int m_speed = 1;

    private bool m_turnControl = false;

    private IsometricVector m_turnDir;
    private int m_turnLength = 0;
    private int m_turnLengthCurrent = 0;

    private bool TurnEnd => m_turnLengthCurrent == m_turnLength && m_turnLength != 0;

    private Animator m_animator;
    private BaseBody m_body;
    private IsometricBlock m_block;

    public void SetInit(IsometricVector Dir, int Speed)
    {
        m_animator = GetComponent<Animator>();
        m_body = GetComponent<BaseBody>();
        m_block = GetComponent<IsometricBlock>();
        //
        TurnManager.SetInit(TurnType.Object, gameObject);
        TurnManager.Instance.onTurn += SetControlTurn;
        TurnManager.Instance.onStepStart += SetControlStep;
        //
        if (m_body != null)
        {
            m_body.onGravity += SetGravity;
        }
        //
        m_speed = Speed;
        m_turnDir = Dir;
        //
        m_turnControl = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        //
        TurnManager.SetRemove(TurnType.Object, gameObject);
        TurnManager.Instance.onTurn -= SetControlTurn;
        TurnManager.Instance.onStepStart -= SetControlStep;
        //
        if (m_body != null)
        {
            m_body.onGravity -= SetGravity;
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
            m_turnLength = m_speed;
            m_turnLengthCurrent = 0;
        }
        //
        m_turnLengthCurrent++;
        //
        IsometricBlock BlockAhead = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + m_turnDir);
        if (BlockAhead != null)
        {
            if (BlockAhead.Tag.Contains(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            else
            if (BlockAhead.Tag.Contains(GameConfigTag.Enermy))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");
            }
            //
            SetHit();
            //
            return;
        }
        //
        if (m_body != null)
        {
            m_body.SetCheckGravity(m_turnDir);
        }
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
                //
                if (TurnEnd)
                {
                    m_turnControl = false;
                    TurnManager.SetEndTurn(TurnType.Object, gameObject); //Follow Object (!)
                }
                else
                {
                    TurnManager.SetEndMove(TurnType.Object, gameObject); //Follow Object (!)
                }
                //
                //Check if Bot can't stand on!!
                //
                SetStandOn();
                //
            });
    }

    public void SetHit()
    {
        m_turnControl = false;
        TurnManager.SetEndTurn(TurnType.Object, gameObject); //Follow Object (!)
        TurnManager.SetRemove(TurnType.Object, gameObject);
        TurnManager.Instance.onTurn -= SetControlTurn;
        TurnManager.Instance.onStepStart -= SetControlStep;
        //
        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.World.Current.SetBlockRemoveInstant(m_block, DESTROY_DELAY);
    } //This is touched by other object!!

    public void SetStandOn()
    {
        if (m_body != null)
        {
            IsometricBlock BlockBot = m_body.GetCheckDir(IsometricVector.Bot);
            if (BlockBot == null)
            {
                return;
            }
            //
            if (BlockBot.Tag.Contains(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //
            if (!BlockBot.Tag.Contains(GameConfigTag.Block))
            {
                SetHit();
            }
        }
    }

    #region Body

    private void SetGravity(bool State)
    {
        if (!State)
        {
            SetStandOn();
        }
    }

    #endregion

    #region Animation

    private void SetControlAnimation(string Name)
    {
        m_animator.SetTrigger(Name);
        //m_animator.ResetTrigger(Name);
    }

    #endregion
}