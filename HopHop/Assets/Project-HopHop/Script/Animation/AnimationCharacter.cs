using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacter : MonoBehaviour
{
    [SerializeField] private string m_valueMove = "Move";
    [SerializeField] private string m_valueJump = "Jump";
    [SerializeField] private string m_valueSwim = "Swim";

    [Space]
    [SerializeField] private Animator m_animator;

    public void SetMove(IsometricBlock From, IsometricBlock To)
    {
        if (From == null)
            return;
        //
        if (To == null)
        {
            m_animator.SetBool(m_valueMove, true);
            m_animator.SetBool(m_valueMove, true);
            //
            m_animator.SetBool(m_valueMove, false);
        }
        else
        {
            if (From.Tag.Contains(GameTag.WATER))
            {
                if (To.Tag.Contains(GameTag.WATER))
                {
                    m_animator.SetBool(m_valueMove, true);
                    m_animator.SetBool(m_valueMove, false);
                }
                else
                {
                    m_animator.SetBool(m_valueMove, true);
                    m_animator.SetBool(m_valueMove, true);
                }
            }
            else
            {
                if (To.Tag.Contains(GameTag.WATER))
                {
                    m_animator.SetBool(m_valueMove, true);
                    m_animator.SetBool(m_valueMove, true);
                }
                else
                {
                    m_animator.SetBool(m_valueMove, true);
                    m_animator.SetBool(m_valueMove, false);
                }
            }
            //
            m_animator.SetBool(m_valueMove, To.Tag.Contains(GameTag.WATER));
        }
    }

    public void SetStand(IsometricBlock On)
    {
        if (On == null)
            return;
        //
        if (On.Tag.Contains(GameTag.WATER))
        {
            m_animator.SetBool(m_valueMove, false);
            m_animator.SetBool(m_valueMove, false);
            m_animator.SetBool(m_valueMove, true);
        }
        else
        {
            m_animator.SetBool(m_valueMove, false);
            m_animator.SetBool(m_valueMove, false);
            m_animator.SetBool(m_valueMove, false);
        }
    }
}
