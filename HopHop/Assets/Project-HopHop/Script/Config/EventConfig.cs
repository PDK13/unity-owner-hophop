using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config", menuName = "HopHop/Event Config", order = 0)]
public class EventConfig : ScriptableObject
{
    public string Code = "";
    public List<EventConfigSingle> Data = new List<EventConfigSingle>();
}

[Serializable]
public class EventConfigSingle
{
    public DialogueConfigSingle Dialogue;
    public List<string> Command = new List<string>();
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfig))]
public class EventConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 150f * 2;
    private const float LABEL_WIDTH = 65f;

    private EventConfig m_target;

    private int m_dataCount;
    private List<int> m_dataCommandCount;
    private List<bool> m_dataCommandShow;

    private Vector2 m_scrollData;

    private void OnEnable()
    {
        m_target = target as EventConfig;
        //
        m_dataCount = m_target.Data.Count;
        m_dataCommandCount = new List<int>();
        m_dataCommandShow = new List<bool>();
        foreach (var Item in m_target.Data)
        {
            m_dataCommandCount.Add(Item.Command.Count);
            m_dataCommandShow.Add(false);
        }
    }

    public override void OnInspectorGUI()
    {
        SetGUIGroupEvent();
        //
        QUnityEditor.SetDirty(m_target);
    }

    private void SetGUIGroupEvent()
    {
        QUnityEditor.SetLabel("EVENT", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        //CODE:
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Code", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.Code = QUnityEditor.SetField(m_target.Code);
        QUnityEditor.SetHorizontalEnd();
        //
        //COUNT:
        QUnityEditor.SetLabel("Data", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_dataCount = QUnityEditor.SetGroupNumberChangeLimitMin(m_dataCount, 0);
        //
        //COUNT:
        while (m_dataCount > m_target.Data.Count)
        {
            m_target.Data.Add(new EventConfigSingle());
            m_dataCommandCount.Add(0);
            m_dataCommandShow.Add(false);
        }
        while (m_dataCount < m_target.Data.Count)
        {
            m_target.Data.RemoveAt(m_target.Data.Count - 1);
            m_dataCommandCount.RemoveAt(m_dataCommandCount.Count - 1);
            m_dataCommandShow.RemoveAt(m_dataCommandCount.Count - 1);
        }
        //
        m_scrollData = QUnityEditor.SetScrollViewBegin(m_scrollData, null);
        for (int i = 0; i < m_target.Data.Count; i++)
        {
            //ITEM:
            //
            //ITEM - NUM:
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditor.GetGUILayoutWidth(25));
            //ITEM - NUM:
            //
            //ITEM - MAIN:
            QUnityEditor.SetVerticalBegin();
            //
            //ITEM - Dialogue
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Dialogue", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
            m_target.Data[i].Dialogue = QUnityEditor.SetField(m_target.Data[i].Dialogue);
            QUnityEditor.SetHorizontalEnd();
            //
            //ITEM - Command
            if (QUnityEditor.SetButton("Command", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter, 10), QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH)))
                m_dataCommandShow[i] = !m_dataCommandShow[i];
            if (m_dataCommandShow[i])
            {
                m_dataCommandCount[i] = QUnityEditor.SetGroupNumberChangeLimitMin(m_dataCommandCount[i], 0);
                //
                while (m_dataCommandCount[i] > m_target.Data[i].Command.Count)
                    m_target.Data[i].Command.Add("");
                while (m_dataCommandCount[i] < m_target.Data[i].Command.Count)
                    m_target.Data[i].Command.RemoveAt(m_target.Data.Count - 1);
                //
                QUnityEditor.SetVerticalBegin();
                for (int j = 0; j < m_target.Data[i].Command.Count; j++)
                {
                    QUnityEditor.SetHorizontalBegin();
                    QUnityEditor.SetLabel(j.ToString(), QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditor.GetGUILayoutWidth(25));
                    m_target.Data[i].Command[j] = QUnityEditor.SetField(m_target.Data[i].Command[j]);
                    QUnityEditor.SetHorizontalEnd();
                }
                QUnityEditor.SetVerticalEnd();
            }
            //ITEM - Command
            //
            QUnityEditor.SetHorizontalEnd();
            //ITEM - MAIN:
            //
            QUnityEditor.SetVerticalEnd();
            //ITEM:
            //
            //NEXT:
            QUnityEditor.SetSpace();
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif