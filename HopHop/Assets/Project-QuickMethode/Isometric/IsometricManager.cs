using QuickMethode;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsometricManager : MonoBehaviour
{
    #region Event

    public Action onWorldCreate;
    public Action onWorldRemove;
    public Action onListRead;

    #endregion

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

    private List<(int PosH, List<IsometricBlock> Block)> m_worldPosH = new List<(int PosH, List<IsometricBlock> Block)>();

    private List<(string Tag, List<IsometricBlock> Block)> m_worldTag = new List<(string Tag, List<IsometricBlock> Block)>();

    #endregion

    #region Varible: Block Manager

    public List<(string Tag, List<IsometricBlock> Block)> BlockList = new List<(string Tag, List<IsometricBlock> Block)>();

    #endregion

    #region Varible: Editor

    public const string CURSON_NAME = "ISO-CURSON";

    #endregion

    #region ======================================================================== Block

    #region Block Create

    public IsometricBlock SetWorldBlockCreate(IsoVector Pos, GameObject BlockPrefab, IsoDataBlockSingle Data = null)
    {
        if (BlockPrefab == null)
        {
            Debug.LogWarningFormat("Block {0} not found!", Pos.Encypt);
            return null;
        }

        if (BlockPrefab.GetComponent<IsometricBlock>() == null)
        {
            Debug.LogWarningFormat("Block {0} {1} not found IsometricBlock!", Pos.Encypt, BlockPrefab.name);
            return null;
        }

        //Create
        GameObject BlockObject = QGameObject.SetCreate(BlockPrefab);

        //Block
        IsometricBlock Block = QComponent.GetComponent<IsometricBlock>(BlockObject);
        Block.WorldManager = this;
        Block.Pos = Pos.Fixed;
        Block.PosPrimary = Pos.Fixed;

        //Block Data
        Block.Data = Data != null ? Data : new IsoDataBlockSingle();

        //Block Renderer
        IsometricRenderer BlockRenderer = BlockObject.GetComponent<IsometricRenderer>();
        if (BlockRenderer != null)
        {
            BlockRenderer.SetSpriteJoin(Pos.Fixed);
        }

        if (Block.Free && Application.isPlaying)
        {
            //When in playing, FREE Block's Pos Primary will not be track, so just can be find by it own Tag!
        }
        else
        {
            //Delete
            SetWorldBlockRemovePrimary(Pos.Fixed);

            //World
            int IndexPosH = GetWorldIndexPosH(Pos.HInt);
            if (IndexPosH == -1)
            {
                m_worldPosH.Add((Pos.Fixed.HInt, new List<IsometricBlock>()));
                IndexPosH = m_worldPosH.Count - 1;
                m_worldPosH[IndexPosH].Block.Add(Block);
            }
            else
                m_worldPosH[IndexPosH].Block.Add(Block);
        }

        //Tag
        List<string> TagFind = Block.GetComponent<IsometricBlock>().Tag;
        if (TagFind.Count == 0)
        {
            //None Tag!
            int TagIndex = GetWorldIndexTag("");
            if (TagIndex == -1)
            {
                this.m_worldTag.Add(("", new List<IsometricBlock>()));
                TagIndex = this.m_worldTag.Count - 1;
                this.m_worldTag[TagIndex].Block.Add(Block);
            }
            else
                this.m_worldTag[TagIndex].Block.Add(Block);
        }
        else
        {
            //Got Tag!
            foreach (string TagCheck in TagFind)
            {
                int TagIndex = GetWorldIndexTag(TagCheck);
                if (TagIndex == -1)
                {
                    this.m_worldTag.Add((TagCheck, new List<IsometricBlock>()));
                    TagIndex = this.m_worldTag.Count - 1;
                    this.m_worldTag[TagIndex].Block.Add(Block);
                }
                else
                    this.m_worldTag[TagIndex].Block.Add(Block);
            }
        }

        //Scene
        Transform ParentPosH = transform.Find(GetWorldNamePosH(Pos.Fixed));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetWorldNamePosH(Pos.Fixed), transform).transform;
            Block.transform.SetParent(ParentPosH, true);
        }

        return Block;
    }

    #endregion

    #region Block Get

    public IsometricBlock GetWorldBlockPrimary(IsoVector Pos)
    {
        //World
        int IndexPosH = GetWorldIndexPosH(Pos.Fixed.HInt);
        if (IndexPosH == -1)
            return null;

        for (int i = 0; i < m_worldPosH[IndexPosH].Block.Count; i++)
        {
            if (m_worldPosH[IndexPosH].Block[i].PosPrimary != Pos.Fixed)
                continue;

            return m_worldPosH[IndexPosH].Block[i];
        }

        return null;
    }

    public IsometricBlock GetWorldBlockCurrent(IsoVector Pos, params string[] Tag)
    {
        if (Tag.Length > 0)
        {
            //Find all Block with know tag - More Quickly!!
            foreach (string TagFind in Tag)
            {
                int TagIndex = GetWorldIndexTag(TagFind);
                if (TagIndex == -1)
                    //Not exist Tag in Tag List!
                    continue;

                for (int BlockIndex = 0; BlockIndex < m_worldTag[TagIndex].Block.Count; BlockIndex++)
                {
                    if (m_worldTag[TagIndex].Block[BlockIndex].Pos.Fixed != Pos.Fixed)
                        continue;

                    return m_worldTag[TagIndex].Block[BlockIndex];
                }
            }
        }
        else
        {
            //Find all block with unknow tag - More slower!! (But always found Block)
            foreach (var TagCheck in m_worldTag)
            {
                foreach (var BlockCheck in TagCheck.Block)
                {
                    if (BlockCheck.Pos.Fixed != Pos.Fixed)
                        continue;

                    return BlockCheck;
                }
            }
        }

        return null;
    }

    public List<IsometricBlock> GetWorldBlockCurrentAll(IsoVector Pos, params string[] Tag)
    {
        List<IsometricBlock> List = new List<IsometricBlock>();

        if (Tag.Length > 0)
        {
            //Find all Block with know tag - More Quickly!!
            foreach (string TagFind in Tag)
            {
                int TagIndex = GetWorldIndexTag(TagFind);
                if (TagIndex == -1)
                    //Not exist Tag in Tag List!
                    continue;

                for (int BlockIndex = 0; BlockIndex < m_worldTag[TagIndex].Block.Count; BlockIndex++)
                {
                    if (m_worldTag[TagIndex].Block[BlockIndex].Pos.Fixed != Pos.Fixed)
                        continue;

                    List.Add(m_worldTag[TagIndex].Block[BlockIndex]);
                }
            }
        }
        else
        {
            //Find all block with unknow tag - More slower!! (But always found Block)
            foreach (var TagCheck in m_worldTag)
            {
                foreach (var BlockCheck in TagCheck.Block)
                {
                    if (BlockCheck.Pos.Fixed != Pos.Fixed)
                        continue;

                    List.Add(BlockCheck);
                }
            }
        }

        return List;
    }

    public List<IsometricBlock> GetWorldBlockCurrentAll(string Tag)
    {
        foreach (var Check in m_worldTag)
        {
            if (Check.Tag != Tag)
                continue;
            return Check.Block;
        }
        return null;
    }

    #endregion

    #region Block Remove

    public void SetWorldBlockRemovePrimary(IsoVector Pos, float Delay = 0)
    {
        //World
        int IndexPosH = GetWorldIndexPosH(Pos.Fixed.HInt);
        if (IndexPosH == -1)
            return;

        for (int i = 0; i < m_worldPosH[IndexPosH].Block.Count; i++)
        {
            if (m_worldPosH[IndexPosH].Block[i].PosPrimary != Pos.Fixed)
                continue;

            IsometricBlock Block = m_worldPosH[IndexPosH].Block[i];

            //World
            m_worldPosH[IndexPosH].Block.Remove(Block);
            if (m_worldPosH[IndexPosH].Block.Count == 0)
                m_worldPosH.RemoveAt(IndexPosH);

            //Tag
            List<string> TagFind = Block.Tag;
            foreach (string TagCheck in TagFind)
            {
                int TagIndex = GetWorldIndexTag(TagCheck);
                if (TagIndex != -1)
                {
                    m_worldTag[TagIndex].Block.Remove(Block);
                    if (m_worldTag[TagIndex].Block.Count == 0)
                        m_worldTag.RemoveAt(TagIndex);
                }
            }

            //Scene
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(Block.gameObject);
            else
                Destroy(Block.gameObject, Delay);

            break;
        }
    }

    public void SetWorldBlockRemoveInstant(IsometricBlock Block, float Delay)
    {
        if (!Block.Free)
        {
            //World
            m_worldPosH[GetWorldIndexPosH(Block.Pos.HInt)].Block.Remove(Block);
        }

        //Tag
        foreach(string TagCheck in Block.Tag)
            m_worldTag[GetWorldIndexTag(TagCheck)].Block.Remove(Block);

        //Scene
        if (Application.isEditor && !Application.isPlaying)
            DestroyImmediate(Block.gameObject);
        else
            Destroy(Block.gameObject, Delay);
    }

    #endregion

    #endregion

    #region ======================================================================== World

    #region World Read

    public void SetWorldRead(Transform WorldManager)
    {
        //Clear Current World!!
        SetWorldRemove();

        //Store Block(s) Found!!
        List<IsometricBlock> BlockFound = WorldManager.GetComponentsInChildren<IsometricBlock>().ToList();
        GameObject BlockStore = QGameObject.SetCreate("BlockStore");
        foreach (IsometricBlock Block in BlockFound)
        {
            if (Block.gameObject.name == CURSON_NAME)
                continue;

            Block.transform.SetParent(BlockStore.transform);
        }

        //Remove All GameObject!!
        for (int i = WorldManager.transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (WorldManager.GetChild(i).gameObject.name == CURSON_NAME)
                continue;
#endif
            if (this.transform.GetChild(i).GetComponent<Camera>() != null)
                continue;

            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(WorldManager.GetChild(i).gameObject);
            else
                Destroy(WorldManager.GetChild(i).gameObject);
        }

        //Add Block(s) Found!!
        foreach (IsometricBlock Block in BlockFound)
        {
            if (Block.gameObject.name == CURSON_NAME)
                continue;

            SetWorldReadBlock(Block);
        }

        //Destroy Block(s) Store!!
        if (Application.isEditor && !Application.isPlaying)
            DestroyImmediate(BlockStore);
        else
            Destroy(BlockStore);

        onWorldCreate?.Invoke();
    }

    public void SetWorldReadBlock(IsometricBlock Block)
    {
        Block.WorldManager = this;
        Block.PosPrimary = Block.Pos;

        //World
        int IndexPosH = GetWorldIndexPosH(Block.Pos.HInt);
        if (IndexPosH == -1)
        {
            m_worldPosH.Add((Block.Pos.HInt, new List<IsometricBlock>()));
            IndexPosH = m_worldPosH.Count - 1;
            m_worldPosH[IndexPosH].Block.Add(Block);
        }
        else
            m_worldPosH[IndexPosH].Block.Add(Block);

        //Tag
        List<string> TagFind = Block.GetComponent<IsometricBlock>().Tag;
        foreach (string TagCheck in TagFind)
        {
            int TagIndex = GetWorldIndexTag(TagCheck);
            if (TagIndex == -1)
            {
                this.m_worldTag.Add((TagCheck, new List<IsometricBlock>()));
                IndexPosH = this.m_worldTag.Count - 1;
                this.m_worldTag[IndexPosH].Block.Add(Block);
            }
            else
                this.m_worldTag[TagIndex].Block.Add(Block);
        }

        //Scene
        Transform ParentPosH = transform.Find(GetWorldNamePosH(Block.Pos));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetWorldNamePosH(Block.Pos), this.transform).transform;
            Block.transform.SetParent(ParentPosH, true);
        }
    }

    #endregion

    #region World Remove

    public void SetWorldRemove(bool Full = false)
    {
        for (int i = m_worldPosH.Count - 1; i >= 0; i--)
        {
            for (int j = m_worldPosH[i].Block.Count - 1; j >= 0; j--)
            {
                IsometricBlock Block = m_worldPosH[i].Block[j];

                if (Block == null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(Block.gameObject);
                else
                    Destroy(Block.gameObject);
            }
        }
        m_worldPosH.Clear();

        for (int i = m_worldTag.Count - 1; i >= 0; i--)
        {
            for (int j = m_worldTag[i].Block.Count - 1; j >= 0; j--)
            {
                IsometricBlock Block = m_worldTag[i].Block[j];

                if (Block == null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(Block.gameObject);
                else
                    Destroy(Block.gameObject);
            }
        }
        m_worldTag.Clear();

        if (Full)
        {
            //Remove All GameObject!!
            for (int i = this.transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (this.transform.GetChild(i).gameObject.name == CURSON_NAME)
                    continue;
#endif
                if (this.transform.GetChild(i).GetComponent<Camera>() != null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(this.transform.GetChild(i).gameObject);
                else
                    Destroy(this.transform.GetChild(i).gameObject);
            }
        }

        onWorldRemove?.Invoke();
    }

    #endregion

    #region World Progess

    private int GetWorldIndexPosH(int PosH)
    {
        for (int i = 0; i < m_worldPosH.Count; i++)
        {
            if (m_worldPosH[i].PosH != PosH)
                continue;
            return i;
        }
        return -1;
    }

    private int GetWorldIndexTag(string Tag)
    {
        for (int i = 0; i < m_worldTag.Count; i++)
        {
            if (m_worldTag[i].Tag != Tag)
                continue;
            return i;
        }
        return -1;
    }

    private string GetWorldNamePosH(IsoVector Pos)
    {
        return Pos.HInt.ToString();
    }

    public void SetWorldOrder()
    {
        m_worldPosH = m_worldPosH.OrderByDescending(h => h.PosH).ToList();
        for (int i = 0; i < m_worldPosH.Count; i++)
            m_worldPosH[i] = (m_worldPosH[i].PosH, m_worldPosH[i].Block.OrderByDescending(a => a.Pos.X).OrderByDescending(b => b.Pos.Y).ToList());
    }

    #endregion

    #endregion

    #region ======================================================================== List

    #region Read

    public void SetList(IsometricConfig IsometricConfig)
    {
        SetList(IsometricConfig.BlockList);
    }

    public void SetList(List<IsometricBlock> BlockList)
    {
        if (this.BlockList == null)
            this.BlockList = new List<(string Tag, List<IsometricBlock> Block)>();
        else
            this.BlockList.Clear();

        foreach (IsometricBlock BlockCheck in BlockList)
        {
            if (BlockCheck == null)
            {
                Debug.LogWarningFormat("Not found IsometricBlock to Read!");
                continue;
            }

            List<string> TagFind = BlockCheck.GetComponent<IsometricBlock>().Tag;
            foreach (string TagCheck in TagFind)
            {
                int TagIndex = GetIndexListTag(TagCheck);
                if (TagIndex == -1)
                {
                    this.BlockList.Add((TagCheck, new List<IsometricBlock>()));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(BlockCheck);
                }
                else
                    this.BlockList[TagIndex].Block.Add(BlockCheck);
            }
        }

        onListRead?.Invoke();
    }

    public void SetList(List<GameObject> BlockList)
    {
        if (this.BlockList == null)
            this.BlockList = new List<(string Tag, List<IsometricBlock> Block)>();
        else
            this.BlockList.Clear();

        foreach (GameObject BlockCheck in BlockList)
        {
            IsometricBlock Block = BlockCheck.GetComponent<IsometricBlock>();
            if (Block == null)
            {
                Debug.LogWarningFormat("Prefab {0} not found IsometricBlock to Read!", BlockCheck.name);
                continue;
            }

            List<string> TagFind = BlockCheck.GetComponent<IsometricBlock>().Tag;
            foreach (string TagCheck in TagFind)
            {
                int TagIndex = GetIndexListTag(TagCheck);
                if (TagIndex == -1)
                {
                    this.BlockList.Add((TagCheck, new List<IsometricBlock>()));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(Block);
                }
                else
                    this.BlockList[TagIndex].Block.Add(Block);
            }
        }

        onListRead?.Invoke();
    }

    public void SetList(params string[] PathChildInResources)
    {
        if (this.BlockList == null)
            this.BlockList = new List<(string Tag, List<IsometricBlock> Block)>();
        else
            this.BlockList.Clear();

        List<GameObject> BlockList = QResources.GetPrefab(PathChildInResources);

        foreach (GameObject BlockPrefab in BlockList)
        {
            IsometricBlock Block = BlockPrefab.GetComponent<IsometricBlock>();
            if (Block == null)
            {
                Debug.LogWarningFormat("Prefab {0} not found IsometricBlock to Read!", BlockPrefab.name);
                continue;
            }

            List<string> TagFind = BlockPrefab.GetComponent<IsometricBlock>().Tag;
            if (TagFind.Count == 0)
            {
                int TagIndex = GetIndexListTag("");
                if (TagIndex == -1)
                {
                    this.BlockList.Add(("", new List<IsometricBlock>()));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(Block);
                }
                else
                    this.BlockList[TagIndex].Block.Add(Block);
            }
            else
            {
                foreach (string TagCheck in TagFind)
                {
                    int TagIndex = GetIndexListTag(TagCheck);
                    if (TagIndex == -1)
                    {
                        this.BlockList.Add((TagCheck, new List<IsometricBlock>()));
                        TagIndex = this.BlockList.Count - 1;
                        this.BlockList[TagIndex].Block.Add(Block);
                    }
                    else
                        this.BlockList[TagIndex].Block.Add(Block);
                }
            }
        }

        onListRead?.Invoke();
    }

    public GameObject GetList(string BlockName, string Tag = "")
    {
        if (Tag != "")
        {
            for (int i = 0; i < BlockList.Count; i++)
            {
                if (BlockList[i].Tag != Tag)
                    continue;

                foreach (IsometricBlock BlockCheck in BlockList[i].Block)
                {
                    if (BlockCheck.Name != BlockName)
                        continue;
                    return BlockCheck.gameObject;
                }
            }
        }
        else
        {
            for (int i = 0; i < BlockList.Count; i++)
            {
                foreach (IsometricBlock BlockCheck in BlockList[i].Block)
                {
                    if (BlockCheck.Name != BlockName)
                        continue;
                    return BlockCheck.gameObject;
                }
            }
        }
        return null;
    }

    #endregion

    #region Progess

    private int GetIndexListTag(string Tag)
    {
        for (int i = 0; i < BlockList.Count; i++)
        {
            if (BlockList[i].Tag != Tag)
                continue;
            return i;
        }
        return -1;
    }

    #endregion

    #endregion

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
        SetWorldOrder();
        //
        List<IsoDataBlock> WorldBlocks = new List<IsoDataBlock>();
        for (int i = 0; i < m_worldPosH.Count; i++)
            for (int j = 0; j < m_worldPosH[i].Block.Count; j++)
                WorldBlocks.Add(new IsoDataBlock(m_worldPosH[i].Block[j].PosPrimary, m_worldPosH[i].Block[j].Name, m_worldPosH[i].Block[j].Data));
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
        SetWorldRemove(true);
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
            Data.MoveData.Type = FileIO.GetReadAutoEnum<IsoDataBlock.DataBlockType>();
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
            Data.ActionData.Type = FileIO.GetReadAutoEnum<IsoDataBlock.DataBlockType>();
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
            SetWorldBlockCreate(PosPrimary, GetList(Name), Data);
        }
        //
        onWorldCreate?.Invoke();
    }

    #endregion

    #endregion

    #region ======================================================================== Json

    private class WorldData
    {
        public List<IsoDataBlock> WorldBlocks = new List<IsoDataBlock>();
    }

    public void SetJsonWrite(string Path)
    {
        var WorldData = new WorldData();
        for (int i = 0; i < m_worldPosH.Count; i++)
            for (int j = 0; j < m_worldPosH[i].Block.Count; j++)
                WorldData.WorldBlocks.Add(new IsoDataBlock(m_worldPosH[i].Block[j].PosPrimary, m_worldPosH[i].Block[j].Name, m_worldPosH[i].Block[j].Data));
        //
        QJSON.SetDataPath(WorldData, Path);
    }

    public void SetJsonRead(string Path)
    {
        var WorldData = new WorldData();
        //
        WorldData = QJSON.GetDataPath<WorldData>(Path);
        //
        for (int i = 0; i < m_worldPosH.Count; i++)
            for (int j = 0; j < m_worldPosH[i].Block.Count; j++)
                SetWorldBlockCreate(WorldData.WorldBlocks[i].PosPrimary, GetList(WorldData.WorldBlocks[i].Name), WorldData.WorldBlocks[i].Data);
    }

    #endregion

    #region ======================================================================== Editor

    //Required "IsoBlockRenderer.cs" component for each Block!

    public bool SetEditorMask(IsoVector Pos, Color Mask, Color UnMask, Color Centre)
    {
        bool CentreFound = false;
        for (int i = 0; i < m_worldPosH.Count; i++)
            for (int j = 0; j < m_worldPosH[i].Block.Count; j++)
            {
                IsometricRenderer BlockSprite = m_worldPosH[i].Block[j].GetComponent<IsometricRenderer>();
                if (BlockSprite == null)
                    continue;

                if (m_worldPosH[i].Block[j].Pos == Pos)
                {
                    CentreFound = true;
                    BlockSprite.SetSpriteColor(Centre, 1f);
                }
                else
                if (m_worldPosH[i].Block[j].Pos.X == Pos.X || m_worldPosH[i].Block[j].Pos.Y == Pos.Y)
                    BlockSprite.SetSpriteColor(Mask, 1f);
                else
                    BlockSprite.SetSpriteColor(UnMask, 1f);
            }
        return CentreFound;
    }

    public void SetEditorHidden(int FromH, float UnMask)
    {
        for (int i = 0; i < m_worldPosH.Count; i++)
            for (int j = 0; j < m_worldPosH[i].Block.Count; j++)
            {
                IsometricRenderer BlockSprite = m_worldPosH[i].Block[j].GetComponent<IsometricRenderer>();
                if (BlockSprite == null)
                    continue;

                if (m_worldPosH[i].Block[j].Pos.H > FromH)
                    BlockSprite.SetSpriteAlpha(UnMask);
                else
                    BlockSprite.SetSpriteAlpha(1f);
            }
    }

    #endregion
}