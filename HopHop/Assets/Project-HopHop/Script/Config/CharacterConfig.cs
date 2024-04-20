using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "character-config", menuName = "HopHop/Character Config", order = 0)]
public class CharacterConfig : ScriptableObject
{
    [Tooltip("Find all animator got true name exist in their name")]
    [SerializeField] private string m_animatorPath = "Assets/Project-HopHop/Animation/Character";
    [SerializeField] private string m_avatarPath = "Assets/Project-HopHop/Sprite/Character";

    [Space]
    public CharacterConfigData Angel;
    public CharacterConfigData Devil;
    public CharacterConfigData Bunny;
    public CharacterConfigData Cat;
    public CharacterConfigData Frog;
    public CharacterConfigData Mow;
    //
    public CharacterConfigData Alphaca;
    public CharacterConfigData Bug;
    public CharacterConfigData Fish;
    public CharacterConfigData Mole;
    public CharacterConfigData Pig;
    public CharacterConfigData Wolf;

    public CharacterConfigData GetConfig(CharacterType Character)
    {
        switch (Character)
        {
            case CharacterType.Angel:
                return Angel;
            case CharacterType.Devil:
                return Devil;
            case CharacterType.Bunny:
                return Bunny;
            case CharacterType.Cat:
                return Cat;
            case CharacterType.Frog:
                return Frog;
            case CharacterType.Mow:
                return Mow;
            //
            case CharacterType.Alphaca:
                return Alphaca;
            case CharacterType.Bug:
                return Bug;
            case CharacterType.Fish:
                return Fish;
            case CharacterType.Mole:
                return Mole;
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
        QUnityEditor.SetDirty(this);
    }

    private void SetRefreshCharacter()
    {
        SetRefreshCharacter(CharacterType.Angel, "Angel");
        SetRefreshCharacter(CharacterType.Devil, "Devil");
        SetRefreshCharacter(CharacterType.Bunny, "Bunny");
        SetRefreshCharacter(CharacterType.Cat, "Cat");
        SetRefreshCharacter(CharacterType.Frog, "Frog");
        SetRefreshCharacter(CharacterType.Mow, "Mow");
        //
        SetRefreshCharacter(CharacterType.Alphaca, "Alphaca");
        SetRefreshCharacter(CharacterType.Bug, "Bug");
        SetRefreshCharacter(CharacterType.Fish, "Fish");
        SetRefreshCharacter(CharacterType.Mole, "Mole");
        SetRefreshCharacter(CharacterType.Pig, "Pig");
        SetRefreshCharacter(CharacterType.Wolf, "Wolf");
    }

    private void SetRefreshCharacter(CharacterType Type, string Name)
    {
        GetConfig(Type).Skin.Clear();
        List<RuntimeAnimatorController> AssetsAnimation = QUnityAssets.GetAnimatorController(Name, true, m_animatorPath);
        List<Sprite> AssetsAvatar = QUnityAssets.GetSprite(string.Format("avt {0}", true, Name), true, m_avatarPath);
        for (int i = 0; i < AssetsAnimation.Count; i++)
            GetConfig(Type).Skin.Add(new CharacterConfigSkinData(AssetsAvatar[i], AssetsAnimation[i]));
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
    [Min(0)] public int MoveStep = 1;
    public bool MoveLock = true;
    public bool MoveFloat = false;

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
        QUnityEditor.SetSpace();
        //
        if (QUnityEditor.SetButton("Refresh"))
        {
            Target.SetRefresh();
            QUnityEditor.SetDirty(this);
        }
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif