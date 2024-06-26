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

    [SerializeField] private EventConfig m_eventConfig;

    public StepType Step => StepType.Event;

    //

    private Coroutine m_iSetEventActive;

    //

    private void Awake()
    {
        SetInstance();
    }

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

    public void ISetTurnStart(int Step) { }

    public void ISetStepEnd(string Step) { }

    public void ISetStepStart(string Step) { }

    public void ISetTurnEnd(int Step) { }

    //Event

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
                yield return new WaitUntil(() => !DialogueManager.Instance.Active);
                yield return new WaitUntil(() => TurnManager.Instance.StepCurrent.Step == StepType.Event.ToString());
            }

            if (Event.Data[i].Choice != null ? Event.Data[i].Choice.Count > 0 : false)
            {
                SetEventChoice(Event.Data[i].Choice);

                //NOTE: Base on Player choice, next event will be occur!
                yield return new WaitUntil(() => !OptionalManager.Instance.Active);
            }
        }

        TurnManager.Instance.SetEndStep(StepType.Event, this);
        onEvent?.Invoke(false);

        m_iSetEventActive = null;
    }

    //Event - Dialogue

    private void SetEventDialogue(DialogueConfigSingle Data)
    {
        DialogueManager.Instance.SetStart(Data);
    }

    //Event - Command

    private void SetEventCommand(List<string> Data)
    {
        foreach (string DataCheck in Data)
        {
            List<string> DataRead = QString.GetUnSplitString("::", DataCheck);
            switch (DataRead[0])
            {
                case KeyCommand.Spawm:
                    //spawm/[BlockName]/[X;Y;H]/[BlockIdentity]
                    SetEventCommandSpawm(DataRead.ToArray());
                    break;
                case KeyCommand.Move:
                    //move/[BlockIdentity]/[Dir]/[Length]
                    SetEventCommandMove(DataRead.ToArray());
                    break;
                case KeyCommand.Character:
                    //character/[BlockIdentity]/[CharacterName]/[SkinIndex]
                    SetEventCommandCharacter(DataRead.ToArray());
                    break;
            }
        }
    }

    private void SetEventCommandSpawm(string[] DataRead)
    {
        //spawm/[BlockName]/[X;Y;H]/[BlockIdentity]
        string BlockName = DataRead[1];
        IsometricVector Pos = IsometricVector.GetUnSplit(DataRead[2]);
        string BlockIdentity = DataRead.Length > 3 ? DataRead[3] : "";
        IsometricManager.Instance.World.Current.SetBlockCreate(Pos, IsometricManager.Instance.List.GetList(BlockName), false, BlockIdentity);
    }

    private void SetEventCommandMove(string[] DataRead)
    {
        //move/[BlockIdentity]/[Dir]/[Length]
        string BlockIdentity = DataRead[1];
        IsometricVector Dir = IsometricVector.GetDirDeEncypt(DataRead[2]);
        int Length = int.Parse(DataRead[3]);
        IsometricBlock Block = IsometricManager.Instance.World.Current.GetBlockByIdentity(BlockIdentity)[0];
        for (int i = 0; i < Length; i++)
            Block.GetComponent<IBodyCommand>().ISetCommandMove(Dir);
    }

    private void SetEventCommandCharacter(string[] DataRead)
    {
        //character/[BlockIdentity]/[CharacterName]/[SkinIndex]
        string BlockIdentity = DataRead[1];
        string CharacterName = DataRead[2];
        int SkinIndex = DataRead.Length > 3 ? int.Parse(DataRead[3]) : 0;
        IsometricBlock Block = IsometricManager.Instance.World.Current.GetBlockByIdentity(BlockIdentity)[0];
        Block.GetComponent<BodyCharacter>().SetCharacter(CharacterName, SkinIndex);
    }

    //Event - Choice

    private void SetEventChoice(List<OptionalConfigSingle> Data)
    {
        OptionalManager.Instance.onInvoke += OnEventChoiceInvoke;
        OptionalManager.Instance.SetInit(Data.ToArray());
        OptionalManager.Instance.SetStart();
    }

    private void OnEventChoiceInvoke(int Index, OptionalConfigSingle Optional)
    {
        OptionalManager.Instance.onInvoke -= OnEventChoiceInvoke;
        switch (Optional.Type)
        {
            case OptionalType.None:
                //...
                break;
            case OptionalType.Event:
                SetEventActive(Optional as EventConfigSingle); //NOTE: Scriptable type can be force back to it's original!
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