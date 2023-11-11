using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "level-config", menuName = "", order = 0)]
public class LevelConfig : ScriptableObject
{
    public List<GroupLevel> Level;
}

[Serializable]
public class GroupLevel
{
    public string Name = "";
    public List<TextAsset> Level;
}