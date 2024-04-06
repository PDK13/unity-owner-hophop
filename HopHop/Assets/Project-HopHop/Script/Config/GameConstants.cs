public class GameConfigInit
{
    public enum Key
    {
        None,
        //Follow
        FollowIdentity,
        FollowIdentityCheck,
        //Switch
        SwitchIdentity,
        SwitchIdentityCheck,
        //Move
        MoveCheckAhead,
        MoveCheckAheadBot,
        //Body
        BodyStatic,
    }

    public const string None = "";

    //Follow
    public const string FollowIdentity = "follow-identity"; //follow-identity-[identity]
    public const string FollowIdentityCheck = "follow-identity-check"; //follow-identity-check-[identity]

    //Switch
    public const string SwitchIdentity = "switch-identity"; //switch-identity-[identity]
    public const string SwitchIdentityCheck = "switch-identity-check"; //switch-identity-check-[identity]

    //Move
    public const string MoveCheckAhead = "move-check-ahead";
    public const string MoveCheckAheadBot = "move-check-ahead-bot";

    //Body
    public const string BodyStatic = "body-static";

    //

    public static bool GetExist(IsometricDataInit Data, Key Key)
    {
        if (Data == null || Key == Key.None)
            return false;
        //
        string KeyCheck = GetKey(Key);
        //
        foreach (string DataCheck in Data.Data)
        {
            if (!DataCheck.Contains(KeyCheck))
                continue;
            //
            return true;
        }
        //
        return false;
    }

    public static string GetData(IsometricDataInit Data, Key Key, bool Full = true)
    {
        if (Data == null || Key == Key.None)
            return None;
        //
        string KeyCheck = GetKey(Key);
        //
        foreach (string DataCheck in Data.Data)
        {
            if (!DataCheck.Contains(KeyCheck))
                continue;
            //
            return Full ? DataCheck : DataCheck.Replace(KeyCheck, "");
        }
        //
        return None;
    }

    public static string GetKey(Key Key)
    {
        switch (Key)
        {
            //Follow
            case Key.FollowIdentity:
                return FollowIdentity;
            case Key.FollowIdentityCheck:
                return FollowIdentityCheck;
            //Switch
            case Key.SwitchIdentity:
                return SwitchIdentity;
            case Key.SwitchIdentityCheck:
                return SwitchIdentityCheck;
            //Move
            case Key.MoveCheckAhead:
                return MoveCheckAhead;
            case Key.MoveCheckAheadBot:
                return MoveCheckAheadBot;
            //Body
            case Key.BodyStatic:
                return BodyStatic;
        }
        return None;
    }
}

public class GameConfigAction
{
    public enum Key
    {
        None,
        //
        Shoot,
    }

    public const string None = "";

    //Shoot
    public const string Shoot = "shoot"; //shoot-[spawm]-[move]-[speed]

    //

    public static bool GetExist(IsometricDataInit Data, Key Key)
    {
        if (Data == null || Key == Key.None)
            return false;
        //
        string KeyCheck = GetKey(Key);
        //
        foreach (string DataCheck in Data.Data)
        {
            if (!DataCheck.Contains(KeyCheck))
                continue;
            //
            return true;
        }
        //
        return false;
    }

    public static string GetData(IsometricDataInit Data, Key Key, bool Full = true)
    {
        if (Data == null || Key == Key.None)
            return None;
        //
        string KeyCheck = GetKey(Key);
        //
        foreach (string DataCheck in Data.Data)
        {
            if (!DataCheck.Contains(KeyCheck))
                continue;
            //
            return Full ? DataCheck : DataCheck.Replace(KeyCheck, "");
        }
        //
        return None;
    }

    public static string GetKey(Key Key)
    {
        switch (Key)
        {
            case Key.Shoot:
                return Shoot;
        }
        return None;
    }
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

    public const string Interactive = "interactive";
    public const string Switch = "switch";
}

public enum TurnType
{
    None,
    //
    Gravity,
    Player,
    Bullet,
    Shoot,
    MovePhysic,
    MoveStatic,
}