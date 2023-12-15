using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "isometric-config", menuName = "QConfig/Isometric Block", order = 0)]
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
            foreach (BlockListSingle BlockListCheck in m_blockList)
            {
                foreach (IsometricBlock BlockCheck in BlockListCheck.Block)
                {
                    BlockList.Add(BlockCheck);
                }
            }
            return BlockList;
        }
    }

    public void SetRefresh()
    {
        foreach (BlockListSingle BlockListCheck in m_blockList)
        {
            BlockListCheck.Block = BlockListCheck.Block.Where(x => x != null).ToList();
            BlockListCheck.Block = BlockListCheck.Block.OrderBy(t => t.name).ToList();
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricConfig))]
public class IsometricConfigEditor : Editor
{
    private IsometricConfig m_target;

    private SerializedProperty m_blockList;

    private void OnEnable()
    {
        m_target = (target as IsometricConfig);

        m_blockList = QUnityEditorCustom.GetField(this, "m_blockList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        QUnityEditorCustom.SetField(m_blockList);

        if (QUnityEditor.SetButton("Refresh"))
        {
            m_target.SetRefresh();
        }

        QUnityEditorCustom.SetApply(this);
    }
}

#endif