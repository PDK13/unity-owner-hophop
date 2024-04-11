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

    private IsometricVector m_moveLastXY;
    private IsometricVector? m_moveForceXY;

    public IsometricVector MoveLastXY => m_moveLastXY;

    //

    private IsometricBlock m_block;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_bodyStatic = m_bodyStatic || GameConfigInit.GetExist(GetComponent<IsometricDataInit>(), GameConfigInit.Key.BodyStatic);
    }

    #region Turn

    public void ISetTurn(int Turn) { }

    public void ISetStepStart(string Step)
    {
        if (Step != TurnType.Gravity.ToString())
        {
            m_turnActive = false;
            return;
        }
        //
        m_turnActive = true;
        //
        SetControlGravity();
    }

    public void ISetStepEnd(string Step) { }

    #endregion

    #region Move

    public void SetControlMoveReset()
    {
        m_moveLastXY = IsometricVector.None;
    }

    public void SetControlMove(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;
        //
        SetCheckGravity(Dir);
        //
        m_moveLastXY = Dir;
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
        if (!m_moveForceXY.HasValue)
            return false; //Fine to continue own control!!
        //
        if (m_bodyStatic)
        {
            m_moveForceXY = IsometricVector.None;
            return false;
        }
        //
        SetCheckGravity(m_moveForceXY.Value);
        //
        Vector3 MoveDir = IsometricVector.GetVector(m_moveForceXY.Value);
        Vector3 MoveStart = IsometricVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsometricVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
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
        //
        SetNextPush(m_moveForceXY.Value);
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
            if (Block.GetTag(GameConfigTag.Bullet))
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
        TurnManager.SetAdd(TurnType.Gravity, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
    }

    private void SetControlGravity()
    {
        IsometricBlock Block = GetCheckDir(IsometricVector.Bot);
        if (Block != null)
        {
            if (Block.GetTag(GameConfigTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                //
                Block.GetComponent<IBodyBullet>().IHit();
            }
            else
            {
                TurnManager.SetEndStep(TurnType.Gravity, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
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
            m_moveLastXY = Dir;
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
            m_moveLastXY = Dir;
        }
        //
        Vector3 MoveVectorDir = IsometricVector.GetVector(Dir);
        Vector3 MoveVectorStart = IsometricVector.GetVector(m_block.Pos.Fixed);
        Vector3 MoveVectorEnd = IsometricVector.GetVector(m_block.Pos.Fixed) + MoveVectorDir * 1;
        DOTween.To(() => MoveVectorStart, x => MoveVectorEnd = x, MoveVectorEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                onForce?.Invoke(true, Dir);
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsometricVector(MoveVectorEnd);
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
        if (GetCheckDir(IsometricVector.Bot).GetTag(GameConfigTag.Slow))
        {
            m_moveForceXY = IsometricVector.None;
        }
        else
        if (GetCheckDir(IsometricVector.Bot).GetTag(GameConfigTag.Slip))
        {
            m_moveForceXY = m_moveLastXY;
        }
        else
        {
            m_moveForceXY = null;
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

    public void SetEditorBodyStatic()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(GameConfigInit.GetKey(GameConfigInit.Key.BodyStatic));
    }

    public void SetEditorBodyStaticRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(GameConfigInit.GetKey(GameConfigInit.Key.BodyStatic)));
    }

    public bool GetEditorBodyStatic()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(GameConfigInit.GetKey(GameConfigInit.Key.BodyStatic));
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
        //
        QUnityEditorCustom.SetField(m_bodyStatic);
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif