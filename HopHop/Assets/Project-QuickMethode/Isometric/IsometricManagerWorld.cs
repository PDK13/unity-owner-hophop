using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class IsometricManagerWorld
{
    private IsometricManager m_manager;

    [SerializeField] private IsometricManagerMap m_current = null;
    [SerializeField] private List<IsometricManagerMap> m_map = new List<IsometricManagerMap>();

    //

    public IsometricManagerMap Current => m_current;

    public List<string> ListMapName
    {
        get
        {
            List<string> Name = new List<string>();
            foreach (var RoomCheck in m_map)
                Name.Add(RoomCheck.Name);
            return Name;
        }
    }

    //

    public IsometricManagerWorld(IsometricManager Manager)
    {
        if (Manager == null)
        {
            Debug.Log("[Isometric] Manager can't be null!");
            return;
        }
        m_manager = Manager;
    }

    //

    public void SetRefresh()
    {
        m_map = m_map.Where(x => x.Root != null).ToList();
        //
        foreach (IsometricManagerMap BlockSingle in m_map)
            BlockSingle.SetRefresh();
        //
        for (int i = 0; i < m_manager.transform.childCount; i++)
            SetGenerate(m_manager.transform.GetChild(i));
        //
        SetCurrent();
    }

    public void SetCurrent()
    {
        m_current = m_map.Count == 0 ? SetGenerate("Temp") : m_map[0];
        m_current.Active = true;
    }

    public IsometricManagerMap SetGenerate(string Name)
    {
        IsometricManagerMap Room = m_map.Find(t => t.Name.Contains(Name));
        if (Room != null)
        {
            Debug.LogFormat("[Isometric] Manager aldready add {0} at a room in world", Name);
            //
            Room.SetWorldRead();
            //
            return Room;
        }
        //
        Room = new IsometricManagerMap(m_manager);
        Room.SetInit(Name);
        Room.SetWorldRead();
        m_map.Add(Room);
        //
        return Room;
    }

    public IsometricManagerMap SetGenerate(Transform Root)
    {
        if (!Root.name.Contains(IsometricManagerMap.NAME_ROOM))
        {
            Debug.LogFormat("[Isometric] Manager can't add {0} at a room in world", Root.name);
            return null;
        }
        //
        IsometricManagerMap Room = m_map.Find(t => t.Root.Equals(Root));
        if (Room != null)
        {
            Debug.LogFormat("[Isometric] Manager aldready add {0} at a room in world", Root.name);
            //
            Room.SetWorldRead();
            //
            return Room;
        }
        //
        Room = new IsometricManagerMap(m_manager);
        Room.SetInit(Root);
        Room.SetWorldRead();
        //
        m_map.Add(Room);
        //
        return Room;
    }

    public IsometricManagerMap SetActive(string Name)
    {
        IsometricManagerMap RoomFind = m_map.Find(t => t.Name == Name);
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
        IsometricManagerMap RoomFind = m_map.Find(t => t.Name == Name);
        if (RoomFind == null)
            return;
        //
        QGameObject.SetDestroy(RoomFind.Root);
        m_map.Remove(RoomFind);
        //
        SetCurrent();
    }

    public void SetRemove(IsometricManagerMap RoomCheck)
    {
        if (RoomCheck == null)
            return;
        //
        QGameObject.SetDestroy(RoomCheck.Root);
        m_map.Remove(RoomCheck);
        //
        SetCurrent();
    }

    public void SetRemoveAll()
    {
        m_current = null;
        m_map.Clear();
        //
        for (int i = 0; i < m_manager.transform.childCount; i++)
            QGameObject.SetDestroy(m_manager.transform.GetChild(0).gameObject);
    }
}