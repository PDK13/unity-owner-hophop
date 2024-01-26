using System.Collections.Generic;
using UnityEngine;

public class QGizmos
{
    #region ==================================== Primary

    public static void SetLine(Vector3 PosA, Vector3 PosB, Color Color, float SizePoint = 0f)
    {
        Gizmos.color = Color;
        Gizmos.DrawLine(PosA, PosB);

        if (SizePoint != 0)
        {
            SetWireSphere(PosA, SizePoint, Color);
            SetWireSphere(PosB, SizePoint, Color);
        }
    }

    public static void SetRay(Vector3 Pos, Vector3 Dir, float Distance, Color Color, float SizePoint = 0f)
    {
        Vector3 PosA = Pos;
        Vector3 PosB = PosA + Dir.normalized * Distance;

        Gizmos.color = Color;
        Gizmos.DrawLine(PosA, PosB);

        if (SizePoint != 0)
        {
            SetWireSphere(PosA, SizePoint, Color);
            SetWireSphere(PosB, SizePoint, Color);
        }
    }

    public static void SetWireCube(Vector3 Pos, Vector3 Size, Color Color)
    {
        Gizmos.color = Color;
        Gizmos.DrawWireCube(Pos, Size);
    }

    public static void SetWireCube(Vector2 Pos, Vector3 Size, Color Color)
    {
        Gizmos.color = Color;
        Gizmos.DrawWireCube(Pos, Size);
    }

    public static void SetWireSphere(Vector3 Pos, float Size, Color Color)
    {
        Gizmos.color = Color;
        Gizmos.DrawWireSphere(Pos, Size);
    }

    public static void SetWireSphere(Vector2 Pos, float Size, Color Color)
    {
        Gizmos.color = Color;
        Gizmos.DrawWireSphere(Pos, Size);
    }

    #endregion

    #region ==================================== Cast

    public static void SetBoxcast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, Color Color)
    {
        SetLine(PosStart, PosEnd, Color);
        SetWireCube(PosEnd, Size, Color);
    }

    public static void SetBoxcast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, float Distance, Color Color)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;
        SetLine(PosStart, PosStart + Dir * Distance, Color);
        SetWireCube(PosStart + Dir * Distance, Size, Color);
    }

    public static void SetBoxcastDir(Vector3 PosStart, Vector3 Dir, Vector3 Size, float Distance, Color Color)
    {
        SetLine(PosStart, PosStart + Dir * Distance, Color);
        SetWireCube(PosStart + Dir * Distance, Size, Color);
    }

    public static void SetSpherecast(Vector3 PosStart, Vector3 PosEnd, float Size, Color Color)
    {
        SetLine(PosStart, PosEnd, Color);
        SetWireSphere(PosEnd, Size, Color);
    }

    public static void SetSpherecast(Vector3 PosStart, Vector3 PosEnd, float Size, float Distance, Color Color)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;
        SetLine(PosStart, PosStart + Dir * Distance, Color);
        SetWireSphere(PosStart + Dir * Distance, Size, Color);
    }

    public static void SetSpherecastDir(Vector3 PosStart, Vector3 Dir, float Size, float Distance, Color Color)
    {
        SetLine(PosStart, PosStart + Dir * Distance, Color);
        SetWireSphere(PosStart + Dir * Distance, Size, Color);
    }

    #endregion

    #region ==================================== Camera

    public static void SetCamera(Color Color)
    {
        SetCamera(UnityEngine.Camera.main, Color);
    }

    public static void SetCamera(Camera From, Color Color)
    {
        Gizmos.color = Color;

        Vector2 Resolution = QCamera.GetCameraSizeUnit();
        Gizmos.DrawWireCube((Vector2)From.transform.position, Resolution);
    }

    #endregion

    #region ==================================== Sprite

    public static void SetSprite2D(SpriteRenderer From, Color Color)
    {
        Vector2 Size = QSprite.GetSizeUnit(From.sprite);
        Vector2 Pos = From.transform.position;

        Vector2 TL = Vector2.up * Size.y / 2 + Vector2.left * Size.x / 2;
        Vector2 TR = Vector2.up * Size.y / 2 + Vector2.right * Size.x / 2;
        Vector2 BL = Vector2.down * Size.y / 2 + Vector2.left * Size.x / 2;
        Vector2 BR = Vector2.down * Size.y / 2 + Vector2.right * Size.x / 2;

        SetLine(Pos + TL, Pos + TR, Color);
        SetLine(Pos + TR, Pos + BR, Color);
        SetLine(Pos + BR, Pos + BL, Color);
        SetLine(Pos + BL, Pos + TL, Color);
    }

    #endregion

    #region ==================================== Collider

    #region Collider Pos Self

    public static void SetCollider2D(Collider2D From, Color Color)
    {
        SetWireCube(From.bounds.center, (Vector2)From.bounds.size, Color);
    }

    public static void SetCollider2D(BoxCollider2D From, Color Color)
    {
        SetWireCube(From.bounds.center, (Vector2)From.bounds.size + Vector2.one * From.edgeRadius * 2, Color);
    }

    public static void SetCollider2D(CircleCollider2D From, Color Color)
    {
        SetWireSphere(From.bounds.center, From.radius, Color);
    }

    public static void SetCollider2D(PolygonCollider2D From, Color Color)
    {
        Gizmos.color = Color;

        for (int i = 1; i < From.points.Length; i++)
        {
            Gizmos.DrawLine(From.points[i - 1], From.points[i]);
        }
        Gizmos.DrawLine(From.points[0], From.points[From.points.Length - 1]);
    }

    public static void SetCollider2D(CompositeCollider2D From, bool Square, Color Color)
    {
        List<List<Vector2>> Points = QCollider2D.GetPointsBorderPos(From, Square);

        if (Points.Count == 0)
        {
            return;
        }

        Vector2 Center = From.transform.position;

        for (int Group = 0; Group < Points.Count; Group++)
        {
            for (int Index = 1; Index < Points[Group].Count; Index++)
            {
                SetLine(Center + Points[Group][Index - 1], Center + Points[Group][Index], Color.red, 0.1f);
            }

            SetLine(Center + Points[Group][0], Center + Points[Group][Points[Group].Count - 1], Color.red, 0.1f);
        }
    }

    #endregion

    #region Collider Pos Free

    public static void SetCollider2D(Vector2 Pos, Collider2D From, Color Color)
    {
        SetWireCube(Pos, (Vector2)From.bounds.size, Color);
    }

    public static void SetCollider2D(Vector2 Pos, BoxCollider2D From, Color Color)
    {
        SetWireCube(Pos, (Vector2)From.bounds.size + Vector2.one * From.edgeRadius * 2, Color);
    }

    public static void SetCollider2D(Vector2 Pos, CircleCollider2D From, Color Color)
    {
        SetWireSphere(Pos, From.radius, Color);
    }

    #endregion

    #endregion
}