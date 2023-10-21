using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static LevelConfig LevelConfig;
    public static CharacterConfig CharacterConfig;
    //
    [SerializeField] private LevelConfig m_levelConfig;
    [SerializeField] private CharacterConfig m_characterConfig;
    [SerializeField] private IsometricConfig m_isometricConfig;
    //
    [Space]
    [SerializeField] private IsometricManager m_isometricManager;

    #region Varible: Time

    public static float m_timeMove = 1.2f;
    public static float m_timeRatio = 1f;

    public static float TimeMove => m_timeMove * m_timeRatio;

    #endregion

    #region Varible: Turn

    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //
        LevelConfig = m_levelConfig;
        CharacterConfig = m_characterConfig;
        //
        Application.targetFrameRate = 60;
        //
        Time.timeScale = 2;
    }

    private void Start()
    {
        m_isometricManager.SetInit();
        m_isometricManager.List.SetList(m_isometricConfig, true);

        SetWorldLoad(m_levelConfig.Level[0].Level[0]);
    }

    private void SetWorldLoad(TextAsset WorldData)
    {
        StartCoroutine(ISetWorldLoad(WorldData));
    }

    private IEnumerator ISetWorldLoad(TextAsset WorldData)
    {
        m_isometricManager.World.SetWorldRemove(m_isometricManager.transform);

        yield return new WaitForSeconds(1f);

        IsometricDataFile.SetFileRead(m_isometricManager, WorldData);

        yield return new WaitForSeconds(1f);

        TurnManager.SetStart();
    }
}