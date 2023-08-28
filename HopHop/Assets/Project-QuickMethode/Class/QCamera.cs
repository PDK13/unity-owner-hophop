using UnityEngine;

public class QCamera
{
    //Required only ONE Main Camera (with tag Main Camera) for the true result!!

    #region ==================================== Pos of World & Canvas

    public static Vector3 GetPosMouseToWorld()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static Vector2 GetPosMouseToCanvas()
    {
        //NOTE: The value just apply for RecTransform got Anchors Centre and Pivot Centre!
        return GetPosWorldToCanvas(GetPosMouseToWorld());
    }

    public static Vector2 GetPosWorldToCanvas(Vector3 PosWorld)
    {
        //NOTE: The value just apply for RecTransform got Anchors Centre and Pivot Centre!
        return (Vector2)Camera.main.WorldToScreenPoint(PosWorld) - GetCameraSizePixel() * 0.5f;
    }

    #endregion

    #region ==================================== Camera

    //CAMERA mode ORTHOGRAPHIC - SIZE is a HALF number of UNIT WORLD HEIGHT from Scene to Screen.
    //EX: If Camera orthographic Size is 1, mean need 2 Square 1x1 Unit world to fill full HEIGHT of screen.

    public static Vector2 GetCameraSizePixel()
    {
        return GetCameraSizePixel(Camera.main);
    }

    public static Vector2 GetCameraSizeUnit()
    {
        return GetCameraSizeUnit(Camera.main);
    }

    public static Vector2 GetCameraSizePixel(Camera Camera)
    {
        return new Vector2(Camera.pixelWidth, Camera.pixelHeight);
    }

    public static Vector2 GetCameraSizeUnit(Camera Camera)
    {
        Vector2 SizePixel = GetCameraSizePixel(Camera);
        float HeightUnit = Camera.orthographicSize * 2;
        float WidthUnit = HeightUnit * (SizePixel.x / SizePixel.y);

        return new Vector2(WidthUnit, HeightUnit);
    }

    #endregion

    #region ==================================== Screen

    public static Vector2 GetScreenSizePixel()
    {
        return new Vector2(Screen.width, Screen.height);
    }

    #endregion
} //Note: Current ORTHOGRAPHIC 2D only!!

public class QResolution
{
    #region ==================================== Convert

    public enum UnitScaleType { Width, Height, Span, Primary, Tarket, }

    public static Vector2 GetSizeUnitScaled(Sprite SpritePrimary, Sprite SpriteTarket, UnitScaleType SpriteScale)
    {
        return GetSizeUnitScaled(QSprite.GetSpriteSizeUnit(SpritePrimary), QSprite.GetSpriteSizeUnit(SpriteTarket), SpriteScale);
    }

    public static Vector2 GetSizeUnitScaled(Vector2 SizeUnitPrimary, Vector2 SizeUnitTarket, UnitScaleType SpriteScale)
    {
        Vector2 SizeUnitFinal = new Vector2();

        switch (SpriteScale)
        {
            case UnitScaleType.Width:
                {
                    float OffsetX = SizeUnitTarket.x / SizeUnitPrimary.x;
                    float SizeUnitFinalX = SizeUnitPrimary.x * OffsetX;
                    float SizeUnitFinalY = SizeUnitPrimary.y * OffsetX;
                    SizeUnitFinal = new Vector2(SizeUnitFinalX, SizeUnitFinalY);
                }
                break;
            case UnitScaleType.Height:
                {
                    float OffsetY = SizeUnitTarket.y / SizeUnitPrimary.y;
                    float SizeUnitFinalX = SizeUnitPrimary.x * OffsetY;
                    float SizeUnitFinalY = SizeUnitPrimary.y * OffsetY;
                    SizeUnitFinal = new Vector2(SizeUnitFinalX, SizeUnitFinalY);
                }
                break;
            case UnitScaleType.Span:
                {
                    float OffsetX = SizeUnitTarket.x / SizeUnitPrimary.x;
                    float OffsetY = SizeUnitTarket.y / SizeUnitPrimary.y;
                    if (OffsetX < OffsetY)
                    {
                        SizeUnitFinal = GetSizeUnitScaled(SizeUnitPrimary, SizeUnitTarket, UnitScaleType.Height);
                    }
                    else
                    {
                        SizeUnitFinal = GetSizeUnitScaled(SizeUnitPrimary, SizeUnitTarket, UnitScaleType.Width);
                    }
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

    #endregion
}