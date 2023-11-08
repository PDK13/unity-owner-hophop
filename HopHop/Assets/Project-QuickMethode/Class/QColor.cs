using UnityEngine;
using UnityEngine.UI;

public class QColor
{
    #region ==================================== Color

    //Key "ref" use to immediately save value to primary varible param!

    public static Color GetColor(Color Color, float A)
    {
        return new Color(Color.r, Color.g, Color.b, A);
    }

    #endregion

    #region ==================================== Color - Hex Color

    public static string GetTextHex(Color Color)
    {
        return ColorUtility.ToHtmlStringRGB(Color);
    }

    public static string GetTextHexCode(Color Color)
    {
        string ColorHex = GetTextHex(Color);
        string ColorHexCode = string.Format("#{0}", ColorHex);
        return ColorHexCode;
    }

    public static string GetTextHexFormat(Color Color, string Text)
    {
        string ColorHex = GetTextHex(Color);
        string TextFormat = string.Format("<#{0}>{1}</color>", ColorHex, Text);
        return TextFormat;
    }

    #endregion

    #region ==================================== Color - Mesh Renderer & Material

    //Can be applied to Spine Material!!

    public static void SetMaterial(MeshRenderer MessRenderer, float Alpha)
    {
        if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
            return;
        //
        MessRenderer.material.color = GetColor(MessRenderer.material.color, Alpha);
    }

    public static void SetMaterial(MeshRenderer MessRenderer, Color Color, float Alpha)
    {
        if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
            return;
        //
        MessRenderer.material.color = GetColor(Color, Alpha);
    }

    public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color)
    {
        //Shader of Material must set to "Tint"!!
        //
        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();
        //
        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);
        //
        int IdColor = Shader.PropertyToID("_Color");
        int IdBlack = Shader.PropertyToID("_Black");
        //
        MaterialPropertyBlock.SetColor(IdColor, Color);
        MaterialPropertyBlock.SetColor(IdBlack, Color.black); //Should be "Color.black"!!
        //
        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color, Color Black)
    {
        //Shader of Material must set to "Tint"!!
        //
        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();
        //
        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);
        //
        int IdColor = Shader.PropertyToID("_Color");
        int IdBlack = Shader.PropertyToID("_Black");
        //
        MaterialPropertyBlock.SetColor(IdColor, Color);
        MaterialPropertyBlock.SetColor(IdBlack, Black); //Should be "Color.black"!!
        //
        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    public static void SetMaterialFill(MeshRenderer MessRenderer, Color FillColor, float FillPhase = 1)
    {
        //Shader of Material must set to "Fill"!!
        //
        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();
        //
        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);
        //
        int IdFillColor = Shader.PropertyToID("_FillColor");
        int IdFillPhase = Shader.PropertyToID("_FillPhase");
        //
        MaterialPropertyBlock.SetColor(IdFillColor, FillColor);
        MaterialPropertyBlock.SetFloat(IdFillPhase, FillPhase);
        //
        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    #endregion

    #region ==================================== Color - SpriteRenderer

    public static void SetSprite(SpriteRenderer SpriteRenderer, float Alpha)
    {
        SpriteRenderer.color = GetColor(SpriteRenderer.color, Alpha);
    }

    public static void SetSprite(SpriteRenderer SpriteRenderer, Color Color, float Alpha)
    {
        SpriteRenderer.color = GetColor(Color, Alpha);
    }

    #endregion

    #region ==================================== Color - Image

    public static void SetSprite(Image Image, float Alpha)
    {
        Image.color = GetColor(Image.color, Alpha);
    }

    public static void SetSprite(Image Image, Color Color, float Alpha)
    {
        Image.color = GetColor(Color, Alpha);
    }

    #endregion
}