using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class IsometricBlock : MonoBehaviour
{
    #region Block Manager

    [SerializeField] private string m_name = "";
    [SerializeField] private List<string> m_tag = new List<string>();

    #endregion

    #region World Manager

    [Space]
    [SerializeField] private IsometricPosType m_posType = IsometricPosType.Track;

    //After generate map, this pos will be use to renderer in scene!
    [SerializeField] private IsometricVector m_pos = new IsometricVector();

    //While generate map, this pos will be use to create this block in scene!
    [SerializeField] private IsometricVector m_posPrimary = new IsometricVector();

    #endregion

    #region Scene Manager

    [Space]
    [SerializeField] private IsometricGameDataScene m_sceneData = new IsometricGameDataScene();
    [SerializeField] private Vector3 m_centre = new Vector3();

    #endregion

    private IsometricManager m_worldManager;

    #region Mono

    private void Awake()
    {
        if (Application.isPlaying && this.name == IsometricManagerMap.NAME_CURSON)
            Destroy(this.gameObject);
    }

#if UNITY_EDITOR

    private void Update()
    {
        SetIsometricTransform();
    }

#endif

    #endregion

    #region Block Manager

    public string Name => !string.IsNullOrEmpty(m_name) ? m_name : QGameObject.GetNameReplaceClone(name);

    public List<string> Tag => m_tag;

    public IsometricVector Pos { get => m_pos; set { m_pos = value; SetIsometricTransform(); } }

    public IsometricPosType PosType => m_posType;

    public IsometricVector PosPrimary { get => m_posPrimary; set => m_posPrimary = value; }

    public bool GetTag(string Tag)
    {
        return m_tag.Contains(Tag);
    }

    #endregion

    #region World Manager

    public IsometricManager WorldManager
    {
        get => m_worldManager;
        set
        {
            m_worldManager = value;
            m_sceneData = value.Scene;
        }
    }

    #endregion

    #region Scene Manager

    private Vector3 GetIsometricTransform(IsometricVector Pos)
    {
        IsometricVector PosCentre = m_sceneData.Centre;
        float Angle = 0;
        //
        switch (m_sceneData.Rotate)
        {
            case IsometricRotateType._0:
                Angle = 0 * Mathf.Deg2Rad;
                break;
            case IsometricRotateType._90:
                Angle = 90f * Mathf.Deg2Rad;
                break;
            case IsometricRotateType._180:
                Angle = 180f * Mathf.Deg2Rad;
                break;
            case IsometricRotateType._270:
                Angle = 270f * Mathf.Deg2Rad;
                break;
        }
        IsometricVector PosValue = new IsometricVector(Pos)
        {
            X = (Pos.X - PosCentre.X) * Mathf.Cos(Angle) - (Pos.Y - PosCentre.Y) * Mathf.Sin(Angle) + PosCentre.X,
            Y = (Pos.X - PosCentre.X) * Mathf.Sin(Angle) + (Pos.Y - PosCentre.Y) * Mathf.Cos(Angle) + PosCentre.Y
        };
        //
        Vector3 PosTransform = new Vector3();
        IsometricVector PosValueScale = PosValue;
        //
        //
        switch (m_sceneData.Renderer)
        {
            case IsometricRendererType.H:
                PosValueScale.X *= m_sceneData.Scale.X * 0.5f * -1;
                PosValueScale.Y *= m_sceneData.Scale.Y * 0.5f;
                PosValueScale.H *= m_sceneData.Scale.H * 0.5f;
                //
                PosTransform.x = PosValueScale.X + PosValueScale.Y;
                PosTransform.y = 0.5f * (PosValueScale.Y - PosValueScale.X) + PosValueScale.H;
                PosTransform.z = PosValue.X + PosValue.Y - PosValue.H;
                //
                break;
            case IsometricRendererType.XY:
                PosValueScale.X *= m_sceneData.Scale.X * 0.5f * -1;
                PosValueScale.Y *= m_sceneData.Scale.Y * 0.5f;
                PosValueScale.H *= m_sceneData.Scale.H * 0.5f;
                //
                PosTransform.x = PosValueScale.X + PosValueScale.Y;
                PosTransform.y = 0.5f * (PosValueScale.Y - PosValueScale.X) + PosValueScale.H;
                PosTransform.z = (PosValue.Y + PosValue.X) - PosValue.H * 2;
                //
                break;
            case IsometricRendererType.None: //Testing
                PosValueScale.X *= m_sceneData.Scale.X * 0.5f * -1;
                PosValueScale.Y *= m_sceneData.Scale.Y * 0.5f;
                PosValueScale.H *= m_sceneData.Scale.H * 0.5f;
                //
                PosTransform.x = PosValueScale.X + PosValueScale.Y;
                PosTransform.y = 0.5f * (PosValueScale.Y - PosValueScale.X) + PosValueScale.H;
                PosTransform.z = 0;
                //
                break;
        }
        //
        return PosTransform;
    }

    private void SetIsometricTransform()
    {
        if (WorldManager != null)
        {
            m_sceneData = WorldManager.Scene;
        }

        Vector3 PosTransform = GetIsometricTransform(m_pos);

        PosTransform += m_centre;

        transform.position = PosTransform;
    }

    #endregion

    #region World Check

    public IsometricBlock GetBlock(IsometricVector Dir, params string[] TagFind)
    {
        return WorldManager.World.Current.GetBlockCurrent(Pos.Fixed + Dir, TagFind);
    }

    public IsometricBlock GetBlock(IsometricVector Dir, IsometricVector DirNext, params string[] TagFind)
    {
        return WorldManager.World.Current.GetBlockCurrent(Pos.Fixed + Dir + DirNext, TagFind);
    }

    //

    public List<IsometricBlock> GetBlockAll(IsometricVector Dir, params string[] TagFind)
    {
        return WorldManager.World.Current.GetBlockCurrentAll(Pos.Fixed + Dir, TagFind);
    }

    public List<IsometricBlock> GetBlockAll(IsometricVector Dir, IsometricVector DirNext, params string[] TagFind)
    {
        return WorldManager.World.Current.GetBlockCurrentAll(Pos.Fixed + Dir + DirNext, TagFind);
    }

    #endregion

    #region World Editor

    public void SetSpriteAlpha(float Alpha)
    {
        QColor.SetSprite(GetComponent<SpriteRenderer>(), Alpha);
    }

    public void SetSpriteColor(Color Color, float Alpha = 1)
    {
        GetComponent<SpriteRenderer>().color = Color;
        SetSpriteAlpha(Alpha);
    }

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricBlock))]
[CanEditMultipleObjects]
public class IsometricBlockEditor : Editor
{
    private IsometricBlock m_target;

