using QuickMethode;
using System;
using System.Collections.Generic;

[Serializable]
public class IsoDataBlockMove
{
    public string KeyStart = "Move-Start";
    public string KeyEnd = "Move-End";
<<<<<<< Updated upstream
    public List<IsoDataBlockMoveSingle> Data;
=======
    public bool Loop = false;
    public List<IsoDataBlockMoveSingle> Data = new List<IsoDataBlockMoveSingle>();
    [HideInInspector] public int Index = 0;
>>>>>>> Stashed changes
}

[Serializable]
public struct IsoDataBlockMoveSingle
{
    public const char KEY_VALUE_ENCYPT = '|';

    public IsoDir Dir;
    public int Length;

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