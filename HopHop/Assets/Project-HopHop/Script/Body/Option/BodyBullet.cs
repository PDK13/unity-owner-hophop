using DG.Tweening;
using UnityEngine;

public class BodyBullet : MonoBehaviour, ITurnManager, IBodyBullet
{
    private const string ANIM_BLOW = "Blow";

    //

    private const float DESTROY_DELAY = 0.3f;

    //

    private bool m_turnActive = false;

    public TurnType Turn => m_turn != null ? m_turn.Turn : TurnType.Bullet;

    //

    private int m_speed = 1;

    private IsometricVector m_turnDir;
    private int m_moveStep = 0;
    private int m_moveStepCurrent = 0;

    private bool TurnEnd => m_moveStepCurrent == m_moveStep && m_moveStep != 0;

    //

    private Animator m_animator;
    private IsometricBlock m_block;
    private BodyPhysic m_body;
    private BodyTurn m_turn;

    //

    private void OnDestroy()
    {
        TurnManager.SetRemove(Turn, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
        //
        if (m_body != null)
            m_body.onGravity -= SetGravity;
    }

    //Turn

    public void ISetTurn(int Turn)
    {
        //Reset!!
        m_moveStep = 0;
        m_moveStepCurrent = 0;
        //
        m_turnActive = true;
    }

    public void ISetStepStart(string Step)
    {
        if (!m_turnActive)
            return;
        //
        if (Step != Turn.ToString())
            return;
        //
        SetControlMove();
    }

    public void ISetStepEnd(string Step) { }

    public void IInit(IsometricVector Dir, int Speed)
    {
        m_animator = GetComponent<Animator>();
        m_block = GetComponent<IsometricBlock>();
        m_turn = GetComponent<BodyTurn>();
        m_body = GetComponent<BodyPhysic>();
        //
        TurnManager.SetInit(Turn, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
        //
        if (m_body != null)
            m_body.onGravity += SetGravity;
        //
        m_speed = Speed;
        m_turnDir = Dir;
        //
        m_turnActive = false;
    }

    public void IHit()
    {
        m_turnActive = false;
        TurnManager.SetEndStep(Turn, this);
        TurnManager.SetRemove(Turn, this);
        TurnManager.Instance.onTurn -= ISetTurn;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
        //
        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.World.Current.SetBlockRemoveInstant(m_block, DESTROY_DELAY);
    }

    //Move

    private void SetControlMove()
    {
        if (m_moveStep == 0)
        {
            m_moveStep = m_speed;
            m_moveStepCurrent = 0;
        }
        //
        m_moveStepCurrent++;
        //
        IsometricBlock BlockAhead = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + m_turnDir);
        if (BlockAhead != null)
        {
            if (BlockAhead.GetTag(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            else
            if (BlockAhead.GetTag(GameConfigTag.Enermy))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");
            }
            //
            IHit();
            //
            return;
        }
        //
        if (m_body != null)
        {
            m_body.SetCheckGravity(m_turnDir);
        }
        //
        Vector3 MoveVectorDir = IsometricVector.GetVector(m_turnDir);
        Vector3 MoveVectorStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveVectorEnd = IsometricVector.GetVector(m_block.Pos) + MoveVectorDir * 1;
        DOTween.To(() => MoveVectorStart, x => MoveVectorEnd = x, MoveVectorEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //Start Animation!!
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveVectorEnd);
            })
            .OnComplete(() =>
            {
                //End Animation!!
                //
                if (TurnEnd)
                {
                    m_turnActive = false;
                    TurnManager.SetEndStep(Turn, this);
                }
                else
                {
                    TurnManager.SetEndMove(Turn, this);
                }
                //
                //Check if Bot can't stand on!!
                //
                SetStandOn();
                //
            });
    }

    private void SetStandOn()
    {
        if (m_body != null)
        {
            IsometricBlock BlockBot = m_body.GetCheckDir(IsometricVector.Bot);
            if (BlockBot == null)
            {
                return;
            }
            //
            if (BlockBot.GetTag(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                IHit();
            }
            //
            if (!BlockBot.GetTag(GameConfigTag.Block))
            {
                IHit();
            }
        }
    }

    //Body

    private void SetGravity(bool State)
    {
        if (!State)
        {
            SetStandOn();
        }
    }

    //Animation

    private void SetControlAnimation(string Name)
    {
        m_animator.SetTrigger(Name);
        //m_animator.ResetTrigger(Name);
    }
}