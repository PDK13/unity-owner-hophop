using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBodyTurn
{
    bool ITurnActive { get; set; }

    void IOnTurn(int Turn);

    void IOnStep(string Name);
}