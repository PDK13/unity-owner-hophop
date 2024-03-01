using System;
using System.Collections.Generic;
using UnityEngine;

public class QueueEventManager : SingletonManager<QueueEventManager>
{
    [SerializeField] private List<QueueEventData> m_queueEvent = new List<QueueEventData>();

    /// <summary>
    /// Add new or get the exist of group queue event!
    /// </summary>
    public QueueEventData SetGroup(string GroupName = "")
    {
        QueueEventData EventData = m_queueEvent.Find(t => t.GroupName == GroupName);
        if (EventData != null)
            return EventData;
        //
        EventData = new QueueEventData(GroupName);
        m_queueEvent.Add(EventData);
        return EventData;
    }

    /// <summary>
    /// Remove group queue!
    /// </summary>
    /// <param name="GroupName"></param>
    /// <returns></returns>
    public void SetRemove(string GroupName = "")
    {
        QueueEventData EventData = m_queueEvent.Find(t => t.GroupName == GroupName);
        if (EventData == null)
            return;
        m_queueEvent.Remove(EventData);
    }
}

[Serializable]
public class QueueEventData
{
    public string GroupName = "";

    private List<IQueueEvent> m_eventQueue = new List<IQueueEvent>();
    private int m_eventQueueIndex = 0;

    //

    public QueueEventData(string GroupName)
    {
        this.GroupName = GroupName;
        //
        m_eventQueue = new List<IQueueEvent>();
        m_eventQueueIndex = 0;
    }

    //

    /// <summary>
    /// Add a event to queue!
    /// </summary>
    /// <param name="EventQueue"></param>
    /// <param name="AddForce">When TRUE, the event will be add without check exist!</param>
    public void SetAdd(IQueueEvent EventQueue, bool AddForce = false)
    {
        if (!AddForce && m_eventQueue.Contains(EventQueue))
            return;
        //
        m_eventQueue.Add(EventQueue);
    }

    /// <summary>
    /// Start invoke the first event or continue the next event in queue!
    /// </summary>
    /// <param name="EventFinal">When event(s) in queue are invoked, then invoke this final event!</param>
    public void SetInvoke(IQueueEvent EventFinal = null)
    {
        if (m_eventQueueIndex <= m_eventQueue.Count - 1)
        {
            m_eventQueue[m_eventQueueIndex].SetInvoke();
            m_eventQueueIndex++;
        }
        else
        if (EventFinal != null)
            EventFinal.SetInvoke();
    }

    /// <summary>
    /// Reset index of event(s) in queue!
    /// </summary>
    public void SetReset(bool ClearQueue)
    {
        if (ClearQueue)
            m_eventQueue = new List<IQueueEvent>();
        m_eventQueueIndex = 0;
    }
}

public interface IQueueEvent
{
    void SetInvoke();
}