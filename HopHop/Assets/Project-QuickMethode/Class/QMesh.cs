using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class QMesh
{
    public static Mesh GetMesh(List<Vector2> Points)
    {
        Vector2[] Vertices2D = Points.ToArray();
        //
        int[] Triangles = GetTriangles(Points);
        //
        Vector3[] Vertices = new Vector3[Vertices2D.Length];
        for (int i = 0; i < Vertices.Length; i++)
            Vertices[i] = new Vector3(Vertices2D[i].x, Vertices2D[i].y, 0);
        //
        Mesh Mesh = new Mesh();
        Mesh.vertices = Vertices;
        Mesh.triangles = Triangles;
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        //
        return Mesh;
    }

    #region Mesh Caculator

    private static int[] GetTriangles(List<Vector2> Points)
    {
        List<int> indices = new List<int>();

        int n = Points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (GetArea(Points) > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (GetSnip(Points, u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private static float GetArea(List<Vector2> Points)
    {
        int n = Points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = Points[p];
            Vector2 qval = Points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private static bool GetSnip(List<Vector2> Points, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = Points[V[u]];
        Vector2 B = Points[V[v]];
        Vector2 C = Points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = Points[V[p]];
            if (GetInsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private static bool GetInsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }

    #endregion
}

public class QMeshCircum
{
    //Mesh - Create new: Should use in Editor Mode!!

    public static Mesh SetGenerate(Vector3[] Points, int[] Triangles)
    {
        Mesh Mesh = new Mesh();
        Mesh.name = string.Format("{0}-{1}", Points.Length, Triangles.Length);
        Mesh.vertices = Points;
        Mesh.triangles = Triangles;
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        //
        return Mesh;
    }

    public static Mesh SetGenerate(QCircum Circum)
    {
        Mesh Mesh = new Mesh();
        Mesh.name = string.Format("{0}-{1}-{2}", Circum.Point, Circum.Radius + (Circum.Hollow ? "H" + Circum.RadiusHollow : "F"), Circum.Deg);
        Mesh.vertices = Circum.Points;
        Mesh.triangles = Circum.Triangles;
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        //
        return Mesh;
    }

    public static Mesh SetFilledGenerate(int Point, float Radius, float Deg)
    {
        QCircum Circum = new QCircum();
        Circum.Point = Point;
        Circum.Radius = Radius;
        Circum.Deg = Deg;
        Circum.SetFilledGenerate();
        //
        Mesh Mesh = new Mesh();
        Mesh.name = string.Format("{0}-{1}-{2}", Circum.Point, Circum.Radius + (Circum.Hollow ? "H" + Circum.RadiusHollow : "F"), Circum.Deg);
        Mesh.vertices = Circum.Points;
        Mesh.triangles = Circum.Triangles;
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        //
        return Mesh;
    }

    public static Mesh SetHollowGenerate(int Point, float Radius, float RadiusHollow, float Deg)
    {
        QCircum Circum = new QCircum();
        Circum.Point = Point;
        Circum.Radius = Radius;
        Circum.RadiusHollow = RadiusHollow;
        Circum.Deg = Deg;
        Circum.SetHollowGenerate();
        //
        Mesh Mesh = new Mesh();
        Mesh.name = string.Format("{0}-{1}-{2}", Circum.Point, Circum.Radius + (Circum.Hollow ? "H" + Circum.RadiusHollow : "F"), Circum.Deg);
        Mesh.vertices = Circum.Points;
        Mesh.triangles = Circum.Triangles;
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        //
        return Mesh;
    }

    //Mesh - Clear All: Should use in Play Mode!!

    public static void SetGenerate(MeshFilter MeshFilter, Vector3[] Points, int[] Triangles)
    {
        if (!Application.isPlaying)
        {
            MeshFilter.mesh = SetGenerate(Points, Triangles);
            return;
        }
        //
        MeshFilter.mesh.Clear();
        MeshFilter.mesh.name = string.Format("{0}-{1}", Points.Length, Triangles.Length);
        MeshFilter.mesh.vertices = Points;
        MeshFilter.mesh.triangles = Triangles;
        MeshFilter.mesh.RecalculateNormals();
        MeshFilter.mesh.RecalculateBounds();
    }

    public static void SetGenerate(MeshFilter MeshFilter, QCircum Circum)
    {
        if (!Application.isPlaying)
        {
            MeshFilter.mesh = SetGenerate(Circum);
            return;
        }
        //
        MeshFilter.mesh.Clear();
        MeshFilter.mesh.name = string.Format("{0}-{1}-{2}", Circum.Point, Circum.Radius + (Circum.Hollow ? "H" + Circum.RadiusHollow : "F"), Circum.Deg);
        MeshFilter.mesh.vertices = Circum.Points;
        MeshFilter.mesh.triangles = Circum.Triangles;
        MeshFilter.mesh.RecalculateNormals();
        MeshFilter.mesh.RecalculateBounds();
    }

    public static void SetFilledGenerate(MeshFilter MeshFilter, int Point, float Radius, float Deg)
    {
        if (!Application.isPlaying)
        {
            MeshFilter.mesh = SetFilledGenerate(Point, Radius, Deg);
            return;
        }
        //
        QCircum Circum = new QCircum();
        Circum.Point = Point;
        Circum.Radius = Radius;
        Circum.Deg = Deg;
        Circum.SetFilledGenerate();
        //
        MeshFilter.mesh.Clear();
        MeshFilter.mesh.name = string.Format("{0}-{1}-{2}", Circum.Point, Circum.Radius + (Circum.Hollow ? "H" + Circum.RadiusHollow : "F"), Circum.Deg);
        MeshFilter.mesh.vertices = Circum.Points;
        MeshFilter.mesh.triangles = Circum.Triangles;
        MeshFilter.mesh.RecalculateNormals();
        MeshFilter.mesh.RecalculateBounds();
    }

    public static void SetHollowGenerate(MeshFilter MeshFilter, int Point, float Radius, float RadiusHollow, float Deg)
    {
        if (!Application.isPlaying)
        {
            MeshFilter.mesh = SetHollowGenerate(Point, Radius, RadiusHollow, Deg);
            return;
        }
        //
        QCircum Circum = new QCircum();
        Circum.Point = Point;
        Circum.Radius = Radius;
        Circum.RadiusHollow = RadiusHollow;
        Circum.Deg = Deg;
        Circum.SetHollowGenerate();
        //
        MeshFilter.mesh.Clear();
        MeshFilter.mesh.name = string.Format("{0}-{1}-{2}", Circum.Point, Circum.Radius + (Circum.Hollow ? "H" + Circum.RadiusHollow : "F"), Circum.Deg);
        MeshFilter.mesh.vertices = Circum.Points;
        MeshFilter.mesh.triangles = Circum.Triangles;
        MeshFilter.mesh.RecalculateNormals();
        MeshFilter.mesh.RecalculateBounds();
    }
}