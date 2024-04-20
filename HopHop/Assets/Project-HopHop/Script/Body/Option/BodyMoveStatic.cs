using DG.Tweening;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMoveStatic : MonoBehaviour, ITurnManager
{
    public static Action<string, IsometricVector> onFollow;

    //

    protected bool m_turnActive = false;

    private bool m_avaibleFollow = false; //This follow current on check identity!

    private IsometricDataMove m_move;
    private string m_followIdentityBase;
    private string m_followIdentityCheck;

    private IsometricVector m_turnDir;
    private int m_moveStep = 0;
    private int m_moveStepCurrent = 0;

    private IsometricBlock m_block;
    private BodyTurn m_turn;
    private BodySwitch m_switch;

    //

    public bool State => m_switch != null ? m_switch.State : true;

    public TurnType Turn => m_turn != null ? m_turn.Turn : TurnType.MoveStatic;

    public bool AvaibleSwitch => m_avaibleFollow;

    private bool TurnEnd => m_moveStepCurrent == m_moveStep && m_moveStep > 0;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_turn = GetComponent<BodyTurn>();
        m_switch = GetComponent<BodySwitch>();
    }

    protected void Start()
    {
        m_move = GetComponent<IsometricDataMove>();
        m_followIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.FollowIdentityBase, false);
        m_followIdentityCheck = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.FollowIdentityCheck, false);
        //
        m_avaibleFollow = !string.IsNullOrEmpty(m_followIdentityCheck);
        //
        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                TurnManager.SetInit(Turn, this); //Move-Static
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }
        //
        if (m_avaibleFollow)
            onFollow += SetControlFollow;
    }

    protected void OnDestroy()
    {
        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                TurnManager.SetRemove(Turn, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
            }
        }
        //
        if (m_avaibleFollow)
            onFollow -= SetControlFollow;
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
        if (!State)
        {
            m_turnActive = false;
            TurnManager.SetEndStep(Turn, this);
            return;
        }
        //
        SetControlMove();
    }

    public void ISetStepEnd(string Step) { }

    //Move

    private void SetControlMove()
    {
        if (m_moveStep == 0)
        {
            m_turnDir = m_move.DirCombineCurrent;
            m_moveStep = m_move.Data[m_move.Index].Duration;
            m_moveStep = Mathf.Clamp(m_moveStep, 1, m_moveStep); //Avoid bug by duration 0 value!
            m_moveStepCurrent = 0;
        }
        //
        m_moveStepCurrent++;
        //
        Vector3 MoveDir = IsometricVector.GetVector(m_turnDir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove * 1)
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
                    m_turnActive = false;
                    TurnManager.SetEndStep(Turn, this);
                    //
                    m_turnDir = IsometricVector.None;
                }
                else
                {
                    TurnManager.SetEndMove(Turn, this);
                }
            });
        //
        if (!string.IsNullOrEmpty(m_followIdentityBase))
            SetFollow(m_followIdentityBase, m_turnDir);
        //
        SetMovePush(m_turnDir);
        //
        SetMoveTop(m_turnDir);
        //
        if (TurnEnd)
            m_move.SetDirNext();
    }

    private void SetControlFollow(string Identity, IsometricVector Dir)
    {
        if (m_followIdentityCheck == "" || Identity != m_followIdentityCheck)
        {
            return;
        }
        //
        Vector3 MoveVectorDir = IsometricVector.GetVector(Dir);
        Vector3 MoveVectorStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveVectorEnd = IsometricVector.GetVector(m_block.Pos) + MoveVectorDir * 1;
        DOTween.To(() => MoveVectorStart, x => MoveVectorEnd = x, MoveVectorEnd, GameManager.Instance.TimeMove * 1)
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
        {
            return;
        }
        //
        IsometricBlock BlockPush = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir);
        if (BlockPush != null)
        {
            BodyPhysic BodyPush = BlockPush.GetComponent<BodyPhysic>();
            if (BodyPush != null)
            {
                BodyPush.SetControlPush(Dir, Dir * -1); //Push!!
            }
        }
    }

    private void SetMoveTop(IsometricVector Dir)
    {
        //Top!!
        IsometricBlock BlockTop = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + IsometricVector.Top);
        if (BlockTop != null)
        {
            BodyPhysic BodyTop = BlockTop.GetComponent<BodyPhysic>();
            if (BodyTop != null)
            {
                if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
                {
                    BodyTop.SetControlForce(Dir); //Force!!
                }
                else
                {
                    BodyTop.SetControlPush(Dir, IsometricVector.Bot); //Push!!
                }
            }
        }
    }

    //

    public static void SetFollow(string Identity, IsometricVector Dir)
    {
        if (string.IsNullOrEmpty(Identity))
            return;
        //
        onFollow?.Invoke(Identity, Dir);
    }

    //

#if UNITY_EDITOR

    public void SetEditorFollowIdentityBase()
    {
        SetEditorFollowIdentityRemove();
        //
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase) + m_block.Pos.ToString());
    }

    //

    public void SetEditorFollowIdentityCheck(IsometricBlock BlockFollow)
    {
        SetEditorFollowIdentityRemove();
        //
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck) + BlockFollow.Pos.ToString());
    }

    //

    public void SetEditorFollowIdentityRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase)) || t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck)));
    }

    public bool GetEditorFollowIdentity()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityBase)) || t.Contains(KeyInit.GetKey(KeyInit.Key.FollowIdentityCheck)));
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyMoveStatic))]
[CanEditMultipleObjects]
public class BodyMoveStaticEditor : Editor
{
    private BodyMoveStatic m_target;

    private void OnEnable()
    {
        m_target = target as BodyMoveStatic;
        //
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //QUnityEditorCustom.SetUpdate(this);
        //
        //...
        //
        //QUnityEditorCustom.SetApply(this);
    }
}

#endif