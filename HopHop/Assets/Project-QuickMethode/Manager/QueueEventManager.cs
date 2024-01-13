using System;
using System.Collections.Generic;
using UnityEngine;

public class QueueEventManager : SingletonManager<QueueEventManager>
{
    [SerializeField] private List<QueueEventData> m_queueEvent = new List<QueueEventData>();

    /// <summary>
    /// Add new or get the exist of group queue event!
    /// </summary>
    public QueueEventData SetGroup(string GroupName = "", bool ClearExist = false)
    {
        QueueEventData EventData = m_queueEvent.Find(t => t.GroupName == GroupName);
        if (EventData != null)
        {
            if (ClearExist)
                EventData.SetClear();
            //
            return EventData;
        }
        //
        EventData = new QueueEventData(GroupName);
        m_queueEvent.Add(EventData);
        return EventData;
    }

    /// <summary>
    /// Invoke group queue event!
    /// </summary>
    public bool SetInvoke(string GroupName = "")
    {
        QueueEventData EventData = m_queueEvent.Find(t => t.GroupName == GroupName);
        if (EventData == null)
            return false;
        //
        if (!EventData.Avaible)
            return false;
        //
        EventData.SetInvoke();
        //
        return true;
    }

    /// <summary>
    /// Remove group queue!
    /// </summary>
    /// <param name="GroupName"></param>
    /// <returns></returns>
    public bool SetRemove(string GroupName = "")
    {
        QueueEventData EventData = m_queueEvent.Find(t => t.GroupName == GroupName);
        if (EventData == null)
            return false;
        //
        m_queueEvent.Remove(EventData);
        //
        return true;
    }
}

[Serializable]
public class QueueEventData
{
    public string GroupName = "";

    public bool ClearOnFinish = true;

    private List<IQueueEvent> m_eventList = new List<IQueueEvent>();
    private int m_eventIndex = 0;

    public bool Avaible
    {
        get
        {
            if (m_eventList == null)
                m_eventList = new List<IQueueEvent>();
            //
            return m_eventIndex < m_eventList.Count;
        }
    }

    //

    public QueueEventData(string GroupName)
    {
        this.GroupName = GroupName;
        //
        m_eventList = new List<IQueueEvent>();
        m_eventIndex = 0;
    }

    //

    /// <summary>
    /// Add a event to queue!
    /// </summary>
    /// <param name="Event"></param>
    public void SetAdd(IQueueEvent Event)
    {
        if (m_eventList == null)
            m_eventList = new List<IQueueEvent>();
        //
        if (m_eventList.Contains(Event))
            return;
        //
        m_eventList.Add(Event);
    }

    /// <summary>
    /// Start invoke the first event or continue the next event in queue!
    /// </summary>
    public void SetInvoke()
    {
        if (m_eventList == null)
            m_eventList = new List<IQueueEvent>();
        //
        if (!Avaible)
        {
            if (ClearOnFinish)
                SetClear();
            return;
        }
        //
        m_eventList[m_eventIndex].SetInvoke();
        m_eventIndex++;
    }

    /// <summary>
    /// Clear all event(s) in queue!
    /// </summary>
    public void SetClear()
    {
        if (m_eventList == null)
            m_eventList = new List<IQueueEvent>();
        //
        m_eventList.Clear();
        m_eventIndex = 0;
    }
}

public interface IQueueEvent
{
    void SetInvoke();
}