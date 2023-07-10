using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<TypeTurn, bool> onTurn;

    public static Action<string, bool> onKey;
    public static Action<string, IsoVector, int> onKeyFollow;

    public static void SetTurn(TypeTurn Turn, bool State)
    {
        if (State)
        {

        }
        else
        {
            switch (Turn)
            {
                case TypeTurn.Player:
                    if (GameData.m_objectTurnCount > 0)
                    {
                        SetTurn(TypeTurn.Object, true);
                        SetKey(ConstGameKey.TURN_OBJECT, true);
                    }
                    else
                    {
                        SetTurn(TypeTurn.Player, true);
                        SetKey(ConstGameKey.TURN_PLAYER, true);
                    }
                    break;
                case TypeTurn.Object:
                    GameData.m_objectTurnEnd++;
                    if (GameData.ObjectTurnEnd)
                    {
                        GameData.m_objectTurnEnd = 0;
                        SetTurn(TypeTurn.Player, true);
                        SetKey(ConstGameKey.TURN_PLAYER, true);
                    }
                    break;
            }
        }

        onTurn?.Invoke(Turn, State);
    }

    //Key

    public static void SetKey(string Key, bool State)
    {
        onKey?.Invoke(Key, State);
    }

    public static void SetKeyFollow(string KeyFollow, IsoVector Dir, int Length)
    {
        onKeyFollow?.Invoke(KeyFollow, Dir, Length);
    }
}