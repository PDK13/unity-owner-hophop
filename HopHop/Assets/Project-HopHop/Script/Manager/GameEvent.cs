using System;

public class GameEvent
{
    public static Action<string, bool> onKey;

    public static Action<string, IsometricVector> onFollow;

    public static Action<string, bool> onSwitch;

    public static void SetKey(string Key, bool State)
    {
        if (string.IsNullOrEmpty(Key))
            return;
        //
        onKey?.Invoke(Key, State);
    }

    public static void SetFollow(string Identity, IsometricVector Dir)
    {
        if (string.IsNullOrEmpty(Identity))
            return;
        //
        onFollow?.Invoke(Identity, Dir);
    }

    public static void SetSwitch(string Key, bool State)
    {
        if (string.IsNullOrEmpty(Key))
            return;
        //
        onSwitch?.Invoke(Key, State);
    }
}