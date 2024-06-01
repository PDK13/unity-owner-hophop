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

    public IsometricManagerMap Current
    {
        get
        {
            if (m_current != null)
            {
                if (m_current.Root == null)
                    m_current = null;
            }
            return m_current;
        }
        set
        {
            if (value.Root.parent != m_manager.transform)
                return;
            m_current = value;
        }
    }

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
            Debug.Log("Manager can't be null!");
            return;
        }
        m_manager = Manager;
    }

    //

    public void SetRefresh()
    {
        m_map = m_map.Where(x => x.Root != null).ToList();
        //
        foreach (IsometricManagerMap MapCheck in m_map)
            MapCheck.SetRefresh();
        //
        for (int i = 0; i < m_manager.transform.childCount; i++)
            SetGenerate(m_manager.transform.GetChild(i));
        //
        for (int i = 0; i < m_map.Count; i++)
            m_map[i].Active = false;
        //
        m_current = null;
    }

    public IsometricManagerMap SetGenerate(string Name)
    {
        IsometricManagerMap Room = m_map.Find(t => t.Name == Name);
        if (Room != null)
        {
            Room.SetWorldRead();
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
            return null;
        //
        IsometricManagerMap Room = m_map.Find(t => t.Root.Equals(Root));
        if (Room != null)
        {
            Room.SetWorldRead();
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

    //

    public void SetActive(string Name)
    {
        for (int i = 0; i < m_map.Count; i++)
        {
            if (m_map[i].Name == Name)
            {
                m_current = m_map[i];
                m_current.Active = true;
            }
            else
                m_map[i].Active = false;
        }
    }

    //

    public void SetRemove(string Name)
    {
        IsometricManagerMap RoomFind = m_map.Find(t => t.Name == Name);
        if (RoomFind == null)
            return;
        //
        QGameObject.SetDestroy(RoomFind.Root);
        m_map.Remove(RoomFind);
    }

    public void SetRemove(IsometricManagerMap RoomCheck)
    {
        if (RoomCheck == null)
            return;
        //
        QGameObject.SetDestroy(RoomCheck.Root.gameObject);
        m_map.Remove(RoomCheck);
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