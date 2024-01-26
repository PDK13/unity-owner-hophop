using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum IsoDir { Stop = -1, None = 0, Up = 1, Down = 2, Left = 3, Right = 4, Top = 5, Bot = 6 }

[Serializable]
public struct IsometricVector : IEquatable<IsometricVector>
{
    #region Primary

    public IsometricVector(float XUD, float YLR, float HTB)
    {
        X = XUD;
        Y = YLR;
        H = HTB;
    }

    public IsometricVector(IsometricVector IsometricVector)
    {
        X = IsometricVector.X;
        Y = IsometricVector.Y;
        H = IsometricVector.H;
    }

    public IsometricVector(Vector3 Vector)
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

    public IsometricVector Fixed => new IsometricVector(XInt, YInt, HInt);

    #endregion

    #region Primary Dir

    public static IsometricVector Up => new IsometricVector(1, 0, 0);
    public static IsometricVector Down => new IsometricVector(-1, 0, 0);
    public static IsometricVector Left => new IsometricVector(0, -1, 0);
    public static IsometricVector Right => new IsometricVector(0, 1, 0);
    public static IsometricVector Top => new IsometricVector(0, 0, 1);
    public static IsometricVector Bot => new IsometricVector(0, 0, -1);
    public static IsometricVector None => new IsometricVector(0, 0, 0);

    //

    public static IsoDir GetDir(IsometricVector Dir)
    {
        if (Dir == Up)
            return IsoDir.Up;
        if (Dir == Down)
            return IsoDir.Down;
        if (Dir == Left)
            return IsoDir.Left;
        if (Dir == Right)
            return IsoDir.Right;
        if (Dir == Top)
            return IsoDir.Top;
        if (Dir == Bot)
            return IsoDir.Bot;
        if (Dir == None)
            return IsoDir.None;
        //
        Debug.LogError("[Caution] Data not correct!!");
        return IsoDir.None;
    }

    public static IsoDir GetDirDeEncyptEnum(string Data)
    {
        switch (Data)
        {
            case "U":
            case "u":
            case "Up":
            case "up":
                return IsoDir.Up;
            case "D":
            case "d":
            case "Down":
            case "down":
                return IsoDir.Down;
            case "L":
            case "l":
            case "Left":
            case "left":
                return IsoDir.Left;
            case "R":
            case "r":
            case "Right":
            case "right":
                return IsoDir.Right;
            case "T":
            case "t":
            case "Top":
            case "top":
                return IsoDir.Top;
            case "B":
            case "b":
            case "Bot":
            case "bot":
                return IsoDir.Bot;
            case "N":
            case "n":
            case "None":
            case "none":
                return IsoDir.None;
        }
        Debug.LogError("[Caution] Data not correct!!");
        return IsoDir.None;
    }

    public static IsometricVector GetDir(IsoDir Dir)
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

    public static IsometricVector GetDirDeEncypt(string Data)
    {
        switch (Data)
        {
            case "U":
            case "u":
            case "Up":
            case "up":
                return Up;
            case "D":
            case "d":
            case "Down":
            case "down":
                return Down;
            case "L":
            case "l":
            case "Left":
            case "left":
                return Left;
            case "R":
            case "r":
            case "Right":
            case "right":
                return Right;
            case "T":
            case "t":
            case "Top":
            case "top":
                return Top;
            case "B":
            case "b":
            case "Bot":
            case "bot":
                return Bot;
            case "N":
            case "n":
            case "None":
            case "none":
                return None;
        }
        Debug.LogError("[Caution] Data not correct!!");
        return None;
    }

    public static Vector3 GetVector(IsoDir Dir)
    {
        switch (Dir)
        {
            case IsoDir.Up:
                return GetVector(Up);
            case IsoDir.Down:
                return GetVector(Down);
            case IsoDir.Left:
                return GetVector(Left);
            case IsoDir.Right:
                return GetVector(Right);
            case IsoDir.Top:
                return GetVector(Top);
            case IsoDir.Bot:
                return GetVector(Bot);
        }
        return GetVector(None);
    }

    public static Vector3 GetVector(IsometricVector Dir)
    {
        return new Vector3(Dir.X, Dir.Y, Dir.H);
    }

    public static string GetDirEncypt(IsoDir Dir)
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

    //Rotate

    public static IsoDir GetRotateDir(IsoDir Dir, IsometricRotateType Rotate = IsometricRotateType._0)
    {
        switch (Dir)
        {
            case IsoDir.Up:
                switch (Rotate)
                {
                    case IsometricRotateType._0:
                        return IsoDir.Up;
                    case IsometricRotateType._90:
                        return IsoDir.Left;
                    case IsometricRotateType._180:
                        return IsoDir.Down;
                    case IsometricRotateType._270:
                        return IsoDir.Right;
                }
                break;
            case IsoDir.Down:
                switch (Rotate)
                {
                    case IsometricRotateType._0:
                        return IsoDir.Down;
                    case IsometricRotateType._90:
                        return IsoDir.Right;
                    case IsometricRotateType._180:
                        return IsoDir.Up;
                    case IsometricRotateType._270:
                        return IsoDir.Left;
                }
                break;
            case IsoDir.Left:
                switch (Rotate)
                {
                    case IsometricRotateType._0:
                        return IsoDir.Left;
                    case IsometricRotateType._90:
                        return IsoDir.Down;
                    case IsometricRotateType._180:
                        return IsoDir.Right;
                    case IsometricRotateType._270:
                        return IsoDir.Up;
                }
                break;
            case IsoDir.Right:
                switch (Rotate)
                {
                    case IsometricRotateType._0:
                        return IsoDir.Right;
                    case IsometricRotateType._90:
                        return IsoDir.Up;
                    case IsometricRotateType._180:
                        return IsoDir.Left;
                    case IsometricRotateType._270:
                        return IsoDir.Down;
                }
                break;
            case IsoDir.Top:
                return IsoDir.Top;
            case IsoDir.Bot:
                return IsoDir.Bot;
        }
        return IsoDir.None;
    }

    public static IsometricVector GetRotateDir(IsometricVector Dir, IsometricRotateType Rotate = IsometricRotateType._0)
    {
        if (Dir == Up)
            switch (Rotate)
            {
                case IsometricRotateType._0:
                    return Up;
                case IsometricRotateType._90:
                    return Left;
                case IsometricRotateType._180:
                    return Down;
                case IsometricRotateType._270:
                    return Right;
            }
        //
        if (Dir == Down)
            switch (Rotate)
            {
                case IsometricRotateType._0:
                    return Down;
                case IsometricRotateType._90:
                    return Right;
                case IsometricRotateType._180:
                    return Up;
                case IsometricRotateType._270:
                    return Left;
            }
        //
        if (Dir == Left)
            switch (Rotate)
            {
                case IsometricRotateType._0:
                    return Left;
                case IsometricRotateType._90:
                    return Down;
                case IsometricRotateType._180:
                    return Right;
                case IsometricRotateType._270:
                    return Up;
            }
        //
        if (Dir == Right)
            switch (Rotate)
            {
                case IsometricRotateType._0:
                    return Right;
                case IsometricRotateType._90:
                    return Up;
                case IsometricRotateType._180:
                    return Left;
                case IsometricRotateType._270:
                    return Down;
            }
        //
        if (Dir == Top)
            return Top;
        //
        if (Dir == Bot)
            return Bot;
        //
        return None;
    }

    #endregion

    #region Operator

    public static IsometricVector operator +(IsometricVector IsometricVector)
    {
        return IsometricVector;
    }

    public static IsometricVector operator -(IsometricVector IsometricVector)
    {
        return new IsometricVector(IsometricVector.X * -1, IsometricVector.Y * -1, IsometricVector.H * -1);
    }

    public static IsometricVector operator +(IsometricVector IsometricVectorA, IsometricVector IsometricVectorB)
    {
        return new IsometricVector(IsometricVectorA.X + IsometricVectorB.X, IsometricVectorA.Y + IsometricVectorB.Y, IsometricVectorA.H + IsometricVectorB.H);
    }

    public static IsometricVector operator -(IsometricVector IsometricVectorA, IsometricVector IsometricVectorB)
    {
        return new IsometricVector(IsometricVectorA.X - IsometricVectorB.X, IsometricVectorA.Y - IsometricVectorB.Y, IsometricVectorA.H - IsometricVectorB.H);
    }

    public static IsometricVector operator *(IsometricVector IsometricVectorA, float Number)
    {
        return new IsometricVector(IsometricVectorA.X * Number, IsometricVectorA.Y * Number, IsometricVectorA.H * Number);
    }

    public static IsometricVector operator /(IsometricVector IsometricVectorA, float Number)
    {
        return new IsometricVector(IsometricVectorA.X / Number, IsometricVectorA.Y / Number, IsometricVectorA.H / Number);
    }

    public static bool operator ==(IsometricVector IsometricVectorA, IsometricVector IsometricVectorB)
    {
        return IsometricVectorA.X == IsometricVectorB.X && IsometricVectorA.Y == IsometricVectorB.Y && IsometricVectorA.H == IsometricVectorB.H;
    }

    public static bool operator !=(IsometricVector IsometricVectorA, IsometricVector IsometricVectorB)
    {
        return IsometricVectorA.X != IsometricVectorB.X || IsometricVectorA.Y != IsometricVectorB.Y || IsometricVectorA.H != IsometricVectorB.H;
    }

    #endregion

    #region Encypt

    [NonSerialized]
    public const char KEY_VECTOR_ENCYPT = ';';

    public string Encypt => "[" + QEncypt.GetEncypt(KEY_VECTOR_ENCYPT, X, Y, H) + "]";

    public static IsometricVector GetDencypt(string m_Encypt)
    {
        m_Encypt = m_Encypt.Replace("[", "");
        m_Encypt = m_Encypt.Replace("]", "");
        List<int> DataDencypt = QEncypt.GetDencyptInt(KEY_VECTOR_ENCYPT, m_Encypt);
        return new IsometricVector(DataDencypt[0], DataDencypt[1], DataDencypt[2]);
    }

    #endregion

    #region Overide

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"[{X},{Y},{H}]";
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(IsometricVector other)
    {
        return base.Equals(other);
    }

    #endregion
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(IsometricVector))]
public class IsometricVectorEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return QUnityEditorObject.GetContainer(
            property,
            nameof(IsometricVector.X),
            nameof(IsometricVector.Y),
            nameof(IsometricVector.H));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        QUnityEditorObject.SetPropertyBegin(position, property, label);
        //
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        //
        QUnityEditor.IndentLevel = 0;
        //
        float SpaceBetween = 5f;

        float WidthLabel = 15f;
        float WidthField = (position.width - WidthLabel * 3) / 3f - 5f;

        float PosXLabel = position.x;
        float SpaceXLabel = WidthLabel + WidthField;

        float PosXField = PosXLabel + WidthLabel;
        float SpaceXField = WidthField + WidthLabel;
        //
        Rect RecLabelX = new Rect(PosXLabel + SpaceXLabel * 0 + SpaceBetween * 0, position.y, WidthLabel, position.height);
        Rect RecLabelY = new Rect(PosXLabel + SpaceXLabel * 1 + SpaceBetween * 1, position.y, WidthLabel, position.height);
        Rect RecLabelH = new Rect(PosXLabel + SpaceXLabel * 2 + SpaceBetween * 2, position.y, WidthLabel, position.height);

        Rect RecFieldX = new Rect(PosXField + SpaceXField * 0 + SpaceBetween * 0, position.y, WidthField, position.height);
        Rect RecFieldY = new Rect(PosXField + SpaceXField * 1 + SpaceBetween * 1, position.y, WidthField, position.height);
        Rect RecFieldH = new Rect(PosXField + SpaceXField * 2 + SpaceBetween * 2, position.y, WidthField, position.height);
        //
        QUnityEditor.SetLabel("X", RecLabelX);
        QUnityEditorObject.SetField(property, nameof(IsometricVector.X), RecFieldX, false);

        QUnityEditor.SetLabel("Y", RecLabelY);
        QUnityEditorObject.SetField(property, nameof(IsometricVector.Y), RecFieldY, false);

        QUnityEditor.SetLabel("H", RecLabelH);
        QUnityEditorObject.SetField(property, nameof(IsometricVector.H), RecFieldH, false);
        //
        QUnityEditorObject.SetPropertyEnd();
    }
}

#endif