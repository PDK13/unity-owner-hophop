using QuickMethode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricDataBlockAction
{
    public string Key = "";
    public DataBlockType Type = DataBlockType.Forward;
    public List<string> Action = new List<string>();
    public List<int> Time = new List<int>();

    [HideInInspector]
    public int Index = 0;
    [HideInInspector]
    public int Quantity = 1;

    public List<IsometricDataBlockActionSingle> Data
    {
        get
        {
            List<IsometricDataBlockActionSingle> Data = new List<IsometricDataBlockActionSingle>();
            for (int i = 0; i < Action.Count; i++)
                Data.Add(new IsometricDataBlockActionSingle(Action[i], (Action.Count == Time.Count ? Time[i] : 1)));
            return Data;
        }
    }

    public int DataCount => Action.Count;

    public void SetDataNew()
    {
        Action = new List<string>();
        Time = new List<int>();
    }

    public void SetDataAdd(IsometricDataBlockActionSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        Action.Add(DataSingle.Action);
        Time.Add(DataSingle.Time);
    }

    public bool DataExist => Action == null ? false : Action.Count == 0 ? false : true;
}

[Serializable]
public class IsometricDataBlockActionSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Action = "";
    public int Time = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Time.ToString(), Action);

    public IsometricDataBlockActionSingle(string Action, int Time)
    {
        this.Action = Action;
        this.Time = Time;
    }

    public static IsometricDataBlockActionSingle GetDencypt(string Value)
    {
        if (Value == "")
            return null;
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockActionSingle(DataString[1], int.Parse(DataString[0]));
    }
}