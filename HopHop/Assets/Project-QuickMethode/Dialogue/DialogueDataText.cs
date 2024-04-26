using System;
using UnityEngine;

[Serializable]
public class DialogueDataText
{
    public int AuthorIndex; //Use for index of 'AuthorName' and 'AuthorAvatar'
    public string Dialogue;

    public DialogueDataTextDelay Delay;

    public string TriggerCode; //NOTE: Need group this same as 'Author Index'
    public GameObject TriggerObject;

    public DialogueDataText()
    {
        //...
    }

    public DialogueDataText(DialogueDataTextDelay Delay)
    {
        this.Delay = Delay;
    }
}