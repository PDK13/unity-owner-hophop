using System.Collections.Generic;

public class KeyAction
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
            case Key.Shoot:
                return Shoot;
        }
        return None;
    }
}
