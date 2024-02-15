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

    #region ==================================== Keyboard

    public static string GetKeyboardFormat(KeyCode KeyCode)
    {
        switch (KeyCode)
        {
            case KeyCode.Escape:
                return "Esc";
            case KeyCode.Return:
                return "Enter";
            case KeyCode.Delete:
                return "Del";
            case KeyCode.Backspace:
                return "B-Space";

            case KeyCode.Mouse0:
                return "L-Mouse";
            case KeyCode.Mouse1:
                return "R-Mouse";
            case KeyCode.Mouse2:
                return "M-Mouse";

            case KeyCode.LeftBracket:
                return "[";
            case KeyCode.RightBracket:
                return "]";

            case KeyCode.LeftCurlyBracket:
                return "{";
            case KeyCode.RightCurlyBracket:
                return "}";

            case KeyCode.LeftParen:
                return "(";
            case KeyCode.RightParen:
                return ")";

            case KeyCode.LeftShift:
                return "L-Shift";
            case KeyCode.RightShift:
                return "R-Shift";

            case KeyCode.LeftAlt:
                return "L-Alt";
            case KeyCode.RightAlt:
                return "R-Alt";

            case KeyCode.PageUp:
                return "Page-U";
            case KeyCode.PageDown:
                return "Page-D";
        }

        return KeyCode.ToString();
    }

    #endregion
}