using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BodyInteractiveSwitch : MonoBehaviour, IBodyInteractive, IBodySwitch
{
    #region Const

    private const string ANIM_ON = "On";
    private const string ANIM_OFF = "Off";

    #endregion

    #region Action

    public static Action<string, bool> onSwitch;

    public Action<bool> onState;

    #endregion

    #region Switch

    [SerializeField] private bool m_once = false; //Switch only trigged once time!
    [SerializeField] private bool m_state = true;
    [SerializeField] private bool m_follow = true; //Switch follow value of base switch!

    private bool m_avaibleSwitch = false; //This switch current on check identity!

    private string m_switchIdentityBase;
    private List<string> m_switchIdentityCheck;

    private bool m_activeSwitch = false;

    #endregion

    #region Get

    public bool State => m_avaibleSwitch ? m_state : true; //Value base on state when avaible, else always true!

    public bool AvaibleSwitch => m_avaibleSwitch;

    #endregion

    #region Component

    private Animator m_animator;
    private IsometricBlock m_block;

    #endregion

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_switchIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.SwitchIdentityBase, false);
        m_switchIdentityCheck = KeyInit.GetDataList(GetComponent<IsometricDataInit>(), KeyInit.Key.SwitchIdentityCheck, false);
        //
        m_avaibleSwitch = !string.IsNullOrEmpty(m_switchIdentityBase);
        //
        if (m_avaibleSwitch)
            onSwitch += ISwitchIdentity;
        //
        TurnManager.Instance.onTurnEnd += SetSwitchReset;
        //
        SetControlAnimation();
    }

    private void OnDestroy()
    {
        if (m_avaibleSwitch)
            onSwitch -= ISwitchIdentity;
        //
        TurnManager.Instance.onTurnEnd -= SetSwitchReset;
    }

    #region IBodySwitch

    public void ISwitch()
    {
        ISwitch(!m_state);
    }

    public void ISwitch(bool State)
    {
        if (m_once && m_state)
            return;
        //
        if (m_activeSwitch)
            return;
        m_activeSwitch = true;
        //
        m_state = State;
        onState?.Invoke(m_state);
        //
        SetControlAnimation();
        //
        SetSwitch(m_switchIdentityBase, State);
    }

    public void ISwitchIdentity(string Identity, bool State)
    {
        if (!m_switchIdentityCheck.Exists(t => t == Identity))
            return;
        //
        if (m_activeSwitch)
            return;
        m_activeSwitch = true;
        //
        if (m_follow)
            m_state = State;
        else
            m_state = !m_state;
        onState?.Invoke(m_state);
        //
        SetControlAnimation();
        //
        SetSwitch(m_switchIdentityBase, m_state);
    }

    #endregion

    #region IBodyInteractive

    public bool IInteractive()
    {
        if (m_once)
            ISwitch(true);
        else
            ISwitch();
        return true;
    }

    public bool IInteractive(IsometricVector Dir) { return false; }

    #endregion

    #region Switch

    private void SetSwitchReset(int Turn)
    {
        m_activeSwitch = false;
    }

    private void SetSwitch(string Identity, bool State)
    {
        if (string.IsNullOrEmpty(Identity))
            return;
        //
        onSwitch?.Invoke(Identity, State);
    }

    #endregion

    #region Animation

    private void SetControlAnimation()
    {
        m_animator.SetTrigger(m_state ? ANIM_ON : ANIM_OFF);
    }

    #endregion

#if UNITY_EDITOR

    public void SetEditorSwitchIdentityBase()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string Identity = !string.IsNullOrEmpty(m_block.Identity) ? m_block.Identity : m_block.Pos.ToString();
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.SwitchIdentityBase) + Identity);
    }

    public void SetEditorSwitchIdentityBaseRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.SwitchIdentityBase)));
    }

    public bool GetEditorSwitchIdentityBase()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.SwitchIdentityBase)));
    }

    //

    public void SetEditorSwitchIdentityCheck(IsometricBlock BlockFollow)
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string Identity = !string.IsNullOrEmpty(BlockFollow.Identity) ? BlockFollow.Identity : BlockFollow.Pos.ToString();
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.SwitchIdentityCheck) + Identity);
    }

    public void SetEditorSwitchIdentityCheckRemove(IsometricBlock BlockFollow)
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string BlockCheckIdentity = !string.IsNullOrEmpty(BlockFollow.Identity) ? BlockFollow.Identity : BlockFollow.Pos.ToString();
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.SwitchIdentityCheck) + BlockCheckIdentity));
    }

    public bool GetEditorSwitchIdentityCheck(IsometricBlock BlockFollow)
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        string BlockCheckIdentity = !string.IsNullOrEmpty(BlockFollow.Identity) ? BlockFollow.Identity : BlockFollow.Pos.ToString();
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.SwitchIdentityCheck) + BlockCheckIdentity));
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyInteractiveSwitch))]
[CanEditMultipleObjects]
public class BodySwitchEditor : Editor
{
    private BodyInteractiveSwitch m_target;

    private void OnEnable()
    {
        m_target = target as BodyInteractiveSwitch;
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