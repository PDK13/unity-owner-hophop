using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    //Config

    [SerializeField] private CharacterConfig m_characterConfig;

    //Time

    private static float m_timeScale = 1f;
    private static float m_timeMove = 1.2f;
    private static float m_timeRatio = 1f * 0.5f;

    //Character

    private List<CharacterType> m_character = new List<CharacterType>();

    //

    public static CharacterConfig CharacterConfig => Instance.m_characterConfig;

    public static float TimeScale { get => m_timeScale; set => m_timeScale = value; }

    public static float TimeMove => m_timeMove * m_timeRatio * m_timeScale;

    public static CharacterType[] Character => Instance.m_character.ToArray();

    //

    private void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, true);
        //
        SetWorldLoad(IsometricManager.Instance.Config.Map.ListAssets[0]);
    }

    //World

    private void SetWorldLoad(TextAsset WorldData)
    {
        StartCoroutine(ISetWorldLoad(WorldData));
    }

    private IEnumerator ISetWorldLoad(TextAsset WorldData)
    {
        IsometricManager.Instance.SetInit();
        //
        yield return new WaitForSeconds(1f);
        //
        IsometricDataFile.SetFileRead(IsometricManager.Instance, WorldData);
        //
        yield return new WaitForSeconds(1f);
        //
        TurnManager.SetAutoRemove(TurnType.None);
        TurnManager.SetAutoRemove(TurnType.Gravity);
        TurnManager.SetStart();
    }

    //Camera

    public static void SetCameraFollow(Transform Target)
    {
        if (Camera.main == null)
        {
            Debug.Log("[Manager] Main camera not found, so can't change target!");
            return;
        }
        //
        if (Target == null)
            Camera.main.transform.parent = Instance.transform;
        else
            Camera.main.transform.parent = Target;
        //
        Camera.main.transform.localPosition = Vector3.back * 100f;
    }

    public static void SetCameraShake() { }

    //Character

    public static bool SetCharacterAdd(CharacterType Type)
    {
        if (Instance.m_character.Exists(t => t == Type))
            return false;
        Instance.m_character.Add(Type);
        return true;
    }

    public static bool SetCharacterRemove(CharacterType Type)
    {
        if (!Instance.m_character.Exists(t => t == Type))
            return false;
        Instance.m_character.RemoveAll(t => t == Type);
        return true;
    }
}