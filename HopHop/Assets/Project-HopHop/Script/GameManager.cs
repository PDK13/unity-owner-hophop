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
        yield return null;
        GameEvent.SetTriggerStart(GameKey.PLAYER);
    }
}