using System;
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

    public bool CurrentAvaible
    {
        get
        {
            if (m_current == null)
                return false;
            //
            if (m_current.Root == null)
                return false;
            //
            return true;
        }
    }

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

    public void SetRefresh()
    {
        m_room = m_room.Where(x => x.Root != null).ToList();
        foreach (IsometricManagerRoom BlockSingle in m_room)
            BlockSingle.SetRefresh();
    }

    public void SetInit()
    {
        if (Manager == null)
        {
            Debug.LogFormat("[Isometric] Manager not found to read map!");
            return;
        }
        //
        m_room.Clear();
        for (int i = 0; i < Manager.transform.childCount; i++)
            SetGenerate(Manager.transform.GetChild(i), false, false);
        //
        SetCurrent();
    }

    public void SetCurrent()
    {
        m_current = m_room.Count == 0 ? SetGenerate("Temp") : m_room[0];
        m_current.Active = true;
    }

    public IsometricManagerRoom SetGenerate(string Name, bool Current = true, bool Active = true)
    {
        IsometricManagerRoom Room = m_room.Find(t => t.Name.Contains(Name));
        if (Room != null)
        {
            Debug.LogFormat("[Isometric] Manager aldready add {0} at a room in world", Name);
            //
            if (Current)
            {
                if (m_current != null)
                    m_current.Active = false;
                m_current = Room;
            }
            //
            Room.Active = Active;
            Room.SetWorldRead();
            //
            return Room;
        }
        //
        Room = new IsometricManagerRoom(Manager, Name);
        m_room.Add(Room);
        //
        if (Current)
        {
            if (m_current != null)
                m_current.Active = false;
            m_current = Room;
        }
        //
        Room.Active = Active;
        Room.SetWorldRead();
        //
        return Room;
    }

    public IsometricManagerRoom SetGenerate(Transform Root, bool Current = true, bool Active = true)
    {
        if (!Root.name.Contains(IsometricManagerRoom.NAME_ROOM))
        {
            Debug.LogFormat("[Isometric] Manager can't add {0} at a room in world", Root.name);
            return null;
        }
        //
        IsometricManagerRoom Room = m_room.Find(t => t.Root.Equals(Root));
        if (Room != null)
        {
            Debug.LogFormat("[Isometric] Manager aldready add {0} at a room in world", Root.name);
            //
            if (Current)
            {
                if (m_current != null)
                    m_current.Active = false;
                m_current = Room;
            }
            //
            Room.Active = Active;
            Room.SetWorldRead();
            //
            return Room;
        }
        //
        Room = new IsometricManagerRoom(Manager, Root);
        m_room.Add(Room);
        //
        if (Current)
        {
            if (m_current != null)
                m_current.Active = false;
            m_current = Room;
        }
        //
        Room.Active = Active;
        Room.SetWorldRead();
        //
        return Room;
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
        m_current.Active = true;
        //
        return RoomFind;
    }

    public void SetRemove(string Name)
    {
        IsometricManagerRoom RoomFind = m_room.Find(t => t.Name == Name);
        if (RoomFind == null)
            return;
        //
        m_room.Remove(RoomFind);
        RoomFind.SetDestroy();
        //
        SetCurrent();
    }

    public void SetRemove(IsometricManagerRoom RoomCheck)
    {
        if (RoomCheck == null)
            return;
        //
        m_room.Remove(RoomCheck);
        RoomCheck.SetDestroy();
        //
        SetCurrent();
    }
}