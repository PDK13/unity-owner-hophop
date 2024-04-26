using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : SingletonManager<CharacterManager>
{
    public Action onCharacter;

    //

    [SerializeField] private CharacterConfig m_characterConfig;

    //

    private int m_characterIndex = 0;
    private List<CharacterType> m_characterParty = new List<CharacterType>()
    {
        CharacterType.Angel,
        CharacterType.Bunny,
        CharacterType.Cat,
        CharacterType.Frog,
        CharacterType.Mow,
    };

    //

    public CharacterConfig CharacterConfig => m_characterConfig;

    public CharacterType CharacterCurrent => m_characterParty[m_characterIndex];

    public CharacterType[] CharacterParty => m_characterParty.ToArray();

    //

    public bool SetCharacterCurrent(CharacterType Character)
    {
        if (!Instance.m_characterParty.Exists(t => t == Character))
            return false;
        Instance.m_characterIndex = Instance.m_characterParty.FindIndex(t => t == Character);
        //
        onCharacter?.Invoke();
        //
        return true;
    }

    public void SetCharacterCurrent(int Index)
    {
        Instance.m_characterIndex = Mathf.Clamp(Index, 0, Instance.m_characterParty.Count - 1);
        //
        onCharacter?.Invoke();
        //
    }

    public void SetCharacterNext()
    {
        Instance.m_characterIndex++;
        if (Instance.m_characterIndex > Instance.m_characterParty.Count - 1)
            Instance.m_characterIndex = 0;
        //
        onCharacter?.Invoke();
        //
    }

    public void SetCharacterPrev()
    {
        Instance.m_characterIndex--;
        if (Instance.m_characterIndex < 0)
            Instance.m_characterIndex = Instance.m_characterParty.Count - 1;
        //
        onCharacter?.Invoke();
        //
    }

    public bool SetCharacterPartyAdd(CharacterType Character)
    {
        if (Instance.m_characterParty.Exists(t => t == Character))
            return false;
        Instance.m_characterParty.Add(Character);
        return true;
    }

    public bool SetCharacterPartyRemove(CharacterType Character)
    {
        if (!Instance.m_characterParty.Exists(t => t == Character))
            return false;
        Instance.m_characterParty.RemoveAll(t => t == Character);
        return true;
    }
}