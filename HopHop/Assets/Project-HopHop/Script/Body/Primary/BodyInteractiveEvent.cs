using System.Collections.Generic;
using UnityEngine;

public class BodyInteractiveEvent : MonoBehaviour, IBodyInteractive, ITurnManager
{
    #region Event

    [SerializeField] private bool m_eventTop = false;
    [SerializeField] private bool m_eventSide = false;

    [Space]
    [SerializeField] private bool m_eventPlayer = false;

    private string m_eventIdentityBase;
#if UNITY_EDITOR
    [SerializeField] private EventConfigSingle m_eventIdentityData;
#endif

    private List<IsometricBlock> m_eventInteract = new List<IsometricBlock>();

    #endregion

    #region Get

    public bool EventTop => m_eventTop;

    public bool EventSide => m_eventSide;

    public bool EventAvaible => !string.IsNullOrEmpty(m_eventIdentityBase);

#if UNITY_EDITOR

    public string EditorEventIdentityBase => m_eventIdentityBase;

    public EventConfigSingle EditorEventIdentityData { get => m_eventIdentityData; set => m_eventIdentityData = value; }

#endif

    #endregion

    #region Component

    private IsometricBlock m_block;

    #endregion

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_eventIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.EventIdentitytBase, false);

        if (m_eventTop && EventAvaible)
        {
            TurnManager.Instance.onTurnStart += ISetTurnStart;
            TurnManager.Instance.onStepStart += ISetStepStart;
            TurnManager.Instance.onStepEnd += ISetStepEnd;
            TurnManager.Instance.onTurnEnd += ISetTurnEnd;
        }
    }

    private void OnDestroy()
    {
        if (m_eventTop && EventAvaible)
        {
            TurnManager.Instance.onTurnStart -= ISetTurnStart;
            TurnManager.Instance.onStepStart -= ISetStepStart;
            TurnManager.Instance.onStepEnd -= ISetStepEnd;
            TurnManager.Instance.onTurnEnd -= ISetTurnEnd;
        }
    }

    #region ITurnManager

    public void ISetTurnStart(int Step) { }

    public void ISetStepStart(string Step) { }

    public void ISetStepEnd(string Step)
    {
        List<IsometricBlock> Block = m_block.GetBlockAll(IsometricVector.Top);
        foreach (var BlockCheck in Block)
        {
            if (m_eventInteract.Exists(t => t.Equals(BlockCheck)))
                continue;

            m_eventInteract.Add(BlockCheck);

            if (BlockCheck.GetTag(KeyTag.Player))
                IInteractive();
            else
            if (!m_eventPlayer)
                IInteractive();
        }
    }

    public void ISetTurnEnd(int Step)
    {
        m_eventInteract.Clear();
    }

    #endregion

    #region IBodyInteractive

    public bool IInteractive()
    {
        return EventManager.Instance.SetEventActive(m_eventIdentityBase);
    }

    public bool IInteractive(IsometricVector Dir) { return false; }

    #endregion

#if UNITY_EDITOR

    public void SetEditorEventDataFind()
    {
        m_eventIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.EventIdentitytBase, false);
        //
        if (string.IsNullOrEmpty(m_eventIdentityBase))
            return;
        //
        var EventConfigFound = QUnityAssets.GetScriptableObject<EventConfigSingle>(m_eventIdentityBase, false);
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