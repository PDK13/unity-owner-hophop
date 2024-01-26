using UnityEngine;
using UnityEngine.UI;

public class QColor
{
    //Alpha value in range [0..1] in code and [0..255] in editor!

    #region ==================================== Color

    /// <summary>
    /// Get Color with custom alpha value [0..1]
    /// </summary>
    /// <param name="Color"></param>
    /// <param name="Alpha">Alpha value [0..1]</param>
    /// <returns></returns>
    public static Color GetColor(Color Color, float Alpha = 1f)
    {
        return new Color(Color.r, Color.g, Color.b, Mathf.Clamp(Alpha, 0f, 1f));
    }

    #endregion

    #region ==================================== Color - Mesh Renderer & Material

    //Can be applied to Spine Material!!

    public static void SetMaterial(MeshRenderer MessRenderer, float Alpha = 1f)
    {
        if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
            return;
        //
        MessRenderer.material.color = GetColor(MessRenderer.material.color, Alpha);
    }

    public static void SetMaterial(MeshRenderer MessRenderer, Color Color, float Alpha = 1f)
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

    public static void SetSprite(SpriteRenderer SpriteRenderer, float Alpha = 1f)
    {
        SpriteRenderer.color = GetColor(SpriteRenderer.color, Alpha);
    }

    public static void SetSprite(SpriteRenderer SpriteRenderer, Color Color, float Alpha = 1f)
    {
        SpriteRenderer.color = GetColor(Color, Alpha);
    }

    #endregion

    #region ==================================== Color - Image

    public static void SetSprite(Image Image, float Alpha = 1f)
    {
        Image.color = GetColor(Image.color, Alpha);
    }

    public static void SetSprite(Image Image, Color Color, float Alpha = 1f)
    {
        Image.color = GetColor(Color, Alpha);
    }

    #endregion
}