using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricDataAction : MonoBehaviour
{
    public DataBlockType Type = DataBlockType.Forward;

    //

    [SerializeField] private List<IsometricDataBlockActionSingle> m_data = new List<IsometricDataBlockActionSingle>();
    private int m_index = 0;
    private int m_quantity = 1;

    //

    public List<IsometricDataBlockActionSingle> Data
    {
        private set => m_data = value;
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

    public void SetValue(IsometricDataAction Value)
    {
        Type = Value.Type;
        Data = Value.Data;
    }

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
        m_data.Add(new IsometricDataBlockActionSingle(Action));
    }

    //

    public List<string> ActionCurrent => Data[Index].Action;

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

    public List<string> Action = new List<string>();

    public string Encypt => QString.GetSplit(KEY_VALUE_ENCYPT, QString.GetSplit(KEY_VALUE_ENCYPT, Action));

    public IsometricDataBlockActionSingle(List<string> Action)
    {
        this.Action = Action;
    }

    public IsometricDataBlockActionSingle(string ActionSingle)
    {
        this.Action = new List<string>() { ActionSingle };
    }

    public static IsometricDataBlockActionSingle GetUnSplit(string Value)
    {
        if (Value == "")
        {
            return null;
        }
        //
        List<string> DataString = QString.GetUnSplitString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockActionSingle(DataString);
    }
}