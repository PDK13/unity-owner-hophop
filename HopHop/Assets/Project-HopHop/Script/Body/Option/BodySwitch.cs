using UnityEditor;
using UnityEngine;

public class BodySwitch : MonoBehaviour, IBodySwitch
{
    [SerializeField] private bool m_state = true;

    //
    private string m_switchIdentity;
    private string m_switchIdentityCheck;
    //
    private IsometricBlock m_block;
    //
    protected virtual void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    protected virtual void Start()
    {
        m_switchIdentity = GameConfigInit.GetData(GetComponent<IsometricDataInit>(), GameConfigInit.Key.SwitchIdentity, false);
        m_switchIdentityCheck = GameConfigInit.GetData(GetComponent<IsometricDataInit>(), GameConfigInit.Key.SwitchIdentityCheck, false);
        //
        GameEvent.onSwitch += ISwitch;
    }

    protected virtual void OnDestroy()
    {
        GameEvent.onSwitch -= ISwitch;
    }

    //

    public void ISwitch(string Identity, bool State)
    {
        if (string.IsNullOrEmpty(m_switchIdentityCheck) || Identity != m_switchIdentityCheck)
            return;
        SetSwitch(Identity, State);
    }

    public void ISwitch(bool State)
    {
        m_state = State;
        GameEvent.SetSwitch(m_switchIdentity, State);
    }

    //

    public bool State => m_state;

    protected virtual void SetSwitch(string Identity, bool State)
    {
        //...
    }

    public virtual void SetSwitch(bool State)
    {
        ISwitch(State);
    }

    public virtual void SetSwitch()
    {
        ISwitch(!m_state);
    }

#if UNITY_EDITOR

    public void SetEditorSwitchIdentity()
    {
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentity) + "-" + m_block.Pos.ToString());
    }

    public void SetEditorSwitchIdentityCheck(IsometricBlock BlockFollow)
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentityCheck) + "-" + BlockFollow.Pos.ToString());
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