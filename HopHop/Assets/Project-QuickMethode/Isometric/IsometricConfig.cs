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
        SetEditorRefresh();
    }

#if UNITY_EDITOR

    public void SetEditorRefresh()
    {
        Block.SetEditorRefresh();
        Map.SetEditorRefresh();
    }

#endif
}

#region BLOCK

[Serializable]
public class IsometricConfigBlockData
{
    [Tooltip("Find all assets got tag exist in their name")]
    public string AssetsPath = "Assets/Project-HopHop/Block";

    public List<IsometricConfigBlockDataList> Data = new List<IsometricConfigBlockDataList>();

    public IsometricBlock[] DataAssets
    {
        get
        {
#if UNITY_EDITOR
            SetEditorRefresh();
#endif
            List<IsometricBlock> BlockList = new List<IsometricBlock>();
            foreach (var DataCheck in Data)
                foreach (var BlockCheck in DataCheck.Block)
                    BlockList.Add(BlockCheck);
            return BlockList.ToArray();
        }
    }

    public List<string> DataName
    {
        get
        {
#if UNITY_EDITOR
            SetEditorRefresh();
#endif
            List<string> BlockList = new List<string>();
            foreach (var DataCheck in Data)
                foreach (var BlockCheck in DataCheck.Block)
                    BlockList.Add(BlockCheck.name);
            return BlockList;
        }
    }

#if UNITY_EDITOR

    public void SetEditorRefresh()
    {
        List<IsometricBlock> AssetsGet;
        //
        foreach (var BlockSingle in Data)
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

    public int EditorDataListCount
    {
        get => Data.Count;
        set
        {
            while (Data.Count > value)
                Data.RemoveAt(Data.Count - 1);
            while (Data.Count < value)
                Data.Add(new IsometricConfigBlockDataList());
        }
    }

    public bool EditorDataListCommand { get; set; } = false;

#endif
}

[Serializable]
public class IsometricConfigBlockDataList
{
    public string Name;
    public List<IsometricBlock> Block = new List<IsometricBlock>();

#if UNITY_EDITOR

    public int EditorBlockListCount
    {
        get => Block.Count;
        set
        {
            while (Block.Count > value)
                Block.RemoveAt(Block.Count - 1);
            while (Block.Count < value)
                Block.Add(null);
        }
    }

    public bool EditorBlockListCommand { get; set; } = false;

    public bool EditorBlockListShow { get; set; } = false;

#endif
}

#endregion

#region MAP

[Serializable]
public class IsometricConfigMapData
{
    [Tooltip("Find all assets got tag exist in their name")]
    public string AssetsPath = "Assets/Project-HopHop/Map";

    public List<IsometricConfigMapDataList> Data = new List<IsometricConfigMapDataList>();

    public TextAsset[] DataAssets
    {
        get
        {
#if UNITY_EDITOR
            SetEditorRefresh();
#endif
            List<TextAsset> BlockList = new List<TextAsset>();
            foreach (var DataCheck in Data)
                foreach (var BlockCheck in DataCheck.Map)
                    BlockList.Add(BlockCheck);
            return BlockList.ToArray();
        }
    }

    public string[] DataName
    {
        get
        {
#if UNITY_EDITOR
            SetEditorRefresh();
#endif
            List<string> BlockList = new List<string>();
            foreach (var DataCheck in Data)
                foreach (var BlockCheck in DataCheck.Map)
                    BlockList.Add(BlockCheck.name);
            return BlockList.ToArray();
        }
    }

#if UNITY_EDITOR

    public void SetEditorRefresh()
    {
        List<TextAsset> AssetsGet;
        //
        foreach (var BlockSingle in Data)
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

    public int EditorDataListCount
    {
        get => Data.Count;
        set
        {
            while (Data.Count > value)
                Data.RemoveAt(Data.Count - 1);
            while (Data.Count < value)
                Data.Add(new IsometricConfigMapDataList());
        }
    }

    public bool EditorDataListCommand { get; set; } = false;

#endif
}

[Serializable]
public class IsometricConfigMapDataList
{
    public string Name = "";
    public List<TextAsset> Map = new List<TextAsset>();

#if UNITY_EDITOR

