using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsometricDataBlockTeleport
{
    public string Key = "";
    public List<IsometricDataBlockTeleportSingle> Data = new List<IsometricDataBlockTeleportSingle>();

    public void SetDataAdd(IsometricDataBlockTeleportSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        Data.Add(DataSingle);
    }

    public bool DataExist => Data == null ? false : Data.Count == 0 ? false : true;
}

[Serializable]
public class IsometricDataBlockTeleportSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Name;
    public IsoVector Pos;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Name, Pos.Encypt);

    public IsometricDataBlockTeleportSingle(string Name, IsoVector Value)
    {
        this.Name = Name;
        this.Pos = Value;
    }

    public static IsometricDataBlockTeleportSingle GetDencypt(string Value)
    {
        if (Value == "")
            return null;
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockTeleportSingle(DataString[0], IsoVector.GetDencypt(DataString[1]));
    }
}