using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class QPlayerPrefs
{
    #region ==================================== Set

    #region ------------------------------------ Set Clear

    public static void SetValueClearAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public static void SetValueClear(string Name)
    {
        if (!GetValueExist(Name))
        {
            Debug.LogError("SetValueClear: Not Exist" + "\"" + Name + "\"");
            return;
        }
        PlayerPrefs.DeleteKey(Name);
        PlayerPrefs.Save();
    }

    #endregion

    #region ------------------------------------ Set Primary

    //Single

    public static void SetValue(string Name, string Value)
    {
        PlayerPrefs.SetString(Name, Value);
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, int Value)
    {
        PlayerPrefs.SetInt(Name, Value);
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, float Value)
    {
        PlayerPrefs.SetFloat(Name, Value);
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, bool Value)
    {
        PlayerPrefs.SetInt(Name, (Value) ? 1 : 0);
        PlayerPrefs.Save();
    }

    //Params

    public static void SetValue(string Name, char Key, params string[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, char Key, params int[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, char Key, params float[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, char Key, params bool[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    #endregion

    #region ------------------------------------ Set Vector

    public static void SetValue(string Name, Vector2 Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector2(';', Value));
    }

    public static void SetValue(string Name, Vector2Int Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector2Int(';', Value));
    }

    public static void SetValue(string Name, Vector3 Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector3(';', Value));
    }

    public static void SetValue(string Name, Vector3Int Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector3Int(';', Value));
    }

    #endregion

    #region ------------------------------------ Set Enum

    //Single

    public static void SetValue<T>(string Name, T Value) where T : Enum
    {
        PlayerPrefs.SetInt(Name, QEnum.GetChoice(Value));
        PlayerPrefs.Save();
    }

    //Params

    public static void SetValueEnum<T>(string Name, char Key, params T[] Value) where T : Enum
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    #endregion

    #region ------------------------------------ Set DateTime

    public static void SetValue(string Name, DateTime Value, string Format)
    {
        SetValue(Name, Value.ToString(Format));
    }

    #endregion

    #endregion

    #region ==================================== Get

    #region ------------------------------------ Get Exist

    public static bool GetValueExist(string Name)
    {
        return PlayerPrefs.HasKey(Name);
    }

    #endregion

    #region ------------------------------------ Get Primary

    //Single

    public static string GetValueString(string Name, string Default = "")
    {
        return PlayerPrefs.GetString(Name, Default);
    }

    public static int GetValueInt(string Name, int Default = 0)
    {
        return PlayerPrefs.GetInt(Name, Default);
    }

    public static float GetValueFloat(string Name, float Default = 0.0f)
    {
        return PlayerPrefs.GetFloat(Name, Default);
    }

    public static bool GetValueBool(string Name, bool Default = false)
    {
        if (PlayerPrefs.GetInt(Name, 0) == 1)
        {
            return true;
        }
        return Default;
    }

    //Params

    public static List<string> GetValueString(string Name, char Key)
    {
        return QEncypt.GetDencyptString(Key, GetValueString(Name));
    }

    public static List<int> GetValueInt(string Name, char Key)
    {
        return QEncypt.GetDencyptInt(Key, GetValueString(Name));
    }

    public static List<float> GetValueFloat(string Name, char Key)
    {
        return QEncypt.GetDencyptFloat(Key, GetValueString(Name));
    }

    public static List<bool> GetValueBool(string Name, char Key)
    {
        return QEncypt.GetDencyptBool(Key, GetValueString(Name));
    }

    #endregion

    #region ------------------------------------ Get Vector

    public static Vector2 SetValueVector2(string Name)
    {
        return QEncypt.GetDencyptVector2(';', GetValueString(Name));
    }

    public static Vector2Int SetValueVector2Int(string Name)
    {
        return QEncypt.GetDencyptVector2Int(';', GetValueString(Name));
    }

    public static Vector3 SetValueVector3(string Name)
    {
        return QEncypt.GetDencyptVector3(';', GetValueString(Name));
    }

    public static Vector3Int SetValueVector3Int(string Name)
    {
        return QEncypt.GetDencyptVector3Int(';', GetValueString(Name));
    }

    #endregion

    #region ------------------------------------ Get Enum

    //Single

    public static T GetValueEnum<T>(string Name)
    {
        return QEnum.GetChoice<T>(PlayerPrefs.GetInt(Name));
    }

    //Params

    public static List<T> GetValueEnum<T>(string Name, char Key) where T : Enum
    {
        return QEncypt.GetDencyptEnum<T>(Key, GetValueString(Name));
    }

    #endregion

    #region ------------------------------------ Get Time

    public static DateTime GetValueDateTime(string Name, string FormatTime)
    {
        return QDateTime.GetConvert(GetValueString(Name), FormatTime);
    }

    #endregion

    #endregion
}