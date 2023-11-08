using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Camera ORTHOGRAPHIC 2D only!
/// </summary>
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
}

public class QScreenCapture
{
    //Get Image from 'QSprite.GetScreenShot()' class!

#if UNITY_EDITOR

    [MenuItem("Tools/ScreenCapture")]
    private static void SetScreenCapture()
    {
        int Index = 0;
        while (QPath.GetPathFileExist(QPath.GetPath(QPath.PathType.Picture, string.Format("{0}_{1}.png", Application.productName, Index)))) Index++;
        SetScreenCapture(QPath.GetPath(QPath.PathType.Picture, string.Format("{0}_{1}.png", Application.productName, Index)));
    }

#endif

    public static void SetScreenCapture(string Path)
    {
        ScreenCapture.CaptureScreenshot(Path);
        Debug.Log("[ScreenCapture] " + Path);
    }
}

public class QScreenShot
{
    //Get Image from 'QSprite.GetScreenShot()' class!
    
    /// <summary>
    /// Work inside 'OnPostRender()' methode with Camera component!
    /// </summary>
    public static void SetScreenShotFullScreen()
    {
        int Index = 0;
        while (QPath.GetPathFileExist(QPath.GetPath(QPath.PathType.Picture, string.Format("{0}_{1}.png", Application.productName, Index)))) Index++;
        SetScreenShotFullScreen(QPath.GetPath(QPath.PathType.Picture, string.Format("{0}_{1}.png", Application.productName, Index)));
    }

    /// <summary>
    /// Work inside 'OnPostRender()' methode with Camera component!
    /// </summary>
    public static void SetScreenShotFullScreen(string Path)
    {
        SetScreenShot(Camera.main.pixelWidth, Camera.main.pixelHeight, 0f, 0f, Path);
    }

    /// <summary>
    /// Work inside 'OnPostRender()' methode with Camera component!
    /// </summary>
    public static void SetScreenShot(int Width, int Height, float PosX, float PosY, string Path)
    {
        Texture2D TextureScreen = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        Rect RectScreen = new Rect(PosX, PosY, Width, Height);
        TextureScreen.ReadPixels(RectScreen, 0, 0);
        //
        byte[] ByteEncode = TextureScreen.EncodeToPNG();
        File.WriteAllBytes(Path, ByteEncode);
        //
        Debug.Log("[ScreenShot] " + Path);
    }
}