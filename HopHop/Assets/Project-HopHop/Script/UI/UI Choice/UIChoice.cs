using UnityEngine;

public class UIChoice : MonoBehaviour
{
    [SerializeField] private Transform m_content;
    [SerializeField] private GameObject m_item;

    private void Start()
    {
        ChoiceManager.Instance.onStart += OnStart;
        ChoiceManager.Instance.onInit += OnInit;
        ChoiceManager.Instance.onClear += OnClear;

        m_item.SetActive(false);
    }

    private void OnDestroy()
    {
        ChoiceManager.Instance.onStart -= OnStart;
        ChoiceManager.Instance.onInit -= OnInit;
        ChoiceManager.Instance.onClear -= OnClear;
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

    private void OnInit(EventConfigOptional Data)
    {
        GameObject ItemClone = QGameObject.SetCreate(m_item, m_content);
        ItemClone.gameObject.SetActive(true);
        ItemClone.GetComponent<UIChoiceItem>().SetInit(Data);
    }

    private void OnNext()
    {
        ChoiceManager.Instance.SetNext();
    }

    private void OnPrev()
    {
        ChoiceManager.Instance.SetPrev();
    }

    private void OnInvoke()
    {
        ChoiceManager.Instance.SetInvoke();
        ChoiceManager.Instance.SetClear();
    }

    private void OnClear()
    {
        SetInputEvent(false);

        for (int i = m_content.childCount - 1; i >= 0; i--)
            Destroy(m_content.GetChild(i).gameObject);
    }
}