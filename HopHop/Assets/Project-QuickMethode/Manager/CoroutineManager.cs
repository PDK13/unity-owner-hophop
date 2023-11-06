using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//From: Kiệt
public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance { get; private set; }

    private class CoroutineTask
    {
        public long Id { get; set; }
        public bool Running { get; set; }
        public bool Paused { get; set; }

        public CoroutineTask(long id)
        {
            Id = id;
            Running = true;
            Paused = false;
        }

        public IEnumerator ISetCoroutineWrapper(IEnumerator co)
        {
            IEnumerator coroutine = co;
            while (Running)
            {
                if (Paused)
                {
                    yield return null;
                }
                else
                {
                    if (coroutine != null)
                    {
                        try
                        {
                            bool ret = coroutine.MoveNext();
                            if (ret == false)
                            {
                                Running = false;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message + "\n" + e.StackTrace);
                        }
                        yield return coroutine.Current;
                    }
                    else
                    {
                        Running = false;
                    }
                }
            }
            m_iCoroutines.Remove(Id.ToString());
        }
    }

    private static Dictionary<string, CoroutineTask> m_iCoroutines;

    private void Awake()
    {
        Instance = this;
        m_iCoroutines = new Dictionary<string, CoroutineTask>();
    }

    private void OnDestroy()
    {
        foreach (CoroutineTask task in m_iCoroutines.Values)
        {
            task.Running = false;
        }

        m_iCoroutines.Clear();
    }

    private long m_currentID;

    private long GetNewId()
    {
        return ++m_currentID;
    } //Get a new ID after each new start coroutine!!

    #region Coroutine

    //NOTE: ID can be get when called 'SetStartCoroutine' methode!!

    public long SetCoroutineStart(IEnumerator Coroutine)
    {
        if (gameObject.activeSelf)
        {
            CoroutineTask task = new CoroutineTask(GetNewId());
            m_iCoroutines.Add(task.Id.ToString(), task);
            StartCoroutine(task.ISetCoroutineWrapper(Coroutine));
            return task.Id;
        }
        return -1;
    } //When called start coroutine, use a varible to stored id!!

    public void SetCoroutineStop(long ID)
    {
        if (m_iCoroutines.ContainsKey(ID.ToString()))
        {
            CoroutineTask task = m_iCoroutines[ID.ToString()];
            task.Running = false;
            m_iCoroutines.Remove(ID.ToString());
        }
    } //Use id stored to stop coroutine!!

    public void SetCoroutinePause(long ID)
    {
        if (m_iCoroutines.ContainsKey(ID.ToString()))
        {
            CoroutineTask task = m_iCoroutines[ID.ToString()];
            task.Paused = true;
        }
        else
        {
            Debug.LogError("coroutine: " + ID.ToString() + " is not exist!");
        }
    } //Use id stored to pause coroutine!!

    public void SetCoroutineResume(long ID)
    {
        if (m_iCoroutines.ContainsKey(ID.ToString()))
        {
            CoroutineTask task = m_iCoroutines[ID.ToString()];
            task.Paused = false;
        }
        else
        {
            Debug.LogError("coroutine: " + ID.ToString() + " is not exist!");
        }
    } //Use id stored to resume coroutine!!

    #endregion

    #region Action

    public long SetActionDelay(float Delay, Action onCalled)
    {
        return SetCoroutineStart(ISetActionDelay(Delay, onCalled));
    }

    private IEnumerator ISetActionDelay(float Delay, Action onCalled)
    {
        if (Delay >= 0)
        {
            yield return new WaitForSeconds(Delay);
        }

        if (onCalled != null)
        {
            onCalled();
        }
    }

    public long SetActionDelay(float Delay, Action<object> onCalled, object Param)
    {
        return SetCoroutineStart(ISetActionDelay(Delay, onCalled, Param));
    }

    private IEnumerator ISetActionDelay(float Delay, Action<object> onCalled, object Param)
    {
        if (Delay >= 0)
        {
            yield return new WaitForSeconds(Delay);
        }

        if (onCalled != null)
        {
            onCalled(Param);
        }
    }

    public long SetActionDelayRealTime(float Delay, Action onCalled)
    {
        return SetCoroutineStart(ISetActionDelayRealTime(Delay, onCalled));
    }

    private IEnumerator ISetActionDelayRealTime(float Delay, Action onCalled)
    {
        if (Delay >= 0)
        {
            yield return new WaitForSecondsRealtime(Delay);
        }

        if (onCalled != null)
        {
            onCalled();
        }
    }

    public long SetActionDelayRealTime(float Delay, Action<object> onCalled, object Param)
    {
        return SetCoroutineStart(ISetActionDelayRealTime(Delay, onCalled, Param));
    }

    private IEnumerator ISetActionDelayRealTime(float Delay, Action<object> onCalled, object Param)
    {
        if (Delay >= 0)
        {
            yield return new WaitForSecondsRealtime(Delay);
        }

        if (onCalled != null)
        {
            onCalled(Param);
        }
    }

    #endregion
}