using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config-mode", menuName = "HopHop/Event Config Mode", order = 0)]
public class EventConfigMode : EventConfigOptional
{
    public string Mode = "";

    //

    public override OptionalType Type => OptionalType.Mode;

#if UNITY_EDITOR

    public override string EditorName => $"{Type.ToString()} : {Option} : {Mode}";

#endif
}