using QuickMethode;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsometricManager : MonoBehaviour
{
    #region Enum

    public enum IsoRendererType { XY, H, None, }

    #endregion

    #region Varible: World Manager

    [SerializeField] private string m_name = "";
    [SerializeField] private IsoDataScene m_scene = new IsoDataScene();

    public string WorldName => m_name;
    public IsoDataScene Scene => m_scene;

    private List<(int PosH, List<IsometricBlock> Block)> m_worldPosH = new List<(int PosH, List<IsometricBlock> Block)>();

    private List<(string Tag, List<IsometricBlock> Block)> m_worldTag = new List<(string Tag, List<IsometricBlock> Block)>();

    #endregion

    #region Varible: Block Manager

    public List<(string Tag, List<GameObject> Block)> BlockList = new List<(string Tag, List<GameObject> Block)>();

    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    #region ======================================================================== World & Block

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
        Block.Pos = Pos;
        Block.PosPrimary = Pos;

        //Block Data
        Block.Data = Data;

        //Block Renderer
        IsometricRenderer BlockRenderer = BlockObject.GetComponent<IsometricRenderer>();
        if (BlockRenderer != null)
        {
            BlockRenderer.SetSpriteJoin(Pos);
        }

        if (Block.Free && Application.isPlaying)
        {
            //When in playing, FREE Block's Pos Primary will not be track, so just can be find by it own Tag!
        }
        else
        {
            //Delete
            SetWorldBlockRemovePrimary(Pos);

            //World
            int IndexPosH = GetIndexWorldPosH(Pos.HInt);
            if (IndexPosH == -1)
            {
                m_worldPosH.Add((Pos.HInt, new List<IsometricBlock>()));
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
            int TagIndex = GetIndexWorldTag("");
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
                int TagIndex = GetIndexWorldTag(TagCheck);
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
        Transform ParentPosH = transform.Find(GetNameWorldPosH(Pos));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetNameWorldPosH(Pos), transform).transform;
            Block.transform.SetParent(ParentPosH, true);
        }

        return Block;
    }

    #endregion

    #region Block Get

    public IsometricBlock GetWorldBlockPrimary(IsoVector Pos)
    {
        //World
        int IndexPosH = GetIndexWorldPosH(Pos.HInt);
        if (IndexPosH == -1)
            return null;

        for (int i = 0; i < m_worldPosH[IndexPosH].Block.Count; i++)
        {
            if (m_worldPosH[IndexPosH].Block[i].PosPrimary != Pos)
                continue;

            return m_worldPosH[IndexPosH].Block[i];
        }

        return null;
    }

    public List<IsometricBlock> GetWorldBlockCurrent(IsoVector Pos, params string[] Tag)
    {
        List<IsometricBlock> List = new List<IsometricBlock>();

        if (Tag.Length > 0)
        {
            //Find all Block with know tag - More Quickly!!
            foreach(string TagFind in Tag)
            {
                int TagIndex = GetIndexWorldTag(TagFind);
                if (TagIndex == -1)
                    //Not exist Tag in Tag List!
                    continue;

                for (int BlockIndex = 0; BlockIndex < m_worldTag[TagIndex].Block.Count; BlockIndex++)
                {
                    if (m_worldTag[TagIndex].Block[BlockIndex].Pos != Pos)
                        continue;

                    List.Add(m_worldTag[TagIndex].Block[BlockIndex]);
                }
            }
        }
        else
        {
            //Find all block with unknow tag - More slower!!
            foreach(var PosH in m_worldPosH)
            {
                foreach(IsometricBlock Block in PosH.Block)
                {
                    if (Block.Pos != Pos)
                        continue;

                    List.Add(Block);
                }
            }
        }

        return List.Count > 0 ? List : null;
    }

    public List<IsometricBlock> GetWorldBlockCurrentTag(string Tag)
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

    public void SetWorldBlockRemovePrimary(IsoVector Pos)
    {
        //World
        int IndexPosH = GetIndexWorldPosH(Pos.HInt);
        if (IndexPosH == -1)
            return;

        for (int i = 0; i < m_worldPosH[IndexPosH].Block.Count; i++)
        {
            if (m_worldPosH[IndexPosH].Block[i].PosPrimary != Pos)
                continue;

            IsometricBlock Block = m_worldPosH[IndexPosH].Block[i];

            //World
            m_worldPosH[IndexPosH].Block.Remove(Block);
            if (m_worldPosH[IndexPosH].Block.Count == 0)
                m_worldPosH.RemoveAt(IndexPosH);

            //Tag
            List<string> TagFind = Block.Tag;
            foreach(string TagCheck in TagFind)
            {
                int TagIndex = GetIndexWorldTag(TagCheck);
                if (TagIndex != -1)
                {
                    m_worldTag[TagIndex].Block.Remove(Block);
                    if (m_worldTag[TagIndex].Block.Count == 0)
                        m_worldTag.RemoveAt(TagIndex);
                }
            }

            //Scene
            if (Application.isEditor)
                DestroyImmediate(Block.gameObject);
            else
                Destroy(Block.gameObject);

            break;
        }
    }

    public void SetWorldBlockRemoveInstant(IsometricBlock Block)
    {
        //World
        m_worldPosH[GetIndexWorldPosH(Block.Pos.HInt)].Block.Remove(Block);

        //Tag
        m_worldTag[GetIndexWorldTag(Block.tag)].Block.Remove(Block);

        //Scene
        if (Application.isEditor)
            DestroyImmediate(Block.gameObject);
        else
            Destroy(Block.gameObject);
    }

    #endregion

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
            if (Block.gameObject.name == IsometricTool.CURSON_NAME)
                continue;

            Block.transform.SetParent(BlockStore.transform);
        }

        //Remove All GameObject!!
        for (int i = WorldManager.transform.childCount - 1; i >= 0; i--)
        {
            if (WorldManager.GetChild(i).gameObject.name == IsometricTool.CURSON_NAME)
                continue;

            if (Application.isEditor)
                DestroyImmediate(WorldManager.GetChild(i).gameObject);
            else
                Destroy(WorldManager.GetChild(i).gameObject);
        }

        //Add Block(s) Found!!
        foreach (IsometricBlock Block in BlockFound)
        {
            if (Block.gameObject.name == IsometricTool.CURSON_NAME)
                continue;

            SetWorldBlockRead(Block);
        }

        //Destroy Block(s) Store!!
        if (Application.isEditor)
            DestroyImmediate(BlockStore);
        else
            Destroy(BlockStore);
    }

    public void SetWorldBlockRead(IsometricBlock Block)
    {
        Block.WorldManager = this;
        Block.PosPrimary = Block.Pos;

        //World
        int IndexPosH = GetIndexWorldPosH(Block.Pos.HInt);
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
        foreach(string TagCheck in TagFind)
        {
            int TagIndex = GetIndexWorldTag(TagCheck);
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
        Transform ParentPosH = transform.Find(GetNameWorldPosH(Block.Pos));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetNameWorldPosH(Block.Pos), transform).transform;
            Block.transform.SetParent(ParentPosH, true);
        }
    }

    #endregion

    #region World Remove

    public void SetWorldRemove()
    {
        for (int i = m_worldPosH.Count - 1; i >= 0; i--)
        {
            for (int j = m_worldPosH[i].Block.Count - 1; j >= 0; j--)
            {
                IsometricBlock Block = m_worldPosH[i].Block[j];

                if (Application.isEditor)
                    DestroyImmediate(Block.gameObject);
                else
                    Destroy(Block.gameObject);
            }
        }
        m_worldPosH.Clear();
        m_worldTag.Clear();
    }

    #endregion

    #region World Progess

    private int GetIndexWorldPosH(int PosH)
    {
        for (int i = 0; i < m_worldPosH.Count; i++)
        {
            if (m_worldPosH[i].PosH != PosH)
                continue;
            return i;
        }
        return -1;
    }

    private int GetIndexWorldTag(string Tag)
    {
        for (int i = 0; i < m_worldTag.Count; i++)
        {
            if (m_worldTag[i].Tag != Tag)
                continue;
            return i;
        }
        return -1;
    }

    private string GetNameWorldPosH(IsoVector Pos)
    {
        return Pos.HInt.ToString();
    }

    public void SetOrderWorld()
    {
        m_worldPosH = m_worldPosH.OrderByDescending(h => h.PosH).ToList();
        for (int i = 0; i < m_worldPosH.Count; i++)
            m_worldPosH[i] = (m_worldPosH[i].PosH, m_worldPosH[i].Block.OrderByDescending(a => a.Pos.X).OrderByDescending(b => b.Pos.Y).ToList());
    }

    #endregion

    #endregion

    #region ======================================================================== List & Block

    #region Read

    public void SetBlockList(List<IsometricBlock> BlockList)
    {
        if (this.BlockList == null)
            this.BlockList = new List<(string Tag, List<GameObject> Block)>();
        else
            this.BlockList.Clear();

        foreach (IsometricBlock BlockPrefab in BlockList)
        {
            List<string> TagFind = BlockPrefab.GetComponent<IsometricBlock>().Tag;
            foreach(string TagCheck in TagFind)
            {
                int TagIndex = GetIndexBlockListTag(TagCheck);
                if (TagIndex == -1)
                {
                    this.BlockList.Add((TagCheck, new List<GameObject>()));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(BlockPrefab.gameObject);
                }
                else
                    this.BlockList[TagIndex].Block.Add(BlockPrefab.gameObject);
            }
        }
    }

    public void SetBlockList(List<GameObject> BlockList)
    {
        if (this.BlockList == null)
            this.BlockList = new List<(string Tag, List<GameObject> Block)>();
        else
            this.BlockList.Clear();

        foreach (GameObject BlockPrefab in BlockList)
        {
            if (BlockPrefab.GetComponent<IsometricBlock>() == null)
            {
                Debug.LogWarningFormat("Prefab {0} not found IsometricBlock to Read!", BlockPrefab.name);
                continue;
            }

            List<string> TagFind = BlockPrefab.GetComponent<IsometricBlock>().Tag;
            foreach(string TagCheck in TagFind)
            {
                int TagIndex = GetIndexBlockListTag(TagCheck);
                if (TagIndex == -1)
                {
                    this.BlockList.Add((TagCheck, new List<GameObject>()));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(BlockPrefab);
                }
                else
                    this.BlockList[TagIndex].Block.Add(BlockPrefab);
            }
        }
    }

    public void SetBlockList(params string[] PathChildInResources)
    {
        if (this.BlockList == null)
            this.BlockList = new List<(string Tag, List<GameObject> Block)>();
        else
            this.BlockList.Clear();

        List<GameObject> BlockList = QResources.GetPrefab(PathChildInResources);

        foreach (GameObject BlockPrefab in BlockList)
        {
            if (BlockPrefab.GetComponent<IsometricBlock>() == null)
            {
                Debug.LogWarningFormat("Prefab {0} not found IsometricBlock to Read!", BlockPrefab.name);
                continue;
            }

            List<string> TagFind = BlockPrefab.GetComponent<IsometricBlock>().Tag;
            if (TagFind.Count == 0)
            {
                int TagIndex = GetIndexBlockListTag("");
                if (TagIndex == -1)
                {
                    this.BlockList.Add(("", new List<GameObject>()));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(BlockPrefab);
                }
                else
                    this.BlockList[TagIndex].Block.Add(BlockPrefab);
            }
            else
            {
                foreach (string TagCheck in TagFind)
                {
                    int TagIndex = GetIndexBlockListTag(TagCheck);
                    if (TagIndex == -1)
                    {
                        this.BlockList.Add((TagCheck, new List<GameObject>()));
                        TagIndex = this.BlockList.Count - 1;
                        this.BlockList[TagIndex].Block.Add(BlockPrefab);
                    }
                    else
                        this.BlockList[TagIndex].Block.Add(BlockPrefab);
                }
            }
        }
    }

    public GameObject GetBlockList(string BlockName, string Tag = "")
    {
        if (Tag != "")
        {
            for (int i = 0; i < BlockList.Count; i++)
            {
                if (BlockList[i].Tag != Tag)
                    continue;

                foreach (GameObject BlockPrefab in BlockList[i].Block)
                {
                    if (BlockPrefab.name != BlockName)
                        continue;
                    return BlockPrefab;
                }
            }
        }
        else
        {
            for (int i = 0; i < BlockList.Count; i++)
            {
                foreach (GameObject BlockPrefab in BlockList[i].Block)
                {
                    if (BlockPrefab.name != BlockName)
                        continue;
                    return BlockPrefab;
                }
            }
        }
        return null;
    }

    #endregion

    #region Progess

    private int GetIndexBlockListTag(string Tag)
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

    #region ======================================================================== Data & World

    #region Fild Save

    public void SetWorldFileSave(QPath.PathType PathType, params string[] PathChild)
    {
        QFileIO FileIO = new QFileIO();

        SetWorldFileWrite(FileIO);

        FileIO.SetWriteStart(QPath.GetPath(PathType, PathChild));
    }

    private void SetWorldFileWrite(QFileIO FileIO)
    {
        SetOrderWorld();

        List<IsoDataBlock> WorldBlocks = new List<IsoDataBlock>();
        for (int i = 0; i < m_worldPosH.Count; i++)
            for (int j = 0; j < m_worldPosH[i].Block.Count; j++)
                WorldBlocks.Add(new IsoDataBlock(m_worldPosH[i].Block[j].PosPrimary, m_worldPosH[i].Block[j].Name, m_worldPosH[i].Block[j].Data));

        FileIO.SetWriteAdd("[WORLD NAME]");
        FileIO.SetWriteAdd((WorldName != "") ? WorldName : "...");

        FileIO.SetWriteAdd("[WORLD BLOCK]");
        FileIO.SetWriteAdd(WorldBlocks.Count);
        for (int BlockIndex = 0; BlockIndex < WorldBlocks.Count; BlockIndex++)
        {
            FileIO.SetWriteAdd();
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].PosPrimary.Encypt);
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Name);

            FileIO.SetWriteAdd("<MOVE DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData.Count);
            for (int MoveIndex = 0; MoveIndex < WorldBlocks[BlockIndex].Data.MoveData.Count; MoveIndex++)
            {
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData[MoveIndex].KeyStart);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData[MoveIndex].KeyEnd);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData[MoveIndex].Data.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.MoveData[MoveIndex].Data.Count; DataIndex++)
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.MoveData[MoveIndex].Data[DataIndex].Encypt);
                }
            }

            FileIO.SetWriteAdd("<EVENT DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData.Count);
            for (int JoinIndex = 0; JoinIndex < WorldBlocks[BlockIndex].Data.EventData.Count; JoinIndex++)
            {
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData[JoinIndex].KeyStart);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData[JoinIndex].KeyEnd);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData[JoinIndex].Data.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.EventData[JoinIndex].Data.Count; DataIndex++)
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.EventData[JoinIndex].Data[DataIndex].Encypt);
                }
            }

            FileIO.SetWriteAdd("<TELEPORT DATA>");
            FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData.Count);
            for (int JoinIndex = 0; JoinIndex < WorldBlocks[BlockIndex].Data.TeleportData.Count; JoinIndex++)
            {
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData[JoinIndex].KeyStart);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData[JoinIndex].KeyEnd);
                FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData[JoinIndex].Data.Count);
                for (int DataIndex = 0; DataIndex < WorldBlocks[BlockIndex].Data.TeleportData[JoinIndex].Data.Count; DataIndex++)
                {
                    FileIO.SetWriteAdd(WorldBlocks[BlockIndex].Data.TeleportData[JoinIndex].Data[DataIndex].Encypt);
                }
            }
        }
    }

    #endregion

    #region File Read

    //File Path

    public void SetWorldFileRead(QPath.PathType PathType, params string[] PathChild)
    {
        QFileIO FileIO = new QFileIO();

        FileIO.SetReadStart(QPath.GetPath(PathType, PathChild));

        SetWorldFileRead(FileIO);
    }

    private void SetWorldFileRead(QFileIO FileIO)
    {
        FileIO.GetReadAuto();
        m_name = FileIO.GetReadAutoString();

        FileIO.GetReadAuto();
        int BlockCount = FileIO.GetReadAutoInt();
        for (int BlockIndex = 0; BlockIndex < BlockCount; BlockIndex++)
        {
            FileIO.GetReadAuto();
            IsoVector PosPrimary = IsoVector.GetDencypt(FileIO.GetReadAutoString());
            string Name = FileIO.GetReadAutoString();

            IsoDataBlockSingle Data = new IsoDataBlockSingle();

            FileIO.GetReadAuto();
            Data.MoveData = new List<IsoDataBlockMove>();
            int MoveCount = FileIO.GetReadAutoInt();
            for (int MoveIndex = 0; MoveIndex < MoveCount; MoveIndex++)
            {
                Data.MoveData.Add(new IsoDataBlockMove());
                Data.MoveData[MoveIndex].KeyStart = FileIO.GetReadAutoString();
                Data.MoveData[MoveIndex].KeyEnd = FileIO.GetReadAutoString();
                Data.MoveData[MoveIndex].Data = new List<IsoDataBlockMoveSingle>();
                int DataCount = FileIO.GetReadAutoInt();
                for (int DataIndex = 0; DataIndex < DataCount; DataIndex++)
                {
                    Data.MoveData[MoveIndex].Data.Add(IsoDataBlockMoveSingle.GetDencypt(FileIO.GetReadAutoString()));
                }
            }

            FileIO.GetReadAuto();
            Data.EventData = new List<IsoDataBlockEvent>();
            int EventCount = FileIO.GetReadAutoInt();
            for (int EventIndex = 0; EventIndex < EventCount; EventIndex++)
            {
                Data.EventData.Add(new IsoDataBlockEvent());
                Data.EventData[EventIndex].KeyStart = FileIO.GetReadAutoString();
                Data.EventData[EventIndex].KeyEnd = FileIO.GetReadAutoString();
                Data.EventData[EventIndex].Data = new List<IsoDataBlockEventSingle>();
                int DataCount = FileIO.GetReadAutoInt();
                for (int DataIndex = 0; DataIndex < DataCount; DataIndex++)
                {
                    Data.EventData[EventIndex].Data.Add(IsoDataBlockEventSingle.GetDencypt(FileIO.GetReadAutoString()));
                }
            }

            FileIO.GetReadAuto();
            Data.TeleportData = new List<IsoDataBlockTeleport>();
            int TeleportCount = FileIO.GetReadAutoInt();
            for (int TeleportIndex = 0; TeleportIndex < TeleportCount; TeleportIndex++)
            {
                Data.TeleportData.Add(new IsoDataBlockTeleport());
                Data.TeleportData[TeleportIndex].KeyStart = FileIO.GetReadAutoString();
                Data.TeleportData[TeleportIndex].KeyEnd = FileIO.GetReadAutoString();
                Data.TeleportData[TeleportIndex].Data = new List<IsoDataBlockTeleportSingle>();
                int DataCount = FileIO.GetReadAutoInt();
                for (int DataIndex = 0; DataIndex < DataCount; DataIndex++)
                {
                    Data.TeleportData[TeleportIndex].Data.Add(IsoDataBlockTeleportSingle.GetDencypt(FileIO.GetReadAutoString()));
                }
            }

            SetWorldBlockCreate(PosPrimary, GetBlockList(Name), Data);
        }
    }

    //File Text

    public void SetWorldFileRead(TextAsset WorldFile)
    {
        QFileIO FileIO = new QFileIO();

        FileIO.SetReadStart(WorldFile);

        SetWorldFileRead(FileIO);
    }

    #endregion

    #endregion

    #region ======================================================================== Editor

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