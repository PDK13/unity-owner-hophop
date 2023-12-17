using System;
using System.Collections.Generic;
using UnityEngine;

public class IsometricDataFile
{
    private const string KEY_WORLD_NAME = "#WORLD-NAME";
    private const string KEY_WORLD_COMMAND = "#WORLD-COMMAND";
    private const string KEY_WORLD_BLOCK = "#WORLD-BLOCK";
    private const string KEY_WORLD_END = "#WORLD-END";

    private const string KEY_BLOCK_INIT = "#BLOCK-INIT";
    private const string KEY_BLOCK_MOVE = "#BLOCK-MOVE";
    private const string KEY_BLOCK_FOLLOW = "#BLOCK-FOLLOW";
    private const string KEY_BLOCK_FOLLOW_GET = "#BLOCK-FOLLOW_GET";
    private const string KEY_BLOCK_ACTION = "#BLOCK-ACTION";
    private const string KEY_BLOCK_EVENT = "#BLOCK-EVENT";
    private const string KEY_BLOCK_EVENT_GET = "#BLOCK-EVENT_GET";
    private const string KEY_BLOCK_TELEPORT = "#BLOCK-TELEPORT";
    private const string KEY_BLOCK_END = "#BLOCK-END";

    #region Fild Write

    public static void SetFileWrite(IsometricManager Manager, string Path)
    {
        QDataFile FileIO = new QDataFile();

        SetFileWrite(Manager, FileIO);

        FileIO.SetWriteStart(Path);
    }

    private static void SetFileWrite(IsometricManager Manager, QDataFile FileIO)
    {
        Manager.World.Current.SetWorldOrder();
        //
        List<IsometricDataFileBlock> WorldBlocks = new List<IsometricDataFileBlock>();
        for (int i = 0; i < Manager.World.Current.PosH.Count; i++)
        {
            for (int j = 0; j < Manager.World.Current.PosH[i].Block.Count; j++)
            {
                WorldBlocks.Add(new IsometricDataFileBlock(Manager.World.Current.PosH[i].Block[j].PosPrimary, Manager.World.Current.PosH[i].Block[j].Name, (Manager.World.Current.PosH[i].Block[j].Data)));
            }
        }
        //
        //WORLD START!!
        //
        FileIO.SetWriteAdd(KEY_WORLD_NAME);
        FileIO.SetWriteAdd((Manager.World.Current.Name != "") ? Manager.World.Current.Name : "...");
        //
        FileIO.SetWriteAdd();
        //
        FileIO.SetWriteAdd(KEY_WORLD_COMMAND);
        FileIO.SetWriteAdd(Manager.World.Current.Command.Count);
        for (int i = 0; i < Manager.World.Current.Command.Count; i++)
        {
            FileIO.SetWriteAdd(Manager.World.Current.Command[i]);
        }
        //
        FileIO.SetWriteAdd();
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
            if (WorldBlocks[BlockIndex].Data.Init.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_INIT);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Init.Data.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Init.Data.Count; DataIndex++)
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Init.Data[DataIndex]);
                }
            }
            //
            if (WorldBlocks[BlockIndex].Data.Move.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_MOVE);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Move.Type);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Move.Data.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Move.Data.Count; DataIndex++)
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Move.Data[DataIndex].Encypt);
                }
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
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Action.Data[DataIndex].Encypt);
                }
            }
            //
            if (WorldBlocks[BlockIndex].Data.Event.DataExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_EVENT);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.Identity.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Event.Identity.Count; DataIndex++)
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.Identity[DataIndex]);
                }
            }
            if (WorldBlocks[BlockIndex].Data.Event.DataGetExist)
            {
                FileIO.SetWriteAdd(KEY_BLOCK_EVENT_GET);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.IdentityGet.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.Event.IdentityGet.Count; DataIndex++)
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.Event.IdentityGet[DataIndex]);
                }
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
        QDataFile FileIO = new QDataFile();

        FileIO.SetReadStart(Path);

        SetFileRead(Manager, FileIO);
    }

    public static void SetFileRead(IsometricManager Manager, TextAsset WorldFile)
    {
        QDataFile FileIO = new QDataFile();

        FileIO.SetReadStart(WorldFile);

        SetFileRead(Manager, FileIO);
    }

    private static void SetFileRead(IsometricManager Manager, QDataFile FileIO)
    {
        //
        //WORLD START!!
        //
        bool EndGroupWorld = false;
        do
        {
            switch (FileIO.GetReadAutoString())
            {
                case KEY_WORLD_NAME:
                    string MapName = FileIO.GetReadAutoString();
                    if (Manager.World.Current.Name == MapName)
                        Manager.World.Current.SetWorldRemove(true);
                    Manager.World.SetGenerate(MapName);
                    Manager.World.SetActive(MapName);
                    break;
                case KEY_WORLD_COMMAND:
                    int CommandCount = FileIO.GetReadAutoInt();
                    //
                    Manager.World.Current.Command = new List<string>();
                    for (int CommandIndex = 0; CommandIndex < CommandCount; CommandIndex++)
                    {
                        Manager.World.Current.Command.Add(FileIO.GetReadAutoString());
                    }
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
                                case KEY_BLOCK_INIT:
                                    Data.Init.Data = new List<string>();
                                    int InitCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < InitCount; DataIndex++)
                                    {
                                        Data.Init.Data.Add(FileIO.GetReadAutoString());
                                    }
                                    break;
                                case KEY_BLOCK_MOVE:
                                    Data.Move = new IsometricDataMove
                                    {
                                        Type = FileIO.GetReadAutoEnum<DataBlockType>()
                                    };
                                    Data.Move.SetDataNew();
                                    int MoveCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < MoveCount; DataIndex++)
                                    {
                                        Data.Move.SetDataAdd(IsometricDataBlockMoveSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    }
                                    break;
                                case KEY_BLOCK_FOLLOW:
                                    Data.Follow.Identity = FileIO.GetReadAutoString();
                                    break;
                                case KEY_BLOCK_FOLLOW_GET:
                                    Data.Follow.IdentityGet = FileIO.GetReadAutoString();
                                    break;
                                case KEY_BLOCK_ACTION:
                                    Data.Action = new IsometricDataAction
                                    {
                                        Type = FileIO.GetReadAutoEnum<DataBlockType>()
                                    };
                                    Data.Action.SetDataNew();
                                    int ActionCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < ActionCount; DataIndex++)
                                    {
                                        Data.Action.SetDataAdd(IsometricDataBlockActionSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    }
                                    break;
                                case KEY_BLOCK_EVENT:
                                    Data.Event.Identity = new List<string>();
                                    int EventSetCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < EventSetCount; DataIndex++)
                                    {
                                        Data.Event.Identity.Add(FileIO.GetReadAutoString());
                                    }
                                    break;
                                case KEY_BLOCK_EVENT_GET:
                                    Data.Event.IdentityGet = new List<string>();
                                    int EventGetCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < EventGetCount; DataIndex++)
                                    {
                                        Data.Event.IdentityGet.Add(FileIO.GetReadAutoString());
                                    }
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
                        Manager.World.Current.SetBlockCreate(PosPrimary, Manager.List.GetList(Name), Data);
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
        Manager.World.Current.onCreate?.Invoke();
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
        PosPrimary = Pos;
        this.Name = Name;
        this.Data = Data;
    }
}

[Serializable]
public class IsometricDataFileBlockData
{
    public IsometricDataInit Init = new IsometricDataInit();
    public IsometricDataMove Move = new IsometricDataMove();
    public IsometricDataFollow Follow = new IsometricDataFollow();
    public IsometricDataAction Action = new IsometricDataAction();
    public IsometricDataEvent Event = new IsometricDataEvent();
    public IsometricDataTeleport Teleport = new IsometricDataTeleport();
}