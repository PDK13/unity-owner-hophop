using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTurn : MonoBehaviour
{
    [SerializeField] private TurnType m_turn = TurnType.None;

    public TurnType Turn => m_turn;
}