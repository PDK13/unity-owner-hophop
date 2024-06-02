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

    public string Encypt => QString.GetSplit(KEY_VALUE_ENCYPT, Name, Pos.Encypt);

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

    public static IsometricDataTeleport GetUnSplit(string Value)
    {
        if (Value == "")
        {
            return null;
        }
        //
        List<string> DataString = QString.GetUnSplitString(KEY_VALUE_ENCYPT, Value);
        return new IsometricDataTeleport(DataString[0], IsometricVector.GetUnSplit(DataString[1]));
    }

    public bool DataExist => Name == null ? false : Name == "" ? false : true;
}