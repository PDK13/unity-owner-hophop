using QuickMethode;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsoDataBlockMove
{
    public string Key = "";
    public bool Loop = false;
    public List<IsoDir> Dir = new List<IsoDir>();
    public List<int> Length = new List<int>();

    [HideInInspector]
    public int Index = 0;
    [HideInInspector]
    public int Quantity = 1;

    public List<IsoDataBlockMoveSingle> Data
    {
        get
        {
            List<IsoDataBlockMoveSingle> Data = new List<IsoDataBlockMoveSingle>();
            for (int i = 0; i < Dir.Count; i++)
                Data.Add(new IsoDataBlockMoveSingle(Dir[i], (Length.Count == Dir.Count ? Length[i] : 1)));
            return Data;
        }
    }

    public int DataCount => Dir.Count;

    public void SetDataNew()
    {
        Dir = new List<IsoDir>();
        Length = new List<int>();
    }

    public void SetDataAdd(IsoDataBlockMoveSingle DataSingle)
    {
        Dir.Add(DataSingle.Dir);
        Length.Add(DataSingle.Length);
    }

    public bool DataExist => Dir == null ? false : Dir.Count == 0 ? false : true;
}

[Serializable]
public class IsoDataBlockMoveSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public IsoDir Dir = IsoDir.None;
    public int Length = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, (int)Dir, Length);

    public IsoDataBlockMoveSingle(IsoDir Dir, int Value)
    {
        this.Dir = Dir;
        this.Length = Value;
    }

    public static IsoDataBlockMoveSingle GetDencypt(string Value)
    {
        List<int> DataString = QEncypt.GetDencyptInt(KEY_VALUE_ENCYPT, Value);
        return new IsoDataBlockMoveSingle((IsoDir)DataString[0], DataString[1]);
    }
}