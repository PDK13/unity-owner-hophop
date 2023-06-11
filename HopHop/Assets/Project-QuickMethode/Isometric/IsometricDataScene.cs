using System;
using UnityEngine;
using static IsometricManager;

[Serializable]
public class IsoDataScene
{
    public IsoRendererType Renderer = IsoRendererType.H;
    public IsoVector Scale = new IsoVector(1f, 1f, 1f);
}