using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig m_gameConfig;

    [HideInInspector] private IsometricManager m_manager;

    private void Awake()
    {
        m_manager = GetComponent<IsometricManager>();
    }

    //private IEnumerator Start()
    //{
    //    m_manager.SetList();
    //    yield return null;
    //    m_manager.SetWorldRead(m_manager.transform);
    //    yield return null;
    //    m_manager.SetWorldRemove();
    //    yield return null;
    //    m_manager.SetFileRead(m_gameConfig.m_level[0].Level[0]);
    //    yield return null;
    //    GameEvent.SetKey(GameKey.PLAYER, true);
    //}
}