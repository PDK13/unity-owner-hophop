public class WorldManager : SingletonManager<WorldManager>
{
    public IsometricBlock Player { get; set; } = null;

    private void Awake()
    {
        SetInstance();
    }
}