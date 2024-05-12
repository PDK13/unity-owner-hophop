using TMPro;
using UnityEngine;

public class UIChoiceItem : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_panel;
    [SerializeField] private TextMeshProUGUI m_tmp;

    private EventConfigOptional m_data;

    //

    public EventConfigOptional Data => m_data;

    //

    public void SetInit(EventConfigOptional Data)
    {
        m_data = Data;
        m_tmp.text = Data.OptionName;

        ChoiceManager.Instance.onIndex += OnIndex;
    }

    private void OnDestroy()
    {
        ChoiceManager.Instance.onIndex -= OnIndex;
    }

    //

    private void OnIndex(int Index, EventConfigOptional Data)
    {
        bool Choice = m_data.Equals(Data);

        m_panel.alpha = Choice ? 1f : 0.5f;
    }
}