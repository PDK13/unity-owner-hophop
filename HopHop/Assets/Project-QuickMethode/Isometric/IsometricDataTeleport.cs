using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsometricDataTeleport
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Name = "";
    public IsometricVector Pos = IsometricVector.None;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Name, Pos.Encypt);

    public IsometricDataTeleport()
    {
        this.Name = "";
        this.Pos = IsometricVector.None;
    }

    public IsometricDataTeleport(string Name, IsometricVector Value)
    {
        this.Name = Name;
        this.Pos = Value;
    }

    public static IsometricDataTeleport GetDencypt(string Value)
    {
        if (Value == "")
            return null;
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataTeleport(DataString[0], IsometricVector.GetDencypt(DataString[1]));
    }

    public bool DataExist => Name == null ? false : Name == "" ? false : true;
}