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

    public void SetMove(IsometricBlock From, IsometricBlock To)
    {
        if (From == null)
            return;
        //
        if (To == null)
        {
            m_animator.SetBool("Move", true);
            m_animator.SetBool("Jump", true);
            //
            m_animator.SetBool("Sit", false);
        }
        else
        {
            if (From.Tag.Contains(GameTag.WATER))
            {
                if (To.Tag.Contains(GameTag.WATER))
                {
                    m_animator.SetBool("Move", true);
                    m_animator.SetBool("Jump", false);
                }
                else
                {
                    m_animator.SetBool("Move", true);
                    m_animator.SetBool("Jump", true);
                }
            }
            else
            {
                if (To.Tag.Contains(GameTag.WATER))
                {
                    m_animator.SetBool("Move", true);
                    m_animator.SetBool("Jump", true);
                }
                else
                {
                    m_animator.SetBool("Move", true);
                    m_animator.SetBool("Jump", false);
                }
            }
            //
            m_animator.SetBool("Sit", To.Tag.Contains(GameTag.WATER));
        }
    }

    public void SetStand(IsometricBlock On)
    {
        if (On == null)
            return;
        //
        if (On.Tag.Contains(GameTag.WATER))
        {
            m_animator.SetBool("Move", false);
            m_animator.SetBool("Jump", false);
            m_animator.SetBool("Sit", true);
        }
        else
        {
            m_animator.SetBool("Move", false);
            m_animator.SetBool("Jump", false);
            m_animator.SetBool("Sit", false);
        }
    }
}
