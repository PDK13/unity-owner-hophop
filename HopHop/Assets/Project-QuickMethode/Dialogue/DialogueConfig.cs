using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "dialogue-config", menuName = "Dialogue/Dialogue Config", order = 1)]
public class DialogueConfig : ScriptableObject
{
    public string AuthorNone = "None";
    public List<DialogueDataAuthor> Author = new List<DialogueDataAuthor>();
    public DialogueDataTextDelay DelayDefault;

    //

    public string[] AuthorName
    {
        get
        {
            if (Author == null)
                return null;

            if (Author.Count == 0)
                return null;

            List<string> Data = new List<string>() { AuthorNone };

            foreach (DialogueDataAuthor AuthorItem in Author)
                Data.Add(AuthorItem.Name);

            return Data.ToArray();
        }
    }

    public Sprite[] AuthorAvatar
    {
        get
        {
            if (Author == null)
                return null;

            if (Author.Count == 0)
                return null;

            List<Sprite> Data = new List<Sprite>() { null };

            foreach (DialogueDataAuthor AuthorItem in Author)
                Data.Add(AuthorItem.Avatar);
            //
            return Data.ToArray();
        }
    }

    //

    public DialogueDataAuthor GetAuthor(int AuthorIndex)
    {
        return Author[AuthorIndex];
    }

    public DialogueDataAuthor GetAuthor(string Name)
    {
        return Author.Find(t => t.Name == Name);
    }

#if UNITY_EDITOR

    public int EditorAuthorListCount
    {
        get => Author.Count;
        set
        {
            while (Author.Count > value)
                Author.RemoveAt(Author.Count - 1);
            while (Author.Count < value)
                Author.Add(new DialogueDataAuthor());
        }
    }

    public bool EditorAuthorListCommand { get; set; } = false;

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(DialogueConfig))]
public class DialogueConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 300f;
    private const float LABEL_WIDTH = 65f;

    private DialogueConfig m_target;

    private Vector2 m_scrollAuthor;

    private void OnEnable()
    {
        m_target = target as DialogueConfig;

        SetConfigFixed();
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

        SetGUIGroupAuthor();

        QUnityEditor.SetSpace();

        SetGUIGroupSetting();

        QUnityEditorCustom.SetApply(this);

        QUnityEditor.SetDirty(m_target);
    }

    //

    private void SetConfigFixed()
    {
        bool RemoveEmty = false;
        int Index = 0;
        while (Index < m_target.Author.Count)
        {
            if (m_target.Author[Index].Name == "")
            {
                RemoveEmty = true;
                m_target.Author.RemoveAt(Index);
            }
            else
                Index++;
        }
        QUnityEditor.SetDirty(m_target);
        //
        if (RemoveEmty)
            Debug.Log("[Dialogue] Author(s) emty have been remove from list");
    }

    //

    private void SetGUIGroupAuthor()
    {
        QUnityEditor.SetLabel("AUTHOR", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_target.EditorAuthorListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Author", m_target.EditorAuthorListCount, 0);
        //LIST
        m_scrollAuthor = QUnityEditor.SetScrollViewBegin(m_scrollAuthor, QUnityEditor.GetGUILayoutHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.Author.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.EditorAuthorListCommand = !m_target.EditorAuthorListCommand;
            m_target.Author[i].Name = QUnityEditor.SetField(m_target.Author[i].Name);
            m_target.Author[i].Avatar = QUnityEditor.SetField(m_target.Author[i].Avatar, QUnityEditor.GetGUILayoutSizeSprite());
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_target.EditorAuthorListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Author, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Author, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    m_target.Author.RemoveAt(i);
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private void SetGUIGroupSetting()
    {
        QUnityEditor.SetLabel("SETTING", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        #region DELAY - DEFAULT
        QUnityEditor.SetLabel("Delay Default", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH * 1.5f));

        #region DELAY - DEFAULT - ITEM
        QUnityEditor.SetVerticalBegin();

        #region DELAY - DEFAULT - ITEM - ALPHA
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Alpha", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.DelayDefault.Alpha = QUnityEditor.SetField(m_target.DelayDefault.Alpha);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        #region DELAY - DEFAULT - ITEM - SPACE
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Space", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.DelayDefault.Space = QUnityEditor.SetField(m_target.DelayDefault.Space);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        #region DELAY - DEFAULT - ITEM - MARK
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Mark", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.DelayDefault.Mark = QUnityEditor.SetField(m_target.DelayDefault.Mark);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        QUnityEditor.SetVerticalEnd();
        #endregion

        #endregion
    }
}

#endif