using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "string-config", menuName = "", order = 0)]
public class StringConfig : ScriptableObject
{
    [SerializeField] private string m_colorClear = "#clear";
    [SerializeField]
    private List<StringColorConfig> m_color = new List<StringColorConfig>()
    {
        new StringColorConfig("#red", Color.red),
        new StringColorConfig("#blue", Color.blue),
        new StringColorConfig("#green", Color.green),
        new StringColorConfig("#yellow", Color.yellow),
        new StringColorConfig("#white", Color.white),
        new StringColorConfig("#cyan", Color.cyan),
        new StringColorConfig("#mageta", Color.magenta),
        new StringColorConfig("#black", Color.black),
    };

    [Space]
    [SerializeField] private string m_codeEmty = "#emty";
    [SerializeField] private string m_codeReturn = "#return";
    [SerializeField] private List<StringCodeConfig> m_code = new List<StringCodeConfig>();

    [Space]
    [SerializeField] private List<SpriteCodeConfig> m_sprite = new List<SpriteCodeConfig>(); //See detail in methode below!

    public string GetColorHexFormatReplace(string Value)
    {
        //COLOR:
        Value = Value.Replace(m_colorClear, "</color>");
        foreach (StringColorConfig Item in m_color)
            Value = Value.Replace(Item.Code, string.Format("<{0}>", QColor.GetTextHexCode(Item.Color)));
        //
        //CODE:
        Value = Value.Replace(m_codeEmty, "");
        Value = Value.Replace(m_codeReturn, "\n");
        foreach (StringCodeConfig Item in m_code)
            Value = Value.Replace(Item.Code, Item.Value);
        //
        //SPRITE:
        //*NOTE*
        //To create an "TMP_Sprite Assets", first select an Texture, then Right-Mouse/Create/TextMeshPro/SpriteAssets.
        //To add an "TMP_Sprite Assets" to an TextMeshPro component, expain "Extra Setting" at bottom of component.
        //To easy use on "TMP_Sprite Assets", just drag Sprite from that Texture to list, then set Code.
        //If change name of Sprite from Sprite Asset, this will not work anymore.
        //*NOTE*
        foreach (SpriteCodeConfig Item in m_sprite)
            Value = Value.Replace(Item.Code, string.Format("<sprite name=\"{0}\">", Item.Sprite.name));
        //
        return Value;
    }
}

[Serializable]
public class StringColorConfig
{
    public string Code;
    public Color Color;

    public StringColorConfig(string Code, Color Color)
    {
        this.Code = Code;
        this.Color = Color;
    }
}

[Serializable]
public class StringCodeConfig
{
    public string Code;
    public string Value;

    public StringCodeConfig(string Code, string Value)
    {
        this.Code = Code;
        this.Value = Value;
    }
}

[Serializable]
public class SpriteCodeConfig
{
    public string Code;
    public Sprite Sprite;

    public SpriteCodeConfig(string Code, Sprite Sprite)
    {
        this.Code = Code;
        this.Sprite = Sprite;
    }
}