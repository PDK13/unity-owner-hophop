using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConfigOptional : ScriptableObject
{
    public string Option = "Option";
    public DialogueConfigSingle DialogueTip;

    //

    public virtual OptionalType Type => OptionalType.None;

#if UNITY_EDITOR

    public virtual string EditorName => $"{Type.ToString()} : {Option}";

#endif
}