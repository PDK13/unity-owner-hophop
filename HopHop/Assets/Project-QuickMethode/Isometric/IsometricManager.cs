using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class IsometricManager : SingletonManager<IsometricManager>
{
    #region Varible: Game Config

    [SerializeField] private IsometricConfig m_isometricConfig;

    private string m_debugError = "";

    public IsometricConfig IsometricConfig => m_isometricConfig;

    #endregion

    #region Varible: World Manager

    public IsometricGameDataScene Scene = new IsometricGameDataScene();
    public IsometricManagerWorld World;
    public IsometricManagerList List = new IsometricManagerList();

    #endregion

    protected override void Awake()
    {
        base.Awake();
        //
        World = new IsometricManagerWorld(this);
        List = new IsometricManagerList();
        //
#if UNITY_EDITOR
        SetConfigFind();
#endif
    }

    private void Reset()
    {
        World = new IsometricManagerWorld(this);
        List = new IsometricManagerList();
        //
#if UNITY_EDITOR
        SetConfigFind();
#endif
    }

#if UNITY_EDITOR

    public void SetConfigFind()
    {
        if (m_isometricConfig != null)
            return;
        //
        var Config = QUnityAssets.GetScriptableObject<IsometricConfig>("", "");
        //
        if (Config == null)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Message] " + m_debugError);
            return;
        }
        //
        if (Config.Count == 0)
        {
            m_debugError = "Config not found, please create one";
            Debug.Log("[Message] " + m_debugError);
            return;
        }
        //
        if (Config.Count > 1)
            Debug.Log("[Message] Config found more than one, get the first one found");
        //
        m_isometricConfig = Config[0];
        //
        m_debugError = "";
    }

#endif
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

    private SerializedProperty Scene;
    private SerializedProperty World;
    private SerializedProperty List;

    private void OnEnable()
    {
        m_target = target as IsometricManager;
        //
        m_isometricConfig = QUnityEditorCustom.GetField(this, "m_isometricConfig");
        //
        Scene = QUnityEditorCustom.GetField(this, "Scene");
        World = QUnityEditorCustom.GetField(this, "World");
        List = QUnityEditorCustom.GetField(this, "List");
        //
        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_isometricConfig);
        //
        QUnityEditorCustom.SetField(Scene);
        //
        QUnityEditor.SetDisableGroupBegin();
        QUnityEditorCustom.SetField(World);
        QUnityEditorCustom.SetField(List);
        QUnityEditor.SetDisableGroupEnd();
        //
        QUnityEditor.SetDisableGroupEnd();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif