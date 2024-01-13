using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BodyEnermy : MonoBehaviour, IBodyTurn
{
    protected bool m_turnActive = false;
    //
    protected bool m_checkPlayerHit = true;
    protected bool m_checkStopBot = false;
    protected bool m_checkStopAhead = false;
    //
    protected BaseBody m_body;
    protected BaseCharacter m_character;
    protected IsometricBlock m_block;

    protected void Awake()
    {
        m_character = GetComponent<BaseCharacter>();
        m_body = GetComponent<BaseBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    protected virtual void Start()
    {
        TurnManager.SetInit(TurnType.Enermy, gameObject);
        TurnManager.Instance.onTurn += IOnTurn;
        TurnManager.Instance.onStepStart += IOnStep;
    }

    protected virtual void OnDestroy()
    {
        TurnManager.SetRemove(TurnType.Enermy, gameObject);
        TurnManager.Instance.onTurn -= IOnTurn;
        TurnManager.Instance.onStepStart -= IOnStep;
    }

    //

    public bool ITurnActive
    {
        get => m_turnActive;
        set => m_turnActive = value;
    }

    public virtual void IOnStep(string Name)
    {

    }

    public virtual void IOnTurn(int Turn)
    {

    }
}