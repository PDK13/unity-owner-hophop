using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour
{
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

        DialogueManager.Instance.onStage += OnDialogueStage;
        DialogueManager.Instance.onText += OnDialogueText;
    }

    private void Start()
    {
        m_dialogueAuthor.gameObject.SetActive(false);
        m_dialogueNone.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DialogueManager.Instance.onStage -= OnDialogueStage;
        DialogueManager.Instance.onText -= OnDialogueText;
    }

    //

    private void OnDialogueStage(DialogueStageType Stage)
    {
        switch (Stage)
        {
            case DialogueStageType.Start:
                //...
                break;
            case DialogueStageType.End:
                m_dialogueAuthor.gameObject.SetActive(false);
                m_dialogueNone.gameObject.SetActive(false);
                break;
        }
    }

    private void OnDialogueText(DialogueDataText Text)
    {
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