#if UNITY_EDITOR

using QuickMethode;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolPlayerRefs : EditorWindow
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

    private string ListPath => QPath.GetPath(QPath.PathType.Assets, @"..", "ToolPlayRefs.txt");

    [MenuItem("Tools/PlayerPrefs")]
    public static void Init()
    {
        GetWindow<ToolPlayerRefs>("PlayerPrefs");
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
        QEditor.SetLabel("REF", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        QEditor.SetHorizontalBegin();
        {
            QEditor.SetLabel("Type", QEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter),QEditorWindow.GetGUILayoutWidth(this, 0.2f));
            m_type = (VaribleType)QEditor.SetPopup<VaribleType>((int)m_type, QEditorWindow.GetGUILayoutWidth(this, 0.775f));
        }
        QEditor.SetHorizontalEnd();
        //
        QEditor.SetHorizontalBegin();
        {
            QEditor.SetLabel("Name", QEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.2f));
            m_name = QEditor.SetField(m_name, null, QEditorWindow.GetGUILayoutWidth(this, 0.775f));
        }
        QEditor.SetHorizontalEnd();
        //
        QEditor.SetHorizontalBegin();
        {
            QEditor.SetLabel("Value", QEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.2f));
            m_value = QEditor.SetField(m_value, null, QEditorWindow.GetGUILayoutWidth(this, 0.775f));
        }
        QEditor.SetHorizontalEnd();
        //
        QEditor.SetSpace(10f);
    }

    #endregion

    #region Option

    private void SetGUIOpption()
    {
        QEditor.SetLabel("OPPTION", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        QEditor.SetHorizontalBegin();
        {
            SetGUIButtonSet();
            SetGUIButtonGet();
            SetGUIButtonClear();
            SetGUIButtonClearAll();
        }
        QEditor.SetHorizontalEnd();
        //
        QEditor.SetSpace(10f);
    }

    private void SetGUIButtonSet()
    {
        if (m_name == "" || m_value == "")
            QEditor.SetDisableGroupBegin();
        //
        if (QEditor.SetButton("Set", null, QEditorWindow.GetGUILayoutWidth(this, 0.25f)))
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
            QEditor.SetDisableGroupEnd();
    }

    private void SetGUIButtonGet()
    {
        if (m_name == "")
            QEditor.SetDisableGroupBegin();
        //
        if (QEditor.SetButton("Get", null, QEditorWindow.GetGUILayoutWidth(this, 0.25f)))
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
            QEditor.SetDisableGroupEnd();
    }

    private void SetGUIButtonClear()
    {
        if (m_name == "")
            QEditor.SetDisableGroupBegin();
        //
        if (QEditor.SetButton("Clear", null, QEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            QPlayerPrefs.SetValueClear(m_name);
        }
        //
        if (m_name == "")
            QEditor.SetDisableGroupEnd();
    }

    private void SetGUIButtonClearAll()
    {
        if (QEditor.SetButton("Clear all", null, QEditorWindow.GetGUILayoutWidth(this, 0.25f)))
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
                QFileIO FileIO = new QFileIO();
                FileIO.SetReadStart(ListPath);
                int Count = FileIO.GetReadAutoInt();
                for (int i = 0; i < Count; i++)
                    m_list.Add(new ListSingle(FileIO.GetReadAutoString(), (VaribleType)FileIO.GetReadAutoInt()));
            }
            catch
            {
                QFileIO FileIO = new QFileIO();
                FileIO.SetWriteAdd(0);
                FileIO.SetWriteStart(ListPath);
            }
        }
        else
        {
            QFileIO FileIO = new QFileIO();
            FileIO.SetWriteAdd(0);
            FileIO.SetWriteStart(ListPath);
        }
    } //Refresh List from File!!

    private void SetListSave()
    {
        QFileIO FileIO = new QFileIO();
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
        QEditor.SetLabel("LIST", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter));
        //
        if (QEditor.SetButton("Refresh", QEditor.GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f, 0.25f)))
            SetListRefresh();
        //
        QEditor.SetScrollViewBegin(m_listScrollView);
        //
        for (int i = 0; i <= m_list.Count; i++)
        {
            QEditor.SetHorizontalBegin();
            //
            if (i == m_list.Count)
            {
                //Data New!!
                //
                if (m_name == "")
                    QEditor.SetDisableGroupBegin();
                //
                if (QEditor.SetButton("[New]", QEditor.GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f, 0.25f)))
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
                    QEditor.SetDisableGroupEnd();
            }
            else
            {
                //Data Single!!
                //
                if (QEditor.SetButton(m_list[i].Name, QEditor.GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.7f)))
                {
                    m_name = m_list[i].Name;
                    m_type = m_list[i].Type;
                }
                //
                VaribleType TypeChild = (VaribleType)QEditor.SetPopup<VaribleType>((int)m_list[i].Type, QEditorWindow.GetGUILayoutWidth(this, 0.2f));
                if (TypeChild != m_list[i].Type)
                {
                    m_list[i].Type = TypeChild;
                    SetListSave();
                }
                //
                if (QEditor.SetButton("Del", QEditor.GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.1f)))
                {
                    m_list.RemoveAt(i);
                    SetListSave();
                }
            }
            //
            QEditor.SetHorizontalEnd();
        }
        //
        QEditor.SetScrollViewEnd();
    }

    #endregion
}

#endif