public class ConstGameKey
{
    public const string TURN_PLAYER = "turn-player";
    public const string TURN_OBJECT = "turn-object";

    public const string EVENT_FOLLOW = "event-follow";
}

public class ConstDataKey
{
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