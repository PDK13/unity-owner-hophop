using UnityEngine;

public class BodyEvent : MonoBehaviour, IBodyInteractive
{
    [SerializeField] private bool m_triggerTop = false;
    [SerializeField] private bool m_triggerSide = false;

    private string m_eventIdentityBase;

    private IsometricBlock m_block;

    //

    public bool TriggerTop => m_triggerTop;

    public bool TriggerSide => m_triggerSide;

    //

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_eventIdentityBase = KeyInit.GetData(GetComponent<IsometricDataInit>(), KeyInit.Key.EvenIdentitytBase, false);
    }

    private void OnDestroy()
    {

    }

    //

    public bool IInteractive()
    {
        return GameManager.Instance.SetEventActive(m_eventIdentityBase);
    }

    //
}