using System.Collections.Generic;
using UnityEngine;

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