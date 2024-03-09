using UnityEditor;
using UnityEngine;

public class BodySwitch : MonoBehaviour, IBodySwitch
{
    [SerializeField] private bool m_state = true;

    //
    private string m_switchIdentity;
    private string m_switchIdentityCheck;
    //
#if UNITY_EDITOR

    [SerializeField] private string m_editorSwitchIdentity;
    [SerializeField] private string m_editorSwitchIdentityCheck;

#endif
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
        m_block = GetComponent<IsometricBlock>();
        m_editorSwitchIdentity = GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentity) + "-" + m_block.Pos.ToString();
        m_editorSwitchIdentityCheck = GameConfigInit.GetKey(GameConfigInit.Key.SwitchIdentityCheck) + "-" + m_block.Pos.ToString();
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodySwitch))]
[CanEditMultipleObjects]
public class BodySwitchEditor : Editor
{
    private BodySwitch m_target;

    private SerializedProperty m_editorSwitchIdentity;
    private SerializedProperty m_editorSwitchIdentityCheck;

    private void OnEnable()
    {
        m_target = target as BodySwitch;

        m_editorSwitchIdentity = QUnityEditorCustom.GetField(this, "m_editorSwitchIdentity");
        m_editorSwitchIdentityCheck = QUnityEditorCustom.GetField(this, "m_editorSwitchIdentityCheck");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_editorSwitchIdentity);
        QUnityEditorCustom.SetField(m_editorSwitchIdentityCheck);
        //
        if (QUnityEditor.SetButton("Editor Generate"))
            m_target.SetEditorSwitchIdentity();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif