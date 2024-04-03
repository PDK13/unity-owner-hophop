using System;

public class GameEvent
{
    public static Action<string, bool> onKey;

    public static void SetKey(string Key, bool State)
    {
        if (string.IsNullOrEmpty(Key))
            return;
        //
        onKey?.Invoke(Key, State);
    }
}