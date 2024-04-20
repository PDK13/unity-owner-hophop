using UnityEngine;

public class BodyEvent : MonoBehaviour, IBodyInteractive
{
    [SerializeField] private bool m_eventTop = false;
    [SerializeField] private bool m_eventSide = false;

    private string m_eventIdentityBase;
#if UNITY_EDITOR
    [SerializeField] private EventConfigSingle m_eventIdentityData;
#endif

    private IsometricBlock m_block;

    //

    public bool EventTop => m_eventTop;

    public bool EventSide => m_eventSide;

#if UNITY_EDITOR

    public string EditorEventIdentityBase => m_eventIdentityBase;

    public EventConfigSingle EditorEventIdentityData { get => m_eventIdentityData; set => m_eventIdentityData = value; }

#endif

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_eventIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.EventIdentitytBase, false);
    }

    private void OnDestroy()
    {

    }

    //

    public bool IInteractive()
    {
        return GameManager.Instance.SetEventActive(m_eventIdentityBase);
    }

    //

#if UNITY_EDITOR

    public void SetEditorEventDataFind()
    {
        m_eventIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.EventIdentitytBase, false);
        //
        if (string.IsNullOrEmpty(m_eventIdentityBase))
            return;
        //
        var EventConfigFound = QUnityAssets.GetScriptableObject<EventConfigSingle>(m_eventIdentityBase, true);
        if (EventConfigFound == null ? EventConfigFound.Count == 0 : false)
        {
            m_block = GetComponent<IsometricBlock>();
            Debug.Log("[BodyEvent] " + m_block.Pos.ToString() + " not found config " + m_eventIdentityBase);
            return;
        }

        EventConfigFound.RemoveAll(t => t.name != m_eventIdentityBase);

        if (EventConfigFound.Count > 1)
        {
            m_block = GetComponent<IsometricBlock>();
            Debug.Log("[BodyEvent] " + m_block.Pos.ToString() + " found more than one, get the first one found");
        }

        m_eventIdentityData = EventConfigFound[0];
        m_eventIdentityBase = m_eventIdentityData.name;
    }

    //

    public void SetEditorEventIdentityBaseFind()
    {
        m_eventIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.EventIdentitytBase, false);
    }

    public void SetEditorEventIdentityBase()
    {
        SetEditorEventIdentityBaseRemove();

        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.EventIdentitytBase) + m_eventIdentityData.name);

        m_eventIdentityBase = m_eventIdentityData.name;
    }

    public void SetEditorEventIdentityBaseRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.EventIdentitytBase)));
    }

    public bool GetEditorEventIdentityBase()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.EventIdentitytBase)));
    }

#endif
}