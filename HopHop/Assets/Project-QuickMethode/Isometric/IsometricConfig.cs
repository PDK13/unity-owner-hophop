using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "isometric-config", menuName = "", order = 0)]
public class IsometricConfig : ScriptableObject
{
    [Serializable]
    private class BlockListSingle
    {
        public string Name;
        public List<IsometricBlock> Block;
    }

    [SerializeField] private List<BlockListSingle> m_blockList;

    public List<IsometricBlock> BlockList
    {
        get
        {
            List<IsometricBlock> BlockList = new List<IsometricBlock>();
            foreach(var BlockListCheck in this.m_blockList)
            {
                foreach(var BlockCheck in BlockListCheck.Block)
                {
                    BlockList.Add(BlockCheck);
                }
            }
            return BlockList;
        }
    }

    public void SetRefresh()
    {
        foreach(var BlockListCheck in m_blockList)
        {
            BlockListCheck.Block = BlockListCheck.Block.Where(x => x != null).ToList();
            BlockListCheck.Block = BlockListCheck.Block.OrderBy(t => t.name).ToList();
        }
    }
}