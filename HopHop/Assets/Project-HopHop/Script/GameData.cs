using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    //Time Move

    public static float m_timeMove = 1f;
    public static float m_timeRatio = 1f;

    public static float TimeMove => m_timeMove * m_timeRatio;

    //Object Control

    public static int m_objectTurnCount = 0;
    public static int m_objectTurnEnd = 0;

    public static bool ObjectControlEnd => m_objectTurnCount == m_objectTurnEnd;
}