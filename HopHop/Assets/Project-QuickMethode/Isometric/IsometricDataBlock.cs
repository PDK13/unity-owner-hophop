using System;
using System.Collections.Generic;

public class IsoDataBlock
{
    public IsoVector PosPrimary;
    public string Name;
    public IsoDataBlockSingle Data;

    public IsoDataBlock(IsoVector Pos, string Name, IsoDataBlockSingle Data)
    {
        this.PosPrimary = Pos;
        this.Name = Name;
        this.Data = Data;
    }
}

[Serializable]
public class IsoDataBlockSingle
{
    public List<IsoDataBlockMove> MoveData;
    public List<IsoDataBlockEvent> EventData;
    public List<IsoDataBlockTeleport> TeleportData;
}