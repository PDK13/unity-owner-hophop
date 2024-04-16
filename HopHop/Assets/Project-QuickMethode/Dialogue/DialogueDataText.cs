using System;
using UnityEngine;

[Serializable]
public class DialogueDataText
{
    public int AuthorIndex; //Use index for 'AuthorName' and 'AuthorAvatar'!!
    public string Dialogue;
    //
    public DialogueDataTextDelay Delay;
    //
    public string TriggerCode;
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