using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMovePhysic : MonoBehaviour, ITurnManager, IBodyPhysic
{
    private bool m_turnActive = false;

    private IsometricDataMove m_dataMove;

    private bool m_moveCheckAhead = false;
    private bool m_moveCheckAheadBot = false;

    private IsometricBlock m_block;
    private BodyPhysic m_body;
    private BodySwitch m_switch;

    //

    public bool State => m_switch != null ? m_switch.State : true;

    public TurnType Turn => TurnType.MovePhysic;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
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
                TurnManager.SetInit(Turn, this);
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }
        //
        m_moveCheckAhead = KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.MoveCheckAheadSide);
        m_moveCheckAheadBot = KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.MoveCheckAheadBot);
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
            if (Block.GetTag(KeyTag.Bullet))
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
        m_body.SetControlMove(Dir, true);
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

    public void SetEditorMoveCheckAheadSide()
    {
        m_block ??= QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadSide));
    }

    public void SetEditorMoveCheckAheadSideRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadSide)));
    }

    public bool GetEditorMoveCheckAheadSide()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadSide));
    }

    //

    public void SetEditorMoveCheckAheadBot()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadBot));
    }

    public void SetEditorMoveCheckAheadBotRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadBot)));
    }

    public bool GetEditorMoveCheckAheadBot()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(KeyInit.GetKey(KeyInit.Key.MoveCheckAheadBot));
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