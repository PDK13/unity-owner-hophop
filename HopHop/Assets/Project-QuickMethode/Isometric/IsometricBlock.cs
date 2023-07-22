using QuickMethode;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class IsometricBlock : MonoBehaviour
{
    #region Varible: Block Manager

    [Header("Manager")]
    [SerializeField] private string m_name = "";
    [SerializeField] private bool m_free = false;
    [SerializeField] private List<string> m_tag = new List<string>();

    #endregion

    #region Varible: World Manager

    [Header("World")]
    [SerializeField] private IsoVector m_pos = new IsoVector();

    private IsoVector m_posPrimary = new IsoVector();

    #endregion

    #region Varible: Data Manager

    [Header("Data")]
    [SerializeField] private IsoDataBlockMove MoveData = new IsoDataBlockMove();
    [SerializeField] private IsometricDataFollow FollowData = new IsometricDataFollow();
    [SerializeField] private IsoDataBlockAction ActionData = new IsoDataBlockAction();
    [SerializeField] private IsoDataBlockEvent EventData = new IsoDataBlockEvent();
    [SerializeField] private IsoDataBlockTeleport TeleportData = new IsoDataBlockTeleport();

    #endregion

    #region Varible: Scene Manager

    [Header("Scene")]
    [SerializeField] private IsoDataScene m_scene = new IsoDataScene();
    [SerializeField] private Vector3 m_centre = new Vector3();

    #endregion

    private IsometricManager m_worldManager;

    #region ================================================================== Mono

#if UNITY_EDITOR

    private void Update()
    {
        SetIsoTransform();
    }

#endif

    #endregion

    #region ================================================================== World Manager

    public bool Free => m_free;

    public string Name => m_name != "" ? m_name : QGameObject.GetNameReplaceClone(this.name);

    public List<string> Tag => m_tag;

    public IsometricManager WorldManager 
    { 
        get => m_worldManager;
        set
        {
            m_worldManager = value;
            m_scene = value.Scene;
        }
    }

    #endregion

    #region ================================================================== World Manager

    public IsoVector Pos { get => m_pos; set { m_pos = value; SetIsoTransform(); } }

    public IsoVector PosPrimary { get => m_posPrimary; set => m_posPrimary = value; }

    #endregion

    #region ================================================================== Data Manager

    public IsoDataBlockSingle Data 
    {
        get 
        {
            IsoDataBlockSingle Data = new IsoDataBlockSingle();
            Data.MoveData = MoveData;
            Data.FollowData = FollowData;
            Data.ActionData = ActionData;
            Data.EventData = EventData;
            Data.TeleportData = TeleportData;
            return Data;
        }
        set
        {
            MoveData = value.MoveData;
            FollowData = value.FollowData;
            ActionData = value.ActionData;
            EventData = value.EventData;
            TeleportData = value.TeleportData;
        }
    }

    #endregion

    #region ================================================================== Scene Manager

    private Vector3 GetIsoScene(IsoVector PosWorld)
    {
        switch (m_scene.Renderer)
        {
            case IsometricManager.IsoRendererType.H:
                {
                    IsoVector PosWorldFinal = new IsoVector(PosWorld);
                    PosWorldFinal.X *= m_scene.Scale.X * 0.5f * -1;
                    PosWorldFinal.Y *= m_scene.Scale.Y * 0.5f;
                    PosWorldFinal.H *= m_scene.Scale.H * 0.5f;

                    float PosX = PosWorldFinal.X + PosWorldFinal.Y;
                    float PosY = 0.5f * (PosWorldFinal.Y - PosWorldFinal.X) + PosWorldFinal.H;
                    float PosZ = PosWorld.X + PosWorld.Y - PosWorld.H;

                    return new Vector3(PosX, PosY, PosZ);
                }
            case IsometricManager.IsoRendererType.XY:
                {
                    IsoVector PosWorldFinal = new IsoVector(PosWorld);
                    PosWorldFinal.X *= m_scene.Scale.X * 0.5f * -1;
                    PosWorldFinal.Y *= m_scene.Scale.Y * 0.5f;
                    PosWorldFinal.H *= m_scene.Scale.H * 0.5f;

                    float PosX = PosWorldFinal.X + PosWorldFinal.Y;
                    float PosY = 0.5f * (PosWorldFinal.Y - PosWorldFinal.X) + PosWorldFinal.H;
                    float PosZ = (PosWorld.Y + PosWorld.X) - PosWorld.H * 2;

                    return new Vector3(PosX, PosY, PosZ);
                }
            case IsometricManager.IsoRendererType.None:
                {
                    //Testing
                    IsoVector PosWorldFinal = new IsoVector(PosWorld);
                    PosWorldFinal.X *= m_scene.Scale.X * 0.5f * -1;
                    PosWorldFinal.Y *= m_scene.Scale.Y * 0.5f;
                    PosWorldFinal.H *= m_scene.Scale.H * 0.5f;

                    float m_PosX = PosWorldFinal.X + PosWorldFinal.Y;
                    float m_PosY = 0.5f * (PosWorldFinal.Y - PosWorldFinal.X) + PosWorldFinal.H;
                    float m_PosZ = 0;

                    return new Vector3(m_PosX, m_PosY, m_PosZ);
                }
        }
        return new Vector3();
    }

    private void SetIsoTransform()
    {
        if (WorldManager != null)
            m_scene = WorldManager.Scene;

        Vector3 PosTransform = GetIsoScene(m_pos);

        PosTransform += (Vector3)m_centre;

        transform.position = PosTransform;
    }

    #endregion

    #region ================================================================== Check

    public List<IsometricBlock> GetCheck(IsoVector Dir, int Length)
    {
        return WorldManager.GetWorldBlockCurrentAll(Pos.Fixed + Dir * Length);
    }

    public List<IsometricBlock> GetCheck(IsoVector Dir, int Length, params string[] TagFind)
    {
        return WorldManager.GetWorldBlockCurrentAll(Pos.Fixed + Dir * Length, TagFind);
    }

    #endregion
}