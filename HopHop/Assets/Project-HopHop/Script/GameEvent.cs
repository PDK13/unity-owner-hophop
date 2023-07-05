using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string> onKeyStart;                            //[Key]
    public static Action<string> onKeyEnd;                              //[Key]
    public static Action<string, IsoDir> onKeyMove;                     //[Key, Dir]

    public static Action<IsoVector, IsoDir, int, bool> onForceMove;     //[Pos, Dir, Length, Revert]

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

    public static void SetKeyMove(string Key, IsoDir Dir)
    {
        onKeyMove?.Invoke(Key, Dir);
    }

    //Force

    public static void SetForceMove(IsoVector Pos, IsoDir Dir, int Length, bool Revert)
    {
        onForceMove?.Invoke(Pos, Dir, Length, Revert);
    }
}