using DG.Tweening;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyPhysic : MonoBehaviour, ITurnManager
{
    private bool m_turnActive = false;
    //
    public Action<bool, IsometricVector> onMove;                    //State
    public Action<bool, IsometricVector> onMoveForce;               //State
    public Action<bool> onGravity;                                  //State
    public Action<bool, IsometricVector> onForce;                   //State, Dir
    public Action<bool, IsometricVector, IsometricVector> onPush;   //State, Dir, From
    //
    [SerializeField] private bool m_bodyStatic = false;
    //
    private IsometricVector MoveLastXY;
    private IsometricVector? MoveForceXY;
    //
#if UNITY_EDITOR

    [Space]
    [SerializeField] private string m_editorBodyStatic;

#endif

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_bodyStatic =
            m_bodyStatic ||
            GameConfigInit.GetExist(GetComponent<IsometricDataInit>(), GameConfigInit.Key.BodyStatic);
    }

    #region Turn

    public bool TurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public void IOnTurn(int Turn) { }

    public void IOnStepStart(string Name)
    {
        if (Name != TurnType.Gravity.ToString())
        {
            m_turnActive = false;
            return;
        }
        //
        m_turnActive = true;
        //
        SetControlGravity();
    }

    public void IOnStepEnd(string Name) { }

    #endregion

    #region Move

    public void SetControlMove(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;
        //
        SetCheckGravity(Dir);
        //
        MoveLastXY = Dir;
        //
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
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
                SetStandOnForce();
                onMove?.Invoke(false, Dir);
            });
        //
        //NOTE: Not use PUSH option for normal move!
        //SetNextPush(Dir);
    }

    public bool SetControlMoveForce()
    {
        if (!MoveForceXY.HasValue)
            return false; //Fine to continue own control!!
        //
        SetCheckGravity(MoveForceXY.Value);
        //
        Vector3 MoveDir = IsometricVector.GetVector(MoveForceXY.Value);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onMoveForce?.Invoke(true, MoveForceXY.Value);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                onMoveForce?.Invoke(false, MoveForceXY.Value);
                MoveForceXY = null;
            });
        //
        SetNextPush(MoveForceXY.Value);
        //
        return true;
    }

    #endregion

    #region Gravity

    public IsometricBlock SetCheckGravity(IsometricVector Dir)
    {
        IsometricBlock Block = GetCheckDir(Dir, IsometricVector.Bot);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameConfigTag.Bullet))
            {
                //Will touch OBJECT BULLET later!!
            }
            else
            {
                //Can't not Fall ahead!!
                return Block;
            }
        }
        //
        SetForceGravity();
        //
        return null;
    }

    private void SetForceGravity()
    {
        TurnManager.SetAdd(TurnType.Gravity, gameObject);
        TurnManager.Instance.onStepStart += IOnStepStart;
    }

    private void SetControlGravity()
    {
        IsometricBlock Block = GetCheckDir(IsometricVector.Bot);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameConfigTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                //
                Block.GetComponent<BodyBullet>().SetHit();
            }
            else
            {
                TurnManager.SetEndTurn(TurnType.Gravity, gameObject);
                TurnManager.Instance.onStepStart -= IOnStepStart;
                //
                SetStandOnForce();
                onGravity?.Invoke(false);
                //
                m_turnActive = false;
                return;
            }
        }
        //
        Vector3 MoveDir = IsometricVector.GetVector(IsometricVector.Bot);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
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
                SetControlGravity();
            });
        //
    }

    #endregion

    #region Push

    public void SetControlPush(IsometricVector Dir, IsometricVector From)
    {
        if (Dir == IsometricVector.None)
            return;
        //
        IsometricBlock BlockNext = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos.Fixed + Dir);
        //
        if (From == IsometricVector.Bot)
        {
            if (BlockNext != null)
            {
                //When Block Bot end move, surely Bot of this will be emty!!
                SetForceGravity();
                return;
            }
        }
        else
        {
            MoveLastXY = Dir;
            //
            if (BlockNext != null)
            {
                Debug.LogError("[Debug] Push to Wall!!");
                return;
            }
            else
            {
                //Can continue move, so check next pos if it emty at Bot?!
                SetCheckGravity(Dir);
            }
        }
        //
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
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
                SetStandOnForce();
                onPush?.Invoke(false, Dir, From);
            });
        //
        SetNextPush(Dir);
    }

    private void SetNextPush(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None || Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
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

    #endregion

    #region Force

    public void SetControlForce(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;
        //
        if (Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
        {
            MoveLastXY = Dir;
        }
        //
        Vector3 MoveDir = IsometricVector.GetVector(Dir);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos.Fixed) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onForce?.Invoke(true, Dir);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                SetStandOnForce();
                onForce?.Invoke(false, Dir);
            });
        //
        SetNextForce(Dir);
    }

    private void SetNextForce(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;
        //
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

    #region Stand On Force

    public void SetStandOnForce()
    {
        if (GetCheckDir(IsometricVector.Bot) == null)
        {
            return;
        }
        //
        if (GetCheckDir(IsometricVector.Bot).Tag.Contains(GameConfigTag.Slow))
        {
            MoveForceXY = IsometricVector.None;
        }
        else
        if (GetCheckDir(IsometricVector.Bot).Tag.Contains(GameConfigTag.Slip))
        {
            MoveForceXY = MoveLastXY;
        }
        else
        {
            MoveForceXY = null;
        }
    }

    #endregion

    #region Check

    public IsometricBlock GetCheckDir(IsometricVector Dir)
    {
        return m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos.Fixed + Dir);
    }

    public IsometricBlock GetCheckDir(IsometricVector Dir, IsometricVector DirNext)
    {
        return m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos.Fixed + Dir + DirNext);
    }

    #endregion

#if UNITY_EDITOR

    public void SetEditorBody()
    {
        m_editorBodyStatic = GameConfigInit.GetKey(GameConfigInit.Key.BodyStatic);
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

    private SerializedProperty m_editorBodyStatic;

    private void OnEnable()
    {
        m_target = target as BodyPhysic;

        m_bodyStatic = QUnityEditorCustom.GetField(this, "m_bodyStatic");

        m_editorBodyStatic = QUnityEditorCustom.GetField(this, "m_editorBodyStatic");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_bodyStatic);
        //
        QUnityEditorCustom.SetField(m_editorBodyStatic);
        //
        if (QUnityEditor.SetButton("Editor Generate"))
            m_target.SetEditorBody();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif