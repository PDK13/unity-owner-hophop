using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricDataAction
{
    public DataBlockType Type = DataBlockType.Forward;

    //

    [SerializeField] private List<IsometricDataBlockActionSingle> m_data = new List<IsometricDataBlockActionSingle>();
    private int m_index = 0;
    private int m_quantity = 1;

    //

    public List<IsometricDataBlockActionSingle> Data
    {
        get
        {
            if (m_data == null)
                m_data = new List<IsometricDataBlockActionSingle>();
            return m_data;
        }
    }

    public int Index => m_index;

    public int Quantity => m_quantity;

    //

    public void SetDataNew()
    {
        m_data = new List<IsometricDataBlockActionSingle>();
    }

    public void SetDataAdd(IsometricDataBlockActionSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        m_data.Add(DataSingle);
    }

    public void SetDataAdd(string Action)
    {
        m_data.Add(new IsometricDataBlockActionSingle(Action, 1));
    }

    public void SetDataAdd(string Action, int Duration)
    {
        m_data.Add(new IsometricDataBlockActionSingle(Action, Duration));
    }

    //

    public string ActionCurrent => Data[Index].Action;

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
public class IsometricDataBlockActionSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Action = "";
    public int Duration = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Duration.ToString(), Action);

    public IsometricDataBlockActionSingle(string Action, int Time)
    {
        this.Action = Action;
        this.Duration = Time;
    }

    public static IsometricDataBlockActionSingle GetDencypt(string Value)
    {
        if (Value == "")
        {
            return null;
        }
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockActionSingle(DataString[1], int.Parse(DataString[0]));
    }
}