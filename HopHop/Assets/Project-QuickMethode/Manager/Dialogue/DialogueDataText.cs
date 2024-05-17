using System;

[Serializable]
public class DialogueDataText
{
    public string Author;
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

    public bool EditorFull { get; set; } = false;

    public bool EditorDelayShow { get; set; } = false;

    public virtual string EditorName => $"{(!string.IsNullOrEmpty(Author) ? Author : "...")} : {Dialogue.ToString()}";

#endif
}