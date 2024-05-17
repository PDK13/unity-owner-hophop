using UnityEngine;

public class UIChoice : MonoBehaviour
{
    [SerializeField] private Transform m_content;
    [SerializeField] private GameObject m_item;

    private void Start()
    {
        OptionalManager.Instance.onStart += OnStart;
        OptionalManager.Instance.onInit += OnInit;
        OptionalManager.Instance.onClear += OnClear;

        m_item.SetActive(false);
    }

    private void OnDestroy()
    {
        OptionalManager.Instance.onStart -= OnStart;
        OptionalManager.Instance.onInit -= OnInit;
        OptionalManager.Instance.onClear -= OnClear;
    }

    //

    private void SetInputEvent(bool Stage)
    {
        if (Stage)
        {
            SetInputEvent(false);
            InputManager.Instance.onChoiceNext += OnNext;
            InputManager.Instance.onChoicePrev += OnPrev;
            InputManager.Instance.onChoiceInvoke += OnInvoke;
        }
        else
        {
            InputManager.Instance.onChoiceNext -= OnNext;
            InputManager.Instance.onChoicePrev -= OnPrev;
            InputManager.Instance.onChoiceInvoke -= OnInvoke;
        }
    }

    //

    private void OnStart()
    {
        SetInputEvent(true);
    }

    private void OnInit(OptionalConfigSingle Data)
    {
        GameObject ItemClone = QGameObject.SetCreate(m_item, m_content);
        ItemClone.gameObject.SetActive(true);
        ItemClone.GetComponent<UIChoiceItem>().SetInit(Data);
    }

    private void OnNext()
    {
        OptionalManager.Instance.SetNext();
    }

    private void OnPrev()
    {
        OptionalManager.Instance.SetPrev();
    }

    private void OnInvoke()
    {
        OptionalManager.Instance.SetInvoke();
        OptionalManager.Instance.SetClear();
    }

    private void OnClear()
    {
        SetInputEvent(false);

        for (int i = m_content.childCount - 1; i >= 0; i--)
            Destroy(m_content.GetChild(i).gameObject);
    }
}