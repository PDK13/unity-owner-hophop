using UnityEngine;

public class GameConfigKey
{
    public const string Player = "turn-player";
    public const string Object = "turn-object";
}

public class GameConfigEvent
{
    [Tooltip("In 'Event Data', create 1 data with name of this and value is the key same as main block follow")]
    public const string Follow = "event-follow";
}

public class GameConfigAction
{
    public const string Wait = "wait";
    public const string Shoot = "shoot"; //shoot-[spawm]-[move]-[speed]
    public const string Move = "move";
}

public class GameConfigInit
{
    public const string MoveCheckAhead = "move-check-ahead";
    public const string MoveCheckAheadBot = "move-check-ahead-bot";

    //Character
    public const string CharacterPush = "character-push";
}

public class GameConfigTag
{
    public const string Player = "player";
    public const string Enermy = "enermy";

    public const string Character = "character";
    public const string Block = "block";
    public const string Object = "object";

    public const string Slip = "slip";
    public const string Slow = "slow";
    public const string Water = "water";

    public const string Bullet = "bullet";
    public const string Trap = "trap";
}

public enum TurnType
{
    None,
    Gravity,
    Player,
    MovePhysic,
    MoveStatic,
    Bullet,
    Shoot,
}