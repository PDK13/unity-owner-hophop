using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    //Time

    private float m_timeScale = 1f;
    private float m_timeMove = 1.2f;
    private float m_timeRatio = 1f * 0.5f;

    //

    public float TimeScale { get => m_timeScale; set => m_timeScale = value; }

    public float TimeMove => m_timeMove * m_timeRatio * m_timeScale;

    //

    private void Start()
    {
        SetInit();
        SetInitWorld(IsometricManager.Instance.Config.Map.DataAssets[0]);
    }

    //World

    private void SetInit()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, true);

        IsometricManager.Instance.SetInit();

        TurnManager.Instance.SetAutoRemove(StepType.None);
        TurnManager.Instance.SetAutoRemove(StepType.Gravity);
        TurnManager.Instance.SetAutoRemove(StepType.Event);
        TurnManager.Instance.SetAutoRemove(StepType.EventCommand);
    }

    private void SetInitWorld(TextAsset WorldData)
    {
        IsometricDataFile.SetFileRead(IsometricManager.Instance, WorldData);

        TurnManager.Instance.SetStart();
    }

    //Camera

    public void SetCameraFollow(Transform Target)
    {
        if (Camera.main == null)
        {
            Debug.Log("[Manager] Main camera not found, so can't change target!");
            return;
        }

        if (Target == null)
            Camera.main.transform.parent = null;
        else
            Camera.main.transform.parent = Target;

        Camera.main.transform.localPosition = Vector3.back * 100f;
    }

    public void SetCameraShake() { }
}

public enum StepType
{
    None,
    //Base
    Gravity,
    Player,
    Bullet,
    Shoot,
    MovePhysic,
    MoveStatic,
    //Special
    Event,
    EventCommand,
} //NOTE: Not used this enum as varible choice on editor, because if this list changed, value will wrong