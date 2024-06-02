using System.Collections.Generic;

public class KeyInit : Key
{
    public enum Key
    {
        None,
        //Shoot
        Shoot,
        //Follow
        FollowIdentityBase,
        FollowIdentityCheck,
        //Switch
        SwitchIdentityBase,
        SwitchIdentityCheck,
        //Move
        MoveCheckAheadSide,
        MoveCheckAheadBot,
        //Body
        BodyGravity,
        BodyDynamic,
        //Event
        EventIdentitytBase,
    }

    public const string None = "";

    //Shoot
    public const string Shoot = "shoot"; //shoot-[spawm]-[move]-[speed]

    //Follow
    public const string FollowIdentityBase = "follow-identity-base-"; //follow-identity-base-[identity]
    public const string FollowIdentityCheck = "follow-identity-check-"; //follow-identity-check-[identity]

    //Switch
    public const string SwitchIdentityBase = "switch-identity-base-"; //switch-identity-base-[identity]
    public const string SwitchIdentityCheck = "switch-identity-check-"; //switch-identity-check-[identity]

    //Move
    public const string MoveCheckAheadSide = "move-check-ahead-side";
    public const string MoveCheckAheadBot = "move-check-ahead-bot";

    //Body
    public const string BodyDynamic = "body-dynamic";
    public const string BodyGarvity = "body-gravity";

    //Event
    public const string EventIdentityBase = "event-identity-base-"; //event-identity-base-[identity]

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

    public static List<string> GetDataList(IsometricDataInit Data, Key Key, bool Full = true)
    {
        if (Data == null || Key == Key.None)
            return null;
        //
        List<string> DataList = new List<string>();
        //
        string KeyCheck = GetKey(Key);
        //
        foreach (string DataCheck in Data.Data)
        {
            if (!DataCheck.Contains(KeyCheck))
                continue;
            //
            DataList.Add(Full ? DataCheck : DataCheck.Replace(KeyCheck, ""));
        }
        //
        return DataList;
    }

    public static string GetKey(Key Key)
    {
        switch (Key)
        {
            //Shoot
            case Key.Shoot:
                return Shoot;
            //Follow
            case Key.FollowIdentityBase:
                return FollowIdentityBase;
            case Key.FollowIdentityCheck:
                return FollowIdentityCheck;
            //Switch
            case Key.SwitchIdentityBase:
                return SwitchIdentityBase;
            case Key.SwitchIdentityCheck:
                return SwitchIdentityCheck;
            //Move
            case Key.MoveCheckAheadSide:
                return MoveCheckAheadSide;
            case Key.MoveCheckAheadBot:
                return MoveCheckAheadBot;
            //Body
            case Key.BodyGravity:
                return BodyGarvity;
            case Key.BodyDynamic:
                return BodyDynamic;
            //Event
            case Key.EventIdentitytBase:
                return EventIdentityBase;
        }
        return None;
    }
}
