using System;
using System.Collections.Generic;

public class ChoiceManager : SingletonManager<ChoiceManager>
{
    /// <summary>
    /// Choice system start, trigger once when invoke, until end and reset
    /// </summary>
    public Action onStart;

    /// <summary>
    /// Choice system init, trigger muti when once init occur
    /// </summary>
    public Action<EventConfigOptional> onInit;

    /// <summary>
    /// Choice system index, trigger when next or prev current choice check
    /// </summary>
    public Action<int, EventConfigOptional> onIndex;

    /// <summary>
    /// Choice system invoke current choice, trigger once when choice occur
    /// </summary>
    public Action<int, EventConfigOptional> onInvoke;

    /// <summary>
    /// Choice system start, trigger once when invoke, until end and reset
    /// </summary>
    public Action onClear;

    //

    private bool m_active = false;

    private int m_choiceIndex = 0;
    private List<EventConfigOptional> m_choice = new List<EventConfigOptional>();

    //

    public bool Active => m_active;

    public int ChoiceIndex => m_choiceIndex;

    public EventConfigOptional[] Choice => m_choice.ToArray();

    //

    private void Awake()
    {
        SetInstance();
    }

    private void Start()
    {

    }

    //

    public void SetStart()
    {
        if (m_active)
            return;

        m_active = true;
        m_choiceIndex = 0;
        onStart?.Invoke();
        onIndex?.Invoke(m_choiceIndex, m_choice[m_choiceIndex]);
    }

    public void SetInit(params EventConfigOptional[] Data)
    {
        foreach (var DataCheck in Data)
        {
            if (DataCheck == null)
                continue;

            if (m_choice.Contains(DataCheck))
                continue;

            m_choice.Add(DataCheck);
            onInit?.Invoke(DataCheck);
        }
    }

    public void SetNext()
    {
        m_choiceIndex++;
        if (m_choiceIndex > m_choice.Count - 1)
            m_choiceIndex = 0;
        onIndex?.Invoke(m_choiceIndex, m_choice[m_choiceIndex]);
    }

    public void SetPrev()
    {
        m_choiceIndex--;
        if (m_choiceIndex < 0)
            m_choiceIndex = m_choice.Count - 1;
        onIndex?.Invoke(m_choiceIndex, m_choice[m_choiceIndex]);
    }

    public void SetInvoke()
    {
        onInvoke?.Invoke(m_choiceIndex, m_choice[m_choiceIndex]);
    }

    public void SetClear()
    {
        if (!m_active)
            return;

        m_active = false;
        m_choice.Clear();
        onClear?.Invoke();
    }
}