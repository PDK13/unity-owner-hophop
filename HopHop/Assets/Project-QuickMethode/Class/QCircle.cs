using UnityEngine;

public class QCircle
{
    /* IMPORTANCE NOTE:
     * Deg can be understand as Euler in Unity.
     * Deg Caculated from X-Axis Right of World.
     * Pos Caculated from Center Zero of World (Should use Local Pos instead World Pos).
     * Deg caculated from this class all base on 0 to 360 degree.
     * Command "transform.eulerAngles" use Deg180 in Editor and Deg360 in Script.
    */

    #region ==================================== Primary

    public static float GetDeg180(float Deg360)
    {
        return Deg360 <= 180 ? Deg360 : -(360 - Deg360);
    }

    public static float GetDeg360(float Deg180)
    {
        return Deg180 <= 180 && Deg180 >= 0 ? Deg180 : 360 + Deg180;
    }

    #endregion

    #region ==================================== Pos by Deg

    public static Vector3 GetPosXY(float Deg360, float Radius)
    {
        //Get Pos from Center (0; 0)

        return new Vector3(Mathf.Cos(Deg360 * Mathf.Deg2Rad), Mathf.Sin(Deg360 * Mathf.Deg2Rad), 0) * Radius;
    }

    public static Vector3 GetPosXZ(float Deg360, float Radius)
    {
        //Get Pos from Center (0; 0; 0)

        return new Vector3(Mathf.Cos(Deg360 * Mathf.Deg2Rad), 0, Mathf.Sin(Deg360 * Mathf.Deg2Rad)) * Radius;
    }

    #endregion

    #region ==================================== Deg by Pos & Dir

    public static float GetDegPoint(Vector2 PointA, Vector2 PointB, bool Convert360 = false)
    {
        //Get Deg from Point A and B with Vector Right Primary
        //
        float Deg = Mathf.Atan2(PointB.y - PointA.y, PointB.x - PointA.x) * Mathf.Rad2Deg;
        return Deg < 0 && Convert360 ? 360 + Deg : Deg;
    }

    public static float GetDegDir(Vector2 Dir, bool Convert360 = false)
    {
        //Get Deg from Center (0;0) with Vector Right Primary
        //
        float Deg = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        return Deg < 0 && Convert360 ? 360 + Deg : Deg;
    }

    #endregion

    #region ==================================== Opposite (Đối diện)

    public static float GetDegOppositeUD(float Deg360)
    {
        if (0f == Deg360 || Deg360 == 360f)
        {
            return 180f;
        }

        if (0f < Deg360 && Deg360 < 90f)
        {
            return 90f + (90f - Deg360);
        }

        if (Deg360 == 90f)
        {
            return 90f;
        }

        if (90f < Deg360 && Deg360 < 180f)
        {
            return 90f - (Deg360 - 90f);
        }

        if (Deg360 == 180f)
        {
            return 0f;
        }

        if (180f < Deg360 && Deg360 < 270f)
        {
            return 270f + (270f - Deg360);
        }

        if (Deg360 == 270f)
        {
            return 270f;
        }

        if (270f < Deg360 && Deg360 < 360f)
        {
            return 270f - (Deg360 - 270f);
        }

        Debug.LogError("Sonething wrong here!");
        return 90f;
    }

    public static float GetDegOppositeLR(float Deg360)
    {
        if (0f == Deg360 || Deg360 == 360f)
        {
            return 0f;
        }

        if (0f < Deg360 && Deg360 < 90f)
        {
            return 360f - (0f + Deg360);
        }

        if (Deg360 == 90f)
        {
            return 270f;
        }

        if (90f < Deg360 && Deg360 < 180f)
        {
            return 180f + (180f - Deg360);
        }

        if (Deg360 == 180f)
        {
            return 180f;
        }

        if (180f < Deg360 && Deg360 < 270f)
        {
            return 180f + (Deg360 - 180f);
        }

        if (Deg360 == 270f)
        {
            return 90f;
        }

        if (270f < Deg360 && Deg360 < 360f)
        {
            return 0f - (360f - Deg360);
        }

        Debug.LogError("Sonething wrong here!");
        return 0f;
    }

    #endregion

    #region ==================================== Deg to Target: Use for Face to a Target

    public static float GetDegTargetOffset(Transform Body, Vector3 BodyDir, Transform Target)
    {
        //Get value of deg remain to target:
        //- Value > 0 mean rotate follow anti-clockwise to head target
        //- Value < 0 mean rotate follow clockwise to head target
        Vector2 Dir = (Target.position - Body.position).normalized;
        return Vector3.SignedAngle(BodyDir, Dir, Vector3.forward);
    }

    #endregion
}