using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string, bool> onKey;

    //Key

    public static void SetKey(string Key, bool State)
    {
        if (State)
        {
            //...
        }
        else
        {
            switch (Key)
            {
                case GameKey.PLAYER:
                    if (GameData.m_objectControlCount > 0)
                        SetKey(GameKey.OBJECT, true);
                    else
                        SetKey(GameKey.PLAYER, true);
                    break;
                case GameKey.OBJECT:
                    GameData.m_objectControlEnd++;
                    if (GameData.ObjectControlEnd)
                    {
                        GameData.m_objectControlEnd = 0;
                        SetKey(GameKey.PLAYER, true);
                    }
                    break;
            }
        }

        onKey?.Invoke(Key, State);
    }
}