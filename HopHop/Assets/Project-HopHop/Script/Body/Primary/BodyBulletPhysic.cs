using UnityEngine;

public class BodyBulletPhysic : MonoBehaviour, ITurnManager, IBodyBullet, IBodyPhysic
{
    #region Const

    private const string ANIM_BLOW = "Blow";

    private const float DESTROY_DELAY = 0.3f;

    #endregion

    #region Move

    private IsometricVector m_moveDir;
    private int m_moveDuration = 1;

    private int m_moveDurationCurrent = 0;

    private int m_fallStep = 0;

    #endregion

    #region Get

    public StepType Step => StepType.Bullet;

    private bool StepGravity => m_body.SetGravityControl();

    private bool StepForce => m_body.SetBottomControl();

    public bool StepEnd => m_moveDurationCurrent >= m_moveDuration;

    #endregion

    #region Component

    private Animator m_animator;
    private IsometricBlock m_block;
    private BodyPhysic m_body;

    #endregion

    private void OnDestroy()
    {
        TurnManager.Instance.SetRemove(Step, this);
        TurnManager.Instance.onTurnStart -= ISetTurnStart;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
        TurnManager.Instance.onTurnEnd -= ISetTurnEnd;

        m_body.onGravity -= IGravity;
    }

    #region ITurnManager

    public void ISetTurnStart(int Turn) { }

    public void ISetStepStart(string Step)
    {
        if (Step == this.Step.ToString())
        {
            if (!StepEnd)
                IControl();
        }
    }

    public void ISetStepEnd(string Step) { }

    public void ISetTurnEnd(int Turn)
    {
        m_moveDurationCurrent = 0;
    }

    #endregion

    #region IBodyBullet

    public void IInit(IsometricVector Dir, int Speed)
    {
        m_animator = GetComponent<Animator>();
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();

        TurnManager.Instance.SetInit(Step, this);
        TurnManager.Instance.onTurnStart += ISetTurnStart;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
        TurnManager.Instance.onTurnEnd += ISetTurnEnd;

        m_body.onGravity += IGravity;

        m_moveDir = Dir;
        m_moveDuration = Speed;
    }

    public bool IHit(IsometricBlock Target)
    {
        if (Target == null)
            return false;

        if (m_fallStep >= 10)
        {
            //...
        }

        if (Target.GetTag(KeyTag.Block))
        {
            //...
        }

        if (Target.GetTag(KeyTag.Player))
        {
            Debug.Log("[Debug] Bullet hit Player!!");
        }

        IHit();

        return true;
    } //Destroy when Hit after check Target

    public void IHit()
    {
        TurnManager.Instance.SetEndStep(Step, this);
        TurnManager.Instance.SetRemove(Step, this);
        TurnManager.Instance.onTurnStart -= ISetTurnStart;
        TurnManager.Instance.onStepStart -= ISetStepStart;
        TurnManager.Instance.onStepEnd -= ISetStepEnd;
        TurnManager.Instance.onTurnEnd -= ISetTurnEnd;

        SetControlAnimation(ANIM_BLOW);
        m_block.WorldManager.World.Current.SetBlockRemoveInstant(m_block, DESTROY_DELAY);
    } //Destroy when Hit

    #endregion

    #region IBodyPhysic

    public bool IControl()
    {
        //NOTE: Check Move before excute Move
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + m_moveDir);
        if (IHit(Block))
            return false;
        //NOTE: Fine Move to excute Move

        IControl(m_moveDir);

        return true;
    }

    public bool IControl(IsometricVector Dir)
    {
        m_body.SetMoveControl(Dir);

        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (TurnManager.Instance.StepCurrent.Step == this.Step.ToString() && !StepEnd)
        {
            if (State)
            {
                m_moveDurationCurrent++;
            }
            else
            {
                if (StepEnd || StepGravity || StepForce)
                    TurnManager.Instance.SetEndStep(Step, this);
                else
                    TurnManager.Instance.SetEndMove(Step, this);
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

    #region Animation

    private void SetControlAnimation(string Name)
    {
        m_animator.SetTrigger(Name);
        //m_animator.ResetTrigger(Name);
    }

    #endregion
}