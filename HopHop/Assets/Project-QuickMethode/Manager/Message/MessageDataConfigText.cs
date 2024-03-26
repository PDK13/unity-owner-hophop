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
    private List<bool> m_choiceAuthorShow;
    private List<bool> m_choiceTriggerShow;

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
            QUnityEditor.SetLabel(m_debugError, QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter));
            return;
        }
        //
        SetGUIGroupMessage();
        //
        QUnityEditor.SetSpace(10f);
        //
        SetGUIGroupChoice();
        //
        QUnityEditor.SetDirty(m_target);
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
        m_choiceAuthorShow = new List<bool>();
        while (m_choiceAuthorShow.Count < m_target.Choice.Count) m_choiceAuthorShow.Add(false);
        //
        m_choiceTriggerShow = new List<bool>();
        while (m_choiceTriggerShow.Count < m_target.Choice.Count) m_choiceTriggerShow.Add(false);
        //
        m_debugError = "";
    }

    private void SetGUIGroupMessage()
    {
        QUnityEditor.SetLabel("MESSAGE", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        //COUNT:
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Count", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
        //
        m_messageCount = QUnityEditor.SetField(m_messageCount);
        //
        if (QUnityEditor.SetButton("+"))
            m_messageCount++;
        //
        if (QUnityEditor.SetButton("-"))
            if (m_messageCount > 0)
                m_messageCount--;
        //
        QUnityEditor.SetHorizontalEnd();
        //COUNT:
        //
        while (m_messageCount > m_target.Message.Count)
        {
            m_target.Message.Add(new MessageDataText(m_messageConfig.MessageTextDelayDefault));
            m_messageDelayShow.Add(false);
            m_messageTriggerShow.Add(false);
            m_choiceAuthorShow.Add(false);
            m_choiceTriggerShow.Add(false);
        }
        while (m_messageCount < m_target.Message.Count)
        {
            m_target.Message.RemoveAt(m_target.Message.Count - 1);
            m_messageDelayShow.RemoveAt(m_messageDelayShow.Count - 1);
            m_messageTriggerShow.RemoveAt(m_messageTriggerShow.Count - 1);
            m_choiceAuthorShow.RemoveAt(m_choiceAuthorShow.Count - 1);
            m_choiceTriggerShow.RemoveAt(m_choiceTriggerShow.Count - 1);
        }
        //
        QUnityEditor.SetSpace(10);
        //
        m_scrollMessage = QUnityEditor.SetScrollViewBegin(m_scrollMessage, QUnityEditor.GetGUIHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.Message.Count; i++)
        {
            //ITEM:
            //
            //ITEM - NUM:
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditor.GetGUIWidth(25));
            //ITEM - NUM:
            //
            //ITEM - MAIN:
            QUnityEditor.SetVerticalBegin();
            //
            //ITEM - AUTHOR
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Author", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
            m_target.Message[i].AuthorIndex = QUnityEditor.SetPopup(m_target.Message[i].AuthorIndex, m_authorName);
            QUnityEditor.SetHorizontalEnd();
            //ITEM - AUTHOR
            //
            //ITEM - MESSAGE:
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Message", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
            m_target.Message[i].Message = QUnityEditor.SetField(m_target.Message[i].Message);
            QUnityEditor.SetHorizontalEnd();
            //ITEM - MESSAGE:
            //
            //ITEM - DELAY
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton("Delay", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH)))
                m_messageDelayShow[i] = !m_messageDelayShow[i];
            if (m_messageDelayShow[i])
            {
                QUnityEditor.SetVerticalBegin();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Alpha", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].DelayAlpha = QUnityEditor.SetField(m_target.Message[i].DelayAlpha);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Space", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].DelaySpace = QUnityEditor.SetField(m_target.Message[i].DelaySpace);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Mark", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].DelayMark = QUnityEditor.SetField(m_target.Message[i].DelayMark);
                QUnityEditor.SetHorizontalEnd();
                QUnityEditor.SetVerticalEnd();
            }
            else
            {
                QUnityEditor.SetLabel("Alpha: " + m_target.Message[i].DelayAlpha, null, QUnityEditor.GetGUIWidth(LABEL_WIDTH * 1.25f));
                QUnityEditor.SetLabel("Space: " + m_target.Message[i].DelaySpace, null, QUnityEditor.GetGUIWidth(LABEL_WIDTH * 1.25f));
                QUnityEditor.SetLabel("Mark: " + m_target.Message[i].DelayMark, null, QUnityEditor.GetGUIWidth(LABEL_WIDTH * 1.25f));
            }
            QUnityEditor.SetHorizontalEnd();
            //ITEM - DELAY
            //
            //ITEM - TRIGGER:
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton("Trigger", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH)))
                m_messageTriggerShow[i] = !m_messageTriggerShow[i];
            //
            if (m_messageTriggerShow[i])
            {
                QUnityEditor.SetVerticalBegin();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Code", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].TriggerCode = QUnityEditor.SetField(m_target.Message[i].TriggerCode);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Object", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Message[i].TriggerObject = QUnityEditor.SetField(m_target.Message[i].TriggerObject);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetVerticalEnd();
            }
            else
            {
                QUnityEditor.SetLabel("Code: " + m_target.Message[i].TriggerCode, null, QUnityEditor.GetGUIWidth(LABEL_WIDTH * 2 + 4));
                QUnityEditor.SetLabel("" + (m_target.Message[i].TriggerObject != null ? m_target.Message[i].TriggerObject.name : ""));
            }
            QUnityEditor.SetHorizontalEnd();
            //ITEM - TRIGGER:
            //
            QUnityEditor.SetHorizontalEnd();
            //ITEM - MAIN:
            //
            QUnityEditor.SetVerticalEnd();
            //ITEM:
            //
            //NEXT:
            QUnityEditor.SetSpace(10);
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private void SetGUIGroupChoice()
    {
        QUnityEditor.SetLabel("CHOICE", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        //COUNT:
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Count", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
        //
        m_choiceCount = QUnityEditor.SetField(m_choiceCount);
        //
        if (QUnityEditor.SetButton("+"))
            m_choiceCount++;
        //
        if (QUnityEditor.SetButton("-"))
            if (m_choiceCount > 0)
                m_choiceCount--;
        //
        QUnityEditor.SetHorizontalEnd();
        //COUNT:
        //
        while (m_choiceCount > m_target.Choice.Count)
            m_target.Choice.Add(new MessageDataChoice());
        while (m_choiceCount < m_target.Choice.Count)
            m_target.Choice.RemoveAt(m_target.Choice.Count - 1);
        //
        QUnityEditor.SetSpace(10);
        //
        m_scrollChoice = QUnityEditor.SetScrollViewBegin(m_scrollChoice, QUnityEditor.GetGUIHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.Choice.Count; i++)
        {
            //ITEM:
            //
            //ITEM - NUM:
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditor.GetGUIWidth(25));
            //ITEM - NUM:
            //
            //ITEM - MAIN:
            QUnityEditor.SetVerticalBegin();
            //ITEM - MESSAGE:
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Text", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
            m_target.Choice[i].Text = QUnityEditor.SetField(m_target.Choice[i].Text);
            QUnityEditor.SetHorizontalEnd();
            //ITEM - MESSAGE:
            //
            //ITEM - NEXT:
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Next", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
            m_target.Choice[i].Next = QUnityEditor.SetField<MessageDataConfigText>(m_target.Choice[i].Next);
            QUnityEditor.SetHorizontalEnd();
            //ITEM - NEXT:
            //
            //ITEM - AUTHOR
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton("Extra", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH)))
                m_choiceAuthorShow[i] = !m_choiceAuthorShow[i];
            //
            if (m_choiceAuthorShow[i])
            {
                QUnityEditor.SetVerticalBegin();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Author", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Choice[i].AuthorIndex = QUnityEditor.SetPopup(m_target.Choice[i].AuthorIndex, m_authorName);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Message", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Choice[i].Message = QUnityEditor.SetField(m_target.Choice[i].Message);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetVerticalEnd();
            }
            else
            {
                QUnityEditor.SetLabel(string.Format("{0} : {1}", m_authorName[m_target.Choice[i].AuthorIndex], m_target.Choice[i].Message), null);
            }
            //
            QUnityEditor.SetHorizontalEnd();
            //ITEM - AUTHOR
            //
            //ITEM - TRIGGER:
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton("Trigger", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH)))
                m_choiceTriggerShow[i] = !m_choiceTriggerShow[i];
            //
            if (m_choiceTriggerShow[i])
            {
                QUnityEditor.SetVerticalBegin();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Code", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Choice[i].TriggerCode = QUnityEditor.SetField(m_target.Choice[i].TriggerCode);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Object", null, QUnityEditor.GetGUIWidth(LABEL_WIDTH));
                m_target.Choice[i].TriggerObject = QUnityEditor.SetField(m_target.Choice[i].TriggerObject);
                QUnityEditor.SetHorizontalEnd();
                //
                QUnityEditor.SetVerticalEnd();
            }
            else
            {
                QUnityEditor.SetLabel("Code: " + m_target.Choice[i].TriggerCode, null, QUnityEditor.GetGUIWidth(LABEL_WIDTH * 2 + 4));
                QUnityEditor.SetLabel("" + (m_target.Choice[i].TriggerObject != null ? m_target.Choice[i].TriggerObject.name : ""));
            }
            QUnityEditor.SetHorizontalEnd();
            //ITEM - TRIGGER:
            //
            QUnityEditor.SetHorizontalEnd();
            //ITEM - MAIN:
            //
            QUnityEditor.SetVerticalEnd();
            //ITEM:
            //
            //NEXT:
            QUnityEditor.SetSpace(10);
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif