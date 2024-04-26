using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EventManager : SingletonManager<EventManager>
{
    #region Varible: Config

    [SerializeField] private EventConfig m_eventConfig;

    #endregion

    #region Varible: Event

    private EventConfigSingle m_eventCurrent;

    #endregion

    #region Config

#if UNITY_EDITOR

    public void SetConfigFind()
    {
        if (m_eventConfig != null)
            return;

        var eventConfigFound = QUnityAssets.GetScriptableObject<EventConfig>("", false);

        if (eventConfigFound == null)
        {
            Debug.Log("[Event] Config not found, please create one");
            return;
        }

        if (eventConfigFound.Count == 0)
        {
            Debug.Log("[Event] Config not found, please create one");
            return;
        }

        if (eventConfigFound.Count > 1)
            Debug.Log("[Event] Config found more than one, get the first one found");

        m_eventConfig = eventConfigFound[0];

        QUnityEditor.SetDirty(this);
    }

#endif

    #endregion

    public bool SetEventActive(string Identity)
    {
        Debug.Log("[Event] " + Identity);
        return false;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventManager))]
public class EventManagerEditor : Editor
{
    private EventManager m_target;

    private SerializedProperty m_eventConfig;

    private void OnEnable()
    {
        m_target = target as EventManager;

        m_eventConfig = QUnityEditorCustom.GetField(this, "m_eventConfig");

        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_eventConfig);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif