using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IsometricDataFile
{
    private const string KEY_WORLD_NAME = "#WORLD-NAME";
    private const string KEY_WORLD_COMMAND = "#WORLD-COMMAND";

    //WORLD BLOCK
    private const string KEY_WORLD_BLOCK = "#WORLD-BLOCK";
    private const string KEY_WORLD_BLOCK_END = "#WORLD-BLOCK-END";
    //WORLD BLOCK

    private const string KEY_WORLD_END = "#WORLD-END";

    //BLOCK
    private const string KEY_BLOCK_INIT = "#BLOCK-INIT";
    private const string KEY_BLOCK_MOVE = "#BLOCK-MOVE";
    private const string KEY_BLOCK_ACTION = "#BLOCK-ACTION";
    private const string KEY_BLOCK_TELEPORT = "#BLOCK-TELEPORT";
    private const string KEY_BLOCK_END = "#BLOCK-END";
    //BLOCK

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
                WorldBlocks.Add(new IsometricDataFileBlock(Manager.World.Current.PosH[i].Block[j]));
        }
        //
        //===============================NAME!!
        FileIO.SetWriteAdd(KEY_WORLD_NAME);
        FileIO.SetWriteAdd((!string.IsNullOrEmpty(Manager.World.Current.Name)) ? Manager.World.Current.Name : "...");
        //===============================NAME!!
        //
        //===============================COMMAND!!
        FileIO.SetWriteAdd();
        FileIO.SetWriteAdd(KEY_WORLD_COMMAND);
        FileIO.SetWriteAdd(Manager.World.Current.Command.Count);
        for (int i = 0; i < Manager.World.Current.Command.Count; i++)
        {
            FileIO.SetWriteAdd(Manager.World.Current.Command[i]);
        }
        //===============================COMMAND!!
        //
        //===============================BLOCK!!
        FileIO.SetWriteAdd();
        FileIO.SetWriteAdd(KEY_WORLD_BLOCK);
        //
        //BLOCK START!!
        for (int BlockIndex = 0; BlockIndex < WorldBlocks.Count; BlockIndex++)
        {
            FileIO.SetWriteAdd();
            //
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].PosPrimary.Encypt);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Name);
            //
            //BLOCK START!!
            //
            if (WorldBlocks[BlockIndex].Init != null)
            {
                if (WorldBlocks[BlockIndex].Init.DataExist)
                {
                    FileIO.SetWriteAdd(KEY_BLOCK_INIT);
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Init.Data.Count);
                    for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Init.Data.Count; DataIndex++)
                        FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Init.Data[DataIndex]);
                }
            }
            //
            if (WorldBlocks[BlockIndex].Move != null)
            {
                if (WorldBlocks[BlockIndex].Move.Data.Count > 0)
                {
                    FileIO.SetWriteAdd(KEY_BLOCK_MOVE);
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Move.Type);
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Move.Data.Count);
                    for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Move.Data.Count; DataIndex++)
                        FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Move.Data[DataIndex].Encypt);
                }
            }
            //
            if (WorldBlocks[BlockIndex].Action != null)
            {
                if (WorldBlocks[BlockIndex].Action.Data.Count > 0)
                {
                    FileIO.SetWriteAdd(KEY_BLOCK_ACTION);
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Action.Type);
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Action.Data.Count);
                    for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Action.Data.Count; DataIndex++)
                        FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Action.Data[DataIndex].Encypt);
                }
            }
            //
            if (WorldBlocks[BlockIndex].Teleport != null)
            {
                if (WorldBlocks[BlockIndex].Teleport.DataExist)
                {
                    FileIO.SetWriteAdd(KEY_BLOCK_TELEPORT);
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Teleport.Encypt);
                }
            }
            //
            //...
            //
            //BLOCK END!!
            //
            FileIO.SetWriteAdd(KEY_BLOCK_END);
        }
        FileIO.SetWriteAdd(KEY_WORLD_BLOCK_END);
        //===============================BLOCK!!
        //
        //===============================END!!
        FileIO.SetWriteAdd();
        FileIO.SetWriteAdd(KEY_WORLD_END);
        //===============================END!!
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
                    //NOTE: If world name read equas to world name exist in manager, the exist world will be overide!
                    Manager.World.Current = Manager.World.SetGenerate(MapName);
                    Manager.World.SetActive(MapName);
                    Manager.World.Current.SetWorldRemove(true);
                    break;
                case KEY_WORLD_COMMAND:
                    int CommandCount = FileIO.GetReadAutoInt();
                    //
                    Manager.World.Current.Command = new List<string>();
                    for (int CommandIndex = 0; CommandIndex < CommandCount; CommandIndex++)
                        Manager.World.Current.Command.Add(FileIO.GetReadAutoString());
                    break;
                case KEY_WORLD_BLOCK:
                    //WORLD BLOCK START!!
                    while (FileIO.GetReadAutoString() != KEY_WORLD_BLOCK_END)
                    {
                        IsometricVector PosPrimary = IsometricVector.GetDencypt(FileIO.GetReadAutoString());
                        string Name = FileIO.GetReadAutoString();

                        //BLOCK START!!

                        var Block = Manager.World.Current.SetBlockCreate(PosPrimary, Manager.List.GetList(Name), false);

                        if (Block == null)
                        {
                            Debug.LogWarningFormat("Block '{0}' created not found", Name);
                            continue;
                        }

                        bool EndGroupBlock = false;

                        do
                        {
                            switch (FileIO.GetReadAutoString())
                            {
                                case KEY_BLOCK_INIT:
                                    {
                                        IsometricDataInit BlockData = Block.GetComponent<IsometricDataInit>() ?? Block.AddComponent<IsometricDataInit>();

                                        BlockData.Data = new List<string>();
                                        int InitCount = FileIO.GetReadAutoInt();
                                        for (int DataIndex = 0; DataIndex < InitCount; DataIndex++)
                                            BlockData.Data.Add(FileIO.GetReadAutoString());
                                    }
                                    break;
                                case KEY_BLOCK_MOVE:
                                    {
                                        IsometricDataMove BlockData = Block.GetComponent<IsometricDataMove>() ?? Block.AddComponent<IsometricDataMove>();

                                        BlockData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
                                        BlockData.SetDataNew();
                                        int MoveCount = FileIO.GetReadAutoInt();
                                        for (int DataIndex = 0; DataIndex < MoveCount; DataIndex++)
                                            BlockData.SetDataAdd(IsometricDataBlockMoveSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    }
                                    break;
                                case KEY_BLOCK_ACTION:
                                    {
                                        IsometricDataAction BlockData = Block.GetComponent<IsometricDataAction>() ?? Block.AddComponent<IsometricDataAction>();

                                        BlockData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
                                        BlockData.SetDataNew();
                                        int ActionCount = FileIO.GetReadAutoInt();
                                        for (int DataIndex = 0; DataIndex < ActionCount; DataIndex++)
                                            BlockData.SetDataAdd(IsometricDataBlockActionSingle.GetDencypt(FileIO.GetReadAutoString()));
                                    }
                                    break;
                                case KEY_BLOCK_TELEPORT:
                                    {
                                        IsometricDataTeleport BlockData = Block.GetComponent<IsometricDataTeleport>() ?? Block.AddComponent<IsometricDataTeleport>();

                                        BlockData.SetValue(IsometricDataTeleport.GetDencypt(FileIO.GetReadAutoString()));
                                    }
                                    break;
                                case KEY_BLOCK_END:

                                    //BLOCK END!!

                                    EndGroupBlock = true;
                                    break;
                            }
                        }
                        while (!EndGroupBlock);

                        //BLOCK END!!
                    }
                    //WORLD BLOCK END!!
                    break;
                case KEY_WORLD_END:

                    //WORLD END!!

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
    //
    public IsometricDataInit Init;
    public IsometricDataMove Move;
    public IsometricDataAction Action;
    public IsometricDataTeleport Teleport;

    public IsometricDataFileBlock(IsometricBlock Block)
    {
        PosPrimary = Block.Pos;
        this.Name = Block.Name;
        //
        this.Init = Block.GetComponent<IsometricDataInit>();
        this.Move = Block.GetComponent<IsometricDataMove>();
        this.Action = Block.GetComponent<IsometricDataAction>();
        this.Teleport = Block.GetComponent<IsometricDataTeleport>();
    }
}