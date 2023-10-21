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

    public IsometricVector(IsometricVector IsoVector)
    {
        X = IsoVector.X;
        Y = IsoVector.Y;
        H = IsoVector.H;
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

    public static Vector3 GetVector(IsometricVector Pos)
    {
        return new Vector3(Pos.X, Pos.Y, Pos.H);
    }

    public static IsometricVector GetDir(IsoDir Dir, IsometricRotateType Rotate = IsometricRotateType._0)
    {
        switch (Dir)
        {
            case IsoDir.Up:
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
                break;
            case IsoDir.Down:
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
                break;
            case IsoDir.Left:
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
                break;
            case IsoDir.Right:
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
                break;
            case IsoDir.Top:
                return Top;
            case IsoDir.Bot:
                return Bot;
        }
        return None;
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

    #endregion

    #region Operator

    public static IsometricVector operator +(IsometricVector IsoVector)
    {
        return IsoVector;
    }

    public static IsometricVector operator -(IsometricVector IsoVector)
    {
        return new IsometricVector(IsoVector.X * -1, IsoVector.Y * -1, IsoVector.H * -1);
    }

    public static IsometricVector operator +(IsometricVector IsoVectorA, IsometricVector IsoVectorB)
    {
        return new IsometricVector(IsoVectorA.X + IsoVectorB.X, IsoVectorA.Y + IsoVectorB.Y, IsoVectorA.H + IsoVectorB.H);
    }

    public static IsometricVector operator -(IsometricVector IsoVectorA, IsometricVector IsoVectorB)
    {
        return new IsometricVector(IsoVectorA.X - IsoVectorB.X, IsoVectorA.Y - IsoVectorB.Y, IsoVectorA.H - IsoVectorB.H);
    }

    public static IsometricVector operator *(IsometricVector IsoVectorA, float Number)
    {
        return new IsometricVector(IsoVectorA.X * Number, IsoVectorA.Y * Number, IsoVectorA.H * Number);
    }

    public static IsometricVector operator /(IsometricVector IsoVectorA, float Number)
    {
        return new IsometricVector(IsoVectorA.X / Number, IsoVectorA.Y / Number, IsoVectorA.H / Number);
    }

    public static bool operator ==(IsometricVector IsoVectorA, IsometricVector IsoVectorB)
    {
        return IsoVectorA.X == IsoVectorB.X && IsoVectorA.Y == IsoVectorB.Y && IsoVectorA.H == IsoVectorB.H;
    }

    public static bool operator !=(IsometricVector IsoVectorA, IsometricVector IsoVectorB)
    {
        return IsoVectorA.X != IsoVectorB.X || IsoVectorA.Y != IsoVectorB.Y || IsoVectorA.H != IsoVectorB.H;
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
        return $"[{X}, {Y}, {H}]";
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
public class IsoVectorEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return QEditorObject.GetContainer(
            property,
            nameof(IsometricVector.X),
            nameof(IsometricVector.Y),
            nameof(IsometricVector.H));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        QEditorObject.SetPropertyBegin(position, property, label);
        //
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        //
        QEditor.IndentLevel = 0;
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
        QEditor.SetLabel("X", RecLabelX);
        QEditorObject.SetField(property, nameof(IsometricVector.X), RecFieldX, false);

        QEditor.SetLabel("Y", RecLabelY);
        QEditorObject.SetField(property, nameof(IsometricVector.Y), RecFieldY, false);

        QEditor.SetLabel("H", RecLabelH);
        QEditorObject.SetField(property, nameof(IsometricVector.H), RecFieldH, false);
        //
        QEditorObject.SetPropertyEnd();
    }
}

#endif