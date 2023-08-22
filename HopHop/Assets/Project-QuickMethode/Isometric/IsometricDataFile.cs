using QuickMethode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricDataFile
{
    private const string KEY_WORLD_NAME         = "<1> WORLD-NAME";
    private const string KEY_WORLD_BLOCK        = "<2> WORLD-BLOCK";
    private const string KEY_WORLD_END          = "<!> WORLD";

    private const string KEY_BLOCK_MOVE         = "<2.1> BLOCK-MOVE";
    private const string KEY_BLOCK_FOLLOW       = "<2.2> BLOCK-FOLLOW";
    private const string KEY_BLOCK_FOLLOW_GET   = "<2.2> BLOCK-FOLLOW_GET";
    private const string KEY_BLOCK_ACTION       = "<2.3> BLOCK-ACTION";
    private const string KEY_BLOCK_EVENT        = "<2.4> BLOCK-EVENT";
    private const string KEY_BLOCK_EVENT_GET    = "<2.4> BLOCK-EVENT_GET";
    private const string KEY_BLOCK_TELEPORT     = "<2.5> BLOCK-TELEPORT";
    private const string KEY_BLOCK_END          = "<2.!> BLOCK";

    #region Fild Write

    public static void SetFileWrite(IsometricManager Manager, string Path)
    {
        QFileIO FileIO = new QFileIO();

        SetFileWrite(Manager, FileIO);

        FileIO.SetWriteStart(Path);
    }

    private static void SetFileWrite(IsometricManager Manager, QFileIO FileIO)
    {
        Manager.World.SetWorldOrder();
        //
        List<IsometricDataFileBlock> WorldBlocks = new List<IsometricDataFileBlock>();
        for (int i = 0; i < Manager.World.m_worldPosH.Count; i++)
            for (int j = 0; j < Manager.World.m_worldPosH[i].Block.Count; j++)
                WorldBlocks.Add(new IsometricDataFileBlock(Manager.World.m_worldPosH[i].Block[j].PosPrimary, Manager.World.m_worldPosH[i].Block[j].Name, (Manager.World.m_worldPosH[i].Block[j].Data)));
        //
        //WORLD START!!
        //
        FileIO.SetWriteAdd(KEY_WORLD_NAME);
        FileIO.SetWriteAdd((Manager.Game.Name != "") ? Manager.Game.Name : "...");
        //
        FileIO.SetWriteAdd(KEY_WORLD_BLOCK);
        FileIO.SetWriteAdd(WorldBlocks.Count);
        //
        for (int BlockIndex = 0; BlockIndex < WorldBlocks.Count; BlockIndex++)
        {
            FileIO.SetWriteAdd();
            //
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].PosPrimary.Encypt);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Name);
            //
            //BLOCK START!!
            //
            if (WorldBlocks[BlockIndex].Data.Move.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_MOVE);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Move.Type);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Move.Data.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Move.Data.Count; DataIndex++)
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Move.Data[DataIndex].Encypt);
            }
            //
            if (WorldBlocks[BlockIndex].Data.Follow.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_FOLLOW);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Follow.Identity);
            }
            if (WorldBlocks[BlockIndex].Data.Follow.DataGetExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_FOLLOW_GET);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Follow.IdentityGet);
            }
            //
            if (WorldBlocks[BlockIndex].Data.Action.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_ACTION);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Action.Type);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Action.Data.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Action.Data.Count; DataIndex++)
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Action.Data[DataIndex].Encypt);
            }
            //
            if (WorldBlocks[BlockIndex].Data.Event.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_EVENT);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.Identity.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Event.Identity.Count; DataIndex++)
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.Identity[DataIndex]);
            }
            if (WorldBlocks[BlockIndex].Data.Event.DataGetExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_EVENT_GET);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.IdentityGet.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Event.IdentityGet.Count; DataIndex++)
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.IdentityGet[DataIndex]);
            }
            //
            if (WorldBlocks[BlockIndex].Data.Teleport.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_TELEPORT);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Teleport.Encypt);
            }
            //
            //...
            //
            //BLOCK END!!
            //
            FileIO.SetWriteAdd(KEY_BLOCK_END);
        }
        //
        //WORLD END!!
        //
        FileIO.SetWriteAdd(KEY_WORLD_END);
    }

    #endregion

    #region File Read

    public static void SetFileRead(IsometricManager Manager, string Path)
    {
        QFileIO FileIO = new QFileIO();

        FileIO.SetReadStart(Path);

        SetFileRead(Manager, FileIO);
    }

    public static void SetFileRead(IsometricManager Manager, TextAsset WorldFile)
    {
        QFileIO FileIO = new QFileIO();

        FileIO.SetReadStart(WorldFile);

        SetFileRead(Manager, FileIO);
    }

    private static void SetFileRead(IsometricManager Manager, QFileIO FileIO)
    {
        Manager.World.SetWorldRemove(true);
        //
        //WORLD START!!
        //
        bool EndGroupWorld = false;
        do
        {
            switch (FileIO.GetReadAutoString())
            {
                case KEY_WORLD_NAME:
                    Manager.Game.Name = FileIO.GetReadAutoString();
                    break;
                case KEY_WORLD_BLOCK:
                    int BlockCount = FileIO.GetReadAutoInt();
                    //
                    for (int BlockIndex = 0; BlockIndex < BlockCount; BlockIndex++)
                    {
                        FileIO.GetReadAuto();
                        //
                        IsometricVector PosPrimary = IsometricVector.GetDencypt(FileIO.GetReadAutoString());
                        string Name = FileIO.GetReadAutoString();
                        //
                        //BLOCK START!!
                        //
                        IsometricDataFileBlockData Data = new IsometricDataFileBlockData();
                        //
                        bool EndGroupBlock = false;
                        //
                        do
                        {
                            switch (FileIO.GetReadAutoString())
                            {
                                case KEY_BLOCK_MOVE:
                                    Data.Move = new IsometricDataMove();
                                    Data.Move.Type = FileIO.GetReadAutoEnum<DataBlockType>();
                                    Data.Move.SetDataNew();
                                    int MoveCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < MoveCount; DataIndex++)
                                        Data.Move.SetDataAdd(IsometricDataBlockMoveSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    break;
                                case KEY_BLOCK_FOLLOW:
                                    Data.Follow.Identity = FileIO.GetReadAutoString();
                                    break;
                                case KEY_BLOCK_FOLLOW_GET:
                                    Data.Follow.IdentityGet = FileIO.GetReadAutoString();
                                    break;
                                case KEY_BLOCK_ACTION:
                                    Data.Action = new IsometricDataAction();
                                    Data.Action.Type = FileIO.GetReadAutoEnum<DataBlockType>();
                                    Data.Action.SetDataNew();
                                    int ActionCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < ActionCount; DataIndex++)
                                        Data.Action.SetDataAdd(IsometricDataBlockActionSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    break;
                                case KEY_BLOCK_EVENT:
                                    Data.Event.Identity = new List<string>();
                                    int EventSetCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < EventSetCount; DataIndex++)
                                        Data.Event.Identity.Add(FileIO.GetReadAutoString());
                                    break;
                                case KEY_BLOCK_EVENT_GET:
                                    Data.Event.IdentityGet = new List<string>();
                                    int EventGetCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < EventGetCount; DataIndex++)
                                        Data.Event.IdentityGet.Add(FileIO.GetReadAutoString());
                                    break;
                                case KEY_BLOCK_TELEPORT:
                                    Data.Teleport = IsometricDataTeleport.GetDencypt(FileIO.GetReadAutoString());
                                    break;
                                case KEY_BLOCK_END:
                                    //
                                    //BLOCK END!!
                                    //
                                    EndGroupBlock = true;
                                    break;
                            }
                        }
                        while (!EndGroupBlock);
                        //
                        //BLOCK END!!
                        //
                        Manager.World.SetBlockCreate(PosPrimary, Manager.List.GetList(Name), Data);
                    }
                    break;
                case KEY_WORLD_END:
                    //
                    //WORLD END!!
                    //
                    EndGroupWorld = true;
                    break;
            }
        }
        while (!EndGroupWorld);
        //
        //WORLD END!!
        //
        Manager.World.onCreate?.Invoke();
    }

    #endregion
}

public class IsometricDataFileBlock
{
    public IsometricVector PosPrimary;
    public string Name;
    public IsometricDataFileBlockData Data = new IsometricDataFileBlockData();

    public IsometricDataFileBlock(IsometricVector Pos, string Name, IsometricDataFileBlockData Data)
    {
        this.PosPrimary = Pos;
        this.Name = Name;
        this.Data = Data;
    }
}

[Serializable]
public class IsometricDataFileBlockData
{
    public IsometricDataMove Move = new IsometricDataMove();
    public IsometricDataFollow Follow = new IsometricDataFollow();
    public IsometricDataAction Action = new IsometricDataAction();
    public IsometricDataEvent Event = new IsometricDataEvent();
    public IsometricDataTeleport Teleport = new IsometricDataTeleport();
}