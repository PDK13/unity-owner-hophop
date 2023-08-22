using QuickMethode;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricDataBlockMove
{
    public string KeyGet = "";
    public string KeySet = "";
    public DataBlockType TypeList = DataBlockType.Forward;
    public List<IsoDir> DirList = new List<IsoDir>();
    public List<int> LengthList = new List<int>();

    [HideInInspector]
    public int Index = 0;
    [HideInInspector]
    public int Quantity = 1;

    public List<IsometricDataBlockMoveSingle> Data
    {
        get
        {
            List<IsometricDataBlockMoveSingle> Data = new List<IsometricDataBlockMoveSingle>();
            for (int i = 0; i < DirList.Count; i++)
                Data.Add(new IsometricDataBlockMoveSingle(DirList[i], (LengthList.Count == DirList.Count ? LengthList[i] : 1)));
            return Data;
        }
    }

    public int DataCount => DirList.Count;

    public void SetDataNew()
    {
        DirList = new List<IsoDir>();
        LengthList = new List<int>();
    }

    public void SetDataAdd(IsometricDataBlockMoveSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        DirList.Add(DataSingle.Dir);
        LengthList.Add(DataSingle.Length);
    }

    public bool DataExist => DirList == null ? false : DirList.Count == 0 ? false : true;
}

[Serializable]
public class IsometricDataBlockMoveSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public IsoDir Dir = IsoDir.None;
    public int Length = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Length.ToString(), IsometricVector.GetDirEncypt(Dir));

    public IsometricDataBlockMoveSingle(IsoDir Dir, int Value)
    {
        this.Dir = Dir;
        this.Length = Value;
    }

    public static IsometricDataBlockMoveSingle GetDencypt(string Value)
    {
        if (Value == "")
            return null;
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockMoveSingle(IsometricVector.GetDirDeEncyptEnum(DataString[1]), int.Parse(DataString[0]));
    }
}