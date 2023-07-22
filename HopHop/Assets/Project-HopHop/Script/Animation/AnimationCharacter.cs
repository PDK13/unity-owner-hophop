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
        m_animator.SetBool(m_valueMove, true); //Surely MOVE!!
        //
        if (To == null)
        {
            //Move to NONE!!
            m_animator.SetBool(m_valueJump, true);
            m_animator.SetBool(m_valueSwim, false);
            return;
        }
        //
        //Move to BLOCK!!
        //
        if (From.Tag.Contains(GameTag.WATER))
        {
            //Move from WATER!!
            if (To.Tag.Contains(GameTag.WATER))
                //Move from WATER to WATER!!
                m_animator.SetBool(m_valueJump, false);
            else
                //Move from WATER to NOT WATER!!
                m_animator.SetBool(m_valueJump, true);
        }
        else
        if (From.Tag.Contains(GameTag.SLOW))
            //Move from SLOW!!
            m_animator.SetBool(m_valueJump, true);
        else
        if (From.Tag.Contains(GameTag.SLIP))
            //Move from SLIP!!
            m_animator.SetBool(m_valueJump, true);
        else
        {
            //Move from NORMAL!!
            if (To.Tag.Contains(GameTag.WATER))
                //Move from NORMAL to WATER!!
                m_animator.SetBool(m_valueJump, true);
            else
            if (To.Tag.Contains(GameTag.SLOW))
                //Move from NORMAL to SLOW!!
                m_animator.SetBool(m_valueJump, true);
            else
            if (To.Tag.Contains(GameTag.SLIP))
                //Move from NORMAL to SLIP!!
                m_animator.SetBool(m_valueJump, true);
            else
                //Move from NORMAL to NORMAL!!
                m_animator.SetBool(m_valueJump, false);
        }
        //
        m_animator.SetBool(m_valueSwim, To.Tag.Contains(GameTag.WATER));
    }

    public void SetStand(IsometricBlock On)
    {
        if (On == null)
            return;
        //
        m_animator.SetBool(m_valueMove, false); //Surely NOT MOVE!!
        m_animator.SetBool(m_valueJump, false);
        m_animator.SetBool(m_valueSwim, On.Tag.Contains(GameTag.WATER));
    }
}