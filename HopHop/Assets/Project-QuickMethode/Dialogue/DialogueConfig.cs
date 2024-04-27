using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "dialogue-config", menuName = "Dialogue/Dialogue Config", order = 1)]
public class DialogueConfig : ScriptableObject
{
    public List<DialogueDataAuthor> Author = new List<DialogueDataAuthor>();
    public DialogueDataTextDelay DelayDefault;

    //

    public string[] AuthorName
    {
        get
        {
            List<string> Data = new List<string>();
            //
            if (Author == null)
                return null;
            //
            if (Author.Count == 0)
                return null;
            //
            foreach (DialogueDataAuthor AuthorItem in Author)
                Data.Add(AuthorItem.Name);
            //
            return Data.ToArray();
        }
    }

    public Sprite[] AuthorAvatar
    {
        get
        {
            if (Author == null)
                return null;
            //
            if (Author.Count == 0)
                return null;
            //
            List<Sprite> Data = new List<Sprite>();
            //
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
}

#if UNITY_EDITOR

[CustomEditor(typeof(DialogueConfig))]
public class DialogueConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 150f * 2;
    private const float LABEL_WIDTH = 65f;

    private DialogueConfig m_target;

    private int m_authorCount = 0;

    private Vector2 m_scrollAuthor;

    private void OnEnable()
    {
        m_target = target as DialogueConfig;

        m_authorCount = m_target.Author.Count;

        SetConfigAuthorFixed();
    }

    private void OnDisable()
    {
        SetConfigAuthorFixed();
    }

    private void OnDestroy()
    {
        SetConfigAuthorFixed();
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

    private void SetGUIGroupAuthor()
    {
        QUnityEditor.SetLabel("AUTHOR", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_authorCount = QUnityEditor.SetGroupNumberChangeLimitMin("Author", m_authorCount, 0);

        //COUNT:
        while (m_authorCount > m_target.Author.Count)
            m_target.Author.Add(new DialogueDataAuthor());
        while (m_authorCount < m_target.Author.Count)
            m_target.Author.RemoveAt(m_target.Author.Count - 1);

        //LIST
        m_scrollAuthor = QUnityEditor.SetScrollViewBegin(m_scrollAuthor);
        for (int i = 0; i < m_target.Author.Count; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
            m_target.Author[i].Name = QUnityEditor.SetField(m_target.Author[i].Name);
            m_target.Author[i].Avatar = QUnityEditor.SetField(m_target.Author[i].Avatar);
            QUnityEditor.SetHorizontalEnd();
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

    //

    private void SetConfigAuthorFixed()
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
}

#endif