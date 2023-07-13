using QuickMethode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsoDataBlockAction
{
    public string Key = "";
    public bool Loop = false;
    public List<string> Action = new List<string>();
    public List<int> Time = new List<int>();

    [HideInInspector]
    public int Index = 0;
    [HideInInspector]
    public int Quantity = 1;

    public List<IsoDataBlockActionSingle> Data
    {
        get
        {
            List<IsoDataBlockActionSingle> Data = new List<IsoDataBlockActionSingle>();
            for (int i = 0; i < Action.Count; i++)
                Data.Add(new IsoDataBlockActionSingle(Action[i], (Action.Count == Time.Count ? Time[i] : 1)));
            return Data;
        }
    }

    public int DataCount => Action.Count;

    public void SetDataNew()
    {
        Action = new List<string>();
        Time = new List<int>();
    }

    public void SetDataAdd(IsoDataBlockActionSingle DataSingle)
    {
        Action.Add(DataSingle.Action);
        Time.Add(DataSingle.Time);
    }

    public bool DataExist => Action == null ? false : Action.Count == 0 ? false : true;
}

[Serializable]
public class IsoDataBlockActionSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Action = "";
    public int Time = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Time.ToString(), Action);

    public IsoDataBlockActionSingle(string Action, int Time)
    {
        this.Action = Action;
        this.Time = Time;
    }

    public static IsoDataBlockActionSingle GetDencypt(string Value)
    {
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsoDataBlockActionSingle(DataString[1], int.Parse(DataString[0]));
    }
}