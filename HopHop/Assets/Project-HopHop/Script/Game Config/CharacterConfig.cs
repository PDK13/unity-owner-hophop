using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "character-config", menuName = "", order = 0)]
public class CharacterConfig : ScriptableObject
{
    [SerializeField] private string m_animatorPath = "Assets/Project-HopHop/Animation";

    [Space]
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

    public ConfigCharacter GetConfig(CharacterType Character)
    {
        switch (Character)
        {
            case CharacterType.Alphaca:
                return Alphaca;
            case CharacterType.Angel:
                return Angel;
            case CharacterType.Bug:
                return Bug;
            case CharacterType.Bunny:
                return Bunny;
            case CharacterType.Cat:
                return Cat;
            case CharacterType.Devil:
                return Devil;
            case CharacterType.Fish:
                return Fish;
            case CharacterType.Frog:
                return Frog;
            case CharacterType.Mole:
                return Mole;
            case CharacterType.Mow:
                return Mow;
            case CharacterType.Pig:
                return Pig;
            case CharacterType.Wolf:
                return Wolf;
        }
        //
        return null;
    }

    //Editor

#if UNITY_EDITOR

    public void SetRefresh()
    {
        SetRefreshAnimatorController();
    }

    private void SetRefreshAnimatorController()
    {
        List<RuntimeAnimatorController> AnimatorGet;
        //
        GetConfig(CharacterType.Alphaca).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Alphaca", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Alphaca).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Angel).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Angel", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Angel).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Bug).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Bug", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Bug).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Bunny).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Bunny", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Bunny).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Cat).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Cat", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Cat).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Devil).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Devil", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Devil).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Fish).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Fish", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Fish).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Frog).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Frog", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Frog).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Mole).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Mole", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Mole).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Mow).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Mow", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Mow).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Pig).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Pig", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Pig).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
        //
        GetConfig(CharacterType.Wolf).Skin.Clear();
        AnimatorGet = QUnityAssets.GetAnimatorController("Wolf", m_animatorPath);
        for (int i = 0; i < AnimatorGet.Count; i++)
            GetConfig(CharacterType.Wolf).Skin.Add(new ConfigCharacterSkin(null, AnimatorGet[i]));
    }

#endif
}

public enum CharacterType
{
    Angel = 0,
    Devil = 1,
    Bunny = 2,
    Cat = 3,
    Frog = 4,
    Mow = 5,
    Alphaca = 6,
    Bug = 7,
    Fish = 8,
    Mole = 9,
    Pig = 10,
    Wolf = 11,
}

[Serializable]
public class ConfigCharacter
{
    public List<ConfigCharacterSkin> Skin;
}

[Serializable]
public class ConfigCharacterSkin
{
    public Sprite Avartar;
    public RuntimeAnimatorController Animator;

    public ConfigCharacterSkin(Sprite Avartar, RuntimeAnimatorController Animator)
    {
        this.Avartar = Avartar;
        this.Animator = Animator;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CharacterConfig))]
public class CharacterConfigEditor : Editor
{
    private CharacterConfig Target;

    private void OnEnable()
    {
        Target = target as CharacterConfig;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //
        QUnityEditor.SetSpace(10);
        //
        if (QUnityEditor.SetButton("Refresh"))
            Target.SetRefresh();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif