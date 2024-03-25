using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyMoveStatic : MonoBehaviour, ITurnManager
{
    protected bool m_turnActive = false;

    private IsometricDataMove m_move;
    private string m_followIdentity;
    private string m_followIdentityCheck;
    //
    private IsometricVector m_turnDir;
    private int m_turnLength = 0;
    private int m_turnLengthCurrent = 0;

#if UNITY_EDITOR

    [SerializeField] private string m_editorFollowIdentity;
    [SerializeField] private string m_editorFollowIdentityCheck;

#endif

    private bool TurnEnd => m_turnLengthCurrent == m_turnLength && m_turnLength != 0;
    //
    private IsometricBlock m_block;
    //
    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    protected void Start()
    {
        m_move = GetComponent<IsometricDataMove>();
        m_followIdentity = GameConfigInit.GetData(GetComponent<IsometricDataInit>(), GameConfigInit.Key.FollowIdentity, false);
        m_followIdentityCheck = GameConfigInit.GetData(GetComponent<IsometricDataInit>(), GameConfigInit.Key.FollowIdentityCheck, false);
        //
        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                TurnManager.SetInit(TurnType.MoveStatic, this);
                TurnManager.Instance.onTurn += ITurn;
                TurnManager.Instance.onStepStart += IStepStart;
                TurnManager.Instance.onStepEnd += IStepEnd;
            }
        }
        //
        if (m_followIdentityCheck != "")
            GameEvent.onFollow += SetControlFollow;
    }

    protected void OnDestroy()
    {
        if (m_move != null)
        {
            if (m_move.Data.Count > 0)
            {
                TurnManager.SetRemove(TurnType.MoveStatic, this);
                TurnManager.Instance.onTurn -= ITurn;
                TurnManager.Instance.onStepStart -= IStepStart;
                TurnManager.Instance.onStepEnd -= IStepEnd;
            }
        }
        //
        if (m_followIdentityCheck != "")
            GameEvent.onFollow -= SetControlFollow;
    }

    #region Turn

    public bool TurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public void ITurn(int Turn)
    {
        //Reset!!
        m_turnLength = 0;
        m_turnLengthCurrent = 0;
        //
        m_turnActive = true;
    }

    public void IStepStart(string Name)
    {
        if (m_turnActive)
        {
            if (Name == TurnType.MoveStatic.ToString())
            {
                SetControlMove();
            }
        }
    }

    public void IStepEnd(string Name) { }

    #endregion

    #region Move

    private void SetControlMove()
    {
        if (m_turnLength == 0)
        {
            m_turnDir = m_move.DirCombineCurrent;
            m_turnLength = m_move.Data[m_move.Index].Duration;
            m_turnLength = Mathf.Clamp(m_turnLength, 1, m_turnLength); //Avoid bug by duration 0 value!
            m_turnLengthCurrent = 0;
        }
        //
        m_turnLengthCurrent++;
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
                if (TurnEnd)
                {
                    m_turnActive = false;
                    TurnManager.SetEndStep(TurnType.MoveStatic, this);
                    //
                    m_turnDir = IsometricVector.None;
                }
                else
                {
                    TurnManager.SetEndMove(TurnType.MoveStatic, this);
                }
            });
        //
        if (m_followIdentity != "")
            GameEvent.SetFollow(m_followIdentity, m_turnDir);
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
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
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

    #endregion

#if UNITY_EDITOR

    public void SetEditorFollowIdentity()
    {
        m_block = GetComponent<IsometricBlock>();
        m_editorFollowIdentity = GameConfigInit.GetKey(GameConfigInit.Key.FollowIdentity) + "-" + m_block.Pos.ToString();
        m_editorFollowIdentityCheck = GameConfigInit.GetKey(GameConfigInit.Key.FollowIdentityCheck) + "-" + m_block.Pos.ToString();
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyMoveStatic))]
[CanEditMultipleObjects]
public class BodyMoveStaticEditor : Editor
{
    private BodyMoveStatic m_target;

    private SerializedProperty m_editorFollowIdentity;
    private SerializedProperty m_editorFollowIdentityCheck;

    private void OnEnable()
    {
        m_target = target as BodyMoveStatic;

        m_editorFollowIdentity = QUnityEditorCustom.GetField(this, "m_editorFollowIdentity");
        m_editorFollowIdentityCheck = QUnityEditorCustom.GetField(this, "m_editorFollowIdentityCheck");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_editorFollowIdentity);
        QUnityEditorCustom.SetField(m_editorFollowIdentityCheck);
        //
        if (QUnityEditor.SetButton("Editor Generate"))
            m_target.SetEditorFollowIdentity();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif