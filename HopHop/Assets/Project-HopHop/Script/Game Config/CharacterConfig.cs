using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "character-config", menuName = "", order = 0)]
public class CharacterConfig : ScriptableObject
{
    [Tooltip("Find all assets got true name exist in their name")]
    [SerializeField] private string m_assetsPath = "Assets/Project-HopHop/Animation";

    [Space]
    public CharacterConfigData Alphaca;
    public CharacterConfigData Angel;
    public CharacterConfigData Bug;
    public CharacterConfigData Bunny;
    public CharacterConfigData Cat;
    public CharacterConfigData Devil;
    public CharacterConfigData Fish;
    public CharacterConfigData Frog;
    public CharacterConfigData Mole;
    public CharacterConfigData Mow;
    public CharacterConfigData Pig;
    public CharacterConfigData Wolf;

    public CharacterConfigData GetConfig(CharacterType Character)
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
        SetRefreshCharacter();
    }

    private void SetRefreshCharacter()
    {
        List<RuntimeAnimatorController> AssetsGet;
        //
        GetConfig(CharacterType.Alphaca).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Alphaca", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Alphaca).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Angel).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Angel", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Angel).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Bug).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Bug", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Bug).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Bunny).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Bunny", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Bunny).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Cat).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Cat", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Cat).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Devil).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Devil", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Devil).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Fish).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Fish", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Fish).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Frog).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Frog", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Frog).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Mole).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Mole", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Mole).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Mow).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Mow", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Mow).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Pig).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Pig", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Pig).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
        //
        GetConfig(CharacterType.Wolf).Skin.Clear();
        AssetsGet = QUnityAssets.GetAnimatorController("Wolf", m_assetsPath);
        for (int i = 0; i < AssetsGet.Count; i++)
            GetConfig(CharacterType.Wolf).Skin.Add(new CharacterConfigSkinData(null, AssetsGet[i]));
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
public class CharacterConfigData
{
    public List<CharacterConfigSkinData> Skin;
}

[Serializable]
public class CharacterConfigSkinData
{
    public Sprite Avartar;
    public RuntimeAnimatorController Animator;

    public CharacterConfigSkinData(Sprite Avartar, RuntimeAnimatorController Animator)
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