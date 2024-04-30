using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricManagerList
{
    public Action onListRead;

    public List<IsometricDataBlockListTag> BlockList;

    public List<string> ListTag
    {
        get
        {
            List<string> ListTag = new List<string>();
            for (int i = 0; i < BlockList.Count; i++)
                ListTag.Add(BlockList[i].Tag);
            return ListTag;
        }
    }

    public IsometricManagerList()
    {
        BlockList = new List<IsometricDataBlockListTag>();
    }

    public IsometricManagerList(IsometricConfig IsometricConfig, bool All)
    {
        BlockList = new List<IsometricDataBlockListTag>();
        SetList(IsometricConfig, All);
    }

    #region Read

    public void SetList(IsometricConfig IsometricConfig, bool All)
    {
        SetList(IsometricConfig.Block.DataAssets, All);
    }

    public void SetList(IsometricBlock[] BlockList, bool All)
    {
        if (this.BlockList == null)
            this.BlockList = new List<IsometricDataBlockListTag>();
        else
            this.BlockList.Clear();
        //
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
                    this.BlockList.Add(new IsometricDataBlockListTag(TagCheck));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(BlockCheck);
                }
                else
                {
                    this.BlockList[TagIndex].Block.Add(BlockCheck);
                }
                //
                if (!All)
                    //If not need to check all tag of this block!
                    break;
            }
        }
        //
        onListRead?.Invoke();
    }

    public void SetList(GameObject[] BlockList, bool All)
    {
        if (this.BlockList == null)
            this.BlockList = new List<IsometricDataBlockListTag>();
        else
            this.BlockList.Clear();
        //
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
                    this.BlockList.Add(new IsometricDataBlockListTag(TagCheck));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(Block);
                }
                else
                {
                    this.BlockList[TagIndex].Block.Add(Block);
                }
                //
                if (!All)
                    //If not need to check all tag of this block!
                    break;
            }
        }
        //
        onListRead?.Invoke();
    }

    public void SetListPath(bool All, params string[] PathChildInResources)
    {
        if (this.BlockList == null)
            this.BlockList = new List<IsometricDataBlockListTag>();
        else
            this.BlockList.Clear();
        //
        GameObject[] BlockList = QResources.GetPrefab(PathChildInResources);
        //
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
                    this.BlockList.Add(new IsometricDataBlockListTag(""));
                    TagIndex = this.BlockList.Count - 1;
                    this.BlockList[TagIndex].Block.Add(Block);
                }
                else
                {
                    this.BlockList[TagIndex].Block.Add(Block);
                }
            }
            else
            {
                foreach (string TagCheck in TagFind)
                {
                    int TagIndex = GetIndexListTag(TagCheck);
                    if (TagIndex == -1)
                    {
                        this.BlockList.Add(new IsometricDataBlockListTag(TagCheck));
                        TagIndex = this.BlockList.Count - 1;
                        this.BlockList[TagIndex].Block.Add(Block);
                    }
                    else
                    {
                        this.BlockList[TagIndex].Block.Add(Block);
                    }
                    //
                    if (!All)
                        break;
                }
            }
        }
        //
        onListRead?.Invoke();
    }

    public GameObject GetList(string BlockName, string Tag = "")
    {
        if (!string.IsNullOrEmpty(Tag))
        {
            for (int i = 0; i < BlockList.Count; i++)
            {
                if (BlockList[i].Tag != Tag)
                {
                    continue;
                }

                foreach (IsometricBlock BlockCheck in BlockList[i].Block)
                {
                    if (BlockCheck.Name != BlockName)
                    {
                        continue;
                    }

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
                    {
                        continue;
                    }

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
            {
                continue;
            }

            return i;
        }
        return -1;
    }

    #endregion
}

[Serializable]
public class IsometricDataBlockListTag
{
    public string Tag;
    public List<IsometricBlock> Block;

    public IsometricDataBlockListTag(string Tag)
    {
        this.Tag = Tag;
        Block = new List<IsometricBlock>();
    }

    public IsometricDataBlockListTag(string Tag, List<IsometricBlock> Block)
    {
        this.Tag = Tag;
        this.Block = Block;
    }
}