using QuickMethode;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum IsoDir { Stop = -1, None = 0, Up = 1, Down = 2, Left = 3, Right = 4, Top = 5, Bot = 6 }

[Serializable]
public struct IsoVector : IEquatable<IsoVector>
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
    public static IsoVector None => new IsoVector(0, 0, 0);

    public static Vector3 GetVector(IsoVector Pos)
    {
        return new Vector3(Pos.X, Pos.Y, Pos.H);
    }

    public static Vector3Int GetVector(IsoDir Dir)
    {
        switch (Dir)
        {
            case IsoDir.Up:
                return Vector3Int.right;
            case IsoDir.Down:
                return Vector3Int.left;
            case IsoDir.Left:
                return Vector3Int.down;
            case IsoDir.Right:
                return Vector3Int.up;
            case IsoDir.Top:
                return Vector3Int.forward;
            case IsoDir.Bot:
                return Vector3Int.back;
        }
        return Vector3Int.zero;
    }

    public static IsoVector GetDir(Vector3Int Dir)
    {
        if (Dir.Equals(Vector3Int.right))
            return Up;

        if (Dir.Equals(Vector3Int.left))
            return Down;

        if (Dir.Equals(Vector3Int.down))
            return Left;

        if (Dir.Equals(Vector3Int.up))
            return Right;

        if (Dir.Equals(Vector3Int.forward))
            return Top;

        if (Dir.Equals(Vector3Int.back))
            return Bot;

        return None;
    }

    public static IsoVector GetDir(IsoDir Dir)
    {
        switch (Dir)
        {
            case IsoDir.Up:
                return Up;
            case IsoDir.Down:
                return Down;
            case IsoDir.Left:
                return Left;
            case IsoDir.Right:
                return Right;
            case IsoDir.Top:
                return Top;
            case IsoDir.Bot:
                return Bot;
        }
        return None;
    }

    public static IsoDir GetDirEnum(string Data)
    {
        switch (Data)
        {
            case "U":
            case "u":
                return IsoDir.Up;
            case "D":
            case "d":
                return IsoDir.Down;
            case "L":
            case "l":
                return IsoDir.Left;
            case "R":
            case "r":
                return IsoDir.Right;
            case "T":
            case "t":
                return IsoDir.Top;
            case "B":
            case "b":
                return IsoDir.Bot;
            case "N":
            case "n":
                return IsoDir.None;
        }
        Debug.LogError("[Caution] Data not correct!!");
        return IsoDir.None;
    }

    public static IsoVector GetDirValue(string Data)
    {
        switch (Data)
        {
            case "U":
            case "u":
                return Up;
            case "D":
            case "d":
                return Down;
            case "L":
            case "l":
                return Left;
            case "R":
            case "r":
                return Right;
            case "T":
            case "t":
                return Top;
            case "B":
            case "b":
                return Bot;
            case "N":
            case "n":
                return None;
        }
        Debug.LogError("[Caution] Data not correct!!");
        return None;
    }

    public static string GetEncyptDir(IsoDir Dir)
    {
        switch (Dir)
        {
            case IsoDir.Up:
                return "U";
            case IsoDir.Down:
                return "D";
            case IsoDir.Left:
                return "L";
            case IsoDir.Right:
                return "R";
            case IsoDir.Top:
                return "T";
            case IsoDir.Bot:
                return "B";
        }
        return "N";
    }

    public static string GetEncyptDir(IsoVector Dir)
    {
        if (Dir == Up)
            return "U";

        if (Dir == Down)
            return "U";

        if (Dir == Left)
            return "U";

        if (Dir == Right)
            return "U";

        if (Dir == Top)
            return "U";

        if (Dir == Bot)
            return "U";

        return "N";
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

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString() => $"[{X}, {Y}, {H}]";

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(IsoVector other)
    {
        return base.Equals(other);
    }

    #endregion
}