using System;
using UnityEngine;
using static IsometricManager;

[Serializable]
public class IsoDataScene
{
    public RendererType Renderer = RendererType.H;
    public RotateType Rotate = RotateType._0;
    public IsoVector Scale = new IsoVector(1f, 1f, 1f);
}