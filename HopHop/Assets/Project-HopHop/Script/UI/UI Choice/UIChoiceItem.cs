using TMPro;
using UnityEngine;

public class UIChoiceItem : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_panel;
    [SerializeField] private TextMeshProUGUI m_tmp;

    private OptionalConfigSingle m_data;

    //

    public OptionalConfigSingle Data => m_data;

    //

    public void SetInit(OptionalConfigSingle Data)
    {
        m_data = Data;
        m_tmp.text = Data.OptionName;

        OptionalManager.Instance.onIndex += OnIndex;
    }

    private void OnDestroy()
    {
        OptionalManager.Instance.onIndex -= OnIndex;
    }

    //

    private void OnIndex(int Index, OptionalConfigSingle Data)
    {
        bool Choice = m_data.Equals(Data);

        m_panel.alpha = Choice ? 1f : 0.5f;
    }
}