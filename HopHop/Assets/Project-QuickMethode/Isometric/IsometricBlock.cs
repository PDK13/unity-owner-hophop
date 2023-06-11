using QuickMethode;
using UnityEngine;

[ExecuteAlways]
public class IsometricBlock : MonoBehaviour
{
    #region Varible: Block Manager

    [Header("Block Manager")]
    [SerializeField] private string m_tag = "";
    [SerializeField] private string m_desciption = "";
    
    #endregion

    #region Varible: World Manager

    [Header("World Manager")]
    [SerializeField] private IsometricManager.IsoType m_renderer = IsometricManager.IsoType.H;
    [SerializeField] private IsoVector m_pos = new IsoVector();

    private IsoVector m_posPrimary = new IsoVector();

    #endregion

    #region Varible: Data Manager

    [Header("Data Manager")]
    [SerializeField] private IsoDataBlockSingle m_data = new IsoDataBlockSingle();

    #endregion

    #region Varible: Scene Manager

    [Header("Scene Manager")]
    [SerializeField] private IsoVector m_scale = new IsoVector(1f, 1f, 1f);
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

    public string Name => QGameObject.GetNameReplaceClone(this.name);

    public string Tag => m_tag;

    public string Desciption => (m_desciption != "") ? m_desciption : "...";

    #endregion

    #region ================================================================== World Manager

    public IsometricManager.IsoType Renderer { get => m_renderer; set => m_renderer = value; }

    public IsoVector Pos { get => m_pos; set { m_pos = value; SetIsoTransform(); } }

    public IsoVector PosPrimary { get => m_posPrimary; set => m_posPrimary = value; }

    public void SetPosBackPrimary()
    {
        Pos = PosPrimary;
    }

    #endregion

    #region ================================================================== Data Manager

    public IsoDataBlockSingle Data { get => m_data; set => m_data = value; }

    #endregion

    #region ================================================================== Scene Manager

    private Vector3 GetIsoScene(IsoVector PosWorld)
    {
        switch (m_renderer)
        {
            case IsometricManager.IsoType.H:
                {
                    IsoVector PosWorldFinal = new IsoVector(PosWorld);
                    PosWorldFinal.X *= m_scale.X * 0.5f * -1;
                    PosWorldFinal.Y *= m_scale.Y * 0.5f;
                    PosWorldFinal.H *= m_scale.H * 0.5f;

                    float PosX = PosWorldFinal.X + PosWorldFinal.Y;
                    float PosY = 0.5f * (PosWorldFinal.Y - PosWorldFinal.X) + PosWorldFinal.H;
                    float PosZ = PosWorld.X + PosWorld.Y - PosWorld.H;

                    return new Vector3(PosX, PosY, PosZ);
                }
            case IsometricManager.IsoType.XY:
                {
                    IsoVector PosWorldFinal = new IsoVector(PosWorld);
                    PosWorldFinal.X *= m_scale.X * 0.5f * -1;
                    PosWorldFinal.Y *= m_scale.Y * 0.5f;
                    PosWorldFinal.H *= m_scale.H * 0.5f;

                    float PosX = PosWorldFinal.X + PosWorldFinal.Y;
                    float PosY = 0.5f * (PosWorldFinal.Y - PosWorldFinal.X) + PosWorldFinal.H;
                    float PosZ = (PosWorld.Y + PosWorld.X) - PosWorld.H * 2;

                    return new Vector3(PosX, PosY, PosZ);
                }
            case IsometricManager.IsoType.None:
                {
                    //Testing
                    IsoVector PosWorldFinal = new IsoVector(PosWorld);
                    PosWorldFinal.X *= m_scale.X * 0.5f * -1;
                    PosWorldFinal.Y *= m_scale.Y * 0.5f;
                    PosWorldFinal.H *= m_scale.H * 0.5f;

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
        {
            this.m_renderer = WorldManager.WorldRenderer;
            this.m_scale = WorldManager.WorldScale;
        }

        Vector3 PosTransform = GetIsoScene(m_pos);

        PosTransform += (Vector3)m_centre;

        transform.position = PosTransform;
    }

    #endregion
}