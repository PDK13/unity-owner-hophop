using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MessageManager : SingletonManager<MessageManager>
{
    #region Varible: Setting

    [SerializeField] private MessageDataConfig m_messageConfig;
    [SerializeField] private StringCodeConfig m_stringConfig;

    private string m_debugError = "";

    #endregion

    #region Varible: Debug

    private enum DebugType { None = 0, Primary = 1, Full = int.MaxValue, }

    [Space]
    [SerializeField] private DebugType m_debug = DebugType.Primary;

    #endregion

    #region Event

    /// <summary>
    /// Message system stage current active
    /// </summary>
    public Action<MessageStageType> onStageActive;

    /// <summary>
    /// Message system current author and trigger active
    /// </summary>
    public Action<MessageDataText> onTextActive;

    /// <summary>
    /// Message system current choice check
    /// </summary>
    public Action<int, MessageDataChoice> onChoiceCheck;

    /// <summary>
    /// Message system current choice active
    /// </summary>
    public Action<int, MessageDataChoice> onChoiceActive;

    #endregion

    #region Varible: Message Manager

    private enum MessageCommandType
    {
        None,
        Text,
        Done,
        Wait,
        Next,
        Skip,
        Choice,
    }

    [SerializeField] private MessageCommandType m_command = MessageCommandType.Text;
    [SerializeField] private MessageDataConfigText m_currentData;
    [SerializeField] private string m_currentMessage = "";
    [SerializeField] private bool m_currentActive = false;
    [SerializeField] private bool m_currentChoice = false;
    [SerializeField] private TextMeshProUGUI m_tmp;

    private Coroutine m_iSetMessageShowSingle;

    [SerializeField] private MessageStageType m_stage = MessageStageType.None;

    /// <summary>
    /// Message system stage current
    /// </summary>
    public MessageStageType Stage => m_stage;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        //
#if UNITY_EDITOR
        SetConfigFind();
#endif
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #region Config

#if UNITY_EDITOR

    public void SetConfigFind()
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
        m_debugError = "";
    }

#endif

    #endregion

    #region Main

    /// <summary>
    /// Start message with config data
    /// </summary>
    /// <param name="Tmp"></param>
    /// <param name="MessageData"></param>
    public void SetStart(MessageDataConfigText MessageData)
    {
        if (m_currentActive)
            return;
        //
        StartCoroutine(ISetMessageShow(MessageData));
    }

    private IEnumerator ISetMessageShow(MessageDataConfigText MessageData, bool WaitForNextMessage = false)
    {
        m_currentData = MessageData;
        //
        if (WaitForNextMessage)
            //Not check when first show message!!
            yield return new WaitUntil(() => m_command == MessageCommandType.Next);
        //
        m_command = MessageCommandType.None;
        m_currentActive = true;
        m_currentChoice = false;
        //
        SetStage(MessageStageType.Start);
        //
        SetDebug("[START]", DebugType.Primary);
        //
        for (int i = 0; i < m_currentData.Message.Count; i++)
        {
            Current = m_currentData.Message[i];
            Next = (i < m_currentData.Message.Count - 1) ? m_currentData.Message[i + 1] : null;
            //
            m_currentMessage = m_currentData.Message[i].Message;
            //
            //MESSAGE:
            if (string.IsNullOrEmpty(m_currentMessage))
                m_tmp.text = "...";
            else
            {
                //BEGIN:
                onTextActive?.Invoke(m_currentData.Message[i]);
                //
                m_tmp.text = "";
                //
                if (m_stringConfig != null)
                    m_currentMessage = m_stringConfig.GetColorHexFormatReplace(m_currentMessage);
                //
                //PROGESS:
                m_command = MessageCommandType.Text;
                //
                SetStage(MessageStageType.Text);
                //
                m_iSetMessageShowSingle = StartCoroutine(ISetMessageShowSingle(m_currentData.Message[i]));
                //
                SetDebug(string.Format("[Message] Current: '{0}'", m_currentMessage), DebugType.Full);
                //
                yield return new WaitUntil(() => m_command == MessageCommandType.Skip || m_command == MessageCommandType.Done);
                //
                //DONE:
                m_tmp.text = m_currentMessage;
            }
            //WAIT:
            if (!string.IsNullOrEmpty(m_currentMessage) && i < m_currentData.Message.Count - 1)
            {
                //FINAL:
                m_command = MessageCommandType.Wait;
                //
                SetStage(MessageStageType.Wait);
                //
                SetDebug("[Message] Next?", DebugType.Primary);
                //
                yield return new WaitUntil(() => m_command == MessageCommandType.Next);
            }
        }
        //
        m_command = m_currentData.ChoiceAvaible ? MessageCommandType.Choice : MessageCommandType.None;
        m_currentActive = m_currentData.ChoiceAvaible;
        m_currentChoice = m_currentData.ChoiceAvaible;
        //
        SetStage(m_currentChoice ? MessageStageType.Choice : MessageStageType.End);
        //
        if (m_currentChoice)
        {
            SetDebug("[Message] Choice?", DebugType.Primary);
        }
        else
        {
            SetDebug("[Message] End!", DebugType.Primary);
        }
    }

    private IEnumerator ISetMessageShowSingle(MessageDataText MessageSingle)
    {
        bool HtmlFormat = false;
        //
        foreach (char MessageChar in m_currentMessage)
        {
            //TEXT:
            m_tmp.text += MessageChar;
            //
            //COLOR:
            if (!HtmlFormat && MessageChar == '<')
            {
                HtmlFormat = true;
                continue;
            }
            else
            if (HtmlFormat && MessageChar == '>')
            {
                HtmlFormat = false;
                continue;
            }
            //
            //DELAY:
            if (HtmlFormat)
                continue;
            //
            switch (MessageChar)
            {
                case '.':
                case '?':
                case '!':
                case ':':
                    if (MessageSingle.DelayMark > 0)
                        yield return new WaitForSeconds(MessageSingle.DelayMark);
                    break;
                case ' ':
                    if (MessageSingle.DelaySpace > 0)
                        yield return new WaitForSeconds(MessageSingle.DelaySpace);
                    break;
                default:
                    if (MessageSingle.DelayAlpha > 0)
                        yield return new WaitForSeconds(MessageSingle.DelayAlpha);
                    break;
            }
        }
        //
        m_command = MessageCommandType.Done;
    }

    private void SetStage(MessageStageType Stage)
    {
        SetDebug("[Message] [STAGE] " + Stage.ToString(), DebugType.Full);
        //
        m_stage = Stage;
        onStageActive?.Invoke(Stage);
    }

    #endregion

    #region Control

    /// <summary>
    /// Message current data!
    /// </summary>
    public MessageDataText Current { private set; get; } = null;

    /// <summary>
    /// Message next data!
    /// </summary>
    public MessageDataText Next { private set; get; } = null;

    /// <summary>
    /// Change show message!
    /// </summary>
    /// <param name="Tmp"></param>
    public TextMeshProUGUI TextMeshPro { get => m_tmp; set => m_tmp = value; }

    /// <summary>
    /// Next message; or continue message after choice option delay continue message
    /// </summary>
    public void SetNext()
    {
        if (m_command != MessageCommandType.Wait)
            //When current message in done show up, press Next to move on next message!
            return;
        //
        m_command = MessageCommandType.Next;
        //
        SetDebug("[Message] Next!", DebugType.Primary);
    }

    /// <summary>
    /// Skip current message, until got choice option or end message
    /// </summary>
    public void SetSkip()
    {
        if (m_command != MessageCommandType.Text)
            //When current message is showing up, press Next to skip and show full message!
            return;
        //
        StopCoroutine(m_iSetMessageShowSingle);
        //
        m_command = MessageCommandType.Skip;
        //
        SetDebug("[Message] Skip!", DebugType.Primary);
    }

    /// <summary>
    /// Choice current data!
    /// </summary>
    public List<MessageDataChoice> Choice => m_currentData.Choice; //Should get this data when message at choice stage!!

    /// <summary>
    /// Check choice option of message when avaible
    /// </summary>
    /// <param name="ChoiceIndex"></param>
    public void SetChoiceCheck(int ChoiceIndex)
    {
        if (m_command != MessageCommandType.Choice)
            //When current message in done show up and got choice option, move choice option to get imformation of choice!
            return;
        //
        if (ChoiceIndex < 0 || ChoiceIndex > m_currentData.Choice.Count - 1)
            return;
        //
        onChoiceCheck?.Invoke(ChoiceIndex, m_currentData.Choice[ChoiceIndex]);
        //
        SetDebug(string.Format("[Message] Check {0}: {1}", ChoiceIndex, m_currentData.Choice[ChoiceIndex].Text), DebugType.Full);
    }

    /// <summary>
    /// Choice option of message when avaible
    /// </summary>
    /// <param name="ChoiceIndex"></param>
    /// <param name="NextMessage">If false, must call 'Next' methode for continue message of last option choice</param>
    public void SetChoiceActive(int ChoiceIndex, bool NextMessage = true)
    {
        if (m_command != MessageCommandType.Choice)
            //When current message in done show up and got choice option, press choice option to move on next message!
            return;
        //
        if (ChoiceIndex < 0 || ChoiceIndex > m_currentData.Choice.Count - 1)
            return;
        //
        m_command = NextMessage ? MessageCommandType.Next : MessageCommandType.Wait;
        //
        StartCoroutine(ISetMessageShow(m_currentData.Choice[ChoiceIndex].Next, true));
        //
        onChoiceActive?.Invoke(ChoiceIndex, m_currentData.Choice[ChoiceIndex]);
        //
        SetDebug(string.Format("[Message] Choice {0}: {1}", ChoiceIndex, m_currentData.Choice[ChoiceIndex].Text), DebugType.Primary);
    }

    /// <summary>
    /// Stop message
    /// </summary>
    public void SetStop()
    {
        StopAllCoroutines();
        StopCoroutine(m_iSetMessageShowSingle);
        //
        m_command = MessageCommandType.None;
        m_currentActive = false;
        m_currentChoice = false;
        //
        SetStage(MessageStageType.End);
        //
        m_tmp.text = "";
        //
        SetDebug("[Message] Stop!", DebugType.Primary);
    }

    #endregion

    private static void SetDebug(string Message, DebugType DebugLimit)
    {
        if ((int)Instance.m_debug < (int)DebugLimit)
            return;
        //
        Debug.Log(string.Format("[Message] {0}", Message));
    }
}

