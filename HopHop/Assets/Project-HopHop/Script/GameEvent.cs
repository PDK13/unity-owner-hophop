using System;

public class GameEvent
{
    public static Action<string, bool> onKey;

    public static Action<string, IsometricVector> onFollow;

    public static void SetKey(string Key, bool State)
    {
        onKey?.Invoke(Key, State);
    }

    public static void SetFollow(string Identity, IsometricVector Dir)
    {
        onFollow?.Invoke(Identity, Dir);
    }
}