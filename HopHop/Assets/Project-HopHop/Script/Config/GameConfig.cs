using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "game-config", menuName = "Game Config", order = 0)]
public class GameConfig : ScriptableObject
{
    public List<GroupLevel> m_level;
}

[Serializable]
public class GroupLevel
{
    public string Name = "";
    public List<TextAsset> Level;
}