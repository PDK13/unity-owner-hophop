using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyPhysic : MonoBehaviour, ITurnManager
{
    //NOTE: Base Physic controller for all Character(s) and Object(s), included Gravity and Collide

    #region Action

    public Action<bool, IsometricVector> onMove;                    //State, Dir
    public Action<bool, IsometricVector> onMoveForce;               //State, Dir
    public Action<bool> onGravity;                                  //State, Block
    public Action<bool, IsometricVector, IsometricVector> onForce;  //State, Dir, From
    public Action<bool, IsometricVector, IsometricVector> onPush;   //State, Dir, From

    #endregion

    #region Turn & Move

    [SerializeField] private bool m_gravity = true;
    [SerializeField] private bool m_force = true;

    private IsometricVector m_moveLastXY;
    private IsometricVector? m_moveForceXY;

    #endregion

    #region Get

    public bool Gravity => m_gravity;

    public IsometricVector MoveLastXY => m_moveLastXY;

    #endregion

    private IsometricBlock m_block;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_force = m_force || KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.BodyStatic);
    }

    #region Turn

    public void ISetTurn(int Turn)
    {
        if (m_block.GetBlock(IsometricVector.Bot * 1).Count == 0)
            SetGravityControl();
    }

    public void ISetStepStart(string Step)
    {
        if (Step == StepType.Gravity.ToString())
            SetGravity();
    }

    public void ISetStepEnd(string Step) { }

    #endregion

    #region Move

    public void SetMoveControlReset()
    {
        m_moveLastXY = IsometricVector.None;
    }

    public bool SetMoveControl(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return true;

        IsometricBlock Block = m_block.GetBlock(Dir)[0];
        if (Block != null ? Block.GetComponent<BodyPhysic>() == null : true)
            return false;

        Vector3 MoveDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onMove?.Invoke(true, Dir);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMove?.Invoke(false, Dir);
            });

        m_moveLastXY = Dir;

        return true;
    }

    public bool SetMoveControlForce()
    {
        if (!m_moveForceXY.HasValue)
            //Fine to continue own control!!
            return false;

        IsometricBlock Block = m_block.GetBlock(m_moveForceXY.Value)[0];
        if (Block != null ? Block.GetComponent<BodyPhysic>() == null : true)
        {
            m_moveForceXY = null;
            return false;
        }

        Vector3 MoveDir = IsometricVector.GetDirVector(m_moveForceXY.Value);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onMoveForce?.Invoke(true, m_moveForceXY.Value);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMoveForce?.Invoke(false, m_moveForceXY.Value);
                m_moveForceXY = null;
            });

        return true;
    }

    #endregion

    #region Gravity

    public bool SetGravityControl()
    {
        if (!m_gravity)
            return false;

        IsometricBlock Block = m_block.GetBlock(IsometricVector.Bot)[0];
        if (Block == null)
            return false;

        TurnManager.Instance.SetAdd(StepType.Gravity, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;

        return true;
    }

    private void SetGravity()
    {
        IsometricBlock Block = m_block.GetBlock(IsometricVector.Bot)[0];
        if (Block != null)
        {
            onGravity?.Invoke(false); //NOTE: Check if this Body can fall thought another Body?

            TurnManager.Instance.SetEndStep(StepType.Gravity, this);
            TurnManager.Instance.onTurn -= ISetTurn;
            TurnManager.Instance.onStepStart -= ISetStepStart;
            TurnManager.Instance.onStepEnd -= ISetStepEnd;

            return;
        }

        Vector3 MoveDir = IsometricVector.GetDirVector(IsometricVector.Bot);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onGravity?.Invoke(true);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                SetGravity();
            });
    }

    #endregion

    #region Push

    public bool SetPushControl(IsometricVector Dir, IsometricVector From)
    {
        if (!m_force)
            return false;

        if (Dir == IsometricVector.None)
            return true;

        IsometricBlock Block = m_block.GetBlock(Dir)[0];
        if (Block != null)
        {
            if (Block.GetComponent<BodyStatic>() != null)
                return false;

            if (Block.GetComponent<BodyPhysic>() != null)
            {
                //Body can be Push thought ahead Body!
            }
        }

        m_moveLastXY = Dir;

        Vector3 MoveDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onPush?.Invoke(true, Dir, From);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onPush?.Invoke(false, Dir, From);
            });

        return true;
    }

    #endregion

    #region Bottom

    public bool SetBottomControl()
    {
        if (!m_force)
            return false;

        IsometricBlock Bot = m_block.GetBlock(IsometricVector.Bot)[0];

        if (Bot == null)
            return false;

        if (Bot.GetTag(KeyTag.Slow))
        {
            m_moveForceXY = IsometricVector.None;
            return true;
        }
        else
        if (Bot.GetTag(KeyTag.Slip))
        {
            m_moveForceXY = m_moveLastXY;
            return true;
        }

        m_moveForceXY = null;
        return false;
    }

    #endregion

#if UNITY_EDITOR

    public void SetEditorBodyStatic()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.BodyStatic));
    }

    public void SetEditorBodyStaticRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.BodyStatic)));
    }

    public bool GetEditorBodyStatic()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(KeyInit.GetKey(KeyInit.Key.BodyStatic));
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyPhysic))]
[CanEditMultipleObjects]
public class BaseBodyEditor : Editor
{
    private BodyPhysic m_target;

    private SerializedProperty m_bodyStatic;

    private void OnEnable()
    {
        m_target = target as BodyPhysic;

        m_bodyStatic = QUnityEditorCustom.GetField(this, "m_bodyStatic");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_bodyStatic);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif