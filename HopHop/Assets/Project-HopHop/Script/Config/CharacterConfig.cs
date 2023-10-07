using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "character-config", menuName = "", order = 0)]
public class CharacterConfig : ScriptableObject
{
    [SerializeField] private string m_path = "";

    public ConfigCharacter Alphaca;
    public ConfigCharacter Angel;
    public ConfigCharacter Bug;
    public ConfigCharacter Bunny;
    public ConfigCharacter Cat;
    public ConfigCharacter Devil;
    public ConfigCharacter Fish;
    public ConfigCharacter Frog;
    public ConfigCharacter Mole;
    public ConfigCharacter Mow;
    public ConfigCharacter Pig;
    public ConfigCharacter Wolf;

    public void SetLoadConfig()
    {

    }
}

[Serializable]
public class ConfigCharacter
{
    public List<RuntimeAnimatorController> m_skin;
}