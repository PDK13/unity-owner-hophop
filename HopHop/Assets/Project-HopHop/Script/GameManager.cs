using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig m_gameConfig;

    [HideInInspector] private IsometricManager m_isoManager;

    private void Awake()
    {
        m_isoManager = GetComponent<IsometricManager>();
    }

    private IEnumerator Start()
    {
        m_isoManager.SetBlockList();

        yield return new WaitForSeconds(1f);

        m_isoManager.SetWorldRead(this.transform);

        yield return new WaitForSeconds(1f);

        m_isoManager.SetWorldRemove();

        yield return new WaitForSeconds(1f);

        m_isoManager.SetWorldFileRead(m_gameConfig.m_level[0].Level[0]);
    }

    private void Update()
    {
        switch (GameData.m_turnControl)
        {
            case TypeTurn.Wait:
                //...
                break;
            case TypeTurn.PlayerControl:
                //Keyboard Control
                if (Input.GetKey(KeyCode.UpArrow))
                    GameEvent.SetOnMove(GameKey.PLAYER, IsoDir.Up);
                if (Input.GetKey(KeyCode.DownArrow))
                    GameEvent.SetOnMove(GameKey.PLAYER, IsoDir.Down);
                if (Input.GetKey(KeyCode.LeftArrow))
                    GameEvent.SetOnMove(GameKey.PLAYER, IsoDir.Left);
                if (Input.GetKey(KeyCode.RightArrow))
                    GameEvent.SetOnMove(GameKey.PLAYER, IsoDir.Right);
                //Keyboard Control
                break;
        }
    }

    #region Game

    //Primary

    public static void SetOnMoveDone(string Key)
    {
        switch (Key)
        {
            case GameKey.PLAYER:
                GameData.m_turnControl = TypeTurn.PlayerControl;
                break;
        }
    }

    //Player

    public static void SetOnPlayerMoveSuccess(bool Success)
    {
        if (Success)
            GameData.m_turnControl = TypeTurn.Wait;
    }

    public static void SetOnPlayerCharacter(TypeCharacter Character)
    {

    }

    #endregion
}