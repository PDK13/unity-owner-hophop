using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : SingletonManager<EventManager>
{
    private string m_eventIdentityBaseCurrent;

    public bool SetEventActive(string Identity)
    {
        Debug.Log("[Event] " + Identity);
        return false;
    }
}