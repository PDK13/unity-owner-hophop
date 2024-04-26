using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config", menuName = "HopHop/Event Config", order = 0)]
public class EventConfig : ScriptableObject
{
    public List<EventConfigSingle> Data;
}