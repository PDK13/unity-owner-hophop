using UnityEngine;

public class QTransform
{
    #region ==================================== Convert - World & Local

    public static Vector3 GetPosWorld(Transform From, Vector3 PosLocal)
    {
        return From.TransformPoint(PosLocal);
    }

    public static Vector3 GetPosLocal(Transform From, Vector3 PosWorld)
    {
        return From.InverseTransformPoint(PosWorld);
    }

    #endregion

    #region ==================================== Move - Fixed Update

    public static void SetMoveToward(Transform From, Vector3 Pos, float DeltaDistance, bool World = true)
    {
        //Pos Move Linear by Distance when called - On World!!
        if (World)
        {
            Vector3 PosMove = Vector3.MoveTowards(From.position, Pos, DeltaDistance);
            From.position = PosMove;
        }
        else
        {
            Vector3 PosMove = Vector3.MoveTowards(From.localPosition, Pos, DeltaDistance);
            From.localPosition = PosMove;
        }
    }

    public static void SetMoveLerp(Transform From, Vector3 Pos, float DeltaTime, bool World = true)
    {
        //Pos Move none Linear by Time per called!!
        if (World)
        {
            Vector3 PosMove = Vector3.Lerp(From.position, Pos, DeltaTime);
            From.position = PosMove;
        }
        else
        {
            Vector3 PosMove = Vector3.Lerp(From.localPosition, Pos, DeltaTime);
            From.localPosition = PosMove;
        }
    }

    #endregion

    #region ==================================== Rotation Instantly

    //Axis

    public static void SetRotateAxis(Transform From, Vector3 Dir, Axis Axis)
    {
        switch (Axis)
        {
            case Axis.Right:
                From.right = Dir;
                break;
            case Axis.Up:
                From.up = Dir;
                break;
            case Axis.Forward:
                From.forward = Dir;
                break;
        }
    }

    //2D

    public static void SetRotate2D(Transform From, Vector2 PosStart, Vector2 PosEnd, bool World = true)
    {
        From.position = PosStart;
        Vector2 Dir = (PosEnd - PosStart).normalized;

        SetRotate2D(From, Dir, World);
    }

    public static void SetRotate2D(Transform From, Vector2 Dir, bool World = true)
    {
        float Deg = Mathf.Rad2Deg * Mathf.Atan2(Dir.y, Dir.x);

        SetRotate2D(From, Deg, World);
    }

    public static void SetRotate2D(Transform From, float Deg, bool World = true)
    {
        if (World)
        {
            From.eulerAngles = new Vector3(0, 0, Deg);
        }
        else
        {
            From.localEulerAngles = new Vector3(0, 0, Deg);
        }
    }

    //3D

    public static void SetRotate3DXZ(Transform From, Vector3 PosStart, Vector3 PosEnd, bool World = true)
    {
        From.position = PosStart;
        Vector3 Dir = (PosEnd - PosStart).normalized;

        SetRotate3DXZ(From, Dir, World);
    }

    public static void SetRotate3DXZ(Transform From, Vector3 Dir, bool World = true)
    {
        float Deg = Mathf.Rad2Deg * Mathf.Atan2(Dir.z, Dir.x);

        SetRotate3DXZ(From, Deg, World);
    }

    public static void SetRotate3DXZ(Transform From, float Deg, bool World = true)
    {
        if (World)
        {
            From.eulerAngles = new Vector3(0, Deg, 0);
        }
        else
        {
            From.localEulerAngles = new Vector3(0, Deg, 0);
        }
    }

    #endregion
}

public class QRecTransform
{
    #region ==================================== Move - Fixed Update

    public static void SetMoveToward(RectTransform From, Vector3 PosAnchor, float DeltaDistance)
    {
        //Pos Move Linear by Distance when called - On Canvas!!
        Vector3 PosMove = Vector3.MoveTowards(From.anchoredPosition, PosAnchor, DeltaDistance);
        From.anchoredPosition = PosMove;
    }

    #endregion

    #region ==================================== Anchor Pos Convert

    private static Vector2 GetPosAnchorPivotOffset(RectTransform From, Vector2 ToPivot)
    {
        //Pivot is the centre point UI, which is the Anchor Pos of UI!
        Vector2 PivotOffset = new Vector2((From.pivot.x - ToPivot.x) * From.sizeDelta.x, (From.pivot.y - ToPivot.y) * From.sizeDelta.y);
        return PivotOffset;
    }

    private static Vector2 GetAnchorPosPrimary(RectTransform From)
    {
        //Anchor Primary mean Anchor Min(0;0) and Max(0;0) or at the BL of screen!
        Vector2 AnchorPos = From.anchoredPosition;
        Vector2 ScreenSize = QCamera.GetCameraSizePixel();
        Vector2 AnchorPosMin = new Vector2(ScreenSize.x * From.anchorMin.x, ScreenSize.y * From.anchorMin.y);
        Vector2 AnchorPosMax = new Vector2(ScreenSize.x * From.anchorMax.x, ScreenSize.y * From.anchorMax.y);
        Vector2 AnchorPosBD = AnchorPos + (AnchorPosMax - AnchorPosMin) * 0.5f + AnchorPosMin;
        return AnchorPosBD;
    }

    private static Vector2 GetPosAnchorCentre(Vector2 FromAnchorPosPrimary, Vector2 ToAnchorsMin, Vector2 ToAnchorsMax)
    {
        //Anchor Primary mean Anchor Min(0;0) and Max(0;0) or at the BL of screen!
        //Anchor Centre mean Anchor Min(0.5;0.5) and Max(0.5;0.5) or at the centre of screen!
        Vector2 ScreenSize = QCamera.GetCameraSizePixel();
        Vector2 AnchorPosMin = new Vector2(ScreenSize.x * ToAnchorsMin.x, ScreenSize.y * ToAnchorsMin.y);
        Vector2 AnchorPosMax = new Vector2(ScreenSize.x * ToAnchorsMax.x, ScreenSize.y * ToAnchorsMax.y);
        Vector2 AnchorPos = (AnchorPosMax - AnchorPosMin) * 0.5f + AnchorPosMin - FromAnchorPosPrimary;
        return AnchorPos;
    }

    public static Vector2 GetAnchorPos(RectTransform From, Vector2 ToPivot, Vector2 ToAnchorsMin, Vector2 ToAnchorsMax)
    {
        Vector2 AnchorPosPivotOffset = GetPosAnchorPivotOffset(From, ToPivot);
        Vector2 AnchorPosPrimary = GetAnchorPosPrimary(From);
        Vector2 AnchorPosCentre = GetPosAnchorCentre(AnchorPosPrimary, ToAnchorsMin, ToAnchorsMax);
        return (AnchorPosCentre + AnchorPosPivotOffset) * (-1);
    }

    #endregion
}