using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "string-code-config", menuName = "QConfig/String Code", order = 0)]
public class StringCodeConfig : ScriptableObject
{
    [SerializeField] private string m_colorClear = "#clear";
    [SerializeField]
    private List<StringCodeColorDataConfig> m_color = new List<StringCodeColorDataConfig>()
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
    [SerializeField] private string m_codeEmty = "#emty";
    [SerializeField] private string m_codeReturn = "#return";
    [SerializeField] private List<StringCodeDataConfig> m_code = new List<StringCodeDataConfig>();

    [Space]
    [SerializeField] private List<StringCodeSpriteDataConfig> m_sprite = new List<StringCodeSpriteDataConfig>(); //See detail in methode below!

    public string GetColorHexFormatReplace(string Value)
    {
        //COLOR:
        Value = Value.Replace(m_colorClear, "</color>");
        foreach (StringCodeColorDataConfig Item in m_color)
            Value = Value.Replace(Item.Code, string.Format("<{0}>", QText.GetColorHexCode(Item.Color)));
        //
        //CODE:
        Value = Value.Replace(m_codeEmty, "");
        Value = Value.Replace(m_codeReturn, "\n");
        foreach (StringCodeDataConfig Item in m_code)
            Value = Value.Replace(Item.Code, Item.Value);
        //
        //SPRITE:
        //*NOTE*
        //To create an "TMP_Sprite Assets", first select an Texture, then Right-Mouse/Create/TextMeshPro/SpriteAssets.
        //To add an "TMP_Sprite Assets" to an TextMeshPro component, expain "Extra Setting" at bottom of component.
        //To easy use on "TMP_Sprite Assets", just drag Sprite from that Texture to list, then set Code.
        //If change name of Sprite from Sprite Asset, this will not work anymore.
        //*NOTE*
        foreach (StringCodeSpriteDataConfig Item in m_sprite)
            Value = Value.Replace(Item.Code, string.Format("<sprite name=\"{0}\">", Item.Sprite.name));
        //
        return Value;
    }
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