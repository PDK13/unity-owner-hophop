using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueManager : SingletonManager<DialogueManager>
{
    #region Varible: Setting

    [SerializeField] private DialogueConfig m_dialogueConfig;
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
    /// Dialogue system stage current active
    /// </summary>
    public Action<DialogueStageType> onStageActive;

    /// <summary>
    /// Dialogue system current author and trigger active
    /// </summary>
    public Action<DialogueDataText> onTextActive;

    /// <summary>
    /// Dialogue system current choice check
    /// </summary>
    public Action<int, DialogueDataChoice> onChoiceCheck;

    /// <summary>
    /// Dialogue system current choice active
    /// </summary>
    public Action<int, DialogueDataChoice> onChoiceActive;

    #endregion

    #region Varible: Dialogue Manager

    private enum DialogueCommandType
    {
        None,
        Text,
        Done,
        Wait,
        Next,
        Skip,
        Choice,
    }

    [SerializeField] private DialogueCommandType m_command = DialogueCommandType.Text;
    [SerializeField] private DialogueSingleConfig m_currentData;
    [SerializeField] private string m_currentDialogue = "";
    [SerializeField] private bool m_currentActive = false;
    [SerializeField] private bool m_currentChoice = false;
    [SerializeField] private TextMeshProUGUI m_tmp;

    private Coroutine m_iSetDialogueShowSingle;

    [SerializeField] private DialogueStageType m_stage = DialogueStageType.None;

    /// <summary>
    /// Dialogue system stage current
    /// </summary>
    public DialogueStageType Stage => m_stage;

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
        if (m_dialogueConfig != null)
            return;
        //
        var AuthorConfigFound = QUnityAssets.GetScriptableObject<DialogueConfig>("");
        //
        if (AuthorConfigFound == null)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Dialogue] " + m_debugError);
            return;
        }
        //
        if (AuthorConfigFound.Count == 0)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Dialogue] " + m_debugError);
            return;
        }
        //
        if (AuthorConfigFound.Count > 1)
            Debug.Log("[Dialogue] Config found more than one, get the first one found");
        //
        m_dialogueConfig = AuthorConfigFound[0];
        //
        m_debugError = "";
    }

