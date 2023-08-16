using QuickMethode;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IsometricManager;

public class IsometricManager : MonoBehaviour
{
    #region Varible: Game Config

    [SerializeField] private IsometricConfig m_config;

    public IsometricConfig Config => m_config;

    #endregion

    #region Varible: World Manager

    public IsometricGameData GameData;
    //
    public IsometricDataWorld WorldData;
    //
    public IsometricDataList BlockList;

    #endregion

    public void SetInit()
    {
        WorldData = new IsometricDataWorld(this);
        BlockList = new IsometricDataList();
    }
}

[Serializable]
public class IsometricGameData
{
    public string Name = "";
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