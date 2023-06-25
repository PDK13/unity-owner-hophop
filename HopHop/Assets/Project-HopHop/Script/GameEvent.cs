using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string> onTriggerStart; //Key
    public static Action<string> onTriggerEnd; //Key

    public static void SetTriggerStart(string Key)
    {
        onTriggerStart?.Invoke(Key);
    }

    public static void SetTriggerEnd(string Key)
    {
        switch (Key)
        {
            case GameKey.PLAYER:
                if (GameData.m_objectControlCount > 0)
                    SetTriggerStart(GameKey.OBJECT);
                else
                    SetTriggerStart(GameKey.PLAYER);
                break;
            case GameKey.OBJECT:
                GameData.m_objectControlEnd++;
                if (GameData.ObjectControlEnd)
                {
                    GameData.m_objectControlEnd = 0;
                    SetTriggerStart(GameKey.PLAYER);
                }
                break;
        }
        onTriggerEnd?.Invoke(Key);
    }
}