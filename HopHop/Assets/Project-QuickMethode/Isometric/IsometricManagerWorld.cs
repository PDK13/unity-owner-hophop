using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class IsometricManagerWorld
{
    [HideInInspector] private IsometricManager Manager;

    [SerializeField] private IsometricManagerRoom m_current = null;
    [SerializeField] private List<IsometricManagerRoom> m_room = new List<IsometricManagerRoom>();

    //

    public IsometricManagerRoom Current => m_current;

    public List<string> RoomName
    {
        get
        {
            List<string> Name = new List<string>();
            foreach (var RoomCheck in m_room)
                Name.Add(RoomCheck.Name);
            return Name;
        }
    }

    //

    public IsometricManagerWorld(IsometricManager Manager)
    {
        this.Manager = Manager;
        SetInit();
    }

    public void SetInit()
    {
        if (Manager == null)
        {
            Debug.LogFormat("[Isometric] Manager not found to read map!");
            return;
        }
        //
        for (int i = 0; i < Manager.transform.childCount; i++)
            SetGenerate(Manager, Manager.transform.GetChild(i), false);
        m_current = m_room.Count == 0 ? null : m_room[0];
    }

    //

    public IsometricManagerRoom SetGenerate(IsometricManager Manager, string Name, bool Active = true)
    {
        IsometricManagerRoom RoomGenerate = new IsometricManagerRoom(Manager, Name);
        m_room.Add(RoomGenerate);
        //
        if (Active)
        {
            if (m_current != null)
                m_current.Active = false;
            m_current = RoomGenerate;
        }
        //
        return RoomGenerate;
    }

    public IsometricManagerRoom SetGenerate(IsometricManager Manager, Transform Root, bool Active = true)
    {
        if (!Root.name.Contains(IsometricManagerRoom.NAME_ROOM))
        {
            Debug.LogFormat("[Isometric] Manager can't add {0} at a room in world", Root.name);
            return null;
        }
        IsometricManagerRoom RoomGenerate = new IsometricManagerRoom(Manager, Root);
        //
        m_room.Add(RoomGenerate);
        //
        if (Active)
        {
            if (m_current != null)
                m_current.Active = false;
            m_current = RoomGenerate;
        }
        //
        return RoomGenerate;
    }

    public IsometricManagerRoom SetActive(string Name)
    {
        IsometricManagerRoom RoomFind = m_room.Find(t => t.Name == Name);
        if (RoomFind == null)
            return null;
        //
        if (m_current != null)
            m_current.Active = false;
        m_current = RoomFind;
        //
        return RoomFind;
    }

    public void SetRemove(string Name)
    {
        IsometricManagerRoom RoomFind = m_room.Find(t => t.Name == Name);
        if (RoomFind == null)
            return;
        //
        if (m_current.Equals(RoomFind))
            m_current = null;
        RoomFind.SetDestroy();
        //
        m_room.Remove(RoomFind);
    }

    public void SetRemove(IsometricManagerRoom RoomCheck)
    {
        if (RoomCheck == null)
            return;
        //
        if (m_current.Equals(RoomCheck))
            m_current = null;
        RoomCheck.SetDestroy();
        //
        m_room.Remove(RoomCheck);
    }
}