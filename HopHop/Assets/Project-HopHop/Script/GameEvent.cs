using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<string> onKeyStart;
    public static Action<string> onKeyEnd;
    public static Action<string, IsoDir> onKeyMove;

    public static Action<IsoVector, Vector3Int, int> onMoveFollow;
    public static Action<IsoVector, Vector3Int, int> onMovePush;

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

    public static void SetMoveFollow(IsoVector Pos, Vector3Int Dir, int Length)
    {
        onMoveFollow?.Invoke(Pos, Dir, Length);
    }

    public static void SetMovePush(IsoVector Pos, Vector3Int Dir, int Length)
    {
        onMovePush?.Invoke(Pos, Dir, Length);
    }
}