using System;
using UnityEngine;

[Serializable]
public class DialogueDataChoice
{
    public string Text;
    //
    public DialogueConfigSingle Next;
    //
    public int AuthorIndex; //Use index for 'AuthorName' and 'AuthorAvatar'!!
    public string Dialogue;
    //
    public string TriggerCode;
    public GameObject TriggerObject;
}