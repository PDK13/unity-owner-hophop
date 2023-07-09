using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;

    [SerializeField] private GameConfig m_gameConfig;
    [SerializeField] private IsometricManager m_manager;

    private void Awake()
    {
        m_text.text = "";
    }

    private IEnumerator Start()
    {
        m_text.text = "1";
        m_manager.SetList();
        yield return null;
        m_text.text = "2";
        m_manager.SetWorldRemove(m_manager.transform);
        yield return null;
        m_text.text = "3";
        m_manager.SetFileRead(m_gameConfig.m_level[0].Level[0]);
        yield return null;
        m_text.text = "4";
        GameEvent.SetKey(GameKey.PLAYER, true);

        yield return new WaitForSeconds(3f);
        m_text.text = "5";
        m_manager.SetWorldRemove(true);
    }
}