using QuickMethode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricDataBlockAction
{
    public string KeyGet = "";
    public string KeySet = "";
    public DataBlockType TypeList = DataBlockType.Forward;
    public List<string> ActionList = new List<string>();
    public List<int> TimeList = new List<int>();

    [HideInInspector]
    public int Index = 0;
    [HideInInspector]
    public int Quantity = 1;

    public List<IsometricDataBlockActionSingle> Data
    {
        get
        {
            List<IsometricDataBlockActionSingle> Data = new List<IsometricDataBlockActionSingle>();
            for (int i = 0; i < ActionList.Count; i++)
                Data.Add(new IsometricDataBlockActionSingle(ActionList[i], (ActionList.Count == TimeList.Count ? TimeList[i] : 1)));
            return Data;
        }
    }

    public int DataCount => ActionList.Count;

    public void SetDataNew()
    {
        ActionList = new List<string>();
        TimeList = new List<int>();
    }

    public void SetDataAdd(IsometricDataBlockActionSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        ActionList.Add(DataSingle.Action);
        TimeList.Add(DataSingle.Time);
    }

    public bool DataExist => ActionList == null ? false : ActionList.Count == 0 ? false : true;
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