public enum MessageStageType
{
    None,
    //Trigger when Start
    Start,
    //Trigger when Show
    Text,
    Wait,
    Choice,
    //Trigger when End
    End,
}

#if UNITY_EDITOR

[CustomEditor(typeof(MessageManager))]
public class MessageManagerEditor : Editor
{
    private MessageManager m_target;

    private SerializedProperty m_messageConfig;
    private SerializedProperty m_stringConfig;

    private SerializedProperty m_debug;

    private SerializedProperty m_command;
    private SerializedProperty m_currentData;
    private SerializedProperty m_currentMessage;
    private SerializedProperty m_stage;
    private SerializedProperty m_tmp;

    private void OnEnable()
    {
        m_target = target as MessageManager;
        //
        m_messageConfig = QUnityEditorCustom.GetField(this, "m_messageConfig");
        m_stringConfig = QUnityEditorCustom.GetField(this, "m_stringConfig");
        //
        m_debug = QUnityEditorCustom.GetField(this, "m_debug");
        //
        m_command = QUnityEditorCustom.GetField(this, "m_command");
        m_currentData = QUnityEditorCustom.GetField(this, "m_currentData");
        m_currentMessage = QUnityEditorCustom.GetField(this, "m_currentMessage");
        m_stage = QUnityEditorCustom.GetField(this, "m_stage");
        m_tmp = QUnityEditorCustom.GetField(this, "m_tmp");
        //
        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_messageConfig);
        QUnityEditorCustom.SetField(m_stringConfig);
        //
        QUnityEditorCustom.SetField(m_debug);
        //
        QUnityEditor.SetDisableGroupBegin();
        //
        QUnityEditorCustom.SetField(m_command);
        QUnityEditorCustom.SetField(m_currentData);
        QUnityEditorCustom.SetField(m_currentMessage);
        QUnityEditorCustom.SetField(m_stage);
        QUnityEditorCustom.SetField(m_tmp);
        //
        QUnityEditor.SetDisableGroupEnd();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif