using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour
{
    [SerializeField] private GameObject m_panel;
    [SerializeField] private Transform m_dialogueAuthor;
    [SerializeField] private Transform m_dialogueNone;

    private bool m_active = false;

    private Image m_imgDialogueAuthor;
    private TextMeshProUGUI m_tmpDialogueAuthor;
    private TextMeshProUGUI m_tmpDialogueNone;

    private void Awake()
    {
        m_imgDialogueAuthor = m_dialogueAuthor.GetComponentInChildren<Image>();
        m_tmpDialogueAuthor = m_dialogueAuthor.GetComponentInChildren<TextMeshProUGUI>();
        m_tmpDialogueNone = m_dialogueNone.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetDialogueHide();

        EventManager.Instance.onEventDialogue += OnEventDialogue;
    }

    private void OnDestroy()
    {
        OnEventDialogue(false);

        EventManager.Instance.onEventDialogue -= OnEventDialogue;
    }

    //

    private void OnEventDialogue(bool Stage)
    {
        if (Stage)
        {
            OnEventDialogue(false);

            InputManager.Instance.onEventNext += OnDialogueNext;
            InputManager.Instance.onEventSkip += OnDialogueSkip;

            DialogueManager.Instance.onStage += OnDialogueStage;
            DialogueManager.Instance.onText += OnDialogueText;
        }
        else
        {
            InputManager.Instance.onEventNext -= OnDialogueNext;
            InputManager.Instance.onEventSkip -= OnDialogueSkip;

            DialogueManager.Instance.onStage -= OnDialogueStage;
            DialogueManager.Instance.onText -= OnDialogueText;
        }
    }

    private void OnDialogueNext()
    {
        if (!m_active)
            SetDialogueHide();
        else
            DialogueManager.Instance.SetNext();
    }

    private void OnDialogueSkip()
    {
        if (!m_active)
            SetDialogueHide();
        else
            DialogueManager.Instance.SetSkip();
    }

    private void SetDialogueHide()
    {
        m_panel.SetActive(false);
        m_dialogueAuthor.gameObject.SetActive(false);
        m_dialogueNone.gameObject.SetActive(false);
    }

    //

    private void OnDialogueStage(DialogueStageType Stage)
    {
        switch (Stage)
        {
            case DialogueStageType.Start:
                m_active = true;
                break;
            case DialogueStageType.End:
                m_active = false;
                break;
        }
    }

    private void OnDialogueText(DialogueDataText Text)
    {
        m_panel.SetActive(true);

        Sprite Avatar = DialogueManager.Instance.GetAuthorAvatar(Text);
        if (Avatar != null)
        {
            m_dialogueAuthor.gameObject.SetActive(true);
            m_dialogueNone.gameObject.SetActive(false);

            m_imgDialogueAuthor.sprite = Avatar;
            DialogueManager.Instance.Tmp = m_tmpDialogueAuthor;
        }
        else
        {
            m_dialogueAuthor.gameObject.SetActive(false);
            m_dialogueNone.gameObject.SetActive(true);

            DialogueManager.Instance.Tmp = m_tmpDialogueNone;
        }
    }
}