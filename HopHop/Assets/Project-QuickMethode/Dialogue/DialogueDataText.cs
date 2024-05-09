using System;
using UnityEngine;

[Serializable]
public class DialogueDataText
{
    public int AuthorIndex; //Use for index of 'AuthorName' and 'AuthorAvatar'
    public string Dialogue;

    public DialogueDataTextDelay Delay = new DialogueDataTextDelay();

    public DialogueDataText()
    {
        //...
    }

    public DialogueDataText(DialogueDataTextDelay Delay)
    {
        this.Delay = Delay;
    }

#if UNITY_EDITOR

    public bool EditorDelayShow { get; set; } = false;

#endif
}