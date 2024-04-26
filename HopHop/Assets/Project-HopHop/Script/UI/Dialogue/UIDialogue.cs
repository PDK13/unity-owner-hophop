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

    private void Awake()
    {
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

            m_dialogueAuthor.GetComponentInChildren<Image>().sprite = Avatar;
            DialogueManager.Instance.Tmp = m_dialogueAuthor.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            m_dialogueAuthor.gameObject.SetActive(false);
            m_dialogueNone.gameObject.SetActive(true);

            DialogueManager.Instance.Tmp = m_dialogueNone.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}