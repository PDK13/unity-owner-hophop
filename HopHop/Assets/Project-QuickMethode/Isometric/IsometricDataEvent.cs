using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsoDataBlockEvent
{
    public string KeyStart = "Event-Start";
    public string KeyEnd = "Event-End";
    public List<IsoDataBlockEventSingle> Data;
}

[Serializable]
public struct IsoDataBlockEventSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Name;
    public string Value;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Name, Value);

    public IsoDataBlockEventSingle(string Name, string Value)
    {
        this.Name = Name;
        this.Value = Value;
    }

    public static IsoDataBlockEventSingle GetDencypt(string Value)
    {
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsoDataBlockEventSingle(DataString[0], DataString[1]);
    }
}