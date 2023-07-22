using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsoDataBlockEvent
{
    public string Key = "";
    public List<IsoDataBlockEventSingle> Data = new List<IsoDataBlockEventSingle>();

    public void SetDataAdd(IsoDataBlockEventSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        Data.Add(DataSingle);
    }

    public bool DataExist => Data == null ? false : Data.Count == 0 ? false : true;
}

[Serializable]
public class IsoDataBlockEventSingle
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
        if (Value == "")
            return null;
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsoDataBlockEventSingle(DataString[0], DataString[1]);
    }
}