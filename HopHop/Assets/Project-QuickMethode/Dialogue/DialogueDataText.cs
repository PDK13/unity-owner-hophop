using System;
using UnityEngine;

[Serializable]
public class DialogueDataText
{
    public int AuthorIndex; //Use for index of 'AuthorName' and 'AuthorAvatar'
    public string Dialogue;

    public DialogueDataTextDelay Delay;

    public DialogueDataText()
    {
        //...
    }

    public DialogueDataText(DialogueDataTextDelay Delay)
    {
        this.Delay = Delay;
    }
}