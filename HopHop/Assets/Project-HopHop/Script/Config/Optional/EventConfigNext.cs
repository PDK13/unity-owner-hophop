using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config-next", menuName = "HopHop/Event Config Next", order = 0)]
public class EventConfigNext : EventConfigOptional
{
    public EventConfigSingle Data;

    //

    public override OptionalType Type => OptionalType.Mode;

#if UNITY_EDITOR

    public override string EditorName => $"{Type.ToString()} : {Option} : {Data.name}";

#endif
}