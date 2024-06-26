using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterManager : SingletonManager<CharacterManager>
{
    #region Action

    public Action onCharacter;

    #endregion

    #region Config

    [SerializeField] private CharacterConfig m_characterConfig;

    #endregion

    #region Party

    private int m_characterIndex = 0;
    private List<CharacterType> m_character = new List<CharacterType>()
    {
        CharacterType.Angel,
        CharacterType.Bunny,
        CharacterType.Cat,
        CharacterType.Frog,
        CharacterType.Mow,
    };

    #endregion

    #region Get

    public CharacterConfig CharacterConfig => m_characterConfig;

    public int CharacterIndex => m_characterIndex;

    public CharacterType CharacterCurrent => m_character[m_characterIndex];

    public CharacterType[] Character => m_character.ToArray();

    #endregion

    private void Awake()
    {
        SetInstance();
    }

    #region Config

#if UNITY_EDITOR

    public void SetConfigFind()
    {
        if (m_characterConfig != null)
            return;

        var CharacterConfigFound = QUnityAssets.GetScriptableObject<CharacterConfig>("", false);

        if (CharacterConfigFound == null)
        {
            Debug.Log("[Character] Config not found, please create one");
            return;
        }

        if (CharacterConfigFound.Count == 0)
        {
            Debug.Log("[Character] Config not found, please create one");
            return;
        }

        if (CharacterConfigFound.Count > 1)
            Debug.Log("[Character] Config found more than one, get the first one found");

        m_characterConfig = CharacterConfigFound[0];

        QUnityEditor.SetDirty(this);
    }

#endif

    #endregion

    public bool SetCharacterCurrent(CharacterType Character)
    {
        if (!Instance.m_character.Exists(t => t == Character))
            return false;
        Instance.m_characterIndex = Instance.m_character.FindIndex(t => t == Character);
        //
        onCharacter?.Invoke();
        //
        return true;
    }

    public void SetCharacterCurrent(int Index)
    {
        Instance.m_characterIndex = Mathf.Clamp(Index, 0, Instance.m_character.Count - 1);
        //
        onCharacter?.Invoke();
        //
    }

    public void SetCharacterNext()
    {
        Instance.m_characterIndex++;
        if (Instance.m_characterIndex > Instance.m_character.Count - 1)
            Instance.m_characterIndex = 0;
        //
        onCharacter?.Invoke();
        //
    }

    public void SetCharacterPrev()
    {
        Instance.m_characterIndex--;
        if (Instance.m_characterIndex < 0)
            Instance.m_characterIndex = Instance.m_character.Count - 1;
        //
        onCharacter?.Invoke();
        //
    }

    public bool SetCharacterPartyAdd(CharacterType Character)
    {
        if (Instance.m_character.Exists(t => t == Character))
            return false;
        Instance.m_character.Add(Character);
        return true;
    }

    public bool SetCharacterPartyRemove(CharacterType Character)
    {
        if (!Instance.m_character.Exists(t => t == Character))
            return false;
        Instance.m_character.RemoveAll(t => t == Character);
        return true;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CharacterManager))]
public class CharacterManagerEditor : Editor
{
    private CharacterManager m_target;

    private SerializedProperty m_characterConfig;

    private void OnEnable()
    {
        m_target = target as CharacterManager;

        m_characterConfig = QUnityEditorCustom.GetField(this, "m_characterConfig");

        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_characterConfig);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif