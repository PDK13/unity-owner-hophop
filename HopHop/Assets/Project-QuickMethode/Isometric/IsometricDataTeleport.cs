using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsoDataBlockTeleport
{
    public string Name = "Teleport";
    public List<IsoDataBlockTeleportSingle> Data = new List<IsoDataBlockTeleportSingle>();
}

[Serializable]
public class IsoDataBlockTeleportSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Name;
    public IsoVector Pos;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Name, Pos.Encypt);

    public IsoDataBlockTeleportSingle(string Name, IsoVector Value)
    {
        this.Name = Name;
        this.Pos = Value;
    }

    public static IsoDataBlockTeleportSingle GetDencypt(string Value)
    {
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsoDataBlockTeleportSingle(DataString[0], IsoVector.GetDencypt(DataString[1]));
    }
}