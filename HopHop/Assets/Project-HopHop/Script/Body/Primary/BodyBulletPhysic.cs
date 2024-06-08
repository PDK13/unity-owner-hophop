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

    private bool m_hit = false;

    #endregion

    #region Get

    public StepType Step => StepType.Bullet;

    private bool StepGravity => m_body.SetGravityBottom();

    private bool StepForce => m_body.SetBottomControl();

    public bool StepEnd => m_moveDurationCurrent >= m_moveDuration;

    #endregion

    #region Component

    private Animator m_animator;
    private IsometricBlock m_block;
    private BodyPhysic m_body;

    #endregion

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();
    }

    private void Start()
    {
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
    }

    private void OnDestroy()
    {
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
        m_moveDir = Dir;
        m_moveDuration = Speed;
    } //Init Bullet moving

    public void IHit()
    {
        if (m_hit)
            return;
        m_hit = true;

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
        IControl(m_moveDir);
        return true;
    }

    public bool IControl(IsometricVector Dir)
    {
        ICollide(Dir);
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
                bool End = StepEnd;
                bool Gravity = StepGravity;
                bool Force = StepForce;
                if (End || Gravity || Force)
                    TurnManager.Instance.SetEndStep(Step, this);
                else
                    TurnManager.Instance.SetEndMove(Step, this);
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

            if (BlockCheck.Tag.Contains(KeyTag.Player))
            {
                Debug.Log("Bullet hit Player");
                IHit();
            }
            if (BlockCheck.Tag.Contains(KeyTag.Bullet))
            {
                Debug.Log("Bullet hit Bullet!");
                BlockCheck.GetComponent<IBodyBullet>().IHit();
            }
            if (BlockCheck.Tag.Contains(KeyTag.Dark))
            {
                Debug.Log("Bullet hit Enermy");
            }
            IHit();
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