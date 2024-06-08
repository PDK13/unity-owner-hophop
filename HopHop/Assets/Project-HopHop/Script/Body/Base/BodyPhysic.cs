using DG.Tweening;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyPhysic : MonoBehaviour, ITurnManager
{
    //NOTE: Base Physic controller for all Character(s) and Object(s), included Gravity and Collide

    #region Action

    public Action<bool, IsometricVector> onMove;                    //State, Dir
    public Action<bool, IsometricVector> onMoveForce;               //State, Dir
    public Action<bool, int> onGravity;                             //State, Duration
    public Action<bool, IsometricVector, IsometricVector> onForce;  //State, Dir, From
    public Action<bool, IsometricVector, IsometricVector> onPush;   //State, Dir, From

    #endregion

    #region Turn & Move

    [SerializeField] private bool m_gravity = true;
    [SerializeField] private bool m_dynamic = true; //If TRUE mean can't be control Move by Push, Bottom, etc
    [SerializeField] private bool m_static = false; //If TRUE mean this Body can't be Move same pos with others

    private int m_gravityDurationCurrent = 0;

    private IsometricVector m_moveLastXY;
    private IsometricVector? m_moveForceXY;

    #endregion

    #region Get

    public bool Gravity => m_character == null ? m_gravity : m_character.BodyGravity;

    public bool Dynamic => m_character == null ? m_dynamic : m_character.BodyDynamic;

    public bool Static => m_static;

    public IsometricVector MoveLastXY => m_moveLastXY;

    public IsometricVector? MoveForceXY => m_moveForceXY;

    #endregion

    private IsometricBlock m_block;
    private BodyCharacter m_character;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_character = GetComponent<BodyCharacter>();
    }

    #region ITurnManager

    public void ISetTurnStart(int Turn)
    {
        if (!GetBodyStatic(IsometricVector.Bot))
            SetGravityBottom();
    }

    public void ISetStepStart(string Step)
    {
        if (Step == StepType.Gravity.ToString())
            SetGravity();
    }

    public void ISetStepEnd(string Step) { }

    public void ISetTurnEnd(int Turn) { }

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

        if (GetBodyStatic(Dir))
            return false;

        Vector3 MoveDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos) + MoveDir;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMove?.Invoke(false, Dir);
            });
        onMove?.Invoke(true, Dir);

        m_moveLastXY = Dir;

        return true;
    }

    public bool SetMoveControlForce()
    {
        if (!m_moveForceXY.HasValue)
            //Fine to continue own control!!
            return false;

        if (GetBodyStatic(m_moveForceXY.Value))
            return false;

        Vector3 MoveDir = IsometricVector.GetDirVector(m_moveForceXY.Value);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos) + MoveDir;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMoveForce?.Invoke(false, m_moveForceXY.Value);
                m_moveForceXY = null;
            });
        onMoveForce?.Invoke(true, m_moveForceXY.Value);

        return true;
    }

    #endregion

    #region Gravity

    public bool SetGravityBottom()
    {
        if (!Gravity)
            return false;

        if (GetBodyStatic(IsometricVector.Bot))
            return false;

        TurnManager.Instance.SetAdd(StepType.Gravity, this);
        TurnManager.Instance.onTurnStart += ISetTurnStart;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
        TurnManager.Instance.onTurnEnd += ISetTurnEnd;

        return true;
    }

    private void SetGravity()
    {
        if (GetBodyStatic(IsometricVector.Bot))
        {
            TurnManager.Instance.onTurnStart -= ISetTurnStart;
            TurnManager.Instance.onStepStart -= ISetStepStart;
            TurnManager.Instance.onStepEnd -= ISetStepEnd;
            TurnManager.Instance.onTurnEnd -= ISetTurnEnd;

            onGravity?.Invoke(false, m_gravityDurationCurrent);

            TurnManager.Instance.SetEndStep(StepType.Gravity, this);

            m_gravityDurationCurrent = 0; //Reset Fall!

            return;
        }

        m_gravityDurationCurrent++;

        Vector3 MoveDir = IsometricVector.GetDirVector(IsometricVector.Bot);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos.Fixed) + MoveDir;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                TurnManager.Instance.SetEndMove(StepType.Gravity, this);
            });
        onGravity?.Invoke(true, m_gravityDurationCurrent);
    }

    #endregion

    #region Push

    public bool SetPushControl(IsometricVector Dir, IsometricVector From)
    {
        if (Dir == IsometricVector.None)
            return true;

        if (From != IsometricVector.Top && From != IsometricVector.Bot)
        {
            if (!Dynamic)
                return false;
        }

        if (GetBodyStatic(Dir))
        {
            Debug.LogWarningFormat("[Body] {0} be Push to Static", this.name);
            return false;
        }

        if (Dir != IsometricVector.Top && From != IsometricVector.Bot)
            m_moveLastXY = Dir;

        Vector3 MoveDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos.Fixed) + MoveDir;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onPush?.Invoke(false, Dir, From);
            });
        onPush?.Invoke(true, Dir, From);

        return true;
    }

    #endregion

    #region Force

    public bool SetForceControl(IsometricVector Dir, IsometricVector From, bool Check = true)
    {
        if (Dir == IsometricVector.None)
            return true;

        if (Check)
            return SetPushControl(Dir, From);

        Vector3 MoveDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveStart = IsometricVector.GetDirVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetDirVector(m_block.Pos.Fixed) + MoveDir;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.Instance.TimeMove)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onForce?.Invoke(false, Dir, From);
            });
        onForce?.Invoke(true, Dir, From);

        return true;
    }

    #endregion

    #region Bottom

    public bool SetBottomControl()
    {
        if (!Dynamic)
            return false;

        var Block = m_block.GetBlockAll(IsometricVector.Bot);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            if (BlockCheck.GetTag(KeyTag.Slow))
            {
                m_moveForceXY = IsometricVector.None;
                return true;
            }
            else
            if (BlockCheck.GetTag(KeyTag.Slip))
            {
                m_moveForceXY = m_moveLastXY;
                return true;
            }

            m_moveForceXY = null;

            return false;
        }

        return false;
    }

    #endregion

    #region Static

    public bool GetBodyStatic(IsometricVector Dir)
    {
        var Block = m_block.GetBlockAll(Dir);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            BodyStatic BodyStatic = BlockCheck.GetComponent<BodyStatic>();
            if (BodyStatic != null)
                return true;

            BodyPhysic BodyPhysic = BlockCheck.GetComponent<BodyPhysic>();
            if (BodyPhysic != null)
            {
                if (BodyPhysic.m_static)
                    return true;
            }
        }
        return false;
    }

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyPhysic))]
[CanEditMultipleObjects]
public class BaseBodyEditor : Editor
{
    private BodyPhysic m_target;

    private SerializedProperty m_gravity;
    private SerializedProperty m_dynamic;
    private SerializedProperty m_static;

    private void OnEnable()
    {
        m_target = target as BodyPhysic;

        m_gravity = QUnityEditorCustom.GetField(this, "m_gravity");
        m_dynamic = QUnityEditorCustom.GetField(this, "m_dynamic");
        m_static = QUnityEditorCustom.GetField(this, "m_static");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_gravity);
        QUnityEditorCustom.SetField(m_dynamic);
        QUnityEditorCustom.SetField(m_static);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif