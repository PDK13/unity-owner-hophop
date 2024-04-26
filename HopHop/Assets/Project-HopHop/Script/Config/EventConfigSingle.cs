using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config-single", menuName = "HopHop/Event Config Single", order = 0)]
public class EventConfigSingle : ScriptableObject
{
    public bool Base;
    public List<EventConfigSingleData> Data;
}

[Serializable]
public class EventConfigSingleData
{
    public bool Wait;
    public DialogueConfigSingle Dialogue;
    public List<string> Command = new List<string>();
    public List<EventConfigSingleDataChoice> Choice = new List<EventConfigSingleDataChoice>();

    public bool WaitCheck
    {
        get
        {
            if (Wait)
                return true;

            if (Dialogue != null)
                return true;

            if (Choice != null ? Choice.Count > 0 : false)
                return true;

            return false;
        }
    }
}

[Serializable]
public class EventConfigSingleDataChoice
{
    public string Name;
    public DialogueConfigSingle Dialogue;

    public EventConfigSingle EventNext;
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfigSingle))]
public class EventConfigSingleEditor : Editor
{
    private const float POPUP_HEIGHT = 150f * 2;
    private const float LABEL_WIDTH = 65f;

    private EventConfigSingle m_target;

    private int m_dataCount;
    private List<int> m_dataCommandCount = new List<int>();
    private List<int> m_dataChoiceCount = new List<int>();

    private Vector2 m_scrollData;

    private void OnEnable()
    {
        m_target = target as EventConfigSingle;

        m_dataCount = m_target.Data.Count;

        foreach (var Data in m_target.Data)
        {
            m_dataCommandCount.Add(Data.Command.Count);
            m_dataChoiceCount.Add(Data.Choice.Count);
        }
    }

    public override void OnInspectorGUI()
    {
        SetGUIGroupData();

        QUnityEditor.SetDirty(m_target);
    }

    private void SetGUIGroupData()
    {
        QUnityEditor.SetLabel("EVENT", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_dataCount = QUnityEditor.SetGroupNumberChangeLimitMin("Data", m_dataCount, 0);

        //COUNT:
        while (m_dataCount > m_target.Data.Count)
            m_target.Data.Add(new EventConfigSingleData());
        while (m_dataCount < m_target.Data.Count)
            m_target.Data.RemoveAt(m_target.Data.Count - 1);

        //COUNT - COMMAND:
        while (m_dataCount > m_dataCommandCount.Count)
            m_dataCommandCount.Add(0);
        while (m_dataCount < m_dataCommandCount.Count)
            m_dataCommandCount.RemoveAt(m_dataCommandCount.Count - 1);

        //COUNT - CHOICE:
        while (m_dataCount > m_dataChoiceCount.Count)
            m_dataChoiceCount.Add(0);
        while (m_dataCount < m_dataChoiceCount.Count)
            m_dataChoiceCount.RemoveAt(m_dataChoiceCount.Count - 1);

        //LIST
        m_scrollData = QUnityEditor.SetScrollViewBegin(m_scrollData);
        for (int i = 0; i < m_target.Data.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));

            #region ITEM - MAIN
            QUnityEditor.SetVerticalBegin();

            #region ITEM - MAIN - WAIT
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Wait", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
            m_target.Data[i].Wait = QUnityEditor.SetField(m_target.Data[i].Wait);
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ITEM - MAIN - DIALOGUE
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Dialogue", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
            m_target.Data[i].Dialogue = QUnityEditor.SetField(m_target.Data[i].Dialogue);
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ITEM - MAIN - COMMAND
            //COUNT:
            m_dataCommandCount[i] = QUnityEditor.SetGroupNumberChangeLimitMin("Command", m_dataCommandCount[i], 0);
            //COUNT:
            while (m_dataCommandCount[i] > m_target.Data[i].Command.Count)
                m_target.Data[i].Command.Add("");
            while (m_dataCommandCount[i] < m_target.Data[i].Command.Count)
                m_target.Data[i].Command.RemoveAt(m_target.Data[i].Command.Count - 1);
            //LIST:
            for (int j = 0; j < m_target.Data[i].Command.Count; j++)
            {
                #region ITEM - MAIN - COMMAND - ITEM
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel(j.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                m_target.Data[i].Command[j] = QUnityEditor.SetField(m_target.Data[i].Command[j]);
                QUnityEditor.SetHorizontalEnd();
                #endregion
            }
            #endregion

            #region ITEM - MAIN - CHOICE
            //COUNT:
            m_dataChoiceCount[i] = QUnityEditor.SetGroupNumberChangeLimitMin("Choice", m_dataChoiceCount[i], 0);
            //COUNT:
            while (m_dataChoiceCount[i] > m_target.Data[i].Choice.Count)
                m_target.Data[i].Choice.Add(new EventConfigSingleDataChoice());
            while (m_dataChoiceCount[i] < m_target.Data[i].Choice.Count)
                m_target.Data[i].Choice.RemoveAt(m_target.Data[i].Choice.Count - 1);
            //LIST:
            for (int j = 0; j < m_target.Data[i].Choice.Count; j++)
            {
                #region ITEM - MAIN - CHOICE - ITEM
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel(j.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));

                #region ITEM - MAIN - CHOICE - ITEM - MAIN
                QUnityEditor.SetVerticalBegin();

                #region ITEM - MAIN - CHOICE - ITEM - MAIN - NAME
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Name", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                m_target.Data[i].Choice[j].Name = QUnityEditor.SetField(m_target.Data[i].Choice[j].Name);
                QUnityEditor.SetHorizontalEnd();
                #endregion

                #region ITEM - MAIN - CHOICE - ITEM - MAIN - DIALOGUE
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Dialogue", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                m_target.Data[i].Choice[j].Dialogue = QUnityEditor.SetField(m_target.Data[i].Choice[j].Dialogue);
                QUnityEditor.SetHorizontalEnd();
                #endregion

                #region ITEM - MAIN - CHOICE - ITEM - MAIN - NEXT
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Next", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                m_target.Data[i].Choice[j].EventNext = QUnityEditor.SetField(m_target.Data[i].Choice[j].EventNext);
                QUnityEditor.SetHorizontalEnd();
                #endregion

                QUnityEditor.SetVerticalEnd();
                #endregion

                QUnityEditor.SetHorizontalEnd();
                #endregion
            }
            #endregion

            QUnityEditor.SetVerticalEnd();
            #endregion

            QUnityEditor.SetHorizontalEnd();
            #endregion

            QUnityEditor.SetSpace();
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif