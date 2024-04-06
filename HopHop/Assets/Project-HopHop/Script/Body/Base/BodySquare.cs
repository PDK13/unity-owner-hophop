using UnityEngine;

public class BodySquare : MonoBehaviour
{
    [SerializeField] private string m_none = "None";
    [SerializeField] private string m_grid = "Grid";
    [SerializeField] private string m_red = "Red";
    [SerializeField] private string m_blue = "Blue";

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    //

    public void SetNone()
    {
        m_animator.SetTrigger(m_none);
    }

    public void SetGrid()
    {
        m_animator.SetTrigger(m_grid);
    }

    public void SetRed()
    {
        m_animator.SetTrigger(m_red);
    }

    public void SetBlue()
    {
        m_animator.SetTrigger(m_blue);
    }
}