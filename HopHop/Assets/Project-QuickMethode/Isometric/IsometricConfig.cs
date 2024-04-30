using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "isometric-config", menuName = "Isometric/Isometric Config", order = 0)]
public class IsometricConfig : ScriptableObject
{
    public IsometricConfigBlockData Block = new IsometricConfigBlockData();
    public IsometricConfigMapData Map = new IsometricConfigMapData();

    public void Reset()
    {
        SetRefresh();
    }

    public void SetRefresh()
    {
#if UNITY_EDITOR
        Block.SetRefresh();
        Map.SetRefresh();
#endif
    }
}

#region BLOCK

[Serializable]
public class IsometricConfigBlockData
{
    [Tooltip("Find all assets got tag exist in their name")]
    public string AssetsPath = "Assets/Project-HopHop/Block";

    public List<IsometricConfigBlockDataList> List;

    public IsometricBlock[] ListAssets
    {
        get
        {
#if UNITY_EDITOR
            SetRefresh();
#endif
            List<IsometricBlock> BlockList = new List<IsometricBlock>();
            foreach (var DataCheck in List)
                foreach (var BlockCheck in DataCheck.Block)
                    BlockList.Add(BlockCheck);
            return BlockList.ToArray();
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
            foreach (var DataCheck in List)
                foreach (var BlockCheck in DataCheck.Block)
                    BlockList.Add(BlockCheck.name);
            return BlockList;
        }
    }

#if UNITY_EDITOR

    public void SetRefresh()
    {
        List<IsometricBlock> AssetsGet;
        //
        foreach (var BlockSingle in List)
        {
            AssetsGet = QUnityAssets.GetPrefab<IsometricBlock>(BlockSingle.Name, false, AssetsPath);
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
public class IsometricConfigBlockDataList
{
    public string Name;
    public List<IsometricBlock> Block = new List<IsometricBlock>();
}

#endregion

#region MAP

[Serializable]
public class IsometricConfigMapData
{
    [Tooltip("Find all assets got tag exist in their name")]
    public string AssetsPath = "Assets/Project-HopHop/Map";

    public List<IsometricConfigMapDataList> List;

    public TextAsset[] ListAssets
    {
        get
        {
#if UNITY_EDITOR
            SetRefresh();
#endif
            List<TextAsset> BlockList = new List<TextAsset>();
            foreach (var DataCheck in List)
                foreach (var BlockCheck in DataCheck.Map)
                    BlockList.Add(BlockCheck);
            return BlockList.ToArray();
        }
    }

    public string[] ListName
    {
        get
        {
#if UNITY_EDITOR
            SetRefresh();
#endif
            List<string> BlockList = new List<string>();
            foreach (var DataCheck in List)
                foreach (var BlockCheck in DataCheck.Map)
                    BlockList.Add(BlockCheck.name);
            return BlockList.ToArray();
        }
    }

#if UNITY_EDITOR

    public void SetRefresh()
    {
        List<TextAsset> AssetsGet;
        //
        foreach (var BlockSingle in List)
        {
            AssetsGet = QUnityAssets.GetTextAsset(BlockSingle.Name, false, QPath.ExtensionType.txt, AssetsPath);
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

[Serializable]
public class IsometricConfigMapDataList
{
    public string Name = "";
    public List<TextAsset> Map;
}

#endregion

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