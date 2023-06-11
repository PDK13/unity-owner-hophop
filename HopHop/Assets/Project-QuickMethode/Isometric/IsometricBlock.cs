using QuickMethode;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class IsometricBlock : MonoBehaviour
{
    #region Varible: Block Manager

    [SerializeField] private bool m_free = false;
    [SerializeField] private string m_tag = "";
    
    #endregion

    #region Varible: World Manager

    [SerializeField] private IsoVector m_pos = new IsoVector();

    private IsoVector m_posPrimary = new IsoVector();

    #endregion

    #region Varible: Data Manager

    [SerializeField] private IsoDataBlockSingle m_data = new IsoDataBlockSingle();

    #endregion

    #region Varible: Scene Manager

    [SerializeField] private IsoDataScene m_scene = new IsoDataScene();
    [SerializeField] private Vector3 m_centre = new Vector3();

    #endregion

    [HideInInspector] public IsometricManager WorldManager;

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

    public string Name => QGameObject.GetNameReplaceClone(this.name);

    public string Tag => m_tag;

    #endregion

    #region ================================================================== World Manager

    public IsoVector Pos { get => m_pos; set { m_pos = value; SetIsoTransform(); } }

    public IsoVector PosPrimary { get => m_posPrimary; set => m_posPrimary = value; }

    #endregion

    #region ================================================================== Data Manager

    public IsoDataBlockSingle Data { get => m_data; set => m_data = value; }

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

    public List<IsometricBlock> GetCheck(IsoVector Dir)
    {
        return WorldManager.GetWorldBlockCurrent(Pos.Fixed + Dir);
    }

    public List<IsometricBlock> GetCheck(IsoVector Dir, params string[] TagFind)
    {
        return WorldManager.GetWorldBlockCurrent(Pos.Fixed + Dir, TagFind);
    }

    #endregion
}