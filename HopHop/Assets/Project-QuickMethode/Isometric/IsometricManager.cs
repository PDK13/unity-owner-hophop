using QuickMethode;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsometricManager : MonoBehaviour
{
    #region Enum

    public enum RendererType { XY, H, None, }

    public enum RotateType { _0, _90, _180, _270, }

    #endregion

    #region Varible: Game Config

    [SerializeField] private IsometricConfig m_config;

    public IsometricConfig Config => m_config;

    #endregion

    #region Varible: World Manager

    [Space]
    [SerializeField] private string m_name = "";
    [SerializeField] private IsoDataScene m_scene = new IsoDataScene();

    public string WorldName => m_name;
    public IsoDataScene Scene => m_scene;

    public IsometricDataWorld WorldData;
    public IsometricDataList BlockList;

    #endregion

    public void SetInit()
    {
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
        List<IsoDataBlock> WorldBlocks = new List<IsoDataBlock>();
        for (int i = 0; i < WorldData.m_worldPosH.Count; i++)
            for (int j = 0; j < WorldData.m_worldPosH[i].Block.Count; j++)
                WorldBlocks.Add(new IsoDataBlock(WorldData.m_worldPosH[i].Block[j].PosPrimary, WorldData.m_worldPosH[i].Block[j].Name, WorldData.m_worldPosH[i].Block[j].Data));
        //
        FileIO.SetWriteAdd("[WORLD NAME]");
        FileIO.SetWriteAdd((WorldName != "") ? WorldName : "...");
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
        m_name = FileIO.GetReadAutoString();
        //
        FileIO.GetReadAuto();
        int BlockCount = FileIO.GetReadAutoInt();
        for (int BlockIndex = 0; BlockIndex < BlockCount; BlockIndex++)
        {
            FileIO.GetReadAuto();
            IsoVector PosPrimary = IsoVector.GetDencypt(FileIO.GetReadAutoString());
            string Name = FileIO.GetReadAutoString();
            //
            IsoDataBlockSingle Data = new IsoDataBlockSingle();
            //
            FileIO.GetReadAuto();
            Data.MoveData = new IsoDataBlockMove();
            Data.MoveData.Key = FileIO.GetReadAutoString();
            Data.MoveData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
            Data.MoveData.SetDataNew();
            int MoveCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < MoveCount; DataIndex++)
                Data.MoveData.SetDataAdd(IsoDataBlockMoveSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            FileIO.GetReadAuto();
            Data.FollowData.Key = FileIO.GetReadAutoString();
            Data.FollowData.KeyFollow = FileIO.GetReadAutoString();

            FileIO.GetReadAuto();
            Data.ActionData = new IsoDataBlockAction();
            Data.ActionData.Key = FileIO.GetReadAutoString();
            Data.ActionData.Type = FileIO.GetReadAutoEnum<DataBlockType>();
            Data.ActionData.SetDataNew();
            int ActionCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < ActionCount; DataIndex++)
                Data.ActionData.SetDataAdd(IsoDataBlockActionSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            FileIO.GetReadAuto();
            Data.EventData = new IsoDataBlockEvent();
            Data.EventData.Key = FileIO.GetReadAutoString();
            Data.EventData.Data = new List<IsoDataBlockEventSingle>();
            int EventCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < EventCount; DataIndex++)
                Data.EventData.SetDataAdd(IsoDataBlockEventSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            FileIO.GetReadAuto();
            Data.TeleportData = new IsoDataBlockTeleport();
            Data.TeleportData.Key = FileIO.GetReadAutoString();
            Data.TeleportData.Data = new List<IsoDataBlockTeleportSingle>();
            int TeleportCount = FileIO.GetReadAutoInt();
            for (int DataIndex = 0; DataIndex < TeleportCount; DataIndex++)
                Data.TeleportData.SetDataAdd(IsoDataBlockTeleportSingle.GetDencypt(FileIO.GetReadAutoString()));
            //
            WorldData.SetWorldBlockCreate(PosPrimary, BlockList.GetList(Name), Data);
        }
        //
        WorldData.onWorldCreate?.Invoke();
    }

    #endregion

    #endregion

    #region ======================================================================== Editor

    //Required "IsoBlockRenderer.cs" component for each Block!

    public bool SetEditorMask(IsoVector Pos, Color Mask, Color UnMask, Color Centre)
    {
        bool CentreFound = false;
        for (int i = 0; i < WorldData.m_worldPosH.Count; i++)
            for (int j = 0; j < WorldData.m_worldPosH[i].Block.Count; j++)
            {
                IsometricRenderer BlockSprite = WorldData.m_worldPosH[i].Block[j].GetComponent<IsometricRenderer>();
                if (BlockSprite == null)
                    continue;

                if (WorldData.m_worldPosH[i].Block[j].Pos == Pos)
                {
                    CentreFound = true;
                    BlockSprite.SetSpriteColor(Centre, 1f);
                }
                else
                if (WorldData.m_worldPosH[i].Block[j].Pos.X == Pos.X || WorldData.m_worldPosH[i].Block[j].Pos.Y == Pos.Y)
                    BlockSprite.SetSpriteColor(Mask, 1f);
                else
                    BlockSprite.SetSpriteColor(UnMask, 1f);
            }
        return CentreFound;
    }

    public void SetEditorHidden(int FromH, float UnMask)
    {
        for (int i = 0; i < WorldData.m_worldPosH.Count; i++)
            for (int j = 0; j < WorldData.m_worldPosH[i].Block.Count; j++)
            {
                IsometricRenderer BlockSprite = WorldData.m_worldPosH[i].Block[j].GetComponent<IsometricRenderer>();
                if (BlockSprite == null)
                    continue;

                if (WorldData.m_worldPosH[i].Block[j].Pos.H > FromH)
                    BlockSprite.SetSpriteAlpha(UnMask);
                else
                    BlockSprite.SetSpriteAlpha(1f);
            }
    }

    #endregion
}