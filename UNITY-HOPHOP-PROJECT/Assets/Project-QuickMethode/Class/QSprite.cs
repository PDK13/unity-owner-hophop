using System.IO;
using UnityEngine;

public class QSprite
{
    #region ==================================== File

    public static Sprite GetSprite(string Path)
    {
        Texture2D TextureScreen = null;
        byte[] ByteEncode;

        ByteEncode = File.ReadAllBytes(Path);
        TextureScreen = new Texture2D(100, 100, TextureFormat.RGBA32, false);
        TextureScreen.LoadImage(ByteEncode);
        //
        if (TextureScreen == null)
            return null;
        //
        return Sprite.Create(TextureScreen, new Rect(0, 0, TextureScreen.width, TextureScreen.height), new Vector2(0.5f, 0.5f));
    }

    #endregion

    #region ==================================== Sprite

    public static Vector2 GetSizePixel(Sprite From)
    {
        return GetSizeUnit(From) * GetPixelPerUnit(From) * 1.0f;
    }

    public static Vector2 GetSizeUnit(Sprite From)
    {
        return From.bounds.size * 1.0f;
    }

    public static float GetPixelPerUnit(Sprite From)
    {
        return From.pixelsPerUnit * 1.0f;
    }

    #endregion

    #region ==================================== Sprite Renderer

    //...

    #endregion

    #region ==================================== Border

    public static Vector2 GetBorderPos(SpriteRenderer From, params Direction[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;

        //Caculate Position from Value Data
        foreach (Direction DirBound in Bound)
        {
            switch (DirBound)
            {
                case Direction.Up:
                    Pos.y += Size.y / 2;
                    break;
                case Direction.Down:
                    Pos.y -= Size.y / 2;
                    break;
                case Direction.Left:
                    Pos.x -= Size.x / 2;
                    break;
                case Direction.Right:
                    Pos.x += Size.x / 2;
                    break;
            }
        }
        return Pos;
    }

    public static Vector2 GetBorderPos(SpriteRenderer From, params DirectionX[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;

        //Caculate Position from Value Data
        foreach (DirectionX DirBound in Bound)
        {
            switch (DirBound)
            {
                case DirectionX.Left:
                    Pos.x -= Size.x / 2;
                    break;
                case DirectionX.Right:
                    Pos.x += Size.x / 2;
                    break;
            }
        }
        return Pos;
    }

    public static Vector2 GetBorderPos(SpriteRenderer From, params DirectionY[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;

        //Caculate Position from Value Data
        foreach (DirectionY DirBound in Bound)
        {
            switch (DirBound)
            {
                case DirectionY.Up:
                    Pos.y += Size.y / 2;
                    break;
                case DirectionY.Down:
                    Pos.y -= Size.y / 2;
                    break;
            }
        }
        return Pos;
    }

    #endregion

    #region ==================================== Texture

    //Texture can be used for Window Editor (Button)

    //Ex:
    //Window Editor:
    //Texture2D Texture = QSprite.GetTextureConvert(Sprite);
    //GUIContent Content = new GUIContent("", (Texture)Texture);
    //GUILayout.Button(Content());

    public static Texture2D GetTextureConvert(Sprite From)
    {
        if (From.texture.isReadable == false)
        {
            return null;
        }

        Texture2D Texture = new Texture2D((int)From.rect.width, (int)From.rect.height);

        Color[] ColorPixel = From.texture.GetPixels(
            (int)From.textureRect.x,
            (int)From.textureRect.y,
            (int)From.textureRect.width,
            (int)From.textureRect.height);
        Texture.SetPixels(ColorPixel);
        Texture.Apply();
        return Texture;
    }

    public static string GetTextureConvertName(Sprite From)
    {
        return GetTextureConvert(From).name;
    }

    #endregion
}

public class QSpriteScale
{
    public static Vector2 GetSizeUnitScaled(Sprite SpritePrimary, Sprite SpriteTarket, UnitScaleType SpriteScale)
    {
        return GetSizeUnitScaled(QSprite.GetSizeUnit(SpritePrimary), QSprite.GetSizeUnit(SpriteTarket), SpriteScale);
    }

    public static Vector2 GetSizeUnitScaled(Vector2 SizeUnitPrimary, Vector2 SizeUnitTarket, UnitScaleType SpriteScale)
    {
        Vector2 SizeUnitFinal = new Vector2();

        switch (SpriteScale)
        {
            case UnitScaleType.Width:
                {
                    float RatioX = SizeUnitTarket.x / SizeUnitPrimary.x;
                    float SizeUnitFinalX = SizeUnitPrimary.x * RatioX;
                    float SizeUnitFinalY = SizeUnitPrimary.y * RatioX;
                    SizeUnitFinal = new Vector2(SizeUnitFinalX, SizeUnitFinalY);
                }
                break;
            case UnitScaleType.Height:
                {
                    float RatioY = SizeUnitTarket.y / SizeUnitPrimary.y;
                    float SizeUnitFinalX = SizeUnitPrimary.x * RatioY;
                    float SizeUnitFinalY = SizeUnitPrimary.y * RatioY;
                    SizeUnitFinal = new Vector2(SizeUnitFinalX, SizeUnitFinalY);
                }
                break;
            case UnitScaleType.Span:
                {
                    float RatioX = SizeUnitTarket.x / SizeUnitPrimary.x;
                    float RatioY = SizeUnitTarket.y / SizeUnitPrimary.y;
                    if (RatioX < RatioY)
                        SizeUnitFinal = GetSizeUnitScaled(SizeUnitPrimary, SizeUnitTarket, UnitScaleType.Height);
                    else
                        SizeUnitFinal = GetSizeUnitScaled(SizeUnitPrimary, SizeUnitTarket, UnitScaleType.Width);
                }
                break;
            case UnitScaleType.Primary:
                SizeUnitFinal = SizeUnitPrimary;
                break;
            case UnitScaleType.Tarket:
                SizeUnitFinal = SizeUnitTarket;
                break;
        }

        return SizeUnitFinal;
    }

}

public enum UnitScaleType { Primary, Tarket, Span, Width, Height, }