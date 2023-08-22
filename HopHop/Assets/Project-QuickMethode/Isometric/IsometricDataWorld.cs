using QuickMethode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class IsometricDataWorld
{
    public const string CURSON_NAME = "ISO-CURSON";

    public Action onCreate;
    public Action onRemove;

    public List<IsometricDataWorldPosH> m_worldPosH;
    public List<IsometricDataWorldTag> m_worldTag;

    private IsometricManager m_manager;

    public IsometricDataWorld(IsometricManager Manager)
    {
        m_manager = Manager;
        //
        m_worldPosH = new List<IsometricDataWorldPosH>();
        m_worldTag = new List<IsometricDataWorldTag>();
    }

    #region ======================================================================== Block

    #region Block Create

    public IsometricBlock SetBlockCreate(IsometricVector Pos, GameObject BlockPrefab, IsometricDataFileBlockData Data = null)
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
        Block.WorldManager = m_manager;
        Block.Pos = Pos.Fixed;
        Block.PosPrimary = Pos.Fixed;

        //Block Data
        Block.Data = Data != null ? Data : new IsometricDataFileBlockData();

        //Block Renderer
        IsometricRenderer BlockRenderer = BlockObject.GetComponent<IsometricRenderer>();
        if (BlockRenderer != null)
        {
            BlockRenderer.SetSpriteJoin(Pos.Fixed);
        }

        if (Block.PosType == IsometricPosType.Free && Application.isPlaying)
        {
            //When in playing, FREE Block's Pos Primary will not be track, so just can be find by it own Tag!
        }
        else
        {
            //Delete
            SetBlockRemovePrimary(Pos.Fixed);

            //World
            int IndexPosH = GetWorldIndexPosH(Pos.HInt);
            if (IndexPosH == -1)
            {
                m_worldPosH.Add(new IsometricDataWorldPosH(Pos.Fixed.HInt, new List<IsometricBlock>()));
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
                m_worldTag.Add(new IsometricDataWorldTag("", new List<IsometricBlock>()));
                TagIndex = m_worldTag.Count - 1;
                m_worldTag[TagIndex].Block.Add(Block);
            }
            else
                m_worldTag[TagIndex].Block.Add(Block);
        }
        else
        {
            //Got Tag!
            foreach (string TagCheck in TagFind)
            {
                int TagIndex = GetWorldIndexTag(TagCheck);
                if (TagIndex == -1)
                {
                    m_worldTag.Add(new IsometricDataWorldTag(TagCheck));
                    TagIndex = m_worldTag.Count - 1;
                    m_worldTag[TagIndex].Block.Add(Block);
                }
                else
                    m_worldTag[TagIndex].Block.Add(Block);
            }
        }

        //Scene
        Transform ParentPosH = m_manager.transform.Find(GetWorldNamePosH(Pos.Fixed));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetWorldNamePosH(Pos.Fixed), m_manager.transform).transform;
            Block.transform.SetParent(ParentPosH, true);
        }

        return Block;
    }

    #endregion

    #region Block Get

    public IsometricBlock GetBlockPrimary(IsometricVector Pos)
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

    public IsometricBlock GetBlockCurrent(IsometricVector Pos, params string[] Tag)
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

    public List<IsometricBlock> GetBlockCurrentAll(IsometricVector Pos, params string[] Tag)
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

    public List<IsometricBlock> GetBlockCurrentAll(string Tag)
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

    public void SetBlockRemovePrimary(IsometricVector Pos, float Delay = 0)
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
                GameObject.DestroyImmediate(Block.gameObject);
            else
                GameObject.Destroy(Block.gameObject, Delay);

            break;
        }
    }

    public void SetBlockRemoveInstant(IsometricBlock Block, float Delay)
    {
        if (Block.PosType == IsometricPosType.Track)
        {
            //World
            m_worldPosH[GetWorldIndexPosH(Block.Pos.HInt)].Block.Remove(Block);
        }

        //Tag
        foreach (string TagCheck in Block.Tag)
            m_worldTag[GetWorldIndexTag(TagCheck)].Block.Remove(Block);

        //Scene
        if (Application.isEditor && !Application.isPlaying)
            GameObject.DestroyImmediate(Block.gameObject);
        else
            GameObject.Destroy(Block.gameObject, Delay);
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
            if (m_manager.transform.GetChild(i).GetComponent<Camera>() != null)
                continue;

            if (Application.isEditor && !Application.isPlaying)
                GameObject.DestroyImmediate(WorldManager.GetChild(i).gameObject);
            else
                GameObject.Destroy(WorldManager.GetChild(i).gameObject);
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
            GameObject.DestroyImmediate(BlockStore);
        else
            GameObject.Destroy(BlockStore);

        onCreate?.Invoke();
    }

    public void SetWorldReadBlock(IsometricBlock Block)
    {
        Block.WorldManager = m_manager;
        Block.PosPrimary = Block.Pos;

        //World
        int IndexPosH = GetWorldIndexPosH(Block.Pos.HInt);
        if (IndexPosH == -1)
        {
            m_worldPosH.Add(new IsometricDataWorldPosH(Block.Pos.HInt));
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
                m_worldTag.Add(new IsometricDataWorldTag(TagCheck));
                IndexPosH = m_worldTag.Count - 1;
                m_worldTag[IndexPosH].Block.Add(Block);
            }
            else
                m_worldTag[TagIndex].Block.Add(Block);
        }

        //Scene
        Transform ParentPosH = m_manager.transform.Find(GetWorldNamePosH(Block.Pos));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetWorldNamePosH(Block.Pos), m_manager.transform).transform;
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
                    GameObject.DestroyImmediate(Block.gameObject);
                else
                    GameObject.Destroy(Block.gameObject);
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
                    GameObject.DestroyImmediate(Block.gameObject);
                else
                    GameObject.Destroy(Block.gameObject);
            }
        }
        m_worldTag.Clear();

        if (Full)
        {
            //Remove All GameObject!!
            for (int i = m_manager.transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (m_manager.transform.GetChild(i).gameObject.name == CURSON_NAME)
                    continue;
#endif
                if (m_manager.transform.GetChild(i).GetComponent<Camera>() != null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(m_manager.transform.GetChild(i).gameObject);
                else
                    GameObject.Destroy(m_manager.transform.GetChild(i).gameObject);
            }
        }

        onRemove?.Invoke();
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

    private string GetWorldNamePosH(IsometricVector Pos)
    {
        return Pos.HInt.ToString();
    }

    public void SetWorldOrder()
    {
        m_worldPosH = m_worldPosH.OrderByDescending(h => h.PosH).ToList();
        for (int i = 0; i < m_worldPosH.Count; i++)
            m_worldPosH[i] = new IsometricDataWorldPosH(m_worldPosH[i].PosH, m_worldPosH[i].Block.OrderByDescending(a => a.Pos.X).OrderByDescending(b => b.Pos.Y).ToList());
    }

    #endregion

    #endregion

    #region ======================================================================== Editor

    //Required "IsoBlockRenderer.cs" component for each Block!

    public bool SetEditorMask(IsometricVector Pos, Color Mask, Color UnMask, Color Centre)
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

[Serializable]
public class IsometricDataWorldPosH
{
    public int PosH;
    public List<IsometricBlock> Block;

    public IsometricDataWorldPosH (int PosH)
    {
        this.PosH = PosH;
        this.Block = new List<IsometricBlock>();
    }

    public IsometricDataWorldPosH(int PosH, List<IsometricBlock> Block)
    {
        this.PosH = PosH;
        this.Block = Block;
    }
}

[Serializable]
public class IsometricDataWorldTag
{
    public string Tag;
    public List<IsometricBlock> Block;

    public IsometricDataWorldTag(string Tag)
    {
        this.Tag = Tag;
        this.Block = new List<IsometricBlock>();
    }

    public IsometricDataWorldTag(string Tag, List<IsometricBlock> Block)
    {
        this.Tag = Tag;
        this.Block = Block;
    }
}