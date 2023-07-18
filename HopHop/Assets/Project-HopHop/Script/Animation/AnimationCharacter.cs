using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacter : MonoBehaviour
{
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_MOVE = "Move";
    private const string ANIM_JUMP = "Jump";
    private const string ANIM_SIT = "Sit";
    private const string ANIM_HURT = "Hurt";
    private const string ANIM_DOWN = "Down";
    private const string ANIM_SLEEP = "Sleep";
    private const string ANIM_HAPPY = "Happy";

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetReset()
    {

    }

    public void SetMove()
    {

    }
}
