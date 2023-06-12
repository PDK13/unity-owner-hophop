using QuickMethode;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum IsoDir { None = 0, Up = 1, Down = 2, Left = 3, Right = 4, Top = 5, Bot = 6 }

[Serializable]
public struct IsoVector
{
    #region Primary

    public IsoVector(float XUD, float YLR, float HTB)
    {
        X = XUD;
        Y = YLR;
        H = HTB;
    }

    public IsoVector(IsoVector IsoVector)
    {
        X = IsoVector.X;
        Y = IsoVector.Y;
        H = IsoVector.H;
    }

    public IsoVector(Vector3 Vector)
    {
        X = Vector.x;
        Y = Vector.y;
        H = Vector.z;
    }

    public float X; //Direction Up & Down
    public float Y; //Direction Left & Right
    public float H; //Direction Top & Bot

    #endregion

    #region Value Int

    public int XInt => Mathf.RoundToInt(X); //Direction Up & Down

    public int YInt => Mathf.RoundToInt(Y); //Direction Left & Right

    public int HInt => Mathf.RoundToInt(H); //Direction Top & Bot

    public IsoVector Fixed => new IsoVector(XInt, YInt, HInt);

    #endregion

    #region Primary Dir

    public static IsoVector Up => new IsoVector(1, 0, 0);
    public static IsoVector Down => new IsoVector(-1, 0, 0);
    public static IsoVector Left => new IsoVector(0, -1, 0);
    public static IsoVector Right => new IsoVector(0, 1, 0);
    public static IsoVector Top => new IsoVector(0, 0, 1);
    public static IsoVector Bot => new IsoVector(0, 0, -1);

    public static Vector3 GetVectorDir(IsoDir Dir)
    {
        switch (Dir)
        {
            case IsoDir.Up:
                return Vector3.right;
            case IsoDir.Down:
                return Vector3.left;
            case IsoDir.Left:
                return Vector3.down;
            case IsoDir.Right:
                return Vector3.up;
            case IsoDir.Top:
                return Vector3.forward;
            case IsoDir.Bot:
                return Vector3.back;
        }
        return Vector3.zero;
    }

    #endregion

    #region Operator

    public static IsoVector operator +(IsoVector IsoVector) => IsoVector;
    public static IsoVector operator -(IsoVector IsoVector) => new IsoVector(IsoVector.X * -1, IsoVector.Y * -1, IsoVector.H * -1);
    public static IsoVector operator +(IsoVector IsoVectorA, IsoVector IsoVectorB) => new IsoVector(IsoVectorA.X + IsoVectorB.X, IsoVectorA.Y + IsoVectorB.Y, IsoVectorA.H + IsoVectorB.H);
    public static IsoVector operator -(IsoVector IsoVectorA, IsoVector IsoVectorB) => new IsoVector(IsoVectorA.X - IsoVectorB.X, IsoVectorA.Y - IsoVectorB.Y, IsoVectorA.H - IsoVectorB.H);
    public static IsoVector operator *(IsoVector IsoVectorA, float Number) => new IsoVector(IsoVectorA.X * Number, IsoVectorA.Y * Number, IsoVectorA.H * Number);
    public static IsoVector operator /(IsoVector IsoVectorA, float Number) => new IsoVector(IsoVectorA.X / Number, IsoVectorA.Y / Number, IsoVectorA.H / Number);
    public static bool operator ==(IsoVector IsoVectorA, IsoVector IsoVectorB) => IsoVectorA.X == IsoVectorB.X && IsoVectorA.Y == IsoVectorB.Y && IsoVectorA.H == IsoVectorB.H;
    public static bool operator !=(IsoVector IsoVectorA, IsoVector IsoVectorB) => IsoVectorA.X != IsoVectorB.X || IsoVectorA.Y != IsoVectorB.Y || IsoVectorA.H != IsoVectorB.H;

    #endregion

    #region Encypt

    public const char KEY_VECTOR_ENCYPT = ';';

    public string Encypt => "[" + QEncypt.GetEncypt(KEY_VECTOR_ENCYPT, this.X, this.Y, this.H) + "]";

    public static IsoVector GetDencypt(string m_Encypt)
    {
        m_Encypt = m_Encypt.Replace("[", "");
        m_Encypt = m_Encypt.Replace("]", "");
        List<int> DataDencypt = QEncypt.GetDencyptInt(KEY_VECTOR_ENCYPT, m_Encypt);
        return new IsoVector(DataDencypt[0], DataDencypt[1], DataDencypt[2]);
    }

    #endregion

    #region Overide

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString() => $"[{X}, {Y}, {H}]";

    #endregion
}
