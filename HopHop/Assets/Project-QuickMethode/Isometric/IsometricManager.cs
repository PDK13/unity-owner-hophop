using QuickMethode;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IsometricManager;

public class IsometricManager : MonoBehaviour
{
    #region Varible: Game Config

    [SerializeField] private IsometricConfig m_config;

    public IsometricConfig Config => m_config;

    #endregion

    #region Varible: World Manager

    public IsometricGameData GameData;
    public IsometricDataWorld WorldData;
    public IsometricDataList BlockList;

    #endregion

    public void SetInit()
    {
        GameData = new IsometricGameData();
        WorldData = new IsometricDataWorld(this);
        BlockList = new IsometricDataList();
    }

    #region ======================================================================== File

    #region Fild Write

    public void SetFileWrite(QPath.PathType PathType, params string[] PathChild)
    {
        QFileIO FileIO = new QFileIO();

        SetFileWrite(FileIO);

        FileIO.SetWriteStart(QPath.GetPath(PathType, PathChild));
    }

    private void SetFileWrite(QFileIO FileIO)
    {
        WorldData.SetWorldOrder();
        //
        List<IsometricDataBlock> WorldBlocks = new List<IsometricDataBlock>();
        for (int i = 0; i < WorldData.m_worldPosH.Count; i++)
            for (int j = 0; j < WorldData.m_worldPosH[i].Block.Count; j++)
                WorldBlocks.Add(new IsometricDataBlock(WorldData.m_worldPosH[i].Block[j].PosPrimary, WorldData.m_worldPosH[i].Block[j].Name, WorldData.m_worldPosH[i].Block[j].Data));
        //
        FileIO.SetWriteAdd("[WORLD NAME]");
        FileIO.SetWriteAdd((GameData.Name != "") ? GameData.Name : "...");
        //
        FileIO.SetWriteAdd("[WORLD BLOCK]");
        FileIO.SetWriteAdd(WorldBlocks.Count);
        for (int BlockIndex = 0; BlockIndex < WorldBlocks.Count; BlockIndex++)
        {
            FileIO.SetWriteAdd("---------------------------------------");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].PosPrimary.Encypt);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Name);
            //
            FileIO.SetWriteAdd("<MOVE DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Type);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.MoveData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Data[DataIndex].Encypt);
            //
            FileIO.SetWriteAdd("<FOLLOW DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.FollowData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.FollowData.KeyFollow);
            //
            FileIO.SetWriteAdd("<ACTION DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Type);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.ActionData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.ActionData.Data[DataIndex].Encypt);
            //
            FileIO.SetWriteAdd("<EVENT DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.EventData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData.Data[DataIndex].Encypt);
            //
            FileIO.SetWriteAdd("<TELEPORT DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData.Key);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData.Data.Count);
            for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.TeleportData.Data.Count; DataIndex++)
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData.Data[DataIndex].Encypt);
        }
    }

    #endregion

    #region File Read

    public void SetFileRead(QPath.PathType PathType, params string[] PathChild)
    {
        QFileIO FileIO = new QFileIO();

        FileIO.SetReadStart(QPath.GetPath(PathType, PathChild));

        SetFileRead(FileIO);
    }

    public void SetFileRead(TextAsset WorldFile)
    {
        QFileIO FileIO = new QFileIO();

        FileIO.SetReadStart(WorldFile);

        SetFileRead(FileIO);
    }

    private void SetFileRead(QFileIO FileIO)
    {
        WorldData.SetWorldRemove(true);
        //
        FileIO.GetReadAuto();
        GameData.Name = FileIO.GetReadAutoString();
        //
        FileIO.GetReadAuto();
        int BlockCount = FileIO.GetReadAutoInt();
        for (int BlockIndex = 0; BlockIndex < BlockCount; BlockIndex++)
        {
            FileIO.GetReadAuto();
            IsometricVector PosPrimary = IsometricVector.GetDencypt(FileIO.GetReadAutoString());
            string Name = FileIO.GetReadAutoString();
            //
            IsometricDataBlockSingle Data = new IsometricDataBlockSingle();
            //
            FileIO.GetReadAuto();
            Data.MoveData = new IsometricDataBlockMove();
            Data.MoveData.Key = FileIO.GetReadAutoString();
            Data.MoveData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
            Data.MoveData.SetDataNew();
            int MoveCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < MoveCount; DataIndex++)
                Data.MoveData.SetDataAdd(IsometricDataBlockMoveSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            FileIO.GetReadAuto();
            Data.FollowData.Key = FileIO.GetReadAutoString();
            Data.FollowData.KeyFollow = FileIO.GetReadAutoString();

            FileIO.GetReadAuto();
            Data.ActionData = new IsometricDataBlockAction();
            Data.ActionData.Key = FileIO.GetReadAutoString();
            Data.ActionData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
            Data.ActionData.SetDataNew();
            int ActionCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < ActionCount; DataIndex++)
                Data.ActionData.SetDataAdd(IsometricDataBlockActionSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            FileIO.GetReadAuto();
            Data.EventData = new IsometricDataBlockEvent();
            Data.EventData.Key = FileIO.GetReadAutoString();
            Data.EventData.Data = new List<IsometricDataBlockEventSingle>();
            int EventCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < EventCount; DataIndex++)
                Data.EventData.SetDataAdd(IsometricDataBlockEventSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            FileIO.GetReadAuto();
            Data.TeleportData = new IsometricDataBlockTeleport();
            Data.TeleportData.Key = FileIO.GetReadAutoString();
            Data.TeleportData.Data = new List<IsometricDataBlockTeleportSingle>();
            int TeleportCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < TeleportCount; DataIndex++)
                Data.TeleportData.SetDataAdd(IsometricDataBlockTeleportSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            WorldData.SetBlockCreate(PosPrimary, BlockList.GetList(Name), Data);
        }
        //
        WorldData.onCreate?.Invoke();
    }

    #endregion

    #endregion
}

[Serializable]
public class IsometricGameData
{
    public string Name = "";
    public IsometricGameDataScene Scene = new IsometricGameDataScene();
}

[Serializable]
public class IsometricGameDataScene
{
    public IsometricRendererType Renderer = IsometricRendererType.H;
    public IsometricRotateType Rotate = IsometricRotateType._0;
    public IsometricVector Centre = new IsometricVector();
    public IsometricVector Scale = new IsometricVector(1f, 1f, 1f);
}

public enum IsometricRendererType { XY, H, None, }

public enum IsometricRotateType { _0, _90, _180, _270, }