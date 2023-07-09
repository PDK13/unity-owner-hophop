using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "game-config", menuName = "", order = 0)]
public class GameConfig : ScriptableObject
{
    public List<GroupLevel> m_level;

    public List<ConfigCharacter> m_character;
}

[Serializable]
public class GroupLevel
{
    public string Name = "";
    public List<TextAsset> Level;
}

[Serializable]
public class ConfigCharacter
{
    public List<RuntimeAnimatorController> m_skin;
}