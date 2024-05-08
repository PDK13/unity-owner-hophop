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

    public Action<bool, IsometricVector> onMove;                    //State
    public Action<bool, IsometricVector> onMoveForce;               //State
    public Action<bool> onGravity;                                  //State
    public Action<bool, IsometricVector> onForce;                   //State, Dir
    public Action<bool, IsometricVector, IsometricVector> onPush;   //State, Dir, From

    #endregion

    #region Turn & Move

    [SerializeField] private bool m_bodyStatic = false; //'Static' in Body Physic mean it can't be Force Move

    private IsometricVector m_moveLastXY;
    private IsometricVector? m_moveForceXY;

    #endregion

    #region Get

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
        m_bodyStatic = m_bodyStatic || KeyInit.GetExist(GetComponent<IsometricDataInit>(), KeyInit.Key.BodyStatic);
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

    public void SetMoveControl(IsometricVector Dir, bool Gravity)
    {
        if (Dir == IsometricVector.None)
            return;

        if (Gravity)
            SetGravityControl(Dir);

        m_moveLastXY = Dir;

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
                SetStandControlForce();
                onMove?.Invoke(false, Dir);
            });

        //NOTE: Not use PUSH option for normal move!
        //SetNextPush(Dir);
    } //Move Invoke!

    public bool SetMoveControlForce()
    {
        if (!m_moveForceXY.HasValue)
            //Fine to continue own control!!
            return false;

        if (m_bodyStatic)
        {
            m_moveForceXY = IsometricVector.None;
            return false;
        }

        SetGravityControl(m_moveForceXY.Value);

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

        SetPushNext(m_moveForceXY.Value);

        return true;
    } //Move Invoke!

    #endregion

    #region Gravity

    public IsometricBlock SetGravityControl(IsometricVector Dir)
    {
        IsometricBlock Block = m_block.GetBlock(Dir, IsometricVector.Bot)[0];
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Bullet))
            {
                //Will touch OBJECT BULLET later!!
            }
            else
            {
                //Can't not Fall ahead!!
                return Block;
            }
        }

        SetGravityControl();

        return null;
    }

    private void SetGravityControl()
    {
        TurnManager.Instance.SetAdd(StepType.Gravity, this);
        TurnManager.Instance.onTurn += ISetTurn;
        TurnManager.Instance.onStepStart += ISetStepStart;
        TurnManager.Instance.onStepEnd += ISetStepEnd;
    }

    private void SetGravity()
    {
        IsometricBlock Block = m_block.GetBlock(IsometricVector.Bot)[0];
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");

                Block.GetComponent<IBodyBullet>().IHit();
            }
            else
            {
                TurnManager.Instance.SetEndStep(StepType.Gravity, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;

                SetStandControlForce();
                onGravity?.Invoke(false);

                return;
            }
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

    public void SetPushControl(IsometricVector Dir, IsometricVector From)
    {
        if (Dir == IsometricVector.None)
            return;

        IsometricBlock BlockNext = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos.Fixed + Dir);

        if (From == IsometricVector.Bot)
        {
            if (BlockNext != null)
            {
                //When Block Bot end move, surely Bot of this will be emty!!
                SetGravityControl();
                return;
            }
        }
        else
        {
            m_moveLastXY = Dir;

            if (BlockNext != null)
            {
                Debug.LogError("[Debug] Push to Wall!!");
                return;
            }
            else
            {
                //Can continue move, so check next pos if it emty at Bot?!
                SetGravityControl(Dir);
            }
        }

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
                SetStandControlForce();
                onPush?.Invoke(false, Dir, From);
            });

        SetPushNext(Dir);
    } //Push Invoke!

    private void SetPushNext(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None || Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
            return;

        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + Dir);
        if (Block != null)
        {
            BodyPhysic BodyPhysic = Block.GetComponent<BodyPhysic>();
            if (BodyPhysic != null)
                BodyPhysic.SetPushControl(Dir, Dir * -1); //Push!!
        }
    }

    #endregion

    #region Stand

    public bool SetStandControlForce()
    {
        if (m_block.GetBlock(IsometricVector.Bot) == null)
            return false;

        if (m_block.GetBlock(IsometricVector.Bot)[0].GetTag(KeyTag.Slow))
        {
            m_moveForceXY = IsometricVector.None;
            return true;
        }
        else
        if (m_block.GetBlock(IsometricVector.Bot)[0].GetTag(KeyTag.Slip))
        {
            m_moveForceXY = m_moveLastXY;
            return true;
        }

        m_moveForceXY = null;
        return false;
    } //Stand Invoke!

    #endregion

    #region Force

    public void SetForceControl(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;

        if (Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
            m_moveLastXY = Dir;

        Vector3 MoveVectorDir = IsometricVector.GetDirVector(Dir);
        Vector3 MoveVectorStart = IsometricVector.GetDirVector(m_block.Pos.Fixed);
        Vector3 MoveVectorEnd = IsometricVector.GetDirVector(m_block.Pos.Fixed) + MoveVectorDir * 1;
        DOTween.To(() => MoveVectorStart, x => MoveVectorEnd = x, MoveVectorEnd, GameManager.Instance.TimeMove * 1)
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
                SetStandControlForce();
                onForce?.Invoke(false, Dir);
            });

        SetForceNext(Dir);
    } //Force Invoke!

    private void SetForceNext(IsometricVector Dir)
    {
        if (Dir == IsometricVector.None)
            return;

        //Top!!
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + IsometricVector.Top);
        if (Block != null)
        {
            BodyPhysic BodyPhysic = Block.GetComponent<BodyPhysic>();
            if (BodyPhysic != null)
            {
                if (Dir == IsometricVector.Top || Dir == IsometricVector.Bot)
                    BodyPhysic.SetForceControl(Dir); //Force!!
                else
                    BodyPhysic.SetPushControl(Dir, IsometricVector.Bot); //Push!!
            }
        }
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