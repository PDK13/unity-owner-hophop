using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class IsometricManager : SingletonManager<IsometricManager>
{
    public IsometricConfig Config;

    [Space]
    public IsometricGameDataScene Scene = new IsometricGameDataScene();
    public IsometricManagerWorld World;
    public IsometricManagerList List = new IsometricManagerList();

    private void Reset()
    {
        SetEditorConfigFind();
    }

    public void SetEditorConfigFind()
    {
#if UNITY_EDITOR
        if (this.Config != null)
            return;
        //
        var Config = QUnityAssets.GetScriptableObject<IsometricConfig>("");
        //
        if (Config == null)
        {
            Debug.Log("[Message] Config not found, please create one");
            return;
        }
        //
        if (Config.Count == 0)
        {
            Debug.Log("[Message] Config not found, please create one");
            return;
        }
        //
        if (Config.Count > 1)
            Debug.Log("[Message] Config found more than one, get the first one found");
        //
        this.Config = Config[0];
#endif
    }

    public void SetEditorDataRefresh()
    {
        World = new IsometricManagerWorld(this);
        List = new IsometricManagerList(Config, true);
    }
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

    private SerializedProperty Config;

    private SerializedProperty Scene;
    private SerializedProperty World;
    private SerializedProperty List;

    private void OnEnable()
    {
        m_target = target as IsometricManager;
        //
        Config = QUnityEditorCustom.GetField(this, "Config");
        //
        Scene = QUnityEditorCustom.GetField(this, "Scene");
        World = QUnityEditorCustom.GetField(this, "World");
        List = QUnityEditorCustom.GetField(this, "List");
        //
        m_target.SetEditorConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(Config);
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