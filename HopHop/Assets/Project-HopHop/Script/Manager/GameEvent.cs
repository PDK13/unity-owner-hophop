using System;

public class GameEvent
{
    public static Action<string, bool> onKey;

    public static Action<string, IsometricVector> onFollow;

    public static Action<string, bool> onSwitch;

    public static void SetKey(string Key, bool State)
    {
        onKey?.Invoke(Key, State);
    }

    public static void SetFollow(string Identity, IsometricVector Dir)
    {
        onFollow?.Invoke(Identity, Dir);
    }

    public static void SetSwitch(string Key, bool State)
    {
        onSwitch?.Invoke(Key, State);
    }
}