using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig m_gameConfig;
    [SerializeField] private IsometricConfig m_isometricConfig;

    [Space]
    [SerializeField] private IsometricManager m_isometricManager;

    #region Varible: Time

    public static float m_timeMove = 1f;
    public static float m_timeRatio = 1f;

    public static float TimeMove => m_timeMove * m_timeRatio;

    #endregion

    #region Varible: Turn

    

    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        m_isometricManager.SetList(m_isometricConfig);

        SetWorldLoad(m_gameConfig.m_level[0].Level[0]);

        GameEvent.onTurn += SetTurn;
        GameEvent.onDelay += SetDelay;
    }

    private void OnDestroy()
    {
        GameEvent.onTurn -= SetTurn;
        GameEvent.onDelay -= SetDelay;
    }

    private void SetWorldLoad(TextAsset WorldData)
    {
        StartCoroutine(ISetWorldLoad(WorldData));
    }

    private IEnumerator ISetWorldLoad(TextAsset WorldData)
    {
        m_isometricManager.SetWorldRemove(m_isometricManager.transform);
        yield return null;
        m_isometricManager.SetFileRead(WorldData);
        yield return null;
        GameEvent.SetTurn(TypeTurn.Player, true);
    }

    #region Event

    public static void SetObjectTurn(bool State)
    {
        if (State)
            m_objectTurnCount++;
        else
            m_objectTurnCount--;
    }

    public static void SetObjectDelay(bool State)
    {
        if (State)
            m_objectDelayCount++;
        else
            m_objectDelayCount--;
    }

    private void SetTurn(TypeTurn Turn, bool State)
    {
        if (Turn == TypeTurn.None)
        {

        }
        else
        {
            if (State)
            {
                m_objectTurnEnd = 0;
                m_objectDelayEnd = 0;
            }
            else
            {
                switch (Turn)
                {
                    case TypeTurn.Player:
                        if (m_objectTurnCount > 0)
                            GameEvent.SetTurn(TypeTurn.Object, true);
                        else
                            GameEvent.SetTurn(TypeTurn.Player, true);
                        break;
                    case TypeTurn.Object:
                        if (!ObjectTurnDone)
                            m_objectTurnEnd++;
                        if (ObjectTurnDone && ObjectDelayDone)
                            GameEvent.SetTurn(TypeTurn.Player, true);
                        break;
                }
            }
            m_turn = Turn;
        }
    }

    private void SetDelay(TypeDelay Delay, bool State)
    {
        if (State)
        {
            switch (Delay)
            {
                case TypeDelay.Gravtiy:
                    m_objectDelayCount++;
                    break;
            }
        }
        else
        {
            switch (Delay)
            {
                case TypeDelay.Gravtiy:
                    if (!ObjectDelayDone)
                        m_objectDelayEnd++;
                    if (ObjectDelayDone)
                        SetTurn(m_turn, true);
                    break;
            }
        }
    }

    #endregion
}