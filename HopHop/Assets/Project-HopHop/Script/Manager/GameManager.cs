using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    //Action

    public Action onCharacter;

    //Config

    [SerializeField] private CharacterConfig m_characterConfig;

    //Time

    private float m_timeScale = 1f;
    private float m_timeMove = 1.2f;
    private float m_timeRatio = 1f * 0.5f;

    //Character

    private int m_characterIndex = 0;
    private CharacterType m_characterCurrent = CharacterType.Angel;
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

    public float TimeScale { get => m_timeScale; set => m_timeScale = value; }

    public float TimeMove => m_timeMove * m_timeRatio * m_timeScale;

    public CharacterType CharacterCurrent => m_characterParty[m_characterIndex];

    public CharacterType[] CharacterParty => m_characterParty.ToArray();

    //

    private void Start()
    {
        SetInit();
        SetInitWorld(IsometricManager.Instance.Config.Map.ListAssets[0]);
    }

    //World

    private void SetInit()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, true);
        //
        IsometricManager.Instance.SetInit();
    }

    private void SetInitWorld(TextAsset WorldData)
    {
        IsometricDataFile.SetFileRead(IsometricManager.Instance, WorldData);
        //
        TurnManager.SetAutoRemove(TurnType.None);
        TurnManager.SetAutoRemove(TurnType.Gravity);
        TurnManager.SetStart();
    }

    //Camera

    public void SetCameraFollow(Transform Target)
    {
        if (Camera.main == null)
        {
            Debug.Log("[Manager] Main camera not found, so can't change target!");
            return;
        }
        //
        if (Target == null)
            Camera.main.transform.parent = null;
        else
            Camera.main.transform.parent = Target;
        //
        Camera.main.transform.localPosition = Vector3.back * 100f;
    }

    public void SetCameraShake() { }

    //Character

    public bool SetCharacterCurrent(CharacterType Character)
    {
        if (!Instance.m_characterParty.Exists(t => t == Character))
            return false;
        Instance.m_characterCurrent = Character;
        Instance.m_characterIndex = Instance.m_characterParty.FindIndex(t => t == Character);
        //
        onCharacter?.Invoke();
        //
        return true;
    }

    public void SetCharacterCurrent(int Index)
    {
        Instance.m_characterIndex = Mathf.Clamp(Index, 0, Instance.m_characterParty.Count - 1);
        Instance.m_characterCurrent = CharacterCurrent;
        //
        onCharacter?.Invoke();
        //
    }

    public void SetCharacterNext()
    {
        Instance.m_characterIndex++;
        if (Instance.m_characterIndex > Instance.m_characterParty.Count - 1)
            Instance.m_characterIndex = 0;
        Instance.m_characterCurrent = CharacterCurrent;
        //
        onCharacter?.Invoke();
        //
    }

    public void SetCharacterPrev()
    {
        Instance.m_characterIndex--;
        if (Instance.m_characterIndex < 0)
            Instance.m_characterIndex = Instance.m_characterParty.Count - 1;
        Instance.m_characterCurrent = CharacterCurrent;
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

public enum TurnType
{
    None,
    //
    Gravity,
    Player,
    Bullet,
    Shoot,
    MovePhysic,
    MoveStatic,
}