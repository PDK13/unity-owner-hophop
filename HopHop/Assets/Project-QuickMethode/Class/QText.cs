using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use for Text or TextMeshPro Component!!
/// </summary>
public class QText
{
    #region ==================================== Color - Color Hex

    public static string GetColorHex(Color Color)
    {
        return ColorUtility.ToHtmlStringRGB(Color);
    }

    public static string GetColorHexCode(Color Color)
    {
        string ColorHex = GetColorHex(Color);
        string ColorHexCode = string.Format("#{0}", ColorHex);
        return ColorHexCode;
    }

    public static string GetColorHexFormat(Color Color, string Text)
    {
        string ColorHex = GetColorHex(Color);
        string TextFormat = string.Format("<#{0}>{1}</color>", ColorHex, Text);
        return TextFormat;
    }

    #endregion

    #region ==================================== Sprite - Sprite Assets

    public static string GetSpriteFormat(string SpriteName)
    {
        return string.Format("<sprite name=”{0}”>", SpriteName);
    }

    #endregion
}