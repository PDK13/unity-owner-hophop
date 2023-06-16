using QuickMethode;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsoDataBlockMove
{
    public string KeyStart = "Move-Start";
    public string KeyEnd = "Move-End";
    public bool Loop = false;
    public List<IsoDataBlockMoveSingle> Data;
    [HideInInspector] public int Index = 0;
}

[Serializable]
public class IsoDataBlockMoveSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public IsoDir Dir = IsoDir.None;
    public int Length = 1;

    public string Encypt => QEncypt.GetEncypt(KEY_VALUE_ENCYPT, (int)Dir, Length);

    public IsoDataBlockMoveSingle(IsoDir Dir, int Length)
    {
        this.Dir = Dir;
        this.Length = Length;
    }

    public static IsoDataBlockMoveSingle GetDencypt(string Value)
    {
        List<int> DataString = QEncypt.GetDencyptInt(KEY_VALUE_ENCYPT, Value);
        return new IsoDataBlockMoveSingle((IsoDir)DataString[0], DataString[1]);
    }
}