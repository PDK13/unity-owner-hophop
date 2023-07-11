using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string, bool> onKey;

    public static Action<string, IsoVector> onFollow;

    public static void SetKey(string Key, bool State)
    {
        onKey?.Invoke(Key, State);
    }

    public static void SetFollow(string KeyFollow, IsoVector Dir)
    {
        onFollow?.Invoke(KeyFollow, Dir);
    }
}