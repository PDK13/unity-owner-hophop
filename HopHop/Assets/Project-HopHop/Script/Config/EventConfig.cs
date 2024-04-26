using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "event-config", menuName = "HopHop/Event Config", order = 0)]
public class EventConfig : ScriptableObject
{
    public List<string> Type; //NOTE: Make this to popup in editor
    public List<EventConfigSingle> Data;
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfig))]
public class EventConfigEditor : Editor
{
    private EventConfig m_target;

    private int m_typeCount = 0;
    private int m_dataCount = 0;

    private Vector2 m_scrollType;
    private Vector2 m_scrollData;

    private void OnEnable()
    {
        m_target = target as EventConfig;

        m_typeCount = m_target.Type.Count;
        m_dataCount = m_target.Data.Count;
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        SetGUIGroupType();

        QUnityEditor.SetSpace();

        SetGUIGroupData();

        QUnityEditorCustom.SetApply(this);

        QUnityEditor.SetDirty(m_target);
    }

    //

    private void SetGUIGroupType()
    {
        QUnityEditor.SetLabel("TYPE", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_typeCount = QUnityEditor.SetGroupNumberChangeLimitMin("Type", m_typeCount, 0);

        //COUNT:
        while (m_typeCount > m_target.Type.Count)
            m_target.Type.Add("");
        while (m_typeCount < m_target.Type.Count)
            m_target.Type.RemoveAt(m_target.Type.Count - 1);

        //LIST
        m_scrollType = QUnityEditor.SetScrollViewBegin(m_scrollType);
        for (int i = 0; i < m_target.Type.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
            m_target.Type[i] = QUnityEditor.SetField(m_target.Type[i]);
            QUnityEditor.SetHorizontalEnd();
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private void SetGUIGroupData()
    {
        QUnityEditor.SetLabel("EVENT", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_dataCount = QUnityEditor.SetGroupNumberChangeLimitMin("Event", m_dataCount, 0);

        //COUNT:
        while (m_dataCount > m_target.Data.Count)
            m_target.Data.Add(new EventConfigSingle());
        while (m_dataCount < m_target.Data.Count)
            m_target.Data.RemoveAt(m_target.Data.Count - 1);

        //LIST
        m_scrollData = QUnityEditor.SetScrollViewBegin(m_scrollData);
        for (int i = 0; i < m_target.Data.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
            m_target.Data[i] = QUnityEditor.SetField(m_target.Data[i]);
            QUnityEditor.SetHorizontalEnd();
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif