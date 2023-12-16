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
    public IsometricConfigBlockData BlockData = new IsometricConfigBlockData();
}

[Serializable]
public class IsometricConfigBlockData
{
    [Tooltip("Find all assets got tag exist in their name")]
    [SerializeField] private string m_assetsPath = "Assets/Project-HopHop/Block";

    [Serializable]
    private class BlockListData
    {
        public string Tag;
        public List<IsometricBlock> Block;
    }

    [SerializeField] private List<BlockListData> m_list;

    public List<IsometricBlock> BlockListAll
    {
        get
        {
            List<IsometricBlock> BlockList = new List<IsometricBlock>();
            foreach (BlockListData BlockListCheck in m_list)
                foreach (IsometricBlock BlockCheck in BlockListCheck.Block)
                    BlockList.Add(BlockCheck);
            return BlockList;
        }
    }

#if UNITY_EDITOR

    public void SetRefresh()
    {
        List<IsometricBlock> AssetsGet;
        //
        foreach (BlockListData BlockSingle in m_list)
        {
            AssetsGet = QUnityAssets.GetPrefab<IsometricBlock>(BlockSingle.Tag, m_assetsPath);
            BlockSingle.Block.Clear();
            for (int i = 0; i < AssetsGet.Count; i++)
                BlockSingle.Block.Add(AssetsGet[i]);
            //
            BlockSingle.Block = BlockSingle.Block.Where(x => x != null).ToList();
            BlockSingle.Block = BlockSingle.Block.OrderBy(t => t.name).ToList();
        }
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricConfig))]
public class IsometricConfigEditor : Editor
{
    private IsometricConfig m_target;

    private SerializedProperty BlockData;

    private void OnEnable()
    {
        m_target = (target as IsometricConfig);

        BlockData = QUnityEditorCustom.GetField(this, "BlockData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        QUnityEditorCustom.SetField(BlockData);

        if (QUnityEditor.SetButton("Refresh"))
        {
            m_target.BlockData.SetRefresh();
        }

        QUnityEditorCustom.SetApply(this);
    }
}

#endif