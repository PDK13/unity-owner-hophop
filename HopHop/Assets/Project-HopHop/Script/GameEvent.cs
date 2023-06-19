using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public static Action<TypeTurn> onTurn; //Turn

    public static Action<string> onControlDone; //Key

    public static Action<IsoDir> onPlayerMove; //Dir
    public static Action<string> onObjectTurn; //Key

    public static void SetOnTurn(TypeTurn Turn)
    {
        GameData.m_turnControl = Turn;
        onTurn?.Invoke(Turn);
    }

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