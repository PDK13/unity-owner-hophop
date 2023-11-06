using UnityEngine;

public class QColor
{
    #region ==================================== Color

    //Key "ref" use to immediately save value to primary varible param!

    public static void SetColor(ref Color Color, float A)
    {
        Color = new Color(Color.r, Color.g, Color.b, A);
    }

    public static Color GetColor(Color Color, float A)
    {
        return new Color(Color.r, Color.g, Color.b, A);
    }

    #endregion

    #region ==================================== Color - Hex Color

    private static string GetColorHex(Color Color)
    {
        return ColorUtility.ToHtmlStringRGB(Color);
    }

    public static string GetColorHexFormat(Color Color)
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

    #region ==================================== Color - Mesh Renderer & Material

    //Can be applied to Spine Material!!

    public static void SetMaterial(MeshRenderer MessRenderer, float Alpha)
    {
        if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
        {
            return;
        }

        Color Color = MessRenderer.material.color;
        SetColor(ref Color, Alpha);
        MessRenderer.material.color = Color;
    }

    public static void SetMaterial(MeshRenderer MessRenderer, Color Color, float Alpha)
    {
        if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
        {
            return;
        }

        SetColor(ref Color, Alpha);
        MessRenderer.material.color = Color;
    }

    public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color)
    {
        //Shader of Material must set to "Tint"!!

        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

        int IdColor = Shader.PropertyToID("_Color");
        int IdBlack = Shader.PropertyToID("_Black");

        MaterialPropertyBlock.SetColor(IdColor, Color);
        MaterialPropertyBlock.SetColor(IdBlack, Color.black); //Should be "Color.black"!!

        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color, Color Black)
    {
        //Shader of Material must set to "Tint"!!

        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

        int IdColor = Shader.PropertyToID("_Color");
        int IdBlack = Shader.PropertyToID("_Black");

        MaterialPropertyBlock.SetColor(IdColor, Color);
        MaterialPropertyBlock.SetColor(IdBlack, Black); //Should be "Color.black"!!

        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    public static void SetMaterialFill(MeshRenderer MessRenderer, Color FillColor, float FillPhase = 1)
    {
        //Shader of Material must set to "Fill"!!

        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

        int IdFillColor = Shader.PropertyToID("_FillColor");
        int IdFillPhase = Shader.PropertyToID("_FillPhase");

        MaterialPropertyBlock.SetColor(IdFillColor, FillColor);
        MaterialPropertyBlock.SetFloat(IdFillPhase, FillPhase);

        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    #endregion

    #region ==================================== Color - SpriteRenderer

    public static void SetSprite(SpriteRenderer SpriteRenderer, float Alpha)
    {
        Color Color = SpriteRenderer.color;
        SetColor(ref Color, Alpha);
        SpriteRenderer.color = Color;
    }

    public static void SetSprite(SpriteRenderer SpriteRenderer, Color Color, float Alpha)
    {
        SetColor(ref Color, Alpha);
        SpriteRenderer.color = Color;
    }

    #endregion

    #region ==================================== Color - Image

    public static void SetSprite(UnityEngine.UI.Image Image, float Alpha)
    {
        Color Color = Image.color;
        SetColor(ref Color, Alpha);
        Image.color = Color;
    }

    public static void SetSprite(UnityEngine.UI.Image Image, Color Color, float Alpha)
    {
        SetColor(ref Color, Alpha);
        Image.color = Color;
    }

    #endregion
}
