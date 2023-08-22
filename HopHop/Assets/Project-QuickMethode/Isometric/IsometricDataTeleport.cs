using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsometricDataBlockTeleport
{
    public string KeyGet = "";
    public string KeySet = "";
    public List<IsometricDataBlockTeleportSingle> TeleportList = new List<IsometricDataBlockTeleportSingle>();

    public void SetDataAdd(IsometricDataBlockTeleportSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        TeleportList.Add(DataSingle);
    }

    public bool DataExist => TeleportList == null ? false : TeleportList.Count == 0 ? false : true;
}

[Serializable]
public class IsometricDataBlockTeleportSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Name;
    public IsometricVector Pos;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Name, Pos.Encypt);

    public IsometricDataBlockTeleportSingle(string Name, IsometricVector Value)
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
        return new IsometricDataBlockTeleportSingle(DataString[0], IsometricVector.GetDencypt(DataString[1]));
    }
}