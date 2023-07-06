using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string> onKeyStart;
    public static Action<string> onKeyEnd;
    public static Action<string, IsoDir, int> onKeyMove;

    //Key

    public static void SetKeyStart(string Key)
    {
        onKeyStart?.Invoke(Key);
    }

    public static void SetKeyEnd(string Key)
    {
        switch (Key)
        {
            case GameKey.PLAYER:
                if (GameData.m_objectControlCount > 0)
                    SetKeyStart(GameKey.OBJECT);
                else
                    SetKeyStart(GameKey.PLAYER);
                break;
            case GameKey.OBJECT:
                GameData.m_objectControlEnd++;
                if (GameData.ObjectControlEnd)
                {
                    GameData.m_objectControlEnd = 0;
                    SetKeyStart(GameKey.PLAYER);
                }
                break;
        }
        onKeyEnd?.Invoke(Key);
    }

    public static void SetKeyMove(string Key, IsoDir Dir, int Lenght)
    {
        onKeyMove?.Invoke(Key, Dir, Lenght);
    }
}