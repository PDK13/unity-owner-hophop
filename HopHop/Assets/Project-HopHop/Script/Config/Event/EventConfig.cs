using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "event-config", menuName = "HopHop/Event Config", order = 0)]
public class EventConfig : ScriptableObject
{
    public List<EventConfigSingle> Data;

#if UNITY_EDITOR

    public int EditorDataListCount
    {
        get => Data.Count;
        set
        {
            while (Data.Count > value)
                Data.RemoveAt(Data.Count - 1);
            while (Data.Count < value)
                Data.Add(null);
        }
    }

    public bool EditorDataListCommand { get; set; } = false;

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfig))]
public class EventConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 300f;
    private const float LABEL_WIDTH = 65f;

    private EventConfig m_target;

    private Vector2 m_scrollType;
    private Vector2 m_scrollData;

    private void OnEnable()
    {
        m_target = target as EventConfig;
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

        SetGUIGroupData();

        QUnityEditorCustom.SetApply(this);

        QUnityEditor.SetDirty(m_target);
    }

    //

    private void SetConfigFixed()
    {
        bool RemoveEmty = false;
        int Index = 0;
        while (Index < m_target.Data.Count)
        {
            if (m_target.Data[Index] == null)
            {
                RemoveEmty = true;
                m_target.Data.RemoveAt(Index);
            }
            else
                Index++;
        }
        QUnityEditor.SetDirty(m_target);

        if (RemoveEmty)
            Debug.Log("[Dialogue] Data(s) emty have been remove from list");
    }

    //

    private void SetGUIGroupData()
    {
        QUnityEditor.SetLabel("DATA", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_target.EditorDataListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Data", m_target.EditorDataListCount, 0);
        //LIST
        m_scrollData = QUnityEditor.SetScrollViewBegin(m_scrollData);
        for (int i = 0; i < m_target.Data.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.EditorDataListCommand = !m_target.EditorDataListCommand;

            m_target.Data[i] = QUnityEditor.SetFieldScriptableObject(m_target.Data[i]);

            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_target.EditorDataListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Data, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Data, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    m_target.Data.RemoveAt(i);
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif