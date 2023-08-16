using QuickMethode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricDataFile
{
    private const string KEY_WORLD_NAME     = "<1> WORLD-NAME";
    private const string KEY_WORLD_BLOCK    = "<2> WORLD-BLOCK";
    private const string KEY_WORLD_GROUP    = "<!> WORLD";

    private const string KEY_BLOCK_MOVE     = "<2.1> BLOCK-MOVE";
    private const string KEY_BLOCK_FOLLOW   = "<2.2> BLOCK-FOLLOW";
    private const string KEY_BLOCK_ACTION   = "<2.3> BLOCK-ACTION";
    private const string KEY_BLOCK_EVENT    = "<2.4> BLOCK-EVENT";
    private const string KEY_BLOCK_TELEPORT = "<2.5> BLOCK-TELEPORT";
    private const string KEY_BLOCK_GROUP    = "<2.!> BLOCK";

    #region Fild Write

    public static void SetFileWrite(IsometricManager Manager, QPath.PathType PathType, params string[] PathChild)
    {
        QFileIO FileIO = new QFileIO();

        SetFileWrite(Manager, FileIO);

        FileIO.SetWriteStart(QPath.GetPath(PathType, PathChild));
    }

    private static void SetFileWrite(IsometricManager Manager, QFileIO FileIO)
    {
        Manager.WorldData.SetWorldOrder();
        //
        List<IsometricDataBlock> WorldBlocks = new List<IsometricDataBlock>();
        for (int i = 0; i < Manager.WorldData.m_worldPosH.Count; i++)
            for (int j = 0; j < Manager.WorldData.m_worldPosH[i].Block.Count; j++)
                WorldBlocks.Add(new IsometricDataBlock(Manager.WorldData.m_worldPosH[i].Block[j].PosPrimary, Manager.WorldData.m_worldPosH[i].Block[j].Name, (Manager.WorldData.m_worldPosH[i].Block[j].Data)));
        //
        //WORLD START!!
        //
        FileIO.SetWriteAdd(KEY_WORLD_NAME);
        FileIO.SetWriteAdd((Manager.GameData.Name != "") ? Manager.GameData.Name : "...");
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
            FileIO.SetWriteAdd(KEY_BLOCK_MOVE);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Type);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.MoveData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Data[DataIndex].Encypt);
            //
            FileIO.SetWriteAdd(KEY_BLOCK_FOLLOW);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.FollowData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.FollowData.KeyFollow);
            //
            FileIO.SetWriteAdd(KEY_BLOCK_ACTION);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Type);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.ActionData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Data[DataIndex].Encypt);
            //
            FileIO.SetWriteAdd(KEY_BLOCK_EVENT);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.EventData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData.Data[DataIndex].Encypt);
            //
            FileIO.SetWriteAdd(KEY_BLOCK_TELEPORT);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.TeleportData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData.Data[DataIndex].Encypt);
            //
            //BLOCK END!!
            //
            FileIO.SetWriteAdd(KEY_BLOCK_GROUP);
        }
        //
        //WORLD END!!
        //
        FileIO.SetWriteAdd(KEY_WORLD_GROUP);
    }

    #endregion

    #region File Read

    public static void SetFileRead(IsometricManager Manager, QPath.PathType PathType, params string[] PathChild)
    {
        QFileIO FileIO = new QFileIO();

        FileIO.SetReadStart(QPath.GetPath(PathType, PathChild));

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
        Manager.WorldData.SetWorldRemove(true);
        //
        //WORLD START!!
        //
        bool EndGroupWorld = false;
        do
        {
            switch (FileIO.GetReadAutoString())
            {
                case KEY_WORLD_NAME:
                    Manager.GameData.Name = FileIO.GetReadAutoString();
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
                        IsometricDataBlockSingle Data = new IsometricDataBlockSingle();
                        //
                        bool EndGroupBlock = false;
                        //
                        do
                        {
                            switch (FileIO.GetReadAutoString())
                            {
                                case KEY_BLOCK_MOVE:
                                    Data.MoveData = new IsometricDataBlockMove();
                                    Data.MoveData.Key = FileIO.GetReadAutoString();
                                    Data.MoveData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
                                    Data.MoveData.SetDataNew();
                                    int MoveCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < MoveCount; DataIndex++)
                                        Data.MoveData.SetDataAdd(IsometricDataBlockMoveSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    break;
                                case KEY_BLOCK_FOLLOW:
                                    Data.FollowData.Key = FileIO.GetReadAutoString();
                                    Data.FollowData.KeyFollow = FileIO.GetReadAutoString();
                                    break;
                                case KEY_BLOCK_ACTION:
                                    Data.ActionData = new IsometricDataBlockAction();
                                    Data.ActionData.Key = FileIO.GetReadAutoString();
                                    Data.ActionData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
                                    Data.ActionData.SetDataNew();
                                    int ActionCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < ActionCount; DataIndex++)
                                        Data.ActionData.SetDataAdd(IsometricDataBlockActionSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    break;
                                case KEY_BLOCK_EVENT:
                                    Data.EventData = new IsometricDataBlockEvent();
                                    Data.EventData.Key = FileIO.GetReadAutoString();
                                    Data.EventData.Data = new List<IsometricDataBlockEventSingle>();
                                    int EventCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < EventCount; DataIndex++)
                                        Data.EventData.SetDataAdd(IsometricDataBlockEventSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    break;
                                case KEY_BLOCK_TELEPORT:
                                    Data.TeleportData = new IsometricDataBlockTeleport();
                                    Data.TeleportData.Key = FileIO.GetReadAutoString();
                                    Data.TeleportData.Data = new List<IsometricDataBlockTeleportSingle>();
                                    int TeleportCount = FileIO.GetReadAutoInt();
                                    for (int DataIndex = 0; DataIndex < TeleportCount; DataIndex++)
                                        Data.TeleportData.SetDataAdd(IsometricDataBlockTeleportSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    break;
                                case KEY_BLOCK_GROUP:
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
                        Manager.WorldData.SetBlockCreate(PosPrimary, Manager.BlockList.GetList(Name), Data);
                    }
                    break;
                case KEY_WORLD_GROUP:
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
        Manager.WorldData.onCreate?.Invoke();
    }

    #endregion
}