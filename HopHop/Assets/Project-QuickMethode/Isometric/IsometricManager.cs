using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IsometricManager : MonoBehaviour
{
    #region Varible: Game Config

    [SerializeField] private IsometricConfig m_isometricConfig;

    private string m_debugError = "";

    public IsometricConfig IsometricConfig => m_isometricConfig;

    #endregion

    #region Varible: World Manager

    public IsometricGameData Game;
    //
    public IsometricDataWorld World;
    //
    public IsometricDataList List;

    #endregion

    private void Awake()
    {
#if UNITY_EDITOR
        SetConfigFind();
#endif
    }

    public void SetInit()
    {
        World = new IsometricDataWorld(this);
        List = new IsometricDataList();
    }

#if UNITY_EDITOR

    public void SetConfigFind()
    {
        if (m_isometricConfig != null)
            return;
        //
        var AuthorConfigFound = QAssetsDatabase.GetScriptableObject<IsometricConfig>("");
        //
        if (AuthorConfigFound == null)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Message] " + m_debugError);
            return;
        }
        //
        if (AuthorConfigFound.Count == 0)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Message] " + m_debugError);
            return;
        }
        //
        if (AuthorConfigFound.Count > 1)
            Debug.Log("[Message] Config found more than one, get the first one found");
        //
        m_isometricConfig = AuthorConfigFound[0];
        //
        m_debugError = "";
    }

#endif
}

[Serializable]
public class IsometricGameData
{
    public string Name = "";
    public List<string> Command = new List<string>();
    public IsometricGameDataScene Scene = new IsometricGameDataScene();
}

[Serializable]
public class IsometricGameDataScene
{
    public IsometricRendererType Renderer = IsometricRendererType.H;
    public IsometricRotateType Rotate = IsometricRotateType._0;
    public IsometricVector Centre = new IsometricVector();
    public IsometricVector Scale = new IsometricVector(1f, 1f, 1f);
}

public enum IsometricPosType
{
    Track,
    Free,
}

public enum IsometricRendererType
{
    XY,
    H,
    None,
}

public enum IsometricRotateType
{
    _0,
    _90,
    _180,
    _270,
}

public enum DataBlockType
{
    Forward = 0,
    Loop = 1,
    Revert = 2,
}

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricManager))]
public class IsometricManagerEditor : Editor
{
    private IsometricManager m_target;

    private SerializedProperty m_isometricConfig;

    private SerializedProperty Game;
    private SerializedProperty World;
    private SerializedProperty List;

    private void OnEnable()
    {
        m_target = target as IsometricManager;
        //
        m_isometricConfig = QEditorCustom.GetField(this, "m_isometricConfig");
        //
        Game = QEditorCustom.GetField(this, "Game");
        World = QEditorCustom.GetField(this, "World");
        List = QEditorCustom.GetField(this, "List");
        //
        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QEditorCustom.SetUpdate(this);
        //
        QEditorCustom.SetField(m_isometricConfig);
        //
        QEditorCustom.SetField(Game);
        QEditorCustom.SetField(World);
        QEditorCustom.SetField(List);
        //
        QEditor.SetDisableGroupEnd();
        //
        QEditorCustom.SetApply(this);
    }
}

#endif