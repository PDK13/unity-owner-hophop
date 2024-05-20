using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "dialogue-config-single", menuName = "Dialogue/Dialogue Config Single", order = 1)]
public class DialogueConfigSingle : ScriptableObject
{
    public List<DialogueDataText> Dialogue = new List<DialogueDataText>();

#if UNITY_EDITOR

    public int EditorDialogueListCount
    {
        get => Dialogue.Count;
        set
        {
            while (Dialogue.Count > value)
                Dialogue.RemoveAt(Dialogue.Count - 1);
            while (Dialogue.Count < value)
                Dialogue.Add(new DialogueDataText(QUnityAssets.GetScriptableObject<DialogueConfig>("", false)[0].DelayDefault));
        }
    }

    public bool EditorDialogueListCommand { get; set; } = false;

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(DialogueConfigSingle))]
public class DialogueSingleConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 300f;
    private const float LABEL_WIDTH = 65f;

    private DialogueConfigSingle m_target;

    private DialogueConfig m_dialogueConfig;
    private string m_debugError = "";

    private Vector2 m_scrollDialogue;

    private void OnEnable()
    {
        m_target = target as DialogueConfigSingle;

        SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        if (m_debugError != "")
        {
            QUnityEditor.SetLabel(m_debugError, QUnityEditor.GetGUIStyleLabel());
            return;
        }

        SetGUIGroupDialogue();

        QUnityEditor.SetDirty(m_target);
    }

    //

    private void SetConfigFind()
    {
        if (m_dialogueConfig != null)
            return;

        var AuthorConfigFound = QUnityAssets.GetScriptableObject<DialogueConfig>("", false);

        if (AuthorConfigFound == null)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Dialogue] " + m_debugError);
            return;
        }

        if (AuthorConfigFound.Count == 0)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Dialogue] " + m_debugError);
            return;
        }

        if (AuthorConfigFound.Count > 1)
            Debug.Log("[Dialogue] Config found more than one, get the first one found");

        m_dialogueConfig = AuthorConfigFound[0];

        if (m_dialogueConfig.Author.Count == 0)
        {
            m_debugError = "Author Config not have any data, please add one";
            Debug.Log("[Dialogue] " + m_debugError);
            return;
        }

        //CONTINUE:

        m_debugError = "";
    }

    //

    private void SetGUIGroupDialogue()
    {
        QUnityEditor.SetLabel("DIALOGUE", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_target.EditorDialogueListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Dialogue", m_target.EditorDialogueListCount, 0);
        //LIST
        m_scrollDialogue = QUnityEditor.SetScrollViewBegin(m_scrollDialogue);
        for (int i = 0; i < m_target.Dialogue.Count; i++)
        {
            if (m_target.Dialogue[i] == null)
                continue;

            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.EditorDialogueListCommand = !m_target.EditorDialogueListCommand;

            #region ITEM - MAIN
            QUnityEditor.SetVerticalBegin();

            #region ITEM - MAIN - AUTHOR (FULL)
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(m_target.Dialogue[i].EditorName, QUnityEditor.GetGUIStyleLabel(m_target.Dialogue[i].EditorFull ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleLeft)))
                m_target.Dialogue[i].EditorFull = !m_target.Dialogue[i].EditorFull;
            QUnityEditor.SetHorizontalEnd();
            #endregion

            if (m_target.Dialogue[i].EditorFull)
            {
                #region ITEM - MAIN - AUTHOR
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Author", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                m_target.Dialogue[i].Author = QUnityEditor.SetPopup(m_target.Dialogue[i].Author, m_dialogueConfig.AuthorName);
                QUnityEditor.SetHorizontalEnd();
                #endregion

                #region ITEM - MAIN - DIALOGUE
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("Dialogue", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                m_target.Dialogue[i].Dialogue = QUnityEditor.SetField(m_target.Dialogue[i].Dialogue);
                QUnityEditor.SetHorizontalEnd();
                #endregion

                #region ITEM - MAIN - DELAY
                QUnityEditor.SetHorizontalBegin();
                if (QUnityEditor.SetButton("Delay", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleLeft), QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH)))
                    m_target.Dialogue[i].EditorDelayShow = !m_target.Dialogue[i].EditorDelayShow;
                if (m_target.Dialogue[i].EditorDelayShow)
                {
                    QUnityEditor.SetVerticalBegin();

                    QUnityEditor.SetHorizontalBegin();
                    QUnityEditor.SetLabel("Alpha", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                    m_target.Dialogue[i].Delay.Alpha = QUnityEditor.SetField(m_target.Dialogue[i].Delay.Alpha);
                    QUnityEditor.SetHorizontalEnd();

                    QUnityEditor.SetHorizontalBegin();
                    QUnityEditor.SetLabel("Space", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                    m_target.Dialogue[i].Delay.Space = QUnityEditor.SetField(m_target.Dialogue[i].Delay.Space);
                    QUnityEditor.SetHorizontalEnd();

                    QUnityEditor.SetHorizontalBegin();
                    QUnityEditor.SetLabel("Mark", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
                    m_target.Dialogue[i].Delay.Mark = QUnityEditor.SetField(m_target.Dialogue[i].Delay.Mark);
                    QUnityEditor.SetHorizontalEnd();

                    QUnityEditor.SetVerticalEnd();
                }
                else
                {
                    QUnityEditor.SetLabel("Alpha: " + m_target.Dialogue[i].Delay.Alpha, null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH * 1.25f));
                    QUnityEditor.SetLabel("Space: " + m_target.Dialogue[i].Delay.Space, null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH * 1.25f));
                    QUnityEditor.SetLabel("Mark: " + m_target.Dialogue[i].Delay.Mark, null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH * 1.25f));
                }
                QUnityEditor.SetHorizontalEnd();
                #endregion
            }

            QUnityEditor.SetHorizontalEnd();
            #endregion

            QUnityEditor.SetVerticalEnd();
            #endregion

            #region ARRAY
            if (m_target.EditorDialogueListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Dialogue, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Dialogue, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    m_target.Dialogue.RemoveAt(i);
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion

            if (m_target.Dialogue[i].EditorFull)
                QUnityEditor.SetSpace();
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif