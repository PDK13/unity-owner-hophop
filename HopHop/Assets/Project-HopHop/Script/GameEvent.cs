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
<<<<<<< Updated upstream
}
=======

    public static void SetOnControlDone(string Key)
    {
        switch (Key)
        {
            case GameKey.PLAYER:
                SetOnTurn(TypeTurn.ObjectControl);
                SetOnObjectTurn(GameKey.OBJECT);
                break;
            case GameKey.OBJECT:
                GameData.m_objectControlCount++;
                if (GameData.ObjectControlDone)
                {
                    GameData.m_objectControlDone = 0;
                    SetOnTurn(TypeTurn.PlayerControl);
                    SetOnObjectTurn(GameKey.PLAYER);
                }
                break;
        }
        onControlDone?.Invoke(Key);
    }

    public static void SetOnPlayerMove(IsoDir Dir)
    {
        onPlayerMove?.Invoke(Dir);
    }

    public static void SetOnPlayerMoveSuccess(bool Success)
    {
        if (Success)
            GameData.m_turnControl = TypeTurn.Wait;
    }

    public static void SetOnObjectTurn(string Key)
    {
        onObjectTurn?.Invoke(Key);
    }

    public static void SetOnPlayerCharacter(TypeCharacter Character)
    {

    }
}
>>>>>>> Stashed changes