#endif

    #endregion

    #region Main

    /// <summary>
    /// Start dialogue with config data
    /// </summary>
    /// <param name="Tmp"></param>
    /// <param name="DialogueData"></param>
    public void SetStart(DialogueSingleConfig DialogueData)
    {
        if (m_currentActive)
            return;
        //
        StartCoroutine(ISetDialogueShow(DialogueData));
    }

    private IEnumerator ISetDialogueShow(DialogueSingleConfig DialogueData, bool WaitForNextDialogue = false)
    {
        m_currentData = DialogueData;
        //
        if (WaitForNextDialogue)
            //Not check when first show dialogue!!
            yield return new WaitUntil(() => m_command == DialogueCommandType.Next);
        //
        m_command = DialogueCommandType.None;
        m_currentActive = true;
        m_currentChoice = false;
        //
        SetStage(DialogueStageType.Start);
        //
        SetDebug("[START]", DebugType.Primary);
        //
        for (int i = 0; i < m_currentData.Dialogue.Count; i++)
        {
            Current = m_currentData.Dialogue[i];
            Next = (i < m_currentData.Dialogue.Count - 1) ? m_currentData.Dialogue[i + 1] : null;
            //
            m_currentDialogue = m_currentData.Dialogue[i].Dialogue;
            //
            //Dialogue:
            if (string.IsNullOrEmpty(m_currentDialogue))
                m_tmp.text = "...";
            else
            {
                //BEGIN:
                onTextActive?.Invoke(m_currentData.Dialogue[i]);
                //
                m_tmp.text = "";
                //
                if (m_stringConfig != null)
                    m_currentDialogue = m_stringConfig.GetColorHexFormatReplace(m_currentDialogue);
                //
                //PROGESS:
                m_command = DialogueCommandType.Text;
                //
                SetStage(DialogueStageType.Text);
                //
                m_iSetDialogueShowSingle = StartCoroutine(ISetDialogueShowSingle(m_currentData.Dialogue[i]));
                //
                SetDebug(string.Format("[Dialogue] Current: '{0}'", m_currentDialogue), DebugType.Full);
                //
                yield return new WaitUntil(() => m_command == DialogueCommandType.Skip || m_command == DialogueCommandType.Done);
                //
                //DONE:
                m_tmp.text = m_currentDialogue;
            }
            //WAIT:
            if (!string.IsNullOrEmpty(m_currentDialogue) && i < m_currentData.Dialogue.Count - 1)
            {
                //FINAL:
                m_command = DialogueCommandType.Wait;
                //
                SetStage(DialogueStageType.Wait);
                //
                SetDebug("[Dialogue] Next?", DebugType.Primary);
                //
                yield return new WaitUntil(() => m_command == DialogueCommandType.Next);
            }
        }
        //
        m_command = m_currentData.ChoiceAvaible ? DialogueCommandType.Choice : DialogueCommandType.None;
        m_currentActive = m_currentData.ChoiceAvaible;
        m_currentChoice = m_currentData.ChoiceAvaible;
        //
        SetStage(m_currentChoice ? DialogueStageType.Choice : DialogueStageType.End);
        //
        if (m_currentChoice)
        {
            SetDebug("[Dialogue] Choice?", DebugType.Primary);
        }
        else
        {
            SetDebug("[Dialogue] End!", DebugType.Primary);
        }
    }

    private IEnumerator ISetDialogueShowSingle(DialogueDataText DialogueSingle)
    {
        bool HtmlFormat = false;
        //
        foreach (char DialogueChar in m_currentDialogue)
        {
            //TEXT:
            m_tmp.text += DialogueChar;
            //
            //COLOR:
            if (!HtmlFormat && DialogueChar == '<')
            {
                HtmlFormat = true;
                continue;
            }
            else
            if (HtmlFormat && DialogueChar == '>')
            {
                HtmlFormat = false;
                continue;
            }
            //
            //DELAY:
            if (HtmlFormat)
                continue;
            //
            switch (DialogueChar)
            {
                case '.':
                case '?':
                case '!':
                case ':':
                    if (DialogueSingle.Delay.Mark > 0)
                        yield return new WaitForSeconds(DialogueSingle.Delay.Mark);
                    break;
                case ' ':
                    if (DialogueSingle.Delay.Space > 0)
                        yield return new WaitForSeconds(DialogueSingle.Delay.Space);
                    break;
                default:
                    if (DialogueSingle.Delay.Alpha > 0)
                        yield return new WaitForSeconds(DialogueSingle.Delay.Alpha);
                    break;
            }
        }
        //
        m_command = DialogueCommandType.Done;
    }

    private void SetStage(DialogueStageType Stage)
    {
        SetDebug("[Dialogue] [STAGE] " + Stage.ToString(), DebugType.Full);
        //
        m_stage = Stage;
        onStageActive?.Invoke(Stage);
    }

    #endregion

    #region Control

    /// <summary>
    /// Dialogue current data!
    /// </summary>
    public DialogueDataText Current { private set; get; } = null;

    /// <summary>
    /// Dialogue next data!
    /// </summary>
    public DialogueDataText Next { private set; get; } = null;

    /// <summary>
    /// Change show dialogue!
    /// </summary>
    /// <param name="Tmp"></param>
    public TextMeshProUGUI TextMeshPro { get => m_tmp; set => m_tmp = value; }

    /// <summary>
    /// Next dialogue; or continue dialogue after choice option delay continue dialogue
    /// </summary>
    public void SetNext()
    {
        if (m_command != DialogueCommandType.Wait)
            //When current dialogue in done show up, press Next to move on next dialogue!
            return;
        //
        m_command = DialogueCommandType.Next;
        //
        SetDebug("[Dialogue] Next!", DebugType.Primary);
    }

    /// <summary>
    /// Skip current dialogue, until got choice option or end dialogue
    /// </summary>
    public void SetSkip()
    {
        if (m_command != DialogueCommandType.Text)
            //When current dialogue is showing up, press Next to skip and show full dialogue!
            return;
        //
        StopCoroutine(m_iSetDialogueShowSingle);
        //
        m_command = DialogueCommandType.Skip;
        //
        SetDebug("[Dialogue] Skip!", DebugType.Primary);
    }

    /// <summary>
    /// Choice current data!
    /// </summary>
    public List<DialogueDataChoice> Choice => m_currentData.Choice; //Should get this data when dialogue at choice stage!!

    /// <summary>
    /// Check choice option of dialogue when avaible
    /// </summary>
    /// <param name="ChoiceIndex"></param>
    public void SetChoiceCheck(int ChoiceIndex)
    {
        if (m_command != DialogueCommandType.Choice)
            //When current dialogue in done show up and got choice option, move choice option to get imformation of choice!
            return;
        //
        if (ChoiceIndex < 0 || ChoiceIndex > m_currentData.Choice.Count - 1)
            return;
        //
        onChoiceCheck?.Invoke(ChoiceIndex, m_currentData.Choice[ChoiceIndex]);
        //
        SetDebug(string.Format("[Dialogue] Check {0}: {1}", ChoiceIndex, m_currentData.Choice[ChoiceIndex].Text), DebugType.Full);
    }

    /// <summary>
    /// Choice option of dialogue when avaible
    /// </summary>
    /// <param name="ChoiceIndex"></param>
    /// <param name="NextDialogue">If false, must call 'Next' methode for continue dialogue of last option choice</param>
    public void SetChoiceActive(int ChoiceIndex, bool NextDialogue = true)
    {
        if (m_command != DialogueCommandType.Choice)
            //When current dialogue in done show up and got choice option, press choice option to move on next dialogue!
            return;
        //
        if (ChoiceIndex < 0 || ChoiceIndex > m_currentData.Choice.Count - 1)
            return;
        //
        m_command = NextDialogue ? DialogueCommandType.Next : DialogueCommandType.Wait;
        //
        StartCoroutine(ISetDialogueShow(m_currentData.Choice[ChoiceIndex].Next, true));
        //
        onChoiceActive?.Invoke(ChoiceIndex, m_currentData.Choice[ChoiceIndex]);
        //
        SetDebug(string.Format("[Dialogue] Choice {0}: {1}", ChoiceIndex, m_currentData.Choice[ChoiceIndex].Text), DebugType.Primary);
    }

    /// <summary>
    /// Stop dialogue
    /// </summary>
    public void SetStop()
    {
        StopAllCoroutines();
        StopCoroutine(m_iSetDialogueShowSingle);
        //
        m_command = DialogueCommandType.None;
        m_currentActive = false;
        m_currentChoice = false;
        //
        SetStage(DialogueStageType.End);
        //
        m_tmp.text = "";
        //
        SetDebug("[Dialogue] Stop!", DebugType.Primary);
    }

    #endregion

    private static void SetDebug(string Dialogue, DebugType DebugLimit)
    {
        if ((int)Instance.m_debug < (int)DebugLimit)
            return;
        //
        Debug.Log(string.Format("[Dialogue] {0}", Dialogue));
    }
}

public enum DialogueStageType
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

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    private DialogueManager m_target;

    private SerializedProperty m_dialogueConfig;
    private SerializedProperty m_stringConfig;

    private SerializedProperty m_debug;

    private SerializedProperty m_command;
    private SerializedProperty m_currentData;
    private SerializedProperty m_currentDialogue;
    private SerializedProperty m_stage;
    private SerializedProperty m_tmp;

    private void OnEnable()
    {
        m_target = target as DialogueManager;
        //
        m_dialogueConfig = QUnityEditorCustom.GetField(this, "m_dialogueConfig");
        m_stringConfig = QUnityEditorCustom.GetField(this, "m_stringConfig");
        //
        m_debug = QUnityEditorCustom.GetField(this, "m_debug");
        //
        m_command = QUnityEditorCustom.GetField(this, "m_command");
        m_currentData = QUnityEditorCustom.GetField(this, "m_currentData");
        m_currentDialogue = QUnityEditorCustom.GetField(this, "m_currentDialogue");
        m_stage = QUnityEditorCustom.GetField(this, "m_stage");
        m_tmp = QUnityEditorCustom.GetField(this, "m_tmp");
        //
        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_dialogueConfig);
        QUnityEditorCustom.SetField(m_stringConfig);
        //
        QUnityEditorCustom.SetField(m_debug);
        //
        QUnityEditor.SetDisableGroupBegin();
        //
        QUnityEditorCustom.SetField(m_command);
        QUnityEditorCustom.SetField(m_currentData);
        QUnityEditorCustom.SetField(m_currentDialogue);
        QUnityEditorCustom.SetField(m_stage);
        QUnityEditorCustom.SetField(m_tmp);
        //
        QUnityEditor.SetDisableGroupEnd();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif