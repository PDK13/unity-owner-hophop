using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EventManager : SingletonManager<EventManager>, ITurnManager
{
    private const int COMMAND_IDENTITY = 0;
    private const int COMMAND_EXCUTE = 1;

    public Action<bool> onEvent;
    public Action<bool> onEventDialogue;

    [SerializeField] private EventConfig m_eventConfig;

    public StepType Step => StepType.Event;

    //

    private Coroutine m_iSetEventActive;

    //

#if UNITY_EDITOR

    public void SetConfigFind()
    {
        if (m_eventConfig != null)
            return;

        var eventConfigFound = QUnityAssets.GetScriptableObject<EventConfig>("", false);

        if (eventConfigFound == null)
        {
            Debug.Log("[Event] Config not found, please create one");
            return;
        }

        if (eventConfigFound.Count == 0)
        {
            Debug.Log("[Event] Config not found, please create one");
            return;
        }

        if (eventConfigFound.Count > 1)
            Debug.Log("[Event] Config found more than one, get the first one found");

        m_eventConfig = eventConfigFound[0];

        QUnityEditor.SetDirty(this);
    }

#endif

    //

    public void ISetStepEnd(string Step) { }

    public void ISetStepStart(string Step) { }

    public void ISetTurn(int Step) { }

    //

    public bool SetEventActive(string Identity)
    {
        EventConfigSingle Event = m_eventConfig.Data.Find(t => t.name == Identity);
        return SetEventActive(Event);
    }

    public bool SetEventActive(EventConfigSingle Event)
    {
        if (Event == null)
            return false;

        if (m_iSetEventActive == null)
        {
            TurnManager.Instance.SetAdd(StepType.Event, this); //Should not stop Turn Manager because world need Gravity and somethings else...
            onEvent?.Invoke(true);
        }

        if (m_iSetEventActive != null)
            StopCoroutine(m_iSetEventActive);
        m_iSetEventActive = StartCoroutine(ISetEventActive(Event));

        return true;
    }

    private IEnumerator ISetEventActive(EventConfigSingle Event)
    {
        for (int i = 0; i < Event.Data.Count; i++)
        {
            if (Event.Data[i] == null)
            {
                Debug.LogWarningFormat("Event {0} not found to active", Event.name);
                continue;
            }

            if (Event.Data[i].Dialogue != null)
                SetEventDialogue(Event.Data[i].Dialogue);

            if (Event.Data[i].Command != null ? Event.Data[i].Command.Count > 0 : false)
                //NOTE: As Command Event, it will add more event(s) Step before this base event Step started
                SetEventCommand(Event.Data[i].Command);

            if (Event.Data[i].WaitForce)
            {
                TurnManager.Instance.SetEndMove(StepType.Event, this);

                //NOTE: Turn Manager wait 1 frame to wait all another child of it's Step finish their own code, so delay to check this Step later
                yield return null;
                yield return null;
                yield return new WaitUntil(() => TurnManager.Instance.StepCurrent.Step == StepType.Event.ToString() && !DialogueManager.Instance.Active);
            }

            if (Event.Data[i].Choice != null ? Event.Data[i].Choice.Count > 0 : false)
            {
                SetEventChoice(Event.Data[i].Choice);

                //NOTE: Base on Player choice, next event will be occur!
                yield return new WaitUntil(() => true);
            }
        }

        TurnManager.Instance.SetEndStep(StepType.Event, this);
        onEvent?.Invoke(false);

        m_iSetEventActive = null;
    }

    private void SetEventDialogue(DialogueConfigSingle Data)
    {
        onEventDialogue?.Invoke(true);
        DialogueManager.Instance.SetStart(Data);
    }

    private void SetEventCommand(List<string> Data)
    {
        foreach (string DataCheck in Data)
        {
            List<string> DataRead = QEncypt.GetDencyptString('-', DataCheck);
            switch (DataRead[COMMAND_IDENTITY])
            {
                case KeyCommand.Player:
                    switch (DataRead[COMMAND_EXCUTE])
                    {
                        case KeyCommand.Character:
                            //player-character-[Character]
                            WorldManager.Instance.Player.GetComponent<BodyCharacter>().SetCharacter(DataRead[2], 0);
                            break;
                        case KeyCommand.Move:
                            //player-move-[Dir]-[Length]
                            for (int j = 0; j < int.Parse(DataRead[3]); j++)
                                WorldManager.Instance.Player.GetComponent<IBodyCommand>().ISetCommandMove(IsometricVector.GetDirDeEncypt(DataRead[2]));
                            break;
                    }
                    break;
                default:
                    string Identity = DataRead[COMMAND_IDENTITY];
                    break;
            } //NOTE: First data check identity to excute command
        }
    }

    private void SetEventChoice(List<EventConfigSingleDataChoice> Data)
    {
        foreach (var DataCheck in Data)
        {
            if (DataCheck == null ? true : DataCheck.Event == null)
                continue;

            switch (DataCheck.Event.Type)
            {
                case OptionalType.None:
                    //...
                    break;
                case OptionalType.Event:
                    //...
                    break;
                case OptionalType.Trade:
                    //...
                    break;
                case OptionalType.Mode:
                    //...
                    break;
            }
        }
    }
}

public enum OptionalType
{
    None,
    Event,
    Trade,
    Mode,
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventManager))]
public class EventManagerEditor : Editor
{
    private EventManager m_target;

    private SerializedProperty m_eventConfig;

    private void OnEnable()
    {
        m_target = target as EventManager;

        m_eventConfig = QUnityEditorCustom.GetField(this, "m_eventConfig");

        m_target.SetConfigFind();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);

        QUnityEditorCustom.SetField(m_eventConfig);

        QUnityEditorCustom.SetApply(this);
    }
}

#endif