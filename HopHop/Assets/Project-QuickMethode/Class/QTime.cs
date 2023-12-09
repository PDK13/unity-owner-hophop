using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class QTime
{
    public const string DD_MM_YYYY = @"dd/MM/yyyy";
    public const string YYYY_MM_DD = @"yyyy/MM/dd";

    public const string DDD_DD_MM_YYYY = @"ddd dd/MM/yyyy";

    public const string DDD_DD_MM_YYYY_HH_MM_SS_TT = @"ddd dd/MM/yyyy hh\:mm\:ss tt";

    public const string HH_MM_SS = @"hh\:mm\:ss";
    public const string HH_MM = @"hh\:mm";
    public const string MM_SS = @"mm\:ss";
    public const string SS = @"ss";
}

public class QDateTime
{
    public static DateTime Now => DateTime.Now;

    #region ==================================== Time Format

    public static string GetFormat(DateTime DateValue, string FormatDate, string SpecificCulture = "en-US")
    {
        if (SpecificCulture != "")
            return DateValue.ToString(FormatDate, CultureInfo.CreateSpecificCulture(SpecificCulture));
        else
            return DateValue.ToString(FormatDate, DateTimeFormatInfo.InvariantInfo);
    }

    public static DateTime GetConvert(string DateValue, string FormatDate, string SpecificCulture = "en-US")
    {
        if (SpecificCulture != "")
            return DateTime.ParseExact(DateValue, FormatDate, CultureInfo.CreateSpecificCulture(SpecificCulture));
        else
            return DateTime.ParseExact(DateValue, FormatDate, CultureInfo.InvariantCulture);
    }

    #endregion

    #region ==================================== Time Compare

    public static (bool Past, bool Present, bool Future) GetCompare(DateTime DateFrom, DateTime DateTo)
    {
        return (DateFrom < DateTo, DateFrom == DateTo, DateFrom > DateTo);
    }

    public static (bool Past, bool Present, bool Future) GetCompareDay(DateTime DateFrom, DateTime DateTo)
    {
        //Year
        if (DateFrom.Year > DateTo.Year)
            return (false, false, true);
        if (DateFrom.Year < DateTo.Year)
            return (true, false, false);
        //Month
        if (DateFrom.Month > DateTo.Month)
            return (false, false, true);
        if (DateFrom.Month < DateTo.Month) 
            return (true, false, false);
        //Day
        if (DateFrom.Day > DateTo.Day)
            return (false, false, true);
        if (DateFrom.Day < DateTo.Day)
            return (true, false, false);
        //
        return (false, true, false); //Today!
    }

    public static TimeSpan GetTotal(DateTime DateFrom, DateTime DateTo)
    {
        return DateTo.Subtract(DateFrom);
    }

    #endregion

    public static long GetConvertSecond(int Hour = 0, int Minute = 0, int Second = 0)
    {
        return Second + (60 * Minute) + (60 * 60 * Hour);
    }
}