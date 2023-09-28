using System.Collections.Generic;
using UnityEngine;

public class QCircum
{
    public int Point = 0;
    public float Radius = 0;
    public float RadiusHollow = 0;
    public float Deg = 0;

    private Vector3[] m_points;
    private int[] m_triangle;
    private bool m_hollow = false;

    public Vector3[] Points => m_points;

    public int[] Triangles => m_triangle;

    public bool Hollow => m_hollow;

    //Filled

    public void SetFilledGenerate()
    {
        if (Point < 3)
            return;
        //
        m_points = GetFilledPoints(Point, Radius, Deg).ToArray();
        m_triangle = GetFilledTriangle(m_points);
        m_hollow = false;
    }

    public List<Vector3> GetFilledPoints(int Point, float Radius, float Deg)
    {
        if (Point < 3)
            //One shape must have 3 points at least!!
            return null;
        //
        List<Vector3> Points = new List<Vector3>();
        //
        float RadSpace = (360 / Point) * (Mathf.PI / 180);
        float RadStart = (Deg) * (Mathf.PI / 180);
        float RadCur = RadStart;
        //
        Vector3 PointStart = new Vector3(Mathf.Cos(RadStart) * Radius, Mathf.Sin(RadStart) * Radius, 0f);
        Points.Add(PointStart);
        for (int i = 1; i < Point; i++)
        {
            RadCur += RadSpace;
            Vector3 NewPoint = new Vector3(Mathf.Cos(RadCur) * Radius, Mathf.Sin(RadCur) * Radius, 0f);
            Points.Add(NewPoint);
        }
        //
        return Points;
    }

    public int[] GetFilledTriangle(Vector3[] Points)
    {
        int TriangleCount = Points.Length - 2;
        //
        List<int> Trianges = new List<int>();
        for (int i = 0; i < TriangleCount; i++)
        {
            Trianges.Add(0);
            Trianges.Add(i + 2);
            Trianges.Add(i + 1);
        }
        //
        return Trianges.ToArray();
    }

    //Hollow

    public void SetHollowGenerate()
    {
        if (Point < 3)
            return;
        //
        m_points = GetHollowPoints(Point, Radius, RadiusHollow, Deg).ToArray();
        m_triangle = GetHollowTriangle(m_points);
        m_hollow = true;
    }

    public List<Vector3> GetHollowPoints(int Point, float Radius, float RadiusHollow, float Deg)
    {
        List<Vector3> Points = new List<Vector3>();
        //
        Points.AddRange(GetFilledPoints(Point, Radius, Deg));
        Points.AddRange(GetFilledPoints(Point, RadiusHollow, Deg));
        //
        return Points;
    }

    public int[] GetHollowTriangle(Vector3[] Points)
    {
        int PointCount = Points.Length / 2;
        int TriangleCount = PointCount * 2;
        //
        List<int> Trianges = new List<int>();
        for (int i = 0; i < PointCount; i++)
        {
            int OuterIndex = i;
            int InnerIndex = i + PointCount;
            //
            //First Triangle Start at Outer Edgle i
            Trianges.Add(OuterIndex);
            Trianges.Add(InnerIndex);
            Trianges.Add((i + 1) % PointCount);
            //
            //Second Triangle Start at Outer Edgle i
            Trianges.Add(OuterIndex);
            Trianges.Add(PointCount + ((PointCount + i - 1) % PointCount));
            Trianges.Add(OuterIndex + PointCount);
        }
        //
        return Trianges.ToArray();
    }

    //

    public static List<Vector2> GetGeometryPoints(int Point, float Radius, float Deg)
    {
        if (Point < 3)
            //One shape must have 3 points at least!!
            return null;

        List<Vector2> Points = new List<Vector2>();

        float RadSpace = (360 / Point) * (Mathf.PI / 180);
        float RadStart = (Deg) * (Mathf.PI / 180);
        float RadCur = RadStart;

        Vector2 PointStart = new Vector2(Mathf.Cos(RadStart) * Radius, Mathf.Sin(RadStart) * Radius);
        Points.Add(PointStart);
        for (int i = 1; i < Point; i++)
        {
            RadCur += RadSpace;
            Vector2 NewPoint = new Vector2(Mathf.Cos(RadCur) * Radius, Mathf.Sin(RadCur) * Radius);
            Points.Add(NewPoint);
        }

        return Points;
    }
}