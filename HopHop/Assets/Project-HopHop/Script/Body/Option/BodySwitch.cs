using System;
using UnityEditor;
using UnityEngine;

public class BodySwitch : MonoBehaviour, IBodyInteractive, IBodySwitch
{
    private const string ANIM_ON = "On";
    private const string ANIM_OFF = "Off";
    private const string ANIM_SWITCH = "Switch";

    //

    public static Action<string, bool> onSwitch;

    //

    [SerializeField] private bool m_once = false;
    [SerializeField] private bool m_state = true;

    public bool State => m_avaible ? m_state : true; //Value base on state when avaible, else always true

    //

    private bool m_avaible = false; //This switch curret on check identity

    public bool Avaible => m_avaible;

    //

    public Action<bool> onState;

    //

    private string m_switchIdentity;
    private string m_switchIdentityCheck;

    //

    private Animator m_animator;
    private IsometricBlock m_block;

    //

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_switchIdentity = GameConfigInit.GetData(GetComponent<IsometricDataInit>(), GameConfigInit.Key.SwitchIdentity, false);
        m_switchIdentityCheck = GameConfigInit.GetData(GetComponent<IsometricDataInit>(), GameConfigInit.Key.SwitchIdentityCheck, false);
        //
        m_avaible = string.IsNullOrEmpty(m_switchIdentityCheck);
        //
        if (!m_avaible)
            onSwitch += ISwitchIdentity;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(m_switchIdentityCheck))
            onSwitch -= ISwitchIdentity;
    }

    //

    public void ISwitchIdentity(string Identity, bool State)
    {
        if (Identity != m_switchIdentityCheck)
            return;
        //
        m_state = State;
        onState?.Invoke(State);
        //
        SetControlAnimation(m_state ? ANIM_ON : ANIM_OFF);
        SetControlAnimation(ANIM_SWITCH);
    }

    public void ISwitch(bool State)
    {
        if (m_once && m_state)
            return;
        //
        m_state = State;
        onState?.Invoke(State);
        //
        SetControlAnimation(m_state ? ANIM_ON : ANIM_OFF);
        SetControlAnimation(ANIM_SWITCH);
        //
        SetSwitch(m_switchIdentity, State);
    }

    public void ISwitch()
    {
        ISwitch(!m_state);
    }

    public bool IInteractive()
    {
        if (m_once)
            ISwitch(true);
        else
            ISwitch();
        return true;
    }

    //

    public static void SetSwitch(string Identity, bool State)
    {
        if (string.IsNullOrEmpty(Identity))
            return;
        //
        onSwitch?.Invoke(Identity, State);
    }

    //Animation

    private void SetControlAnimation(string Name)
    {
        m_animator.SetTrigger(Name);
        //m_animator.ResetTrigger(Name);
    }

#if UNITY_EDITOR

    public void SetEditorSwitchIdentity()
    {
        SetEditorMoveCheckAheadRemove();
        //
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentity) + "-" + m_block.Pos.ToString());
    }

    public void SetEditorSwitchIdentityCheck(IsometricBlock BlockFollow)
    {
        SetEditorMoveCheckAheadRemove();
        //
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentityCheck) + "-" + BlockFollow.Pos.ToString());
    }

    public void SetEditorMoveCheckAheadRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentity)));
    }

    public bool GetEditorMoveCheckAhead()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Contains(GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentity));
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodySwitch))]
[CanEditMultipleObjects]
public class BodySwitchEditor : Editor
{
    private BodySwitch m_target;

    private void OnEnable()
    {
        m_target = target as BodySwitch;
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