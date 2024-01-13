using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBodyMove
{
    void IMoveForce(bool State, IsometricVector Dir);

    void IGravity(bool State);

    void IPush(bool State, IsometricVector Dir);

    void IForce(bool State, IsometricVector Dir);

    bool IMove(IsometricVector Dir);
}