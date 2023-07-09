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

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        m_isometricManager.SetList(m_isometricConfig);

        SetWorldLoad(m_gameConfig.m_level[0].Level[0]);
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
        GameEvent.SetKey(GameKey.TURN_PLAYER, true);
    }
}