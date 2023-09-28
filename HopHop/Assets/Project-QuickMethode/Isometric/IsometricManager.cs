using System;
using System.Collections.Generic;
using UnityEngine;

public class IsometricManager : MonoBehaviour
{
    #region Varible: Game Config

    [SerializeField] private IsometricConfig m_config;

    public IsometricConfig Config => m_config;

    #endregion

    #region Varible: World Manager

    public IsometricGameData Game;
    //
    public IsometricDataWorld World;
    //
    public IsometricDataList List;

    #endregion

    public void SetInit()
    {
        World = new IsometricDataWorld(this);
        List = new IsometricDataList();
    }
}

[Serializable]
public class IsometricGameData
{
    public string Name = "";
    public List<string> Command = new List<string>();
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