using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    private const int INDEX_MOVE = 0;
    private const int INDEX_ACTION = 1;

    private const string BOOL_MOVE = "Move";
    private const string BOOL_JUMP = "Jump";
    private const string BOOL_SWIM = "Swim";

    private const string TRIGGER_IDLE = "Idle";
    private const string TRIGGER_SIT = "Sit";
    private const string TRIGGER_HURT = "Hurt";
    private const string TRIGGER_HAPPY = "Happy";

    [Space]
    [SerializeField] private CharacterType m_character = CharacterType.Angel;

    public CharacterType Character => m_character;

    [Space]
    [SerializeField] private Animator m_animator;

    public void SetCharacter(CharacterType Character)
    {
        m_character = Character;
        //

    }

    public void SetMove(IsometricBlock From, IsometricBlock To)
    {
        if (From == null)
        {
            return;
        }
        //
        m_animator.SetLayerWeight(INDEX_ACTION, 0);
        //
        m_animator.SetBool(BOOL_MOVE, true); //Surely MOVE!!
        //
        if (To == null)
        {
            //Move to NONE BLOCK!!
            m_animator.SetBool(BOOL_JUMP, true);
            m_animator.SetBool(BOOL_SWIM, false);
            return;
        }
        //
        //Move to BLOCK!!
        //
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Water))
        {
            //Move from BLOCK WATER!!
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Water))
            {
                //Move from BLOCK WATER to BLOCK WATER!!
                m_animator.SetBool(BOOL_JUMP, false);
            }
            else
            {
                //Move from BLOCK WATER to BLOCK NOT WATER!!
                m_animator.SetBool(BOOL_JUMP, true);
            }
        }
        else
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Slow))
        {
            //Move from BLOCK SLOW!!
            m_animator.SetBool(BOOL_JUMP, true);
        }
        else
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Slip))
        {
            //Move from BLOCK SLIP!!
            m_animator.SetBool(BOOL_JUMP, true);
        }
        else
        {
            //Move from BLOCK NORMAL!!
            //
            if (m_character == CharacterType.Cat)
            {
                m_animator.SetBool(BOOL_JUMP, true);
            }
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Water))
            {
                //Move from BLOCK NORMAL to BLOCK WATER!!
                m_animator.SetBool(BOOL_JUMP, true);
            }
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Slow))
            {
                //Move from BLOCK NORMAL to BLOCK SLOW!!
                m_animator.SetBool(BOOL_JUMP, true);
            }
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Slip))
            {
                //Move from BLOCK NORMAL to BLOCK SLIP!!
                m_animator.SetBool(BOOL_JUMP, true);
            }
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
            {
                //Move from BLOCK NORMAL to OBJECT BULLET!!
                m_animator.SetBool(BOOL_JUMP, true);
            }
            else
            {
                //Move from BLOCK NORMAL to BLOCK NORMAL!!
                m_animator.SetBool(BOOL_JUMP, false);
            }
        }
        //
        m_animator.SetBool(BOOL_SWIM, To.Tag.Contains(GameManager.GameConfig.Tag.Water));
    }

    public void SetStand(IsometricBlock On)
    {
        if (On == null)
        {
            return;
        }
        //
        m_animator.SetLayerWeight(INDEX_ACTION, 0);
        //
        m_animator.SetBool(BOOL_MOVE, false); //Surely NOT MOVE!!
        m_animator.SetBool(BOOL_JUMP, false);
        m_animator.SetBool(BOOL_SWIM, On.Tag.Contains(GameManager.GameConfig.Tag.Water));
    }

    public void SetAction(CharacterActionType Action)
    {
        m_animator.SetLayerWeight(INDEX_ACTION, 1);
        //
        switch (Action)
        {
            case CharacterActionType.Idle:
                m_animator.SetTrigger(TRIGGER_IDLE);
                break;
            case CharacterActionType.Sit:
                m_animator.SetTrigger(TRIGGER_SIT);
                break;
            case CharacterActionType.Hurt:
                m_animator.SetTrigger(TRIGGER_HURT);
                break;
            case CharacterActionType.Happy:
                m_animator.SetTrigger(TRIGGER_HAPPY);
                break;
        }
    }
}

public enum CharacterType
{
    Angel = 0,
    Devil = 1,
    Bunny = 2,
    Cat = 3,
    Frog = 4,
    Mow = 5,
    Alphaca = 6,
    Bug = 7,
    Fish = 8,
    Mole = 9,
    Pig = 10,
    Wolf = 11,
}

public enum CharacterActionType
{
    Idle,
    Sit,
    Hurt,
    Happy,
}