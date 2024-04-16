using System.Collections.Generic;

public class KeyInit
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
