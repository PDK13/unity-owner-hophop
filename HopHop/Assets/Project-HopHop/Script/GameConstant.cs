public class GameKey
{
    public const string TURN_PLAYER = "turn-player";
    public const string TURN_OBJECT = "turn-object";

    public const string COMMAND_WAIT = "wait";
    public const string COMMAND_SHOOT = "shoot";

    public const string EVENT_FOLLOW = "event-follow";
}

public class GameTag
{
    public const string CHARACTER = "character";
    public const string BLOCK = "block";
    public const string OBJECT = "object";

    public const string SLIP = "slip";
    public const string SLOW = "slow";
    public const string WATER = "water";

    public const string BULLET = "bullet";
    public const string TRAP = "trap";
}

public enum TypeCharacter
{
    Angle,
    Bunny,
    Cat,
    Frog,
    Mow,
}

public enum TypeTurn
{
    None,
    //Turn Primary:
    Player,
    Object,
    //Turn Add:
    Gravity,
}

public enum TypeDelay
{
    None,
    Gravtiy,
}