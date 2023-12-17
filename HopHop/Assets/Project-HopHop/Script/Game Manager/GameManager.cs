using System.Collections;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    public static CharacterConfig CharacterConfig;
    //
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

    protected override void Awake()
    {
        base.Awake();
        //
        CharacterConfig = m_characterConfig;
        //
        Application.targetFrameRate = 60;
        //
        Time.timeScale = 2;
        //
        Screen.SetResolution(1920, 1080, true);
    }

    private void Start()
    {
        m_isometricManager.List.SetList(m_isometricConfig, true);

        SetWorldLoad(IsometricManager.Instance.Config.Map.ListAssets[0]);
    }

    //

    public void SetCameraFollow(Transform Target)
    {
        if (Camera.main == null)
        {
            Debug.Log("[Manager] Main camera not found, so can't change target!");
            return;
        }
        //
        if (Target == null)
            Camera.main.transform.parent = this.transform;
        else
            Camera.main.transform.parent = Target;
        //
        Camera.main.transform.localPosition = Vector3.back * 100f;
    }

    //
    private void SetWorldLoad(TextAsset WorldData)
    {
        StartCoroutine(ISetWorldLoad(WorldData));
    }

    private IEnumerator ISetWorldLoad(TextAsset WorldData)
    {
        m_isometricManager.World.Current.SetWorldRemove(m_isometricManager.transform);

        yield return new WaitForSeconds(1f);

        IsometricDataFile.SetFileRead(m_isometricManager, WorldData);

        yield return new WaitForSeconds(1f);

        TurnManager.SetStart();
    }
}