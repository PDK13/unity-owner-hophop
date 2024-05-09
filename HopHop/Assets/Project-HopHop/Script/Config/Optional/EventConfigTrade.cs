using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config-trade", menuName = "HopHop/Event Config Trade", order = 0)]
public class EventConfigTrade : EventConfigOptional
{
    public List<string> Item = new List<string>();

    //

    public override OptionalType Type => OptionalType.Trade;

#if UNITY_EDITOR

    public override string EditorName => $"{Type.ToString()} : {Option} : {Item.Count}";

#endif
}