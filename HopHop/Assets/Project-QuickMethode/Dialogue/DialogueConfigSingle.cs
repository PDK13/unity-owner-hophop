using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "dialogue-config-single", menuName = "Dialogue/Dialogue Config Single", order = 1)]
public class DialogueConfigSingle : ScriptableObject
{
    public List<DialogueDataText> Dialogue = new List<DialogueDataText>();
}

#if UNITY_EDITOR

[CustomEditor(typeof(DialogueConfigSingle))]
public class DialogueSingleConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 150f * 2;
    private const float LABEL_WIDTH = 65f;

    private DialogueConfigSingle m_target;

    private DialogueConfig m_dialogueConfig;
    private string m_debugError = "";

    private int m_dialogueCount = 0;

    private List<bool> m_dialogueDelayShow = new List<bool>();

    private Vector2 m_scrollDialogue;

    private void OnEnable()
    {
        m_target = target as DialogueConfigSingle;

        m_dialogueCount = m_target.Dialogue.Count;

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

    private void SetGUIGroupDialogue()
    {
        QUnityEditor.SetLabel("DIALOGUE", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_dialogueCount = QUnityEditor.SetGroupNumberChangeLimitMin("Dialogue", m_dialogueCount, 0);

        //COUNT:
        while (m_dialogueCount > m_target.Dialogue.Count)
            m_target.Dialogue.Add(new DialogueDataText(m_dialogueConfig.DelayDefault));
        while (m_dialogueCount > m_dialogueDelayShow.Count)
            m_dialogueDelayShow.Add(false);

        while (m_dialogueCount < m_target.Dialogue.Count)
            m_target.Dialogue.RemoveAt(m_target.Dialogue.Count - 1);
        while (m_dialogueCount < m_dialogueDelayShow.Count)
            m_dialogueDelayShow.RemoveAt(m_dialogueDelayShow.Count - 1);

        //LIST
        m_scrollDialogue = QUnityEditor.SetScrollViewBegin(m_scrollDialogue);
        for (int i = 0; i < m_target.Dialogue.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));

            #region ITEM - MAIN
            QUnityEditor.SetVerticalBegin();

            #region ITEM - MAIN - AUTHOR
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Author", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
            m_target.Dialogue[i].AuthorIndex = QUnityEditor.SetPopup(m_target.Dialogue[i].AuthorIndex, m_dialogueConfig.AuthorName);
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
            if (QUnityEditor.SetButton("Delay", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH)))
                m_dialogueDelayShow[i] = !m_dialogueDelayShow[i];
            if (m_dialogueDelayShow[i])
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

            QUnityEditor.SetHorizontalEnd();
            #endregion

            QUnityEditor.SetVerticalEnd();
            #endregion

            QUnityEditor.SetSpace();
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif