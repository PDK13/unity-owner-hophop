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
            //Move to NONE BLOCK!!
            m_animator.SetBool(m_valueJump, true);
            m_animator.SetBool(m_valueSwim, false);
            return;
        }
        //
        //Move to BLOCK!!
        //
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Water))
        {
            //Move from BLOCK WATER!!
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Water))
                //Move from BLOCK WATER to BLOCK WATER!!
                m_animator.SetBool(m_valueJump, false);
            else
                //Move from BLOCK WATER to BLOCK NOT WATER!!
                m_animator.SetBool(m_valueJump, true);
        }
        else
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Slow))
            //Move from BLOCK SLOW!!
            m_animator.SetBool(m_valueJump, true);
        else
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Slip))
            //Move from BLOCK SLIP!!
            m_animator.SetBool(m_valueJump, true);
        else
        {
            //Move from BLOCK NORMAL!!
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Water))
                //Move from BLOCK NORMAL to BLOCK WATER!!
                m_animator.SetBool(m_valueJump, true);
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Slow))
                //Move from BLOCK NORMAL to BLOCK SLOW!!
                m_animator.SetBool(m_valueJump, true);
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Slip))
                //Move from BLOCK NORMAL to BLOCK SLIP!!
                m_animator.SetBool(m_valueJump, true);
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
                //Move from BLOCK NORMAL to OBJECT BULLET!!
                m_animator.SetBool(m_valueJump, true);
            else
                //Move from BLOCK NORMAL to BLOCK NORMAL!!
                m_animator.SetBool(m_valueJump, false);
        }
        //
        m_animator.SetBool(m_valueSwim, To.Tag.Contains(GameManager.GameConfig.Tag.Water));
    }

    public void SetStand(IsometricBlock On)
    {
        if (On == null)
            return;
        //
        m_animator.SetBool(m_valueMove, false); //Surely NOT MOVE!!
        m_animator.SetBool(m_valueJump, false);
        m_animator.SetBool(m_valueSwim, On.Tag.Contains(GameManager.GameConfig.Tag.Water));
    }
}