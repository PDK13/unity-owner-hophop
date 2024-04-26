using System;
using System.Collections;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueManager : SingletonManager<DialogueManager>
{
    #region Varible: Action

    /// <summary>
    /// Dialogue start and trigger once time only, until end and refresh
    /// </summary>
    public Action onStart;

    /// <summary>
    /// Dialogue system stage current active
    /// </summary>
    public Action<DialogueStageType> onStage;

    /// <summary>
    /// Dialogue system current author and trigger active
    /// </summary>
    public Action<DialogueDataText> onText;

    #endregion

    #region Varible: Config

    [SerializeField] private DialogueConfig m_dialogueConfig;
    [SerializeField] private StringConfig m_stringConfig;

    #endregion

    #region Varible: Setting

    [Space]
    [SerializeField] private float m_delayStart = 1f;

    #endregion

    #region Varible: Dialogue

    private enum DialogueCommandType
    {
        None,
        Text,
        Done,
        Wait,
        Next,
        Skip,
    }

    private DialogueCommandType m_command = DialogueCommandType.Text;
    private DialogueStageType m_stage = DialogueStageType.None;

    private bool m_active = false;

    private DialogueConfigSingle m_dataCurrent;

    private string m_text = "";
    private TextMeshProUGUI m_tmp;

    private DialogueDataText m_textCurrent;
    private DialogueDataText m_textNext;

    private Coroutine m_iSetDialogueShowSingle;

    #endregion

    #region Varible: Get

    /// <summary>
    /// Dialogue system stage current
    /// </summary>
    public DialogueStageType Stage => m_stage;

    /// <summary>
    /// Dialogue current data
    /// </summary>
    public DialogueDataText TextCurrent => m_textCurrent;

    /// <summary>
    /// Dialogue next data
    /// </summary>
    public DialogueDataText TextNext => m_textNext;

    /// <summary>
    /// Dialogue last data
    /// </summary>
    public DialogueDataText TextLast => m_dataCurrent != null ? m_dataCurrent.Dialogue[m_dataCurrent.Dialogue.Count - 1] : null;

    /// <summary>
    /// Change show dialogue
    /// </summary>
    /// <param name="Tmp"></param>
    public TextMeshProUGUI Tmp { get => m_tmp; set => m_tmp = value; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

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

        var AuthorConfigFound = QUnityAssets.GetScriptableObject<DialogueConfig>("", false);

        if (AuthorConfigFound == null)
        {
            Debug.Log("[Dialogue] Config not found, please create one");
            return;
        }

        if (AuthorConfigFound.Count == 0)
        {
            Debug.Log("[Dialogue] Config not found, please create one");
            return;
        }

        if (AuthorConfigFound.Count > 1)
            Debug.Log("[Dialogue] Config found more than one, get the first one found");

        m_dialogueConfig = AuthorConfigFound[0];

        QUnityEditor.SetDirty(this);
    }

#endif

    #endregion

    #region Main

    /// <summary>
    /// Start dialogue with config data
    /// </summary>
    /// <param name="DialogueData"></param>
    public void SetStart(DialogueConfigSingle DialogueData)
    {
        if (m_active)
            return;

        onStart?.Invoke();

        StartCoroutine(ISetDialogueShow(DialogueData));
    }

    private IEnumerator ISetDialogueShow(DialogueConfigSingle DialogueData)
    {
        m_dataCurrent = DialogueData;

        m_command = DialogueCommandType.None;
        m_active = true;

        SetStage(DialogueStageType.Start);

        yield return new WaitForSeconds(m_delayStart);

        //START

        for (int i = 0; i < m_dataCurrent.Dialogue.Count; i++)
        {
            m_textCurrent = m_dataCurrent.Dialogue[i];
            m_textNext = (i < m_dataCurrent.Dialogue.Count - 1) ? m_dataCurrent.Dialogue[i + 1] : null;

            m_text = m_dataCurrent.Dialogue[i].Dialogue;

            //DIALOGUE
            if (string.IsNullOrEmpty(m_text))
                m_tmp.text = "...";
            else
            {
                //BEGIN
                onText?.Invoke(m_dataCurrent.Dialogue[i]);

                m_tmp.text = "";
                if (m_stringConfig != null)
                    m_text = m_stringConfig.GetColorHexFormatReplace(m_text);

                //PROGESS
                m_command = DialogueCommandType.Text;
                SetStage(DialogueStageType.Text);
                m_iSetDialogueShowSingle = StartCoroutine(ISetDialogueShowSingle(m_dataCurrent.Dialogue[i]));

                //CURRENT
                yield return new WaitUntil(() => m_command == DialogueCommandType.Skip || m_command == DialogueCommandType.Done);

                //DONE
                m_tmp.text = m_text;
            }

            //WAIT
            if (!string.IsNullOrEmpty(m_text) && i < m_dataCurrent.Dialogue.Count - 1)
            {
                //FINAL
                m_command = DialogueCommandType.Wait;

                SetStage(DialogueStageType.Wait);

                //NEXT?
                yield return new WaitUntil(() => m_command == DialogueCommandType.Next);
            }
        }

        m_command = DialogueCommandType.None;
        m_active = false;

        SetStage(DialogueStageType.End);

        //END:
    }

    private IEnumerator ISetDialogueShowSingle(DialogueDataText DialogueSingle)
    {
        bool HtmlFormat = false;

        foreach (char DialogueChar in m_text)
        {
            //TEXT:
            m_tmp.text += DialogueChar;

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

            //DELAY:
            if (HtmlFormat)
                continue;

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
        m_stage = Stage;
        onStage?.Invoke(Stage);
    }

    #endregion

    #region Control

    /// <summary>
    /// Next dialogue; or continue dialogue after choice option delay continue dialogue
    /// </summary>
    public void SetNext()
    {
        if (m_command != DialogueCommandType.Wait)
            //When current dialogue in done show up, press Next to move on next dialogue!
            return;

        m_command = DialogueCommandType.Next;
    }

    /// <summary>
    /// Skip current dialogue, until got choice option or end dialogue
    /// </summary>
    public void SetSkip()
    {
        if (m_command != DialogueCommandType.Text)
            //When current dialogue is showing up, press Next to skip and show full dialogue!
            return;

        StopCoroutine(m_iSetDialogueShowSingle);

        m_command = DialogueCommandType.Skip;
    }

    /// <summary>
    /// Stop dialogue
    /// </summary>
    public void SetStop(bool Clear = false)
    {
        StopAllCoroutines();
        StopCoroutine(m_iSetDialogueShowSingle);

        m_command = DialogueCommandType.None;
        m_active = false;

        SetStage(DialogueStageType.End);

        m_tmp.text = "";

        if (Clear)
        {
            m_dataCurrent = null;
            m_textCurrent = null;
            m_textNext = null;
        }
    }

    #endregion
}

public enum DialogueStageType
{
    None,
    //Trigger when Start
    Start,
    //Trigger when Show
    Text,
    Wait,
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

    private SerializedProperty m_delayStart;

    private void OnEnable()
    {
        m_target = target as DialogueManager;

        m_dialogueConfig = QUnityEditorCustom.GetField(this, "m_dialogueConfig");
        m_stringConfig = QUnityEditorCustom.GetField(this, "m_stringConfig");

        m_delayStart = QUnityEditorCustom.GetField(this, "m_delayStart");

        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_dialogueConfig);
        QUnityEditorCustom.SetField(m_stringConfig);

        QUnityEditorCustom.SetField(m_delayStart);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif