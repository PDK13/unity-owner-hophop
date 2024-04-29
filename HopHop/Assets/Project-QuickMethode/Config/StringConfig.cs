using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "string-config", menuName = "String/String Config", order = 0)]
public class StringConfig : ScriptableObject
{
    public string CodeColorClear = "#clear";
    public List<StringCodeColorDataConfig> CodeColor = new List<StringCodeColorDataConfig>()
    {
        new StringCodeColorDataConfig("#red", Color.red),
        new StringCodeColorDataConfig("#blue", Color.blue),
        new StringCodeColorDataConfig("#green", Color.green),
        new StringCodeColorDataConfig("#yellow", Color.yellow),
        new StringCodeColorDataConfig("#white", Color.white),
        new StringCodeColorDataConfig("#cyan", Color.cyan),
        new StringCodeColorDataConfig("#mageta", Color.magenta),
        new StringCodeColorDataConfig("#black", Color.black),
    };

    [Space]
    public string CodeStringEmty = "#emty";
    public string CodeStringReturn = "#return";
    public List<StringCodeDataConfig> CodeString = new List<StringCodeDataConfig>();

    [Space]
    public List<StringCodeSpriteDataConfig> CodeSprite = new List<StringCodeSpriteDataConfig>(); //See detail in methode below!

    //

    public string GetColorHexFormatReplace(string Value)
    {
        //COLOR:
        Value = Value.Replace(CodeColorClear, "</color>");
        foreach (StringCodeColorDataConfig Item in CodeColor)
            Value = Value.Replace(Item.Code, string.Format("<{0}>", QText.GetColorHexCode(Item.Color)));
        //
        //CODE:
        Value = Value.Replace(CodeStringEmty, "");
        Value = Value.Replace(CodeStringReturn, "\n");
        foreach (StringCodeDataConfig Item in CodeString)
            Value = Value.Replace(Item.Code, Item.Value);
        //
        //SPRITE:
        //*NOTE*
        //To create an "TMP_Sprite Assets", first select an Texture, then Right-Mouse/Create/TextMeshPro/SpriteAssets.
        //To add an "TMP_Sprite Assets" to an TextMeshPro component, expain "Extra Setting" at bottom of component.
        //To easy use on "TMP_Sprite Assets", just drag Sprite from that Texture to list, then set Code.
        //If change name of Sprite from Sprite Asset, this will not work anymore.
        //*NOTE*
        foreach (StringCodeSpriteDataConfig Item in CodeSprite)
            Value = Value.Replace(Item.Code, string.Format("<sprite name=\"{0}\">", Item.Sprite.name));
        //
        return Value;
    }

#if UNITY_EDITOR

    public int EditorCodeColorListCount
    {
        get => CodeColor.Count;
        set
        {
            while (CodeColor.Count > value)
                CodeColor.RemoveAt(CodeColor.Count - 1);
            while (CodeColor.Count < value)
                CodeColor.Add(new StringCodeColorDataConfig("", Color.clear));
        }
    }

    public int EditorCodeStringListCount
    {
        get => CodeString.Count;
        set
        {
            while (CodeString.Count > value)
                CodeString.RemoveAt(CodeString.Count - 1);
            while (CodeString.Count < value)
                CodeString.Add(new StringCodeDataConfig("", ""));
        }
    }

    public int EditorCodeSpriteListCount
    {
        get => CodeSprite.Count;
        set
        {
            while (CodeSprite.Count > value)
                CodeSprite.RemoveAt(CodeSprite.Count - 1);
            while (CodeSprite.Count < value)
                CodeSprite.Add(new StringCodeSpriteDataConfig("", null));
        }
    }

    public bool EditorCodeColorListCommand { get; set; } = false;
    public bool EditorCodeStringListCommand { get; set; } = false;
    public bool EditorCodeSpriteListCommand { get; set; } = false;

#endif
}

[Serializable]
public class StringCodeColorDataConfig
{
    public string Code;
    public Color Color;

    public StringCodeColorDataConfig(string Code, Color Color)
    {
        this.Code = Code;
        this.Color = Color;
    }
}

[Serializable]
public class StringCodeDataConfig
{
    public string Code;
    public string Value;

    public StringCodeDataConfig(string Code, string Value)
    {
        this.Code = Code;
        this.Value = Value;
    }
}

[Serializable]
public class StringCodeSpriteDataConfig
{
    public string Code;
    public Sprite Sprite;

    public StringCodeSpriteDataConfig(string Code, Sprite Sprite)
    {
        this.Code = Code;
        this.Sprite = Sprite;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(StringConfig))]
public class StringConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 165f;
    private const float LABEL_WIDTH = 65f;

    private Vector2 m_scrollColor;
    private Vector2 m_scrollString;
    private Vector2 m_scrollSprite;

    private StringConfig m_target;

    private void OnEnable()
    {
        m_target = target as StringConfig;

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

        SetGUIGroupColor();

        QUnityEditor.SetSpace();

        SetGUIGroupString();

        QUnityEditor.SetSpace();

        SetGUIGroupSprite();

        QUnityEditorCustom.SetApply(this);

        QUnityEditor.SetDirty(m_target);
    }

    //

    private void SetConfigFixed()
    {
        bool RemoveEmty = false;
        int Index = 0;
        while (Index < m_target.CodeColor.Count)
        {
            if (string.IsNullOrEmpty(m_target.CodeColor[Index].Code))
            {
                RemoveEmty = true;
                m_target.CodeColor.RemoveAt(Index);
            }
            else
                Index++;
        }
        Index = 0;
        while (Index < m_target.CodeString.Count)
        {
            if (string.IsNullOrEmpty(m_target.CodeString[Index].Code))
            {
                RemoveEmty = true;
                m_target.CodeString.RemoveAt(Index);
            }
            else
                Index++;
        }
        Index = 0;
        while (Index < m_target.CodeSprite.Count)
        {
            if (string.IsNullOrEmpty(m_target.CodeSprite[Index].Code))
            {
                RemoveEmty = true;
                m_target.CodeSprite.RemoveAt(Index);
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

    private void SetGUIGroupColor()
    {
        QUnityEditor.SetLabel("COLOR", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        #region ITEM - MAIN - CLEAR
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Clear", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.CodeColorClear = QUnityEditor.SetField(m_target.CodeColorClear);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        //COUNT:
        m_target.EditorCodeColorListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Code", m_target.EditorCodeColorListCount, 0);
        //LIST
        m_scrollColor = QUnityEditor.SetScrollViewBegin(m_scrollColor, QUnityEditor.GetGUILayoutHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.EditorCodeColorListCount; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.EditorCodeColorListCommand = !m_target.EditorCodeColorListCommand;
            m_target.CodeColor[i].Code = QUnityEditor.SetField(m_target.CodeColor[i].Code);
            m_target.CodeColor[i].Color = QUnityEditor.SetField(m_target.CodeColor[i].Color, QUnityEditor.GetGUILayoutWidth(125f));
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_target.EditorCodeColorListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.CodeColor, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.CodeColor, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    m_target.CodeColor.RemoveAt(i);
                    m_target.EditorCodeColorListCount--;
                }
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private void SetGUIGroupString()
    {
        QUnityEditor.SetLabel("STRING", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        #region ITEM - MAIN - EMTY
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Emty", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.CodeStringEmty = QUnityEditor.SetField(m_target.CodeStringEmty);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        #region ITEM - MAIN - RETURN
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Return", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.CodeStringReturn = QUnityEditor.SetField(m_target.CodeStringReturn);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        //COUNT:
        m_target.EditorCodeStringListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Code", m_target.EditorCodeStringListCount, 0);
        //LIST
        m_scrollString = QUnityEditor.SetScrollViewBegin(m_scrollString, QUnityEditor.GetGUILayoutHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.EditorCodeStringListCount; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.EditorCodeStringListCommand = !m_target.EditorCodeStringListCommand;
            m_target.CodeString[i].Code = QUnityEditor.SetField(m_target.CodeString[i].Code);
            m_target.CodeString[i].Value = QUnityEditor.SetField(m_target.CodeString[i].Value, null, QUnityEditor.GetGUILayoutWidth(125f));
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_target.EditorCodeStringListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.CodeString, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.CodeString, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    m_target.CodeString.RemoveAt(i);
                    m_target.EditorCodeStringListCount--;
                }
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private void SetGUIGroupSprite()
    {
        QUnityEditor.SetLabel("SPRITE", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        //COUNT:
        m_target.EditorCodeSpriteListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Code", m_target.EditorCodeSpriteListCount, 0);
        //LIST
        m_scrollSprite = QUnityEditor.SetScrollViewBegin(m_scrollSprite, QUnityEditor.GetGUILayoutHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.EditorCodeSpriteListCount; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.EditorCodeSpriteListCommand = !m_target.EditorCodeSpriteListCommand;
            m_target.CodeSprite[i].Code = QUnityEditor.SetField(m_target.CodeSprite[i].Code);
            m_target.CodeSprite[i].Sprite = QUnityEditor.SetField(m_target.CodeSprite[i].Sprite, QUnityEditor.GetGUILayoutSizeSprite());
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_target.EditorCodeSpriteListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.CodeSprite, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.CodeSprite, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    m_target.CodeSprite.RemoveAt(i);
                    m_target.EditorCodeSpriteListCount--;
                }
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif