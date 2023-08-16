using System;
using System.Collections.Generic;

public class IsometricDataBlock
{
    public IsometricVector PosPrimary;
    public string Name;
    public IsometricDataBlockSingle Data = new IsometricDataBlockSingle();

    public IsometricDataBlock(IsometricVector Pos, string Name, IsometricDataBlockSingle Data)
    {
        this.PosPrimary = Pos;
        this.Name = Name;
        this.Data = Data;
    }
}

[Serializable]
public class IsometricDataBlockSingle
{
    public IsometricDataBlockMove MoveData = new IsometricDataBlockMove();
    public IsometricDataFollow FollowData = new IsometricDataFollow();
    public IsometricDataBlockAction ActionData = new IsometricDataBlockAction();
    public IsometricDataBlockEvent EventData = new IsometricDataBlockEvent();
    public IsometricDataBlockTeleport TeleportData = new IsometricDataBlockTeleport();
}

public enum DataBlockType
{
    Forward = 0,
    Loop = 1,
    Revert = 2,
}