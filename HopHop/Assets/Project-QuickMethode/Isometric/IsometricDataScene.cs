using System;
using UnityEngine;
using static IsometricManager;

[Serializable]
public class IsoDataScene
{
    #region Enum

    public enum RendererType { XY, H, None, }

    public enum RotateType { _0, _90, _180, _270, }

    #endregion

    public RendererType Renderer = RendererType.H;
    public RotateType Rotate = RotateType._0;
    public IsoVector Scale = new IsoVector(1f, 1f, 1f);
}