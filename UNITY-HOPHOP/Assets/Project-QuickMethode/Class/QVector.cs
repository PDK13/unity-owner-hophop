using UnityEngine;

public class QVector
{
    #region ==================================== Middle (Trung điểm)

    public static Vector2 GetMiddlePoint(Vector2 PointA, Vector2 PointB)
    {
        return new Vector2((PointA.x + PointB.x) / 2, (PointA.y + PointB.y) / 2);
    }

    public static Vector3 GetMiddlePoint(Vector3 PointA, Vector3 PointB)
    {
        return new Vector3((PointA.x + PointB.x) / 2, (PointA.y + PointB.y) / 2, (PointA.z + PointB.z) / 2);
    }

    #endregion

    #region ==================================== Reflech (Phản xạ)

    public static Vector2 GetDirReflect(Vector2 Dir, Collision2D Collision)
    {
        //Get Dir Reflect (Phản xạ) from Dir to!!
        return Vector2.Reflect(Dir, Collision.contacts[0].normal);
    }

    public static Vector3 GetDirReflect(Vector3 Dir, Collision Collision)
    {
        //Get Dir Reflect (Phản xạ) from Dir to!!
        return Vector3.Reflect(Dir, Collision.contacts[0].normal);
    }

    #endregion

    #region ==================================== Distance

    public static float GetDistance(params Vector2[] Points)
    {
        if (Points.Length < 2)
            return 0;
        //
        float Distance = 0f;
        for (int i = 1; i < Points.Length; i++)
            Distance += Vector2.Distance(Points[i - 1], Points[i]);
        return Distance;
    }

    public static float GetDistance(params Vector3[] Points)
    {
        if (Points.Length < 2)
            return 0;
        //
        float Distance = 0f;
        for (int i = 1; i < Points.Length; i++)
            Distance += Vector3.Distance(Points[i - 1], Points[i]);
        return Distance;
    }

    #endregion
}