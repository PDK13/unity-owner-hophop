using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string, IsoDir> onMove; //Key, Dir

    public static void SetOnMove(string Key, IsoDir Dir)
    {
        onMove?.Invoke(Key, Dir);
    }
}
