using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "isometric-config", menuName = "QConfig/Isometric Block", order = 0)]
public class IsometricConfig : ScriptableObject
{
    public IsometricConfigBlockData Block = new IsometricConfigBlockData();
    public IsometricConfigMapData Map = new IsometricConfigMapData();

    public void Reset()
    {
        Block.SetRefresh();
        Map.SetRefresh();
    }
}

[Serializable]
public class IsometricConfigBlockData
{
    [Tooltip("Find all assets got tag exist in their name")]
    [SerializeField] private string m_assetsPath = "Assets/Project-HopHop/Block";

    [Serializable]
    private class ListData
    {
        public string Tag;
        public List<IsometricBlock> Block;
    }

    [SerializeField] private List<ListData> m_list;

    public List<IsometricBlock> ListAssets
    {
        get
        {
#if UNITY_EDITOR
            SetRefresh();
#endif
            List<IsometricBlock> BlockList = new List<IsometricBlock>();
            foreach (ListData DataCheck in m_list)
                foreach (IsometricBlock BlockCheck in DataCheck.Block)
                    BlockList.Add(BlockCheck);
            return BlockList;
        }
    }

    public List<string> ListName
    {
        get
        {
#if UNITY_EDITOR
            SetRefresh();
#endif
            List<string> BlockList = new List<string>();
            foreach (ListData DataCheck in m_list)
                foreach (IsometricBlock BlockCheck in DataCheck.Block)
                    BlockList.Add(BlockCheck.name);
            return BlockList;
        }
    }

#if UNITY_EDITOR

    public void SetRefresh()
    {
        List<IsometricBlock> AssetsGet;
        //
        foreach (ListData BlockSingle in m_list)
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

[Serializable]
public class IsometricConfigMapData
{
    [Tooltip("Find all assets got tag exist in their name")]
    [SerializeField] private string m_assetsPath = "Assets/Project-HopHop/Map";

    [Serializable]
    private class ListData
    {
        public string Tag = "";
        public List<TextAsset> Map;
    }

    [SerializeField] private List<ListData> m_list;

    public List<TextAsset> ListAssets
    {
        get
        {
#if UNITY_EDITOR
            SetRefresh();
#endif
            List<TextAsset> BlockList = new List<TextAsset>();
            foreach (ListData DataCheck in m_list)
                foreach (TextAsset BlockCheck in DataCheck.Map)
                    BlockList.Add(BlockCheck);
            return BlockList;
        }
    }

    public List<string> ListName
    {
        get
        {
#if UNITY_EDITOR
            SetRefresh();
#endif
            List<string> BlockList = new List<string>();
            foreach (ListData DataCheck in m_list)
                foreach (TextAsset BlockCheck in DataCheck.Map)
                    BlockList.Add(BlockCheck.name);
            return BlockList;
        }
    }

#if UNITY_EDITOR

    public void SetRefresh()
    {
        List<TextAsset> AssetsGet;
        //
        foreach (ListData BlockSingle in m_list)
        {
            AssetsGet = QUnityAssets.GetTextAsset(BlockSingle.Tag, QPath.ExtensionType.txt, m_assetsPath);
            BlockSingle.Map.Clear();
            for (int i = 0; i < AssetsGet.Count; i++)
                BlockSingle.Map.Add(AssetsGet[i]);
            //
            BlockSingle.Map = BlockSingle.Map.Where(x => x != null).ToList();
            BlockSingle.Map = BlockSingle.Map.OrderBy(t => t.name).ToList();
        }
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricConfig))]
public class IsometricConfigEditor : Editor
{
    private IsometricConfig m_target;

    private SerializedProperty Block;
    private SerializedProperty Map;

    private void OnEnable()
    {
        m_target = (target as IsometricConfig);

        Block = QUnityEditorCustom.GetField(this, "Block");
        Map = QUnityEditorCustom.GetField(this, "Map");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        QUnityEditorCustom.SetField(Block);
        QUnityEditorCustom.SetField(Map);

        if (QUnityEditor.SetButton("Refresh"))
        {
            m_target.Block.SetRefresh();
            m_target.Map.SetRefresh();
        }

        QUnityEditorCustom.SetApply(this);
    }
}

#endif