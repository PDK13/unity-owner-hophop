using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsoDataBlockTeleport
{
    public string KeyStart = "Teleport-Start";
    public string KeyEnd = "Teleport-End";
    public List<IsoDataBlockTeleportSingle> Data;
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