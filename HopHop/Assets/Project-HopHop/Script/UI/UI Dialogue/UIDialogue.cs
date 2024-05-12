using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour
{
    [SerializeField] private GameObject m_panel;
    [SerializeField] private Transform m_dialogueAuthor;
    [SerializeField] private Transform m_dialogueNone;

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

        DialogueManager.Instance.onStage += OnDialogueStage;
        DialogueManager.Instance.onText += OnDialogueText;
    }

    private void OnDestroy()
    {
        SetInputEvent(false);

        DialogueManager.Instance.onStage -= OnDialogueStage;
        DialogueManager.Instance.onText -= OnDialogueText;
    }

    //

    private void SetInputEvent(bool Stage)
    {
        if (Stage)
        {
            SetInputEvent(false);
            InputManager.Instance.onDialogueNext += OnDialogueNext;
            InputManager.Instance.onDialogueSkip += OnDialogueSkip;
        }
        else
        {
            InputManager.Instance.onDialogueNext -= OnDialogueNext;
            InputManager.Instance.onDialogueSkip -= OnDialogueSkip;
        }
    }

    //

    private void OnDialogueNext()
    {
        DialogueManager.Instance.SetNext();
    }

    private void OnDialogueSkip()
    {
        DialogueManager.Instance.SetSkip();
    }

    private void OnDialogueStage(DialogueStageType Stage)
    {
        switch (Stage)
        {
            case DialogueStageType.Start:
                SetInputEvent(true);
                break;
            case DialogueStageType.End:
                SetInputEvent(false);
                SetDialogueHide();
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

    //

    private void SetDialogueHide()
    {
        m_panel.SetActive(false);
        m_dialogueAuthor.gameObject.SetActive(false);
        m_dialogueNone.gameObject.SetActive(false);
    }
}