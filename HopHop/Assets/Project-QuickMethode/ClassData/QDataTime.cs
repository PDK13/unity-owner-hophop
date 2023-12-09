using System.Globalization;
using System;
using UnityEngine;

public class QDataTime
{
    [SerializeField] private string m_name;

    private const string DATE_FORMAT = @"dd/MM/yyyy HH:mm:ss";

    private string TimeNowString => DateTime.Now.ToString(DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US")); //Date now to string format!!

    public QDataTime(string Name)
    {
        this.m_name = Name;
    }

    #region ==================================== Primary

    public bool TimeExist => PlayerPrefs.HasKey(m_name);

    public DateTime? TimeStart => TimeExist ? DateTime.ParseExact(PlayerPrefs.GetString(m_name, TimeNowString), DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US")) : null;

    //

    /// <summary>
    /// Value in <b>Second</b>
    /// </summary>
    public double TimeElapsed => TimeExist ? DateTime.Now.Subtract(TimeStart.Value).TotalSeconds : 0;

    public string GetTimeElapsedFormat(string Format)
    {
        return TimeSpan.FromSeconds(TimeElapsed).ToString(Format);
    }

    //

    public void SetTimeStart()
    {
        PlayerPrefs.SetString(m_name, TimeNowString);
        PlayerPrefs.Save();
    }

    public void SetTimeEnd()
    {
        PlayerPrefs.DeleteKey(m_name);
        PlayerPrefs.Save();
    }

    #endregion

    #region ==================================== Countdown

    /// <param name="DurationMax">Value in <b>Second</b></param>
    /// <returns>Value in <b>Second</b></returns>
    public double GetCountdownLeft(double DurationMax)
    {
        return DurationMax - TimeElapsed;
    }

    /// <param name="DurationMax">Value in <b>Second</b></param>
    /// <returns>Value in <b>Second</b></returns>
    public bool GetCountdownOut(double DurationMax)
    {
        return DurationMax - TimeElapsed <= 0;
    }

    /// <param name="DurationMax">Value in <b>Second</b></param>
    /// <returns>Value in <b>Second</b></returns>
    public string GetCountdownLeftFormat(double DurationMax, string Format)
    {
        return TimeSpan.FromSeconds(DurationMax - TimeElapsed).ToString(Format);
    }

    #endregion
}