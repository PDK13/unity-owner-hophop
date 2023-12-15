using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "message-text-config", menuName = "QConfig/Message Text", order = 1)]
public class MessageDataConfigText : ScriptableObject
{
    public List<MessageDataText> Message = new List<MessageDataText>();

    public List<MessageDataChoice> Choice = new List<MessageDataChoice>();

    public bool ChoiceAvaible => Choice == null ? false : Choice.Count > 0;
}

#if UNITY_EDITOR

[CustomEditor(typeof(MessageDataConfigText))]
public class MessageDataConfigTextEditor : Editor
{
    private const float POPUP_HEIGHT = 150f * 2;
    private const float LABEL_WIDTH = 65f;

    private MessageDataConfigText m_target;

    private MessageDataConfig m_messageConfig;
    private string m_debugError = "";

    private int m_messageCount = 0;
    private int m_choiceCount = 0;

    private List<string> m_authorName;
    private List<bool> m_messageDelayShow;
    private List<bool> m_messageTriggerShow;

    private Vector2 m_scrollMessage;
    private Vector2 m_scrollChoice;

    private void OnEnable()
    {
        m_target = target as MessageDataConfigText;
        //
        m_messageCount = m_target.Message.Count;
        m_choiceCount = m_target.Message.Count;
        //
        SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        if (m_debugError != "")
        {
            QUnityEditorCustom.SetLabel(m_debugError, QUnityEditorCustom.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter));
            return;
        }
        //
        SetGUIGroupMessage();
        //
        QUnityEditorCustom.SetSpace(10f);
        //
        SetGUIGroupChoice();
        //
        QUnityEditorCustom.SetDirty(m_target);
    }

    //

    private void SetConfigFind()
    {
        if (m_messageConfig != null)
            return;
        //
        var AuthorConfigFound = QUnityAssets.GetScriptableObject<MessageDataConfig>("");
        //
        if (AuthorConfigFound == null)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Message] " + m_debugError);
            return;
        }
        //
        if (AuthorConfigFound.Count == 0)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Message] " + m_debugError);
            return;
        }
        //
        if (AuthorConfigFound.Count > 1)
            Debug.Log("[Message] Config found more than one, get the first one found");
        //
        m_messageConfig = AuthorConfigFound[0];
        //
        if (m_messageConfig.Author.Count == 0)
        {
            m_debugError = "Author Config not have any data, please add one";
            Debug.Log("[Message] " + m_debugError);
            return;
        }
        //
        //CONTINUE:
        //
        m_authorName = m_messageConfig.AuthorName;
        //
        m_messageDelayShow = new List<bool>();
        while (m_messageDelayShow.Count < m_target.Message.Count) m_messageDelayShow.Add(false);
        //
        m_messageTriggerShow = new List<bool>();
        while (m_messageTriggerShow.Count < m_target.Message.Count) m_messageTriggerShow.Add(false);
        //
        m_debugError = "";
    }

    private void SetGUIGroupMessage()
    {
        QUnityEditorCustom.SetLabel("MESSAGE", QUnityEditorCustom.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        //COUNT:
        QUnityEditorCustom.SetHorizontalBegin();
        QUnityEditorCustom.SetLabel("Count", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
        //
        m_messageCount = QUnityEditorCustom.SetField(m_messageCount);
        //
        if (QUnityEditorCustom.SetButton("+"))
            m_messageCount++;
        //
        if (QUnityEditorCustom.SetButton("-"))
            if (m_messageCount > 0)
                m_messageCount--;
        //
        QUnityEditorCustom.SetHorizontalEnd();
        //COUNT:
        //
        while (m_messageCount > m_target.Message.Count)
        {
            m_target.Message.Add(new MessageDataText(m_messageConfig.MessageTextDelayDefault));
            m_messageDelayShow.Add(false);
            m_messageTriggerShow.Add(false);
        }
        while (m_messageCount < m_target.Message.Count)
        {
            m_target.Message.RemoveAt(m_target.Message.Count - 1);
            m_messageDelayShow.RemoveAt(m_messageDelayShow.Count - 1);
            m_messageTriggerShow.RemoveAt(m_messageTriggerShow.Count - 1);
        }
        //
        QUnityEditorCustom.SetSpace(10);
        //
        m_scrollMessage = QUnityEditorCustom.SetScrollViewBegin(m_scrollMessage, QUnityEditorCustom.GetGUIHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.Message.Count; i++)
        {
            //ITEM:
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel(i.ToString(), QUnityEditorCustom.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorCustom.GetGUIWidth(25));
            //
            QUnityEditorCustom.SetVerticalBegin();
            //ITEM - AUTHOR
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel("Author", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
            m_target.Message[i].AuthorIndex = QUnityEditorCustom.SetPopup(m_target.Message[i].AuthorIndex, m_authorName);
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM - AUTHOR
            //
            //ITEM - MESSAGE:
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel("Message", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
            m_target.Message[i].Message = QUnityEditorCustom.SetField(m_target.Message[i].Message);
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM - MESSAGE:
            //
            //ITEM - DELAY
            QUnityEditorCustom.SetHorizontalBegin();
            if (QUnityEditorCustom.SetButton("Delay", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH)))
                m_messageDelayShow[i] = !m_messageDelayShow[i];
            //
            if (m_messageDelayShow[i])
            {
                QUnityEditorCustom.SetVerticalBegin();
                //
                QUnityEditorCustom.SetHorizontalBegin();
                QUnityEditorCustom.SetLabel("Alpha", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].DelayAlpha = QUnityEditorCustom.SetField(m_target.Message[i].DelayAlpha);
                QUnityEditorCustom.SetHorizontalEnd();
                //
                QUnityEditorCustom.SetHorizontalBegin();
                QUnityEditorCustom.SetLabel("Space", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].DelaySpace = QUnityEditorCustom.SetField(m_target.Message[i].DelaySpace);
                QUnityEditorCustom.SetHorizontalEnd();
                //
                QUnityEditorCustom.SetHorizontalBegin();
                QUnityEditorCustom.SetLabel("Mark", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].DelayMark = QUnityEditorCustom.SetField(m_target.Message[i].DelayMark);
                QUnityEditorCustom.SetHorizontalEnd();
                QUnityEditorCustom.SetVerticalEnd();
            }
            else
            {
                QUnityEditorCustom.SetLabel("Alpha: " + m_target.Message[i].DelayAlpha, null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
                QUnityEditorCustom.SetLabel("Space: " + m_target.Message[i].DelaySpace, null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
                QUnityEditorCustom.SetLabel("Mark: " + m_target.Message[i].DelayMark, null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
            }
            //
            QUnityEditorCustom.SetHorizontalEnd();
            //
            //ITEM - DELAY
            //
            //ITEM - TRIGGER:
            QUnityEditorCustom.SetHorizontalBegin();
            if (QUnityEditorCustom.SetButton("Trigger", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH)))
                m_messageTriggerShow[i] = !m_messageTriggerShow[i];
            //
            if (m_messageTriggerShow[i])
            {
                QUnityEditorCustom.SetVerticalBegin();
                //
                QUnityEditorCustom.SetHorizontalBegin();
                QUnityEditorCustom.SetLabel("Code", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].TriggerCode = QUnityEditorCustom.SetField(m_target.Message[i].TriggerCode);
                QUnityEditorCustom.SetHorizontalEnd();
                //
                QUnityEditorCustom.SetHorizontalBegin();
                QUnityEditorCustom.SetLabel("Object", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].TriggerObject = QUnityEditorCustom.SetField(m_target.Message[i].TriggerObject);
                QUnityEditorCustom.SetHorizontalEnd();
                //
                QUnityEditorCustom.SetVerticalEnd();
            }
            else
            {
                QUnityEditorCustom.SetLabel("Code: " + m_target.Message[i].TriggerCode, null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH * 2 + 4));
                QUnityEditorCustom.SetLabel("" + (m_target.Message[i].TriggerObject != null ? m_target.Message[i].TriggerObject.name : ""));
            }
            //
            QUnityEditorCustom.SetHorizontalEnd();
            //
            //ITEM - TRIGGER:
            //
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM:
            QUnityEditorCustom.SetVerticalEnd();
            //
            QUnityEditorCustom.SetSpace(10);
        }
        QUnityEditorCustom.SetScrollViewEnd();
    }

    private void SetGUIGroupChoice()
    {
        QUnityEditorCustom.SetLabel("CHOICE", QUnityEditorCustom.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        //COUNT:
        QUnityEditorCustom.SetHorizontalBegin();
        QUnityEditorCustom.SetLabel("Count", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
        //
        m_choiceCount = QUnityEditorCustom.SetField(m_choiceCount);
        //
        if (QUnityEditorCustom.SetButton("+"))
            m_choiceCount++;
        //
        if (QUnityEditorCustom.SetButton("-"))
            if (m_choiceCount > 0)
                m_choiceCount--;
        //
        QUnityEditorCustom.SetHorizontalEnd();
        //COUNT:
        //
        while (m_choiceCount > m_target.Choice.Count)
            m_target.Choice.Add(new MessageDataChoice());
        while (m_choiceCount < m_target.Choice.Count)
            m_target.Choice.RemoveAt(m_target.Choice.Count - 1);
        //
        QUnityEditorCustom.SetSpace(10);
        //
        m_scrollChoice = QUnityEditorCustom.SetScrollViewBegin(m_scrollChoice, QUnityEditorCustom.GetGUIHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.Choice.Count; i++)
        {
            //ITEM:
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel(i.ToString(), QUnityEditorCustom.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorCustom.GetGUIWidth(25));
            //
            QUnityEditorCustom.SetVerticalBegin();
            //ITEM - MESSAGE:
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel("Text", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
            m_target.Choice[i].Text = QUnityEditorCustom.SetField(m_target.Choice[i].Text);
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM - MESSAGE:
            //
            //ITEM - AUTHOR
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel("Author", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
            m_target.Choice[i].AuthorIndex = QUnityEditorCustom.SetPopup(m_target.Choice[i].AuthorIndex, m_authorName);
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM - AUTHOR
            //
            //ITEM - MESSAGE:
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel("Message", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
            m_target.Choice[i].Message = QUnityEditorCustom.SetField(m_target.Choice[i].Message);
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM - MESSAGE:
            //
            //ITEM - NEXT:
            QUnityEditorCustom.SetHorizontalBegin();
            QUnityEditorCustom.SetLabel("Next", null, QUnityEditorCustom.GetGUIWidth(LABEL_WIDTH));
            m_target.Choice[i].Next = QUnityEditorCustom.SetField<MessageDataConfigText>(m_target.Choice[i].Next);
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM - NEXT:
            //
            QUnityEditorCustom.SetHorizontalEnd();
            //ITEM:
            QUnityEditorCustom.SetVerticalEnd();
            //
            QUnityEditorCustom.SetSpace(10);
        }
        QUnityEditorCustom.SetScrollViewEnd();
    }
}

#endif