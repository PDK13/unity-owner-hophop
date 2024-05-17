using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricDataTeleport : MonoBehaviour
{
    public const char KEY_VALUE_ENCYPT = '|';

    public string Name = "";
    public IsometricVector Pos = IsometricVector.None;

    //

    public void SetValue(IsometricDataTeleport Value)
    {
        Name = Value.Name;
        Pos = Value.Pos;
    }

    //

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, Name, Pos.Encypt);

    public IsometricDataTeleport()
    {
        Name = "";
        Pos = IsometricVector.None;
    }

    public IsometricDataTeleport(string Name, IsometricVector Value)
    {
        this.Name = Name;
        Pos = Value;
    }

    public static IsometricDataTeleport GetDencypt(string Value)
    {
        if (Value == "")
        {
            return null;
        }
        //
        List<string> DataString = QEncypt.GetDencyptString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataTeleport(DataString[0], IsometricVector.GetDencypt(DataString[1]));
    }

    public bool DataExist => Name == null ? false : Name == "" ? false : true;
}