using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsometricDataBlockEvent
{
    public string Key = "";
    public List<IsometricDataBlockEventSingle> Data = new List<IsometricDataBlockEventSingle>();

    public void SetDataAdd(IsometricDataBlockEventSingle DataSingle)
    {
        if (DataSingle == null)
            return;
        //
        Data.Add(DataSingle);
    }

    public bool DataExist => Data == null ? false : Data.Count == 0 ? false : true;
}

[Serializable]
public class IsometricDataBlockEventSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Name;
    public string Value;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Name, Value);

    public IsometricDataBlockEventSingle(string Name, string Value)
    {
        this.Name = Name;
        this.Value = Value;
    }

    public static IsometricDataBlockEventSingle GetDencypt(string Value)
    {
        if (Value == "")
            return null;
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataBlockEventSingle(DataString[0], DataString[1]);
    }
}