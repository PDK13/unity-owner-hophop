using QuickMethode;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public static IsoVector GetDir(IsoDir Dir, IsometricRotateType Rotate = IsometricRotateType._0)
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

    public static IsoVector GetDirDeEncypt(string Data)
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

    [NonSerialized]
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

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(IsoVector))]
public class IsoVectorEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return QEditorObject.GetContainer(
            property,
            nameof(IsoVector.X),
            nameof(IsoVector.Y),
            nameof(IsoVector.H));
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
        var RecLabelX = new Rect(PosXLabel + SpaceXLabel * 0 + SpaceBetween * 0, position.y, WidthLabel, position.height);
        var RecLabelY = new Rect(PosXLabel + SpaceXLabel * 1 + SpaceBetween * 1, position.y, WidthLabel, position.height);
        var RecLabelH = new Rect(PosXLabel + SpaceXLabel * 2 + SpaceBetween * 2, position.y, WidthLabel, position.height);

        var RecFieldX = new Rect(PosXField + SpaceXField * 0 + SpaceBetween * 0, position.y, WidthField, position.height);
        var RecFieldY = new Rect(PosXField + SpaceXField * 1 + SpaceBetween * 1, position.y, WidthField, position.height);
        var RecFieldH = new Rect(PosXField + SpaceXField * 2 + SpaceBetween * 2, position.y, WidthField, position.height);
        //
        QEditor.SetLabel("X", RecLabelX);
        QEditorObject.SetField(property, nameof(IsoVector.X), RecFieldX, false);

        QEditor.SetLabel("Y", RecLabelY);
        QEditorObject.SetField(property, nameof(IsoVector.Y), RecFieldY, false);

        QEditor.SetLabel("H", RecLabelH);
        QEditorObject.SetField(property, nameof(IsoVector.H), RecFieldH, false);
        //
        QEditorObject.SetPropertyEnd();
    }
}

#endif