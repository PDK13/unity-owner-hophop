using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Event: Game

    public static Action<string, IsoDir> onMove; //Key, Dir

    #endregion

    #region Varible: Game

    private static TypeTurn m_turnGame = TypeTurn.PlayerControl;

    #endregion

    private void Start()
    {
        
    }

    private void Update()
    {
        switch (m_turnGame)
        {
            case TypeTurn.Wait:
                //...
                break;
            case TypeTurn.PlayerControl:
                //Keyboard Control
                if (Input.GetKey(KeyCode.UpArrow))
                    SetOnMove(GameKey.PLAYER, IsoDir.Up);
                if (Input.GetKey(KeyCode.DownArrow))
                    SetOnMove(GameKey.PLAYER, IsoDir.Down);
                if (Input.GetKey(KeyCode.LeftArrow))
                    SetOnMove(GameKey.PLAYER, IsoDir.Left);
                if (Input.GetKey(KeyCode.RightArrow))
                    SetOnMove(GameKey.PLAYER, IsoDir.Right);
                //Keyboard Control
                break;
        }
    }

    #region Game

    //Primary

    private void SetOnMove(string Key, IsoDir Dir)
    {
        onMove?.Invoke(Key, Dir);
    }

    public static void SetOnMoveDone(string Key)
    {
        switch (Key)
        {
            case GameKey.PLAYER:
                m_turnGame = TypeTurn.PlayerControl;
                break;
        }
    }

    //Player

    public static void SetOnPlayerMoveSuccess(bool Success)
    {
        if (Success)
            m_turnGame = TypeTurn.Wait;
    }

    public static void SetOnPlayerCharacter(TypeCharacter Character)
    {

    }

    #endregion
}