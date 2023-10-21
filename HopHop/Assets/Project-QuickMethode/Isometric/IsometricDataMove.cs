using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricDataMove
{
    public DataBlockType Type = DataBlockType.Forward;
    public List<IsoDir> Dir = new List<IsoDir>();
    public List<int> Duration = new List<int>();

    [HideInInspector]
    public int Index = 0;
    [HideInInspector]
    public int Quantity = 1;

    public List<IsometricDataBlockMoveSingle> Data
    {
        get
        {
            List<IsometricDataBlockMoveSingle> Data = new List<IsometricDataBlockMoveSingle>();
            for (int i = 0; i < Dir.Count; i++)
                Data.Add(new IsometricDataBlockMoveSingle(Dir[i], (Duration.Count == Dir.Count ? Duration[i] : 1)));
            //
            return Data;
        }
    }

    public int DataCount => Dir.Count;

    public void SetDataNew()
    {
        Dir = new List<IsoDir>();
        Duration = new List<int>();
    }

    public void SetDataAdd(IsometricDataBlockMoveSingle DataSingle)
    {
        if (DataSingle == null)
        {
            return;
        }
        //
        Dir.Add(DataSingle.Dir);
        Duration.Add(DataSingle.Duration);
    }

    public void SetDataAdd(IsoDir Dir)
    {
        this.Dir.Add(Dir);
    }

    public void SetDataAdd(IsoDir Dir, int Duration)
    {
        this.Dir.Add(Dir);
        this.Duration.Add(Duration);
    }

    public bool DataExist => Dir == null ? false : Dir.Count == 0 ? false : true;
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