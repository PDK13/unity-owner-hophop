using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class IsometricDataMove
{
    public DataBlockType Type = DataBlockType.Forward;

    //

    [SerializeField] private List<IsometricDataBlockMoveSingle> m_data = new List<IsometricDataBlockMoveSingle>();
    private int m_index = 0;
    private int m_quantity = 1;

    //

    public List<IsometricDataBlockMoveSingle> Data
    {
        get
        {
            if (m_data == null)
                m_data = new List<IsometricDataBlockMoveSingle>();
            return m_data;
        }
    }

    public int Index => m_index;

    public int Quantity => m_quantity;

    //

    public void SetDataNew()
    {
        m_data = new List<IsometricDataBlockMoveSingle>();
    }

    public void SetDataAdd(IsometricDataBlockMoveSingle DataSingle)
    {
        //if (DataSingle == null)
        //    return;
        //
        m_data.Add(DataSingle);
    }

    public void SetDataAdd(IsoDir Dir)
    {
        m_data.Add(new IsometricDataBlockMoveSingle(Dir, 1));
    }

    public void SetDataAdd(IsoDir Dir, int Duration)
    {
        m_data.Add(new IsometricDataBlockMoveSingle(Dir, Duration));
    }

    //

    public IsometricVector DirCombineCurrent => IsometricVector.GetDir(Data[Index].Dir) * Quantity;

    public IsometricVector DirCurrent => IsometricVector.GetDir(Data[Index].Dir);

    public int DurationCurrent => Data[Index].Duration;

    public void SetDirNext()
    {
        m_index += m_quantity;
        //
        if (m_index < 0 || m_index > m_data.Count - 1)
        {
            switch (Type)
            {
                case DataBlockType.Forward:
                    m_index = m_quantity == 1 ? m_data.Count - 1 : 0;
                    break;
                case DataBlockType.Loop:
                    m_index = m_quantity == 1 ? 0 : m_data.Count - 1;
                    break;
                case DataBlockType.Revert:
                    m_quantity *= -1;
                    m_index += Quantity;
                    break;
            }
        }
    }

    public void SetDirRevert()
    {
        m_quantity *= -1;
    }
}

[Serializable]
public struct IsometricDataBlockMoveSingle : IEquatable<IsometricDataBlockMoveSingle>
{
    public const char KEY_VALUE_ENCYPT = '|';

    public IsoDir Dir;
    public int Duration;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Duration.ToString(), IsometricVector.GetDirEncypt(Dir));

    public IsometricDataBlockMoveSingle(IsoDir Dir, int Value)
    {
        this.Dir = Dir;
        Duration = Value;
    }

    public static IsometricDataBlockMoveSingle GetDencypt(string Value)
    {
        //if (Value == "")
        //{
        //    return null;
        //}
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockMoveSingle(IsometricVector.GetDirDeEncyptEnum(DataString[1]), int.Parse(DataString[0]));
    }

    //

    #region Overide

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"[{Dir}, {Duration}]";
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(IsometricDataBlockMoveSingle other)
    {
        return base.Equals(other);
    }

    #endregion
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(IsometricDataBlockMoveSingle))]
public class IsometricDataBlockMoveSingleEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return QUnityEditorObject.GetContainer(
            property,
            nameof(IsometricDataBlockMoveSingle.Dir),
            nameof(IsometricDataBlockMoveSingle.Duration));
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

        float WidthField = position.width / 2f;

        float PosXLabel = position.x;

        float PosXField = PosXLabel;
        float SpaceXField = WidthField;
        //
        Rect RecFieldDir = new Rect(PosXField + SpaceXField * 0 + SpaceBetween * 0, position.y, WidthField, position.height);
        Rect RecFieldDuration = new Rect(PosXField + SpaceXField * 1 + SpaceBetween * 1, position.y, WidthField, position.height);
        //
        QUnityEditorObject.SetField(property, nameof(IsometricDataBlockMoveSingle.Dir), RecFieldDir, false);

        QUnityEditorObject.SetField(property, nameof(IsometricDataBlockMoveSingle.Duration), RecFieldDuration, false);
        //
        QUnityEditorObject.SetPropertyEnd();
    }
}

#endif