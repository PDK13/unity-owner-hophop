using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QCast
{
    #region ==================================== 3D

    #region ------------------------------------ None LayerMask

    public static (GameObject Target, Vector3 Point)? GetLineCast(Vector3 PosStart, Vector3 PosEnd)
    {
        RaycastHit RaycastHit = new RaycastHit();
        Physics.Linecast(PosStart, PosEnd, out RaycastHit);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetRaycast(Vector3 PosStart, Vector3 PosEnd, float Distance)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;

        RaycastHit RaycastHit = new RaycastHit();
        Physics.Raycast(PosStart, Dir, out RaycastHit, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetRaycastDir(Vector3 PosStart, Vector3 Dir, float Distance)
    {
        RaycastHit RaycastHit = new RaycastHit();
        Physics.Raycast(PosStart, Dir.normalized, out RaycastHit, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetBoxCast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, Vector3 Rotation, float Distance)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;
        Quaternion Quaternion = Quaternion.Euler(Rotation);

        RaycastHit RaycastHit = new RaycastHit();
        Physics.BoxCast(PosStart, Size, Dir, out RaycastHit, Quaternion, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetBoxCastDir(Vector3 PosStart, Vector3 Dir, Vector3 Size, Vector3 Rotation, float Distance)
    {
        Quaternion Quaternion = Quaternion.Euler(Rotation);

        RaycastHit RaycastHit = new RaycastHit();
        Physics.BoxCast(PosStart, Size, Dir.normalized, out RaycastHit, Quaternion, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetSphereCast(Vector3 PosStart, Vector3 PosEnd, float Radius, float Distance)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;

        RaycastHit RaycastHit = new RaycastHit();
        Physics.SphereCast(PosStart, Radius / 2, Dir, out RaycastHit, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetSphereCastDir(Vector3 PosStart, Vector3 Dir, float Radius, float Distance)
    {
        RaycastHit RaycastHit = new RaycastHit();
        Physics.SphereCast(PosStart, Radius / 2, Dir.normalized, out RaycastHit, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static List<GameObject> GetBoxOverlap(Vector3 PosStart, Vector3 Size, Vector3 Rotation)
    {
        Quaternion Quaternion = Quaternion.Euler(Rotation);
        Collider[] ObjectsHit = Physics.OverlapBox(PosStart, Size, Quaternion);

        List<GameObject> ObjectsHitList = new List<GameObject>();
        foreach (Collider ObjectHit in ObjectsHit)
        {
            ObjectsHitList.Add(ObjectHit.gameObject);
        }

        return ObjectsHitList;
    }

    public static List<GameObject> GetCircleOverlap(Vector3 PosStart, float Size)
    {
        Collider[] ObjectsHit = Physics.OverlapSphere(PosStart, Size);

        List<GameObject> ObjectsHitList = new List<GameObject>();
        foreach (Collider ObjectHit in ObjectsHit)
        {
            ObjectsHitList.Add(ObjectHit.gameObject);
        }

        return ObjectsHitList;
    }

    #endregion

    #region ------------------------------------ LayerMask

    public static (GameObject Target, Vector3 Point)? GetLineCast(Vector3 PosStart, Vector3 PosEnd, LayerMask Tarket)
    {
        RaycastHit RaycastHit = new RaycastHit();

        Physics.Linecast(PosStart, PosEnd, out RaycastHit, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetRaycast(Vector3 PosStart, Vector3 PosEnd, float Distance, LayerMask Tarket)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;

        RaycastHit RaycastHit = new RaycastHit();
        Physics.Raycast(PosStart, Dir, out RaycastHit, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetRaycastDir(Vector3 PosStart, Vector3 Dir, float Distance, LayerMask Tarket)
    {
        RaycastHit RaycastHit = new RaycastHit();
        Physics.Raycast(PosStart, Dir.normalized, out RaycastHit, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetBoxCast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, Vector3 Rotation, float Distance, LayerMask Tarket)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;
        Quaternion Quaternion = Quaternion.Euler(Rotation);

        RaycastHit RaycastHit = new RaycastHit();
        Physics.BoxCast(PosStart, Size, Dir, out RaycastHit, Quaternion, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetBoxCastDir(Vector3 PosStart, Vector3 Dir, Vector3 Size, Vector3 Rotation, float Distance, LayerMask Tarket)
    {
        Quaternion Quaternion = Quaternion.Euler(Rotation);

        RaycastHit RaycastHit = new RaycastHit();
        Physics.BoxCast(PosStart, Size, Dir.normalized, out RaycastHit, Quaternion, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetSphereCast(Vector3 PosStart, Vector3 PosEnd, float Radius, float Distance, LayerMask Tarket)
    {
        Vector3 Dir = (PosEnd - PosStart).normalized;

        RaycastHit RaycastHit = new RaycastHit();
        Physics.SphereCast(PosStart, Radius / 2, Dir, out RaycastHit, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector3 Point)? GetSphereCastDir(Vector3 PosStart, Vector3 Dir, float Radius, float Distance, LayerMask Tarket)
    {
        RaycastHit RaycastHit = new RaycastHit();
        Physics.SphereCast(PosStart, Radius / 2, Dir.normalized, out RaycastHit, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static List<GameObject> GetBoxOverlap(Vector3 PosStart, Vector3 Size, Vector3 Rotation, LayerMask Tarket)
    {
        Quaternion Quaternion = Quaternion.Euler(Rotation);

        Collider[] ObjectsHit = Physics.OverlapBox(PosStart, Size, Quaternion, Tarket);

        List<GameObject> ObjectsHitList = new List<GameObject>();
        foreach (Collider ObjectHit in ObjectsHit)
        {
            ObjectsHitList.Add(ObjectHit.gameObject);
        }

        return ObjectsHitList;
    }

    public static List<GameObject> GetCircleOverlap(Vector3 PosStart, float Size, LayerMask Tarket)
    {
        Collider[] ObjectsHit = Physics.OverlapSphere(PosStart, Size, Tarket);

        List<GameObject> ObjectsHitList = new List<GameObject>();
        foreach (Collider ObjectHit in ObjectsHit)
        {
            ObjectsHitList.Add(ObjectHit.gameObject);
        }

        return ObjectsHitList;
    }

    #endregion

    #endregion

    #region ==================================== 2D

    #region ------------------------------------ None LayerMask

    public static (GameObject Target, Vector2 Point)? GetLineCast2D(Vector2 PosStart, Vector2 PosEnd)
    {
        RaycastHit2D RaycastHit = Physics2D.Linecast(PosStart, PosEnd);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetRaycast2D(Vector2 PosStart, Vector2 PosEnd, float Distance)
    {
        Vector2 Dir = (PosEnd - PosStart).normalized;

        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetRaycast2DDir(Vector2 PosStart, Vector2 Dir, float Distance)
    {
        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir.normalized, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetBoxCast2D(Vector2 PosStart, Vector2 PosEnd, Vector2 Size, float Rotation, float Distance)
    {
        Vector2 Dir = (PosEnd - PosStart).normalized;

        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetBoxCast2DDir(Vector2 PosStart, Vector2 Dir, Vector2 Size, float Rotation, float Distance)
    {
        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir.normalized, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetCircleCast2D(Vector2 PosStart, Vector2 PosEnd, float Radius, float Distance)
    {
        Vector2 Dir = (PosEnd - PosStart).normalized;

        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetCircleCast2DDir(Vector2 PosStart, Vector2 Dir, float Radius, float Distance)
    {
        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir.normalized, Distance);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static GameObject GetOverlapCircle2D(Vector2 PosStart, float Radius)
    {
        Collider2D ColliderHit = Physics2D.OverlapCircle(PosStart, Radius);

        if (ColliderHit == null)
        {
            return null;
        }

        return ColliderHit.gameObject;
    }

    public static List<GameObject> GetOverlapCircleAll2D(Vector2 PosStart, float Radius)
    {
        Collider2D[] ColliderHit = Physics2D.OverlapCircleAll(PosStart, Radius);

        List<GameObject> ColliderHitList = new List<GameObject>();
        for (int i = 0; i < ColliderHit.Length; i++)
        {
            if (ColliderHit[i] != null)
            {
                ColliderHitList.Add(ColliderHit[i].gameObject);
            }
        }

        return ColliderHitList;
    }

    public static GameObject GetOverlapBox2D(Vector2 PosStart, Vector2 Size, float Rotation)
    {
        Collider2D ColliderHit = Physics2D.OverlapBox(PosStart, Size, Rotation);

        if (ColliderHit == null)
        {
            return null;
        }

        return ColliderHit.gameObject;
    }

    public static List<GameObject> GetOverlapBoxAll2D(Vector2 PosStart, Vector2 Size, float Rotation)
    {
        Collider2D[] ColliderHit = Physics2D.OverlapBoxAll(PosStart, Size, Rotation);

        List<GameObject> ColliderHitList = new List<GameObject>();
        for (int i = 0; i < ColliderHit.Length; i++)
        {
            if (ColliderHit[i] != null)
            {
                ColliderHitList.Add(ColliderHit[i].gameObject);
            }
        }

        return ColliderHitList;
    }

    #endregion

    #region ------------------------------------ LayerMask

    public static (GameObject Target, Vector2 Point)? GetLineCast2D(Vector2 PosStart, Vector2 PosEnd, LayerMask Tarket)
    {
        RaycastHit2D RaycastHit = Physics2D.Linecast(PosStart, PosEnd, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetRaycast2D(Vector2 PosStart, Vector2 PosEnd, float Distance, LayerMask Tarket)
    {
        Vector2 Dir = (PosEnd - PosStart).normalized;

        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetRaycast2DDir(Vector2 PosStart, Vector2 Dir, float Distance, LayerMask Tarket)
    {
        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir.normalized, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetBoxCast2D(Vector2 PosStart, Vector2 PosEnd, Vector2 Size, float Rotation, float Distance, LayerMask Tarket)
    {
        Vector2 Dir = (PosEnd - PosStart).normalized;

        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetBoxCast2DDir(Vector2 PosStart, Vector2 Dir, Vector2 Size, float Rotation, float Distance, LayerMask Tarket)
    {
        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir.normalized, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetCircleCast2D(Vector2 PosStart, Vector2 PosEnd, float Radius, float Distance, LayerMask Tarket)
    {
        Vector2 Dir = (PosEnd - PosStart).normalized;

        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static (GameObject Target, Vector2 Point)? GetCircleCast2DDir(Vector2 PosStart, Vector2 Dir, float Radius, float Distance, LayerMask Tarket)
    {
        if (Dir == Vector2.zero)
        {
            return (null, PosStart);
        }

        RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir.normalized, Distance, Tarket);

        if (RaycastHit.collider == null)
        {
            return null;
        }

        return (RaycastHit.collider.gameObject, RaycastHit.point);
    }

    public static GameObject GetOverlapCircle2D(Vector2 PosStart, float Radius, LayerMask Tarket)
    {
        Collider2D ColliderHit = Physics2D.OverlapCircle(PosStart, Radius, Tarket);

        if (ColliderHit == null)
        {
            return null;
        }

        return ColliderHit.gameObject;
    }

    public static List<GameObject> GetOverlapCircleAll2D(Vector2 PosStart, float Radius, LayerMask Tarket)
    {
        Collider2D[] ColliderHit = Physics2D.OverlapCircleAll(PosStart, Radius, Tarket);

        List<GameObject> ColliderHitList = new List<GameObject>();
        for (int i = 0; i < ColliderHit.Length; i++)
        {
            if (ColliderHit[i] != null)
            {
                ColliderHitList.Add(ColliderHit[i].gameObject);
            }
        }

        return ColliderHitList;
    }

    public static GameObject GetOverlapBox2D(Vector2 PosStart, Vector2 Size, float Rotation, LayerMask Tarket)
    {
        Collider2D ColliderHit = Physics2D.OverlapBox(PosStart, Size, Rotation, Tarket);

        if (ColliderHit == null)
        {
            return null;
        }

        return ColliderHit.gameObject;
    }

    public static List<GameObject> GetOverlapBoxAll2D(Vector2 PosStart, Vector2 Size, float Rotation, LayerMask Tarket)
    {
        Collider2D[] ColliderHit = Physics2D.OverlapBoxAll(PosStart, Size, Rotation, Tarket);

        List<GameObject> ColliderHitList = new List<GameObject>();
        for (int i = 0; i < ColliderHit.Length; i++)
        {
            if (ColliderHit[i] != null)
            {
                ColliderHitList.Add(ColliderHit[i].gameObject);
            }
        }

        return ColliderHitList;
    }

    #endregion

    #endregion
}

public class QCollider2D
{
    #region ==================================== Border

    public static Vector2 GetBorderPos(Collider2D From, params Direction[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;
        float Edge = 0f;

        BoxCollider2D ColliderBox = From.GetComponent<BoxCollider2D>();
        if (ColliderBox != null)
        {
            Edge = ColliderBox.edgeRadius;

            //Option Value: Auto Tilling
            if (ColliderBox.autoTiling)
            {
                SpriteRenderer Sprite = From.GetComponent<SpriteRenderer>();
                if (Sprite != null)
                {
                    Size = Sprite.bounds.size;
                }
            }
        }

        //Caculate Position from Value Data
        foreach (Direction DirBound in Bound)
        {
            switch (DirBound)
            {
                case Direction.Up:
                    Pos.y += (Size.y / 2) + Edge;
                    break;
                case Direction.Down:
                    Pos.y -= (Size.y / 2) + Edge;
                    break;
                case Direction.Left:
                    Pos.x -= (Size.x / 2) + Edge;
                    break;
                case Direction.Right:
                    Pos.x += (Size.x / 2) + Edge;
                    break;
            }
        }
        return Pos;
    }

    public static Vector2 GetBorderPos(Collider2D From, params DirectionX[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;
        float Edge = 0f;

        BoxCollider2D ColliderBox = From.GetComponent<BoxCollider2D>();
        if (ColliderBox != null)
        {
            Edge = ColliderBox.edgeRadius;

            //Option Value: Auto Tilling
            if (ColliderBox.autoTiling)
            {
                SpriteRenderer Sprite = From.GetComponent<SpriteRenderer>();
                if (Sprite != null)
                {
                    Size = Sprite.bounds.size;
                }
            }
        }

        //Caculate Position from Value Data
        foreach (DirectionX DirBound in Bound)
        {
            switch (DirBound)
            {
                case DirectionX.Left:
                    Pos.x -= (Size.x / 2) + Edge;
                    break;
                case DirectionX.Right:
                    Pos.x += (Size.x / 2) + Edge;
                    break;
            }
        }
        return Pos;
    }

    public static Vector2 GetBorderPos(Collider2D From, params DirectionY[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;
        float Edge = 0f;

        BoxCollider2D ColliderBox = From.GetComponent<BoxCollider2D>();
        if (ColliderBox != null)
        {
            Edge = ColliderBox.edgeRadius;

            //Option Value: Auto Tilling
            if (ColliderBox.autoTiling)
            {
                SpriteRenderer Sprite = From.GetComponent<SpriteRenderer>();
                if (Sprite != null)
                {
                    Size = Sprite.bounds.size;
                }
            }
        }

        //Caculate Position from Value Data
        foreach (DirectionY DirBound in Bound)
        {
            switch (DirBound)
            {
                case DirectionY.Up:
                    Pos.y += (Size.y / 2) + Edge;
                    break;
                case DirectionY.Down:
                    Pos.y -= (Size.y / 2) + Edge;
                    break;
            }
        }
        return Pos;
    }

    #endregion

    #region ==================================== Sprite Renderer

    public static void SetMatch(BoxCollider2D From, SpriteRenderer SpriteRenderer) //Check again when Rotation!!
    {
        Vector2 PosPrimary = From.transform.position;
        Vector2 PosRenderer = SpriteRenderer.bounds.center;
        Vector2 Offset = PosRenderer - PosPrimary;

        From.size = SpriteRenderer.bounds.size;
        From.offset = Offset;
    }

    #endregion

    #region ==================================== Composite Collider

    public static List<Vector2> GetPointsCenterPos(CompositeCollider2D From)
    {
        //NOTE: Composite Collider split Points into Group if they're not contact with each other!!
        List<Vector2> PointCenter = new List<Vector2>();
        for (int i = 0; i < From.pathCount; i++)
        {
            //Get Points of this Group!!
            Vector2[] Points = new Vector2[From.GetPathPointCount(i)];
            From.GetPath(i, Points);
            //Caculate CenterPoint of this Group!!
            float CenterX = 0f;
            float CenterY = 0f;
            for (int j = 0; j < Points.GetLength(0); j++)
            {
                CenterX += Points[j].x;
                CenterY += Points[j].y;
            }
            Vector2 Pos = new Vector2(CenterX / Points.GetLength(0), CenterY / Points.GetLength(0));
            PointCenter.Add(Pos);
            //Done caculate Point Center of current Group!!
        }

        //Result local pos Center of Collider!!
        return PointCenter;
    }

    public static List<List<Vector2>> GetPointsBorderPos(CompositeCollider2D From, bool WorldPos, bool Square = true)
    {
        Vector2 TileCenter = WorldPos ? From.transform.position : Vector3.zero;

        //NOTE: Composite Collider split Points into Group if they're not contact with each other!!
        List<List<Vector2>> PointsBorder = new List<List<Vector2>>();
        for (int Group = 0; Group < From.pathCount; Group++)
        {
            //Generate a new Group!!
            PointsBorder.Add(new List<Vector2>());
            //Get Points of this Group!!
            Vector2[] Points = new Vector2[From.GetPathPointCount(Group)];
            From.GetPath(Group, Points);
            for (int Index = 0; Index < Points.Length; Index++)
            {
                //Generate new Points into each Group!!
                if (Square)
                {
                    Vector2Int Pos = new Vector2Int(Mathf.RoundToInt(Points[Index].x), Mathf.RoundToInt(Points[Index].y));
                    if (Points.Contains(Pos))
                    {
                        continue;
                    }

                    PointsBorder[Group].Add(TileCenter + Pos);
                }
                else
                {
                    PointsBorder[Group].Add(TileCenter + Points[Index]);
                }
            }
            //Done Generate current Group!!
        }

        //Result local pos Points of Collider!!
        return PointsBorder;
    }

    #endregion

    #region ==================================== Platform

    public static List<(Vector2 Center, float Length)> GetPlatform(CompositeCollider2D From, bool WorldPos)
    {
        Vector2 TileCenter = WorldPos ? From.transform.position : Vector3.zero;

        //NOTE: Caculate Platform Pos and Length of Collider on each Group!!
        List<(Vector2 Center, float Length)> Platform = new List<(Vector2 Center, float Length)>();

        List<List<Vector2>> Points = GetPointsBorderPos(From, false);
        for (int Group = 0; Group < Points.Count; Group++)
        {
            //Find a highest Point of this Group!!
            int IndexStart = 0;
            float HighStart = 0;
            for (int Index = 0; Index < Points[Group].Count; Index++)
            {
                if (Points[Group][Index].y <= HighStart)
                {
                    continue;
                }

                IndexStart = Index;
                HighStart = Points[Group][Index].y;
            }
            //Check where can Stand!!
            //Check from Left to Right mean Stand, else not Stand!!
            int IndexNext = IndexStart;
            int IndexPrev = IndexNext - 1 >= 0 ? IndexNext - 1 : Points[Group].Count - 1;
            do
            {
                //Index Run!!
                IndexNext++;
                IndexPrev++;
                if (IndexNext >= Points[Group].Count)
                {
                    IndexNext = 0;
                }

                if (IndexPrev >= Points[Group].Count)
                {
                    IndexPrev = 0;
                }
                //Check Start - End Index?!
                if (IndexNext == IndexStart)
                {
                    break;
                }
                //Check at Index!!
                if (Points[Group][IndexPrev].y != Points[Group][IndexNext].y)
                {
                    continue;
                }

                if (Points[Group][IndexPrev].x <= Points[Group][IndexNext].x)
                {
                    continue;
                }

                Vector2 PointA = Points[Group][IndexPrev];
                Vector2 PointB = Points[Group][IndexNext];
                Vector2 Center = QVector.GetMiddlePoint(PointA, PointB);
                float Length = Mathf.Abs(PointB.x - PointA.x);
                Platform.Add((TileCenter + Center, Length));
            }
            while (IndexNext != IndexStart);
            //Done Check current Group!!
        }

        //Result local Center of Platform!!
        return Platform;
    }

    #endregion
}

public class QTrajectory
{
    public static List<Vector3> GetTrajectory(Vector3 From, float Deg, float Force, float GravityScale, float VelocityDrag = 0f)
    {
        //NOTE:
        //- Can be used for LineRenderer Component.
        //- Can be used for Bullet GameObject with Rigidbody and Rigidbody2D Componenet.

        List<Vector3> TrajectoryPath = new List<Vector3>();

        Vector3 GravityGolbal = Vector3.down * (-Physics2D.gravity);
        float Step = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
        Vector3 Accel = GravityGolbal * GravityScale * Step * Step;
        float Drag = 1f - Step * VelocityDrag;
        Vector3 Dir = QCircle.GetPosXY(Deg, 1f).normalized * Force;
        Vector3 Move = Dir * Step;

        Vector3 Pos = From;
        TrajectoryPath.Add(Pos);
        for (int i = 0; i < 500; i++)
        {
            Move += Accel;
            Move *= Drag;
            Pos += Move;
            TrajectoryPath.Add(Pos);
        }

        return TrajectoryPath;
    }

    public static float? GetDegToTarget(Vector3 From, Vector3 To, float Force, float GravityScale, bool DegHigh = true)
    {
        //Get the Deg to hit Target!

        Vector3 Dir = To - From;
        float HeightY = Dir.y;
        Dir.y = 0f;
        float LengthX = Dir.magnitude;
        float Gravity = -Physics2D.gravity.y * GravityScale;
        float SpeedSQR = Force * Force;
        float UnderSQR = (SpeedSQR * SpeedSQR) - Gravity * (Gravity * LengthX * LengthX + 2 * HeightY * SpeedSQR);
        if (UnderSQR >= 0)
        {
            float UnderSQRT = Mathf.Sqrt(UnderSQR);
            float AngleHigh = SpeedSQR + UnderSQRT;
            float AngleLow = SpeedSQR - UnderSQRT;

            return DegHigh ? Mathf.Atan2(AngleHigh, Gravity * LengthX) * Mathf.Rad2Deg : Mathf.Atan2(AngleLow, Gravity * LengthX) * Mathf.Rad2Deg;
        }

        return null;
    }

    public static void SetForceToBullet(Rigidbody2D From, float Deg, float Force, float GravityScale, float VelocityDrag = 0f)
    {
        From.gameObject.SetActive(true);
        From.velocity = QCircle.GetPosXY(Deg, 1f).normalized * Force;
        From.gravityScale = GravityScale;
        From.drag = VelocityDrag;
    }
}