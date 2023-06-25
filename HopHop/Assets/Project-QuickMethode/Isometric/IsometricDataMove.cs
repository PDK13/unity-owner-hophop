using QuickMethode;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsoDataBlockMove
{
    public string Name = "Move";
    public bool Loop = false;
    public List<IsoDataBlockMoveSingle> Data = new List<IsoDataBlockMoveSingle>();
    [HideInInspector] public int Index = 0;
    [HideInInspector] public int Dir = 1;
}

[Serializable]
public class IsoDataBlockMoveSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public IsoDir Dir = IsoDir.None;
    public int Value = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, (int)Dir, Value);

    public IsoDataBlockMoveSingle(IsoDir Dir, int Value)
    {
        this.Dir = Dir;
        this.Value = Value;
    }

    public static IsoDataBlockMoveSingle GetDencypt(string Value)
    {
        List<int> DataString = QEncypt.GetDencyptInt(KEY_VALUE_ENCYPT, Value);
        return new IsoDataBlockMoveSingle((IsoDir)DataString[0], DataString[1]);
    }
}