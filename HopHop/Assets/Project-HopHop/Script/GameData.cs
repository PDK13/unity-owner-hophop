using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    //Time Move

    public static float m_timeMove = 1f;
    public static float m_timeRatio = 1f;

    public static float TimeMove => m_timeMove * m_timeRatio;

    //Turn Move

    public static TypeTurn m_turnControl = TypeTurn.PlayerControl;

    public static int m_objectControlCount = 0;
    public static int m_objectControlDone = 0;

    public static bool ObjectControlDone => m_objectControlCount == m_objectControlDone;
}