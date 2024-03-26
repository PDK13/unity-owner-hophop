using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMovePhysic : MonoBehaviour, ITurnManager, IBodyPhysic
{
    protected bool m_turnActive = false;

    private IsometricDataMove m_dataMove;

    protected bool m_moveCheckAhead = false;
    protected bool m_moveCheckAheadBot = false;

#if UNITY_EDITOR

    [SerializeField] private string m_eMoveCheckAhead;
    [SerializeField] private string m_eMoveCheckAheadBot;

#endif

    protected BodyPhysic m_body;
    protected IsometricBlock m_block;

    protected void Awake()
    {
        m_body = GetComponent<BodyPhysic>();
        m_block = GetComponent<IsometricBlock>();
    }

    protected void Start()
    {
        m_dataMove = GetComponent<IsometricDataMove>();
        //
        if (m_dataMove != null)
        {
            if (m_dataMove.Data.Count > 0)
            {
                TurnManager.SetInit(TurnType.MovePhysic, this);
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }
        //
        m_moveCheckAhead = GameConfigInit.GetExist(GetComponent<IsometricDataInit>(), GameConfigInit.Key.MoveCheckAhead);
        m_moveCheckAheadBot = GameConfigInit.GetExist(GetComponent<IsometricDataInit>(), GameConfigInit.Key.MoveCheckAheadBot);
        //
        m_body.onMove += IMoveForce;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMoveForce;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
    }

    protected void OnDestroy()
    {
        if (m_dataMove != null)
        {
            if (m_dataMove.Data.Count > 0)
            {
                TurnManager.SetRemove(TurnType.MovePhysic, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
            }
        }
        //
        m_body.onMove -= IMoveForce;
        m_body.onForce -= IForce;
        m_body.onMoveForce -= IMoveForce;
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
        if (Step != TurnType.MovePhysic.ToString())
            return;
        //
        if (!m_body.SetControlMoveForce())
        {
            if (!IMove(m_dataMove.DirCombineCurrent))
            {
                m_dataMove.SetDirRevert();
                m_dataMove.SetDirNext();
                if (!IMove(m_dataMove.DirCombineCurrent))
                {
                    m_dataMove.SetDirRevert();
                    m_dataMove.SetDirNext();
                    //
                    m_turnActive = false;
                    TurnManager.SetEndStep(TurnType.MovePhysic, this);
                }
            }
        }
        else
            m_turnActive = false;
    }

    public void ISetStepEnd(string Step) { }

    //

    public void IMoveForce(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(TurnType.MovePhysic, this);
        }
    }

    public void IForce(bool State, IsometricVector Dir)
    {
        //...
    }

    public bool IMove(IsometricVector Dir)
    {
        if (m_dataMove.DirCombineCurrent == IsometricVector.None)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(TurnType.MovePhysic, this); //Follow Enermy (!)
            return true;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir * Length);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameConfigTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Enermy!!");
                //
                Block.GetComponent<BodyBullet>().SetHit();
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

    public void IGravity(bool State)
    {
        //...
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        //...
    }

#if UNITY_EDITOR

    public void SetEditorMove()
    {
        m_eMoveCheckAhead = GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAhead);
        m_eMoveCheckAheadBot = GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAheadBot);
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyMovePhysic))]
[CanEditMultipleObjects]
public class BodyEnermyMoveEditor : Editor
{
    private BodyMovePhysic m_target;

    private SerializedProperty m_eMoveCheckAhead;
    private SerializedProperty m_eMoveCheckAheadBot;

    private void OnEnable()
    {
        m_target = target as BodyMovePhysic;

        m_eMoveCheckAhead = QUnityEditorCustom.GetField(this, "m_eMoveCheckAhead");
        m_eMoveCheckAheadBot = QUnityEditorCustom.GetField(this, "m_eMoveCheckAheadBot");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_eMoveCheckAhead);
        QUnityEditorCustom.SetField(m_eMoveCheckAheadBot);
        //
        if (QUnityEditor.SetButton("Editor Generate"))
            m_target.SetEditorMove();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif