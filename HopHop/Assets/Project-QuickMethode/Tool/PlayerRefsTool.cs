#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerRefsTool : EditorWindow
{
    private enum VaribleType { Int, Float, String, }

    private string m_name = "";
    private VaribleType m_type = VaribleType.Int;
    private string m_value = "";

    private class ListSingle
    {
        public string Name;
        public VaribleType Type = VaribleType.Int;

        public ListSingle(string Name, VaribleType Type)
        {
            this.Name = Name;
            this.Type = Type;
        }
    }

    private List<ListSingle> m_list = new List<ListSingle>();
    private Vector2 m_listScrollView;

    private string ListPath => QPath.GetPath(QPath.PathType.Assets, "PlayerRefs.txt");

    [MenuItem("Tools/PlayerPrefs")]
    public static void Init()
    {
        GetWindow<PlayerRefsTool>("PlayerPrefs");
    }

    private void OnEnable()
    {
        SetListRefresh();
    }

    private void OnGUI()
    {
        SetGUIRef();

        SetGUIOpption();

        SetGUIListShow();
    }

    #region Ref

    private void SetGUIRef()
    {
        QUnityEditor.SetLabel("REF", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        QUnityEditor.SetHorizontalBegin();
        {
            QUnityEditor.SetLabel("Type", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.2f));
            m_type = (VaribleType)QUnityEditor.SetPopup<VaribleType>((int)m_type, QUnityEditorWindow.GetGUILayoutWidth(this, 0.775f));
        }
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetHorizontalBegin();
        {
            QUnityEditor.SetLabel("Name", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.2f));
            m_name = QUnityEditor.SetField(m_name, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.775f));
        }
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetHorizontalBegin();
        {
            QUnityEditor.SetLabel("Value", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.2f));
            m_value = QUnityEditor.SetField(m_value, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.775f));
        }
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetSpace();
    }

    #endregion

    #region Option

    private void SetGUIOpption()
    {
        QUnityEditor.SetLabel("OPPTION", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        QUnityEditor.SetHorizontalBegin();
        {
            SetGUIButtonSet();
            SetGUIButtonGet();
            SetGUIButtonClear();
            SetGUIButtonClearAll();
        }
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetSpace();
    }

    private void SetGUIButtonSet()
    {
        if (m_name == "" || m_value == "")
        {
            QUnityEditor.SetDisableGroupBegin();
        }
        //
        if (QUnityEditor.SetButton("Set", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            switch (m_type)
            {
                case VaribleType.Int:
                    QPlayerPrefs.SetValue(m_name, int.Parse(m_value));
                    break;
                case VaribleType.Float:
                    QPlayerPrefs.SetValue(m_name, float.Parse(m_value));
                    break;
                case VaribleType.String:
                    QPlayerPrefs.SetValue(m_name, m_value);
                    break;
            }
        }
        //
        if (m_name == "" || m_value == "")
        {
            QUnityEditor.SetDisableGroupEnd();
        }
    }

    private void SetGUIButtonGet()
    {
        if (m_name == "")
        {
            QUnityEditor.SetDisableGroupBegin();
        }
        //
        if (QUnityEditor.SetButton("Get", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            switch (m_type)
            {
                case VaribleType.Int:
                    m_value = QPlayerPrefs.GetValueInt(m_name).ToString();
                    break;
                case VaribleType.Float:
                    m_value = QPlayerPrefs.GetValueFloat(m_name).ToString();
                    break;
                case VaribleType.String:
                    m_value = QPlayerPrefs.GetValueString(m_name);
                    break;
            }
        }
        //
        if (m_name == "")
        {
            QUnityEditor.SetDisableGroupEnd();
        }
    }

    private void SetGUIButtonClear()
    {
        if (m_name == "")
        {
            QUnityEditor.SetDisableGroupBegin();
        }
        //
        if (QUnityEditor.SetButton("Clear", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            QPlayerPrefs.SetValueClear(m_name);
        }
        //
        if (m_name == "")
        {
            QUnityEditor.SetDisableGroupEnd();
        }
    }

    private void SetGUIButtonClearAll()
    {
        if (QUnityEditor.SetButton("Clear all", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            QPlayerPrefs.SetValueClearAll();
        }
    }

    #endregion

    #region List

    private void SetListRefresh()
    {
        m_list = new List<ListSingle>();
        //
        if (QPath.GetPathFileExist(ListPath))
        {
            try
            {
                QDataFile FileIO = new QDataFile();
                FileIO.SetReadStart(ListPath);
                int Count = FileIO.GetReadAutoInt();
                for (int i = 0; i < Count; i++)
                {
                    m_list.Add(new ListSingle(FileIO.GetReadAutoString(), (VaribleType)FileIO.GetReadAutoInt()));
                }
            }
            catch
            {
                QDataFile FileIO = new QDataFile();
                FileIO.SetWriteAdd(0);
                FileIO.SetWriteStart(ListPath);
            }
        }
        else
        {
            QDataFile FileIO = new QDataFile();
            FileIO.SetWriteAdd(0);
            FileIO.SetWriteStart(ListPath);
        }
    } //Refresh List from File!!

    private void SetListSave()
    {
        QDataFile FileIO = new QDataFile();
        FileIO.SetWriteAdd(m_list.Count);
        foreach (ListSingle Data in m_list)
        {
            FileIO.SetWriteAdd(Data.Name);
            FileIO.SetWriteAdd((int)Data.Type);
        }

        FileIO.SetWriteStart(ListPath);
    }

    private void SetGUIListShow()
    {
        QUnityEditor.SetLabel("LIST", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        if (QUnityEditor.SetButton("Refresh", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 1f, 0.25f)))
        {
            SetListRefresh();
        }
        //
        QUnityEditor.SetScrollViewBegin(m_listScrollView);
        //
        for (int i = 0; i <= m_list.Count; i++)
        {
            QUnityEditor.SetHorizontalBegin();
            //
            if (i == m_list.Count)
            {
                //Data New!!
                //
                if (m_name == "")
                {
                    QUnityEditor.SetDisableGroupBegin();
                }
                //
                if (QUnityEditor.SetButton("[New]", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 1f, 0.25f)))
                {
                    if (m_name != "")
                    {
                        SetListRefresh();
                        //
                        m_list.Add(new ListSingle(m_name, m_type));
                        //
                        SetListSave();
                    }
                }
                //
                if (m_name == "")
                {
                    QUnityEditor.SetDisableGroupEnd();
                }
            }
            else
            {
                //Data Single!!
                //
                if (QUnityEditor.SetButton(m_list[i].Name, QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.7f)))
                {
                    m_name = m_list[i].Name;
                    m_type = m_list[i].Type;
                }
                //
                VaribleType TypeChild = (VaribleType)QUnityEditor.SetPopup<VaribleType>((int)m_list[i].Type, QUnityEditorWindow.GetGUILayoutWidth(this, 0.2f));
                if (TypeChild != m_list[i].Type)
                {
                    m_list[i].Type = TypeChild;
                    SetListSave();
                }
                //
                if (QUnityEditor.SetButton("Del", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.1f)))
                {
                    m_list.RemoveAt(i);
                    SetListSave();
                }
            }
            //
            QUnityEditor.SetHorizontalEnd();
        }
        //
        QUnityEditor.SetScrollViewEnd();
    }

    #endregion
}

#endif