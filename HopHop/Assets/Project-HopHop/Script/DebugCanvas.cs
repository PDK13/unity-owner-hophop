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
}