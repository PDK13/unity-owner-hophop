using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMovePhysic : MonoBehaviour, IBodyPhysic
{
    protected bool m_turnActive = false;

    private IsometricDataMove m_dataMove;

    protected bool m_moveCheckAhead = false;
    protected bool m_moveCheckAheadBot = false;

#if UNITY_EDITOR

    [SerializeField] private string m_editorMoveCheckAhead;
    [SerializeField] private string m_editorMoveCheckAheadBot;

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
                TurnManager.SetInit(TurnType.MovePhysic, gameObject);
                TurnManager.Instance.onTurn += IOnTurn;
                TurnManager.Instance.onStepStart += IOnStep;
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
                TurnManager.SetRemove(TurnType.MovePhysic, gameObject);
                TurnManager.Instance.onTurn -= IOnTurn;
                TurnManager.Instance.onStepStart -= IOnStep;
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

    public void IOnTurn(int Turn)
    {
        m_turnActive = true;
    }

    public void IOnStep(string Name)
    {
        if (m_turnActive)
        {
            if (Name == TurnType.MovePhysic.ToString())
            {
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
                            TurnManager.SetEndTurn(TurnType.MovePhysic, gameObject);
                        }
                    }
                }
                else
                    m_turnActive = false;
            }
        }
    }

    //

    public void IMoveForce(bool State, IsometricVector Dir)
    {
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndTurn(TurnType.MovePhysic, gameObject);
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
            TurnManager.SetEndTurn(TurnType.MovePhysic, gameObject); //Follow Enermy (!)
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
        m_editorMoveCheckAhead = GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAhead);
        m_editorMoveCheckAheadBot = GameConfigInit.GetKey(GameConfigInit.Key.MoveCheckAheadBot);
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyMovePhysic))]
[CanEditMultipleObjects]
public class BodyEnermyMoveEditor : Editor
{
    private BodyMovePhysic m_target;

    private SerializedProperty m_editorMoveCheckAhead;
    private SerializedProperty m_editorMoveCheckAheadBot;

    private void OnEnable()
    {
        m_target = target as BodyMovePhysic;

        m_editorMoveCheckAhead = QUnityEditorCustom.GetField(this, "m_editorMoveCheckAhead");
        m_editorMoveCheckAheadBot = QUnityEditorCustom.GetField(this, "m_editorMoveCheckAheadBot");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_editorMoveCheckAhead);
        QUnityEditorCustom.SetField(m_editorMoveCheckAheadBot);
        //
        if (QUnityEditor.SetButton("Editor Generate"))
            m_target.SetEditorMove();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif