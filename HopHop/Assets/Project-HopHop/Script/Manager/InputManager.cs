using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonManager<InputManager>
{
    public const float DELAY_PRESS_FRAME = 20;

    //

    public Action onAccept;
    public Action onCancel;

    public Action onUp;
    public Action onDown;
    public Action onLeft;
    public Action onRight;
    public Action onStand;

    public Action onInteracte;

    public Action onCharacterNext;
    public Action onCharacterPrev;

    public Action onDialogueNext;
    public Action onDialogueSkip;

    public Action onChoiceNext;
    public Action onChoicePrev;
    public Action onChoiceInvoke;

    //

    [SerializeField] private KeyCode m_up = KeyCode.UpArrow;
    [SerializeField] private KeyCode m_down = KeyCode.DownArrow;
    [SerializeField] private KeyCode m_left = KeyCode.LeftArrow;
    [SerializeField] private KeyCode m_right = KeyCode.RightArrow;

    [SerializeField] private KeyCode m_primary = KeyCode.Z;
    [SerializeField] private KeyCode m_secondary = KeyCode.X;

    [SerializeField] private KeyCode m_characterNext = KeyCode.Q;
    [SerializeField] private KeyCode m_characterPrev = KeyCode.E;

    //

    private class InputDelay
    {
        public KeyCode Key;
        public float Step;

        public InputDelay(KeyCode Key, float Delay)
        {
            this.Key = Key;
            this.Step = Delay;
        }
    }

    private List<InputDelay> m_delay = new List<InputDelay>();

    //

    private void Awake()
    {
        SetInstance();
    }

    private void Update()
    {
        SetPress(m_up, onUp, onChoicePrev);
        SetPress(m_down, onDown, onChoiceNext);
        SetPress(m_left, onLeft);
        SetPress(m_right, onRight);

        SetPress(m_primary, onInteracte, onAccept, onDialogueNext, onChoiceInvoke);
        SetPress(m_secondary, onStand, onCancel, onDialogueSkip);

        SetPress(m_characterNext, onCharacterNext);
        SetPress(m_characterPrev, onCharacterPrev);
    }

    //

    private void SetPress(KeyCode Key, params Action[] Action)
    {
        if (Input.GetKeyDown(Key))
        {
            SetAction(Action);

            if (m_delay.Exists(t => t.Key == Key))
            {
                Debug.LogWarningFormat("Input {0} delay exist to add", Key);
                return;
            }

            m_delay.Add(new InputDelay(Key, DELAY_PRESS_FRAME));
        }
        else
        {
            var Delay = m_delay.Find(t => t.Key == Key);

            if (!m_delay.Exists(t => t.Key == Key))
            {
                //Debug.LogWarningFormat("Input {0} delay not exist to check", Key);
                return;
            }

            if (Input.GetKey(Key))
            {
                if (Delay.Step > 0)
                    Delay.Step--;
                else
                {
                    SetAction(Action);
                    Delay.Step = DELAY_PRESS_FRAME;
                }
            }
            else
            if (Input.GetKeyUp(Key))
                m_delay.Remove(Delay);
        }
    }

    private void SetAction(params Action[] Action)
    {
        foreach (var Check in Action)
            Check?.Invoke();
    }
}