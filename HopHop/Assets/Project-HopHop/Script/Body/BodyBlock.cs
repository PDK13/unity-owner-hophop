using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBlock : MonoBehaviour, IBodyTurn
{
    protected bool m_turnActive = false;

    protected virtual void Start()
    {
        TurnManager.SetInit(TurnType.Block, gameObject);
        TurnManager.Instance.onTurn += IOnTurn;
        TurnManager.Instance.onStepStart += IOnStep;
    }

    protected virtual void OnDestroy()
    {
        TurnManager.SetRemove(TurnType.Block, gameObject);
        TurnManager.Instance.onTurn -= IOnTurn;
        TurnManager.Instance.onStepStart -= IOnStep;
    }

    //

    public bool ITurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public virtual void IOnTurn(int Turn)
    {

    }

    public virtual void IOnStep(string Name)
    {

    }
}