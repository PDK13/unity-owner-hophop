using System;
using System.Collections.Generic;

public class IsoDataBlock
{
    public IsoVector PosPrimary;
    public string Name;
    public IsoDataBlockSingle Data = new IsoDataBlockSingle();

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
    public IsoDataBlockMove MoveData = new IsoDataBlockMove();
    public IsometricDataFollow FollowData = new IsometricDataFollow();
    public IsoDataBlockAction ActionData = new IsoDataBlockAction();
    public IsoDataBlockEvent EventData = new IsoDataBlockEvent();
    public IsoDataBlockTeleport TeleportData = new IsoDataBlockTeleport();
}

public enum DataBlockType
{
    Forward = 0,
    Loop = 1,
    Revert = 2,
}