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
    [SerializeField] private bool m_bottom = true; //If TRUE mean can't be control Move by Bottom
    [SerializeField] private bool m_static = false; //If TRUE mean this Body can't be Move same pos with others

    private int m_gravityDurationCurrent = 0;

    private IsometricVector m_moveLastXY;
    private IsometricVector? m_moveForceXY;

    #endregion

    #region Get

    public bool Gravity => m_character == null ? m_gravity : m_character.BodyGravity;

    public bool Static => m_static;

    public bool Bottom => m_character == null ? m_bottom : (Gravity && m_character.BodyBottom);

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
        m_moveForceXY = null;
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

        if (Static)
        {
            SetPush(Dir);
            SetForceTop(Dir);
        }

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

        if (Static)
        {
            SetPush(m_moveForceXY.Value);
            SetForceTop(m_moveForceXY.Value);
        }

        return true;
    }

    #endregion

    #region Gravity

    public bool SetGravityBottom(bool Force = false)
    {
        if (!Gravity)
            return false;

        if (!Force)
        {
            if (GetBodyStatic(IsometricVector.Bot))
                return false;
        }

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

        if (GetBodyStatic(Dir))
        {
            SetGravityBottom(true);
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

    public bool SetForceControl(IsometricVector Dir, IsometricVector From, bool Push = true)
    {
        if (Dir == IsometricVector.None)
            return true;

        if (Push)
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
        if (!Bottom)
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

    #region Push (Side)

    private void SetPush(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None || Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
            return;

        var Block = m_block.GetBlockAll(Dir);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            BodyPhysic BodyPhysic = BlockCheck.GetComponent<BodyPhysic>();
            if (BodyPhysic != null)
                BodyPhysic.SetPushControl(Dir, Dir * -1);
        }
    }

    #endregion

    #region Force (Top & Bot)

    private void SetForceTop(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;

        var Block = m_block.GetBlockAll(IsometricVector.Top);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            BodyPhysic BodyPhysic = BlockCheck.GetComponent<BodyPhysic>();
            if (BodyPhysic != null ? BodyPhysic.Gravity : false)
                BodyPhysic.SetForceControl(Dir, IsometricVector.Bot, Dir != IsometricVector.Bot);
        }
    }

    private void SetForceBot(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;

        var Block = m_block.GetBlockAll(IsometricVector.Bot);
        foreach (var BlockCheck in Block)
        {
            if (BlockCheck == null)
                continue;

            BodyPhysic BodyPhysic = BlockCheck.GetComponent<BodyPhysic>();
            if (BodyPhysic != null ? BodyPhysic.Gravity : false)
                BodyPhysic.SetForceControl(Dir, IsometricVector.Top, Dir != IsometricVector.Top);
        }
    }

    #endregion
}

#if UNITY_EDITOR

//[CustomEditor(typeof(BodyPhysic))]
[CanEditMultipleObjects]
public class BaseBodyEditor : Editor
{
    private BodyPhysic m_target;

    private SerializedProperty m_gravity;
    private SerializedProperty m_bottom;
    private SerializedProperty m_static;

    private void OnEnable()
    {
        m_target = target as BodyPhysic;

        m_gravity = QUnityEditorCustom.GetField(this, "m_gravity");
        m_bottom = QUnityEditorCustom.GetField(this, "m_bottom");
        m_static = QUnityEditorCustom.GetField(this, "m_static");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_gravity);
        QUnityEditorCustom.SetField(m_bottom);
        QUnityEditorCustom.SetField(m_static);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif