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
        yield return null;
        m_isoManager.SetWorldRead(this.transform);
        yield return null;
        m_isoManager.SetWorldRemove();
        yield return null;
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
                    GameEvent.SetOnPlayerMove(IsoDir.Up);
                if (Input.GetKey(KeyCode.DownArrow))
                    GameEvent.SetOnPlayerMove(IsoDir.Down);
                if (Input.GetKey(KeyCode.LeftArrow))
                    GameEvent.SetOnPlayerMove(IsoDir.Left);
                if (Input.GetKey(KeyCode.RightArrow))
                    GameEvent.SetOnPlayerMove(IsoDir.Right);
                //Keyboard Control
                break;
        }
    }

    #region Game

    #endregion
}