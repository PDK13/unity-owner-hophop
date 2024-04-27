using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "event-config", menuName = "HopHop/Event Config", order = 0)]
public class EventConfig : ScriptableObject
{
    public List<string> Event; //NOTE: Make this to popup in editor
    public List<EventConfigSingle> Data;
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfig))]
public class EventConfigEditor : Editor
{
    private EventConfig m_target;

    private int m_eventCount = 0;
    private int m_dataCount = 0;

    private bool m_eventArrayCommand = false;
    private bool m_dataArrayCommand = false;

    private Vector2 m_scrollType;
    private Vector2 m_scrollData;

    Event m_event;

    private void OnEnable()
    {
        m_target = target as EventConfig;

        m_eventCount = m_target.Event.Count;
        m_dataCount = m_target.Data.Count;
    }

    private void OnDisable()
    {
        SetConfigFixed();
    }

    private void OnDestroy()
    {
        SetConfigFixed();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        SetGUIGroupEvent();

        QUnityEditor.SetSpace();

        SetGUIGroupData();

        QUnityEditorCustom.SetApply(this);

        QUnityEditor.SetDirty(m_target);
    }

    //

    private void SetConfigFixed()
    {
        bool RemoveEmty = false;
        int Index = 0;
        while (Index < m_target.Event.Count)
        {
            if (string.IsNullOrEmpty(m_target.Event[Index]))
            {
                RemoveEmty = true;
                m_target.Event.RemoveAt(Index);
            }
            else
                Index++;
        }
        QUnityEditor.SetDirty(m_target);
        //
        if (RemoveEmty)
            Debug.Log("[Dialogue] Author(s) emty have been remove from list");
    }

    //

    private void SetGUIGroupEvent()
    {
        QUnityEditor.SetLabel("EVENT", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_eventCount = QUnityEditor.SetGroupNumberChangeLimitMin("Event", m_eventCount, 0);

        //COUNT:
        while (m_eventCount > m_target.Event.Count)
            m_target.Event.Add("");
        while (m_eventCount < m_target.Event.Count)
            m_target.Event.RemoveAt(m_target.Event.Count - 1);

        //LIST
        m_scrollType = QUnityEditor.SetScrollViewBegin(m_scrollType);
        for (int i = 0; i < m_target.Event.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_eventArrayCommand = !m_eventArrayCommand;

            m_target.Event[i] = QUnityEditor.SetField(m_target.Event[i]);

            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_eventArrayCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    QList.SetSwap(m_target.Event, i, i - 1);
                }
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    QList.SetSwap(m_target.Event, i, i + 1);
                }
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    m_target.Event.RemoveAt(i);
                    m_eventCount--;
                }
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private void SetGUIGroupData()
    {
        QUnityEditor.SetLabel("DATA", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_dataCount = QUnityEditor.SetGroupNumberChangeLimitMin("Data", m_dataCount, 0);

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
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_dataArrayCommand = !m_dataArrayCommand;

            m_target.Data[i] = QUnityEditor.SetField(m_target.Data[i]);

            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_dataArrayCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    QList.SetSwap(m_target.Data, i, i - 1);
                }
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    QList.SetSwap(m_target.Data, i, i + 1);
                }
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    m_target.Data.RemoveAt(i);
                    m_dataCount--;
                }
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif