using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class QDateTime
{
    public const string DD_MM_YYYY = "dd/MM/yyyy";
    public const string YYYY_MM_DD = "yyyy/MM/dd";

    public const string DDD_DD_MM_YYYY = "ddd dd/MM/yyyy";

    public const string HH_MM_SS = "HH:mm:ss";
    public const string HH_MM_SS_TT = "hh:mm:ss tt";

    public const string DDD_DD_MM_YYYY_HH_MM_SS_TT = "ddd dd/MM/yyyy hh:mm:ss tt";

    public static DateTime Now => DateTime.Now;

    #region ==================================== Time Format

    public static string GetFormat(DateTime Time, string FormatTime, string Special = "en-US")
    {
        if (Special != "")
        {
            return Time.ToString(FormatTime, CultureInfo.CreateSpecificCulture(Special));
        }
        else
        {
            return Time.ToString(FormatTime, DateTimeFormatInfo.InvariantInfo);
        }
    }

    public static DateTime GetConvert(string Time, string FormatTime, string Special = "en-US")
    {
        if (Special != "")
        {
            return DateTime.ParseExact(Time, FormatTime, CultureInfo.CreateSpecificCulture(Special));
        }
        else
        {
            return DateTime.ParseExact(Time, FormatTime, CultureInfo.InvariantCulture);
        }
    }

    #endregion

    #region ==================================== Time Compare

    public static (bool Prev, bool Equa, bool Next) GetCompare(DateTime TimeFrom, DateTime TimeTo)
    {
        if (TimeFrom < TimeTo)
        {
            return (true, false, false); //Past Time!!
        }

        if (TimeFrom > TimeTo)
        {
            return (false, false, true); //Future Time!!
        }

        return (false, true, false); //Now Time (Maybe not)!!
    }

    public static (bool Prev, bool Equa, bool Next) GetCompareDay(DateTime TimeFrom, DateTime TimeTo)
    {
        if (TimeFrom.Year > TimeTo.Year)
        {
            return (false, false, true); //Future Time!!
        }

        if (TimeFrom.Year < TimeTo.Year)
        {
            return (true, false, false); //Past Time!!
        }

        if (TimeFrom.Month > TimeTo.Month)
        {
            return (false, false, true); //Future Time!!
        }

        if (TimeFrom.Month < TimeTo.Month)
        {
            return (true, false, false); //Past Time!!
        }

        if (TimeFrom.Day > TimeTo.Day)
        {
            return (false, false, true); //Future Time!!
        }

        if (TimeFrom.Day < TimeTo.Day)
        {
            return (true, false, false); //Past Time!!
        }

        return (false, true, false); //Now Time (Maybe not)!!
    }

    public static int GetDay(DateTime From, DateTime To)
    {
        return (To - From).Days;
    }

    #endregion
}

public class QTimeSpan
{
    public const string HH_MM_SS = @"hh\:mm\:ss";
    public const string HH_MM = @"hh\:mm";
    public const string MM_SS = @"mm\:ss";
    public const string SS = @"ss";

    #region ==================================== Count Format

    public static string GetCountFormat(double Second, string FormatCount)
    {
        TimeSpan TimeConvert = TimeSpan.FromSeconds(Second);
        return TimeConvert.ToString(FormatCount);
    }

    public static long GetCountConvertSecond(int Hour = 0, int Minute = 0, int Second = 0)
    {
        return Second + (60 * Minute) + (60 * 60 * Hour);
    }

    #endregion
}

public class QTimeCountdown
{
    #region ==================================== Time Value

    public bool Active;
    public string Name;

    public double TimeRemain; //Second!!

    public string TimeShow; //Primary "hh:mm:ss"!!

    public QTimeCountdown(string Name, bool Active, double TimeRemain, string TimeShow)
    {
        this.Name = Name;
        this.Active = Active;
        this.TimeRemain = (int)TimeRemain;
        this.TimeShow = TimeShow;
    }

    #endregion

    #region ==================================== Time Function

    private const string PREF_START = "QTime-Start-";
    private const string PREF_COUNT = "QTime-Count-";

    public static IEnumerator ISetTime(long SecondMax, string Name, Action<QTimeCountdown> Active, string FormatCount = QTimeSpan.HH_MM_SS)
    {
        string REF_START_TIME = PREF_START + Name;
        string REF_COUNT_DOWN = PREF_COUNT + Name;

        DateTime StartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime TimeNow = DateTime.UtcNow;
        double Time = (TimeNow - StartTime).TotalSeconds;
        double TimeStart = double.Parse(QPlayerPrefs.GetValueString(REF_START_TIME, Time.ToString()));
        long TimeCount = (long)double.Parse(QPlayerPrefs.GetValueString(REF_COUNT_DOWN, SecondMax.ToString()));

        QPlayerPrefs.SetValue(REF_START_TIME, TimeStart.ToString());
        QPlayerPrefs.SetValue(REF_COUNT_DOWN, TimeCount.ToString());

        TimeCount -= (long)(TimeNow - StartTime.AddSeconds(TimeStart)).TotalSeconds;
        Debug.LogFormat("[Debug] QTime: {0} remain {1} second(s)!!", Name, TimeCount);

        while (TimeCount > 0)
        {
            string TimeShow = QTimeSpan.GetCountFormat(TimeCount, FormatCount);
            Active?.Invoke(new QTimeCountdown(Name, true, TimeCount, TimeShow));

            yield return new WaitForSeconds(1f);
            TimeCount--;
        }

        QPlayerPrefs.SetValueClear(REF_START_TIME);
        QPlayerPrefs.SetValueClear(REF_COUNT_DOWN);

        Active?.Invoke(new QTimeCountdown(Name, false, 0, ""));
    }

    public static bool GetTimeExist(string Name)
    {
        return QPlayerPrefs.GetValueExist(PREF_START + Name);
    }

    public static void SetTimeClear(string Name)
    {
        QPlayerPrefs.SetValueClear(PREF_START + Name);
        QPlayerPrefs.SetValueClear(PREF_COUNT + Name);
    }

    #endregion
}