    private SerializedProperty m_name;
    private SerializedProperty m_tag;

    private SerializedProperty m_posType;
    private SerializedProperty m_pos;
    private SerializedProperty m_posPrimary;

    private SerializedProperty m_sceneData;
    private SerializedProperty m_centre;

    private void OnEnable()
    {
        m_target = target as IsometricBlock;
        //
        m_name = QUnityEditorCustom.GetField(this, "m_name");
        m_tag = QUnityEditorCustom.GetField(this, "m_tag");

        m_posType = QUnityEditorCustom.GetField(this, "m_posType");
        m_pos = QUnityEditorCustom.GetField(this, "m_pos");
        m_posPrimary = QUnityEditorCustom.GetField(this, "m_posPrimary");

        m_sceneData = QUnityEditorCustom.GetField(this, "m_sceneData");
        m_centre = QUnityEditorCustom.GetField(this, "m_centre");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_name);
        QUnityEditorCustom.SetField(m_tag);

        QUnityEditorCustom.SetField(m_posType);
        QUnityEditorCustom.SetField(m_pos);
        QUnityEditorCustom.SetField(m_posPrimary);

        QUnityEditorCustom.SetField(m_sceneData);
        QUnityEditorCustom.SetField(m_centre);
        //
        QUnityEditor.SetSpace();
        //
        QUnityEditor.SetHorizontalBegin();
        if (QUnityEditor.SetButton("INIT"))
            QComponent.GetComponent<IsometricDataInit>(m_target);
        if (QUnityEditor.SetButton("MOVE"))
            QComponent.GetComponent<IsometricDataMove>(m_target);
        QUnityEditor.SetHorizontalEnd();
        QUnityEditor.SetHorizontalBegin();
        if (QUnityEditor.SetButton("ACTION"))
            QComponent.GetComponent<IsometricDataAction>(m_target);
        if (QUnityEditor.SetButton("TELEPORT"))
            QComponent.GetComponent<IsometricDataTeleport>(m_target);
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif