using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "message-data-config", menuName = "Message Config/Data Config", order = 0)]
public class MessageDataConfig : ScriptableObject
{
    public List<MessageDataConfigText> Message;

    public List<MessageDataConfigChoice> Choice;

    public bool ChoiceAvaible => Choice == null ? false : Choice.Count > 0;
}

[Serializable]
public class MessageDataConfigText
{
    public string Text;
    public float DelayAlpha;
    public float DelaySpace;
    public float DelayMark;
}

[Serializable]
public class MessageDataConfigChoice 
{
    public string Name;
    public string Description;
    public MessageDataConfig Next;
}