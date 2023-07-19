using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacter : MonoBehaviour
{
    public const string ANIM_IDLE = "Idle";
    public const string ANIM_MOVE = "Move";
    public const string ANIM_JUMP = "Jump";
    public const string ANIM_SIT = "Sit";
    public const string ANIM_HURT = "Hurt";
    public const string ANIM_DOWN = "Down";
    public const string ANIM_SLEEP = "Sleep";
    public const string ANIM_HAPPY = "Happy";
    public const string ANIM_AIR = "Air";

    private string m_current;

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetMove(bool State)
    {
        m_animator.SetBool("Move", State);
    }

    public void SetJump(bool State)
    {
        m_animator.SetBool("Jump", State);
    }
}
