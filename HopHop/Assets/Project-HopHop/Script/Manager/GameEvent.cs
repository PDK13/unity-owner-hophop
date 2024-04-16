using System;
using System.Collections.Generic;

public class GameEvent
{
    public static Action<string, bool> onKey;

    public static void SetKey(string Key, bool State)
    {
        if (string.IsNullOrEmpty(Key))
            return;
        //
        onKey?.Invoke(Key, State);
    }
}

public class GameEventData
{
    public string EventCode = "";
    public List<GameEventDataSingle> EventData = new List<GameEventDataSingle>();
}

public class GameEventDataSingle
{
    public DialogueSingleConfig DialogueData;
    public List<string> CommandData = new List<string>();
}