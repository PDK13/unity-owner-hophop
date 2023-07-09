using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string, bool> onKey;
    public static Action<string, IsoVector, int> onKeyFollow;

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
                case GameKey.TURN_PLAYER:
                    if (GameData.m_objectTurnCount > 0)
                        SetKey(GameKey.TURN_OBJECT, true);
                    else
                        SetKey(GameKey.TURN_PLAYER, true);
                    break;
                case GameKey.TURN_OBJECT:
                    GameData.m_objectTurnEnd++;
                    if (GameData.ObjectControlEnd)
                    {
                        GameData.m_objectTurnEnd = 0;
                        SetKey(GameKey.TURN_PLAYER, true);
                    }
                    break;
            }
        }

        onKey?.Invoke(Key, State);
    }

    public static void SetKeyFollow(string KeyFollow, IsoVector Dir, int Length)
    {
        onKeyFollow?.Invoke(KeyFollow, Dir, Length);
    }
}