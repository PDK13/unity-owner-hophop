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

    public void SetEditorEventDataGet()
    {
        m_eventIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.EventIdentitytBase, false);
        var EventConfigFound = QUnityAssets.GetScriptableObject<EventConfigSingle>(m_eventIdentityBase);
        if (EventConfigFound == null ? EventConfigFound.Count == 0 : false)
        {
            m_block = GetComponent<IsometricBlock>();
            Debug.Log("[BodyEvent] " + m_block.Pos.ToString() + " not found config " + m_eventIdentityBase);
            return;
        }
        //
        if (EventConfigFound.Count > 1)
            Debug.Log("[BodyEvent] " + m_block.Pos.ToString() + " found more than one, get the first one found");
        //
        m_eventIdentityData = EventConfigFound[0];
    }

    public void SetEditorEventIdentityBase()
    {
        SetEditorEventCheckBaseRemove();
        //
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.EventIdentitytBase) + "-" + m_eventIdentityData.name);
    }

    public void SetEditorEventIdentityBase(string Name)
    {
        SetEditorEventCheckBaseRemove();
        //
        m_block = QComponent.GetComponent<IsometricBlock>(this);
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.SetValue(KeyInit.GetKey(KeyInit.Key.EventIdentitytBase) + "-" + Name);
    }

    public void SetEditorEventCheckBaseRemove()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        BlockInit.Data.RemoveAll(t => t.Contains(KeyInit.GetKey(KeyInit.Key.EventIdentitytBase)));
    }

    public bool GetEditorEventBase()
    {
        IsometricDataInit BlockInit = QComponent.GetComponent<IsometricDataInit>(this);
        return BlockInit.Data.Exists(t => t.Contains(KeyInit.GetKey(KeyInit.Key.EventIdentitytBase)));
    }

#endif
}