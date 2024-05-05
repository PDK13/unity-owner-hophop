using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EventManager : SingletonManager<EventManager>, ITurnManager
{
    public Action<bool> onEvent;
    public Action<bool> onEventDialogue;

    [SerializeField] private EventConfig m_eventConfig;

    public TurnType Turn => TurnType.Event;

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

        if (Event == null)
        {
            Debug.LogWarningFormat("Event '{0}' not found", Identity);
            return false;
        }

        StartCoroutine(ISetEventActive(Event));

        return true;
    }

    private IEnumerator ISetEventActive(EventConfigSingle Event)
    {
        TurnManager.SetAdd(TurnType.Event, this); //NOTE: Should not stop Turn Manager because world need Gravity and somethings else...

        onEvent?.Invoke(true);

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
                SetEventCommand(Event.Data[i].Command);

            if (Event.Data[i].Choice != null ? Event.Data[i].Choice.Count > 0 : false)
            {
                //Choice event trigger!
            }

            if (Event.Data[i].WaitForce)
                //Wait until all event done it's work!
                yield return new WaitUntil(() => !DialogueManager.Instance.Active);
        }

        onEvent?.Invoke(false);

        TurnManager.SetEndStep(TurnType.Event, this);
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
            switch (DataRead[0])
            {
                case KeyEvent.Player:
                    switch (DataRead[1])
                    {
                        case KeyEvent.Character:
                            //player-character-[Character]
                            WorldManager.Instance.Player.GetComponent<BodyCharacter>().SetCharacter(DataRead[2], 0);
                            break;
                        case KeyEvent.Move:
                            //player-move-[Dir]-[Length]
                            WorldManager.Instance.Player.GetComponent<BodyPhysic>().SetControlMove(IsometricVector.GetDirDeEncypt(DataRead[2]) * (int.Parse(DataRead[3])), true);
                            break;
                    }
                    break;
                case KeyEvent.All:
                    //...
                    break;
                default:
                    //...
                    break;
            }
        }
    }
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