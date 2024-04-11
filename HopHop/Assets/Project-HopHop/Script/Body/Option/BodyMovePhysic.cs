using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMovePhysic : MonoBehaviour, ITurnManager, IBodyPhysic
{
    private bool m_turnActive = false;

    public bool State => m_switch != null ? m_switch.State : true;

    public TurnType Turn => m_turn != null ? m_turn.Turn : TurnType.MovePhysic;

    private IsometricDataMove m_dataMove;

    private bool m_moveCheckAhead = false;
    private bool m_moveCheckAheadBot = false;

    private IsometricBlock m_block;
    private BodyTurn m_turn;
    private BodyPhysic m_body;
    private BodySwitch m_switch;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_turn = GetComponent<BodyTurn>();
        m_body = GetComponent<BodyPhysic>();
        m_switch = GetComponent<BodySwitch>();
    }

    private void Start()
    {
        m_dataMove = GetComponent<IsometricDataMove>();
        //
        if (m_dataMove != null)
        {
            if (m_dataMove.Data.Count > 0)
            {
                TurnManager.SetInit(Turn, this); //Move-Physic
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }
        //
        m_moveCheckAhead = GameConfigInit.GetExist(GetComponent<IsometricDataInit>(), GameConfigInit.Key.MoveCheckAhead);
        m_moveCheckAheadBot = GameConfigInit.GetExist(GetComponent<IsometricDataInit>(), GameConfigInit.Key.MoveCheckAheadBot);
        //
        m_body.onMove += IMove;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMove;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
    }

    private void OnDestroy()
    {
        if (m_dataMove != null)
        {
            if (m_dataMove.Data.Count > 0)
            {
                TurnManager.SetRemove(Turn, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
            }
        }
        //
        m_body.onMove -= IMove;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMove;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;
    }

    //

    public bool ITurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public void ISetTurn(int Turn)
    {
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
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(Turn, this);
            return;
        }
        //
        if (!m_body.SetControlMoveForce())
        {
            if (!IControl(m_dataMove.DirCombineCurrent))
            {
                m_dataMove.SetDirRevert();
                m_dataMove.SetDirNext();
                if (!IControl(m_dataMove.DirCombineCurrent))
                {
                    m_dataMove.SetDirRevert();
                    m_dataMove.SetDirNext();
                    //
                    m_turnActive = false;
                    TurnManager.SetEndStep(Turn, this);
                }
            }
        }
        else
            m_turnActive = false;
    }

    public void ISetStepEnd(string Step) { }

    //

    public bool IControl(IsometricVector Dir)
    {
        if (m_dataMove.DirCombineCurrent == IsometricVector.None)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(Turn, this); //Follow Enermy (!)
            return true;
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
                Debug.Log("[Debug] Bullet hit Enermy!!");
                //
                Block.GetComponent<IBodyBullet>().IHit();
            }
            else
            if (m_moveCheckAhead)
                //Stop Ahead when there is an burden ahead!!
                return false;
            //else
            {
                //None Stop Ahead and continue check move ahead!!
                //
                BodyPhysic BlockBody = Block.GetComponent<BodyPhysic>();
                //
                if (BlockBody == null)
                {
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return false;
                }
                //Fine to continue push this Block ahead!!
            }
        }
        else
        if (m_moveCheckAheadBot)
        {
            //Continue check move Ahead Bot!!
            //
            IsometricBlock BlockBot = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length + IsometricVector.Bot);
            if (BlockBot == null)
                //Stop Ahead because no block ahead bot!!
                return false;
        }
        //Fine to continue move to pos ahead!!
        //
        m_turnActive = false;
        //
        m_body.SetControlMove(Dir);
        //
        m_dataMove.SetDirNext();
        //
        return true;
    }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(Turn, this);
        }
    }

    public void IForce(bool State, IsometricVector Dir)
    {
        //...
    }

    public void IGravity(bool State)
    {
        //...
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        //...
    }

#if UNITY_EDITOR

    public void SetEditorMoveCheckAhead()
    {
        m_block ??= QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAhead));
    }

    public void SetEditorMoveCheckAheadRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAhead)));
    }

    public bool GetEditorMoveCheckAhead()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAhead));
    }

    //

    public void SetEditorMoveCheckAheadBot()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAheadBot));
    }

    public void SetEditorMoveCheckAheadBotRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAheadBot)));
    }

    public bool GetEditorMoveCheckAheadBot()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAheadBot));
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyMovePhysic))]
[CanEditMultipleObjects]
public class BodyEnermyMoveEditor : Editor
{
    private BodyMovePhysic m_target;

    private void OnEnable()
    {
        m_target = target as BodyMovePhysic;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //
        //QUnityEditorCustom.SetUpdate(this);
        //
        //...
        //
        //QUnityEditorCustom.SetApply(this);
    }
}

#endif