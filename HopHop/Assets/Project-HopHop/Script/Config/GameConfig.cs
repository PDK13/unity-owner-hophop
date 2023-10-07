using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "game-config", menuName = "", order = 0)]
public class GameConfig : ScriptableObject
{
    public GameConfigKey Key;
    public GameConfigEvent Event;
    public GameConfigCommand Command;
    public GameConfigTag Tag;
}

[Serializable]
public class GameConfigKey
{
    public string Player = "turn-player";
    public string Object = "turn-object";
}

[Serializable]
public class GameConfigEvent
{
    [Tooltip("In 'Event Data', create 1 data with name of this and value is the key same as main block follow")]
    public string Follow = "event-follow";
}

[Serializable]
public class GameConfigCommand
{
    public string Wait = "wait";

    [Tooltip("shoot-[1]-[2]-[3]")]
    public string Shoot = "shoot";
}

[Serializable]
public class GameConfigTag
{
    public string Player = "player";

    public string Character = "character";
    public string Block = "block";
    public string Object = "object";

    public string Slip = "slip";
    public string Slow = "slow";
    public string Water = "water";

    public string Bullet = "bullet";
    public string Trap = "trap";
}

public enum TurnType
{
    None,
    Gravity,
    Player,
    Object,
}