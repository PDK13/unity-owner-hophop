using System;
using System.Collections.Generic;
using UnityEngine;

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
        if (DataSingle == null)
            return;
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
public class IsometricDataBlockMoveSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public IsoDir Dir = IsoDir.None;
    public int Duration = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Duration.ToString(), IsometricVector.GetDirEncypt(Dir));

    public IsometricDataBlockMoveSingle(IsoDir Dir, int Value)
    {
        this.Dir = Dir;
        Duration = Value;
    }

    public static IsometricDataBlockMoveSingle GetDencypt(string Value)
    {
        if (Value == "")
        {
            return null;
        }
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockMoveSingle(IsometricVector.GetDirDeEncyptEnum(DataString[1]), int.Parse(DataString[0]));
    }
}