using UnityEngine;

public class EventConfigOptional : ScriptableObject
{
    public string OptionName = "Option";
    public DialogueConfigSingle OptionalTip;

    //

    public virtual OptionalType Type => OptionalType.None;

#if UNITY_EDITOR

    public virtual string EditorName => $"{(!string.IsNullOrEmpty(OptionName) ? OptionName : "...")} || {Type.ToString()} : {this.name}";

    public bool EditorFull { get; set; } = false;

#endif
}