    public int EditorMapListCount
    {
        get => Map.Count;
        set
        {
            while (Map.Count > value)
                Map.RemoveAt(Map.Count - 1);
            while (Map.Count < value)
                Map.Add(null);
        }
    }

    public bool EditorMapListCommand { get; set; } = false;

    public bool EditorMapListShow { get; set; } = false;

#endif
}

#endregion

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricConfig))]
public class IsometricConfigEditor : Editor
{
    private const float POPUP_HEIGHT = 262f;
    private const float LABEL_WIDTH = 65f;

    private IsometricConfig m_target;

    private Vector2 m_scrollBlock;
    private Vector2 m_scrollMap;

    private void OnEnable()
    {
        m_target = (target as IsometricConfig);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        QUnityEditor.SetLabel("MAIN", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        if (QUnityEditor.SetButton("Refresh"))
        {
            m_target.Block.SetEditorRefresh();
            m_target.Map.SetEditorRefresh();
        }

        QUnityEditor.SetSpace();

        SetGUIGroupBlock();

        QUnityEditor.SetSpace();

        SetGUIGroupMap();

        QUnityEditorCustom.SetApply(this);

        QUnityEditor.SetDirty(m_target);
    }

    private void SetGUIGroupBlock()
    {
        QUnityEditor.SetLabel("BLOCK", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        #region ITEM - MAIN - NAME
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Path", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.Block.AssetsPath = QUnityEditor.SetField(m_target.Block.AssetsPath);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        //COUNT
        m_target.Block.EditorDataListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Data", m_target.Block.EditorDataListCount, 0);
        //LIST
        m_scrollBlock = QUnityEditor.SetScrollViewBegin(m_scrollBlock, QUnityEditor.GetGUILayoutHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.Block.EditorDataListCount; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.Block.EditorDataListCommand = !m_target.Block.EditorDataListCommand;

            #region ITEM - MAIN
            QUnityEditor.SetVerticalBegin();

            #region ITEM - MAIN - NAME
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Name", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
            m_target.Block.Data[i].Name = QUnityEditor.SetField(m_target.Block.Data[i].Name);
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ITEM - MAIN - LIST
            //COUNT
            var BlockListEditor = QUnityEditor.SetGroupNumberChangeLimitMin("Block", m_target.Block.Data[i].EditorBlockListShow, m_target.Block.Data[i].EditorBlockListCount, 0);
            m_target.Block.Data[i].EditorBlockListCount = BlockListEditor.Value;
            m_target.Block.Data[i].EditorBlockListShow = BlockListEditor.Switch;
            //LIST
            if (m_target.Block.Data[i].EditorBlockListShow)
            {
                for (int j = 0; j < m_target.Block.Data[i].EditorBlockListCount; j++)
                {
                    #region ITEM - MAIN - LIST - ITEM
                    QUnityEditor.SetHorizontalBegin();
                    if (QUnityEditor.SetButton(j.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                        m_target.Block.Data[i].EditorBlockListCommand = !m_target.Block.Data[i].EditorBlockListCommand;
                    m_target.Block.Data[i].Block[j] = QUnityEditor.SetFieldMonoBehaviour(m_target.Block.Data[i].Block[j]);
                    QUnityEditor.SetHorizontalEnd();
                    #endregion

                    #region ARRAY
                    if (m_target.Block.Data[i].EditorBlockListCommand)
                    {
                        QUnityEditor.SetHorizontalBegin();
                        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                        if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                            QList.SetSwap(m_target.Block.Data[i].Block, j, j - 1);
                        if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                            QList.SetSwap(m_target.Block.Data[i].Block, j, j + 1);
                        if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                        {
                            m_target.Block.Data[i].Block.RemoveAt(i);
                            m_target.Block.Data[i].EditorBlockListCount--;
                        }
                        QUnityEditor.SetHorizontalEnd();
                    }
                    #endregion

                }
            }
            #endregion

            QUnityEditor.SetVerticalEnd();
            #endregion

            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_target.Block.EditorDataListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Block.Data, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Block.Data, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    m_target.Block.Data.RemoveAt(i);
                    m_target.Block.EditorDataListCount--;
                }
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion

            QUnityEditor.SetSpace(10);
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private void SetGUIGroupMap()
    {
        QUnityEditor.SetLabel("MAP", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        #region ITEM - MAIN - NAME
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Path", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.Map.AssetsPath = QUnityEditor.SetField(m_target.Map.AssetsPath);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        //COUNT
        m_target.Map.EditorDataListCount = QUnityEditor.SetGroupNumberChangeLimitMin("Data", m_target.Map.EditorDataListCount, 0);
        //LIST
        m_scrollMap = QUnityEditor.SetScrollViewBegin(m_scrollMap, QUnityEditor.GetGUILayoutHeight(POPUP_HEIGHT));
        for (int i = 0; i < m_target.Map.EditorDataListCount; i++)
        {
            #region ITEM
            QUnityEditor.SetHorizontalBegin();
            if (QUnityEditor.SetButton(i.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                m_target.Map.EditorDataListCommand = !m_target.Map.EditorDataListCommand;

            #region ITEM - MAIN
            QUnityEditor.SetVerticalBegin();

            #region ITEM - MAIN - NAME
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetLabel("Name", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
            m_target.Map.Data[i].Name = QUnityEditor.SetField(m_target.Map.Data[i].Name);
            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ITEM - MAIN - LIST
            //COUNT
            var MapListEditor = QUnityEditor.SetGroupNumberChangeLimitMin("Map", m_target.Map.Data[i].EditorMapListShow, m_target.Map.Data[i].EditorMapListCount, 0);
            m_target.Map.Data[i].EditorMapListCount = MapListEditor.Value;
            m_target.Map.Data[i].EditorMapListShow = MapListEditor.Switch;
            //LIST
            if (m_target.Map.Data[i].EditorMapListShow)
            {
                for (int j = 0; j < m_target.Map.Data[i].EditorMapListCount; j++)
                {
                    #region ITEM - MAIN - LIST - ITEM
                    QUnityEditor.SetHorizontalBegin();
                    if (QUnityEditor.SetButton(j.ToString(), QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25)))
                        m_target.Map.Data[i].EditorMapListCommand = !m_target.Map.Data[i].EditorMapListCommand;
                    m_target.Map.Data[i].Map[j] = QUnityEditor.SetFieldTextAsset(m_target.Map.Data[i].Map[j]);
                    QUnityEditor.SetHorizontalEnd();
                    #endregion

                    #region ARRAY
                    if (m_target.Map.Data[i].EditorMapListCommand)
                    {
                        QUnityEditor.SetHorizontalBegin();
                        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                        if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                            QList.SetSwap(m_target.Map.Data[i].Map, j, j - 1);
                        if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                            QList.SetSwap(m_target.Map.Data[i].Map, j, j + 1);
                        if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                        {
                            m_target.Map.Data[i].Map.RemoveAt(i);
                            m_target.Map.Data[i].EditorMapListCount--;
                        }
                        QUnityEditor.SetHorizontalEnd();
                    }
                    #endregion

                }
            }
            #endregion

            QUnityEditor.SetVerticalEnd();
            #endregion

            QUnityEditor.SetHorizontalEnd();
            #endregion

            #region ARRAY
            if (m_target.Map.EditorDataListCommand)
            {
                QUnityEditor.SetHorizontalBegin();
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(), QUnityEditor.GetGUILayoutWidth(25));
                if (QUnityEditor.SetButton("↑", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Map.Data, i, i - 1);
                if (QUnityEditor.SetButton("↓", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                    QList.SetSwap(m_target.Map.Data, i, i + 1);
                if (QUnityEditor.SetButton("X", QUnityEditor.GetGUIStyleButton(), QUnityEditor.GetGUILayoutWidth(25)))
                {
                    m_target.Map.Data.RemoveAt(i);
                    m_target.Map.EditorDataListCount--;
                }
                QUnityEditor.SetHorizontalEnd();
            }
            #endregion

            QUnityEditor.SetSpace(10);
        }
        QUnityEditor.SetScrollViewEnd();
    }
}

#endif