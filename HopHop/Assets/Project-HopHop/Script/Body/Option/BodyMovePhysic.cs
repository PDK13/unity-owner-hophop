using System.Collections.Generic;
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

    protected BodyPhysic m_body;
    protected IsometricBlock m_block;

    protected void Awake()
    {
        m_body = GetComponent<BodyPhysic>();
        m_block = GetComponent<IsometricBlock>();
    }

    protected void Start()
    {
        m_dataMove = m_block.Data.Move;
        //
        if (m_dataMove.Data.Count > 0)
        {
            TurnManager.SetInit(TurnType.MovePhysic, gameObject);
            TurnManager.Instance.onTurn += IOnTurn;
            TurnManager.Instance.onStepStart += IOnStep;
        }
        //
        m_moveCheckAhead = m_block.Data.Init.Data.Exists(t => t == GameConfigInit.MoveCheckAhead);
        m_moveCheckAheadBot = m_block.Data.Init.Data.Exists(t => t == GameConfigInit.MoveCheckAheadBot);
        //
        m_body.onMove += IMoveForce;
        m_body.onForce += IForce;
        m_body.onMoveForce += IMoveForce;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
    }

    protected void OnDestroy()
    {
        if (m_dataMove.Data.Count > 0)
        {
            TurnManager.SetRemove(TurnType.MovePhysic, gameObject);
            TurnManager.Instance.onTurn -= IOnTurn;
            TurnManager.Instance.onStepStart -= IOnStep;
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

    public void IPush(bool State, IsometricVector Dir)
    {
        //...
    }

    //**Editor**

    public void SetEditorCheckMoveAhead()
    {
        IsometricBlock Block = GetComponent<IsometricBlock>();
        if (!Block.Data.Init.Data.Contains(GameConfigInit.MoveCheckAhead))
            Block.Data.Init.Data.Add(GameConfigInit.MoveCheckAhead);
    }

    public void SetEditorCheckMoveAheadBot()
    {
        IsometricBlock Block = GetComponent<IsometricBlock>();
        if (!Block.Data.Init.Data.Contains(GameConfigInit.MoveCheckAheadBot))
            Block.Data.Init.Data.Add(GameConfigInit.MoveCheckAheadBot);
    }

    //**Editor**
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
        QUnityEditorCustom.SetUpdate(this);
        //
        if (QUnityEditor.SetButton("Move Check Ahead"))
            m_target.SetEditorCheckMoveAhead();
        //
        if (QUnityEditor.SetButton("Move Check Ahead Bot"))
            m_target.SetEditorCheckMoveAheadBot();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif