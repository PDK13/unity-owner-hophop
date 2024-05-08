using System.Collections;
using System.Linq;
using UnityEngine;

public class BodyCharacter : MonoBehaviour, IBodyPhysic
{
    #region Const

    private const int INDEX_MOVE = 0;
    private const int INDEX_ACTION = 1;

    private const string TRIGGER_LAND = "Land";
    private const string TRIGGER_JUMP = "Jump";
    private const string TRIGGER_MOVE = "Move";
    private const string TRIGGER_SWIM = "Swim";

    private const string TRIGGER_IDLE = "Idle";

    private const string TRIGGER_SIT = "Sit";
    private const string TRIGGER_HURT = "Hurt";
    private const string TRIGGER_HAPPY = "Happy";

    #endregion

    #region Character - Animation - Animator

    private string m_animatorName = TRIGGER_IDLE;

    [Space]
    [SerializeField] private CharacterType m_character = CharacterType.Angel;
    [SerializeField] private int m_characterSkin = 0;

    private CharacterConfigData m_configCharacter;

    #endregion

    #region Get

    public CharacterType Character => m_character;

    public int CharacterSkin => m_characterSkin;

    public int MoveStep => CharacterManager.Instance.CharacterConfig.GetConfig(Character).MoveStep;

    public bool MoveLock => CharacterManager.Instance.CharacterConfig.GetConfig(Character).MoveLock;

    public bool MoveFloat => CharacterManager.Instance.CharacterConfig.GetConfig(Character).MoveFloat;

    #endregion

    #region Component

    private IsometricBlock m_block;
    private BodyPhysic m_body;
    private Animator m_animator;

    #endregion

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BodyPhysic>();
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetCharacter(m_character, m_characterSkin);
        //
        m_body.onMove += IMove;
        m_body.onMoveForce += IMove;
        m_body.onGravity += IGravity;
        m_body.onPush += IPush;
        m_body.onForce += IForce;
    }

    private void OnDestroy()
    {
        m_body.onMove -= IMove;
        m_body.onMoveForce -= IMove;
        m_body.onGravity -= IGravity;
        m_body.onPush -= IPush;
        m_body.onForce -= IForce;
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (m_character != CharacterType.Angel)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetAnimationAction(CharacterActionType.Idle);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetAnimationAction(CharacterActionType.Sit);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetAnimationAction(CharacterActionType.Hurt);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetAnimationAction(CharacterActionType.Happy);
    }

#endif

    #region Character - Animator

    public void SetCharacter(CharacterType Character, int Skin = 0)
    {
        m_configCharacter = CharacterManager.Instance.CharacterConfig.GetConfig(Character);
        //
        m_character = Character;
        //
        SetCharacterSkin(Skin);
    }

    public void SetCharacter(string CharacterName, int Skin = 0)
    {
        switch (CharacterName)
        {
            case KeyCharacter.Angel:
                SetCharacter(CharacterType.Angel, Skin);
                break;
            case KeyCharacter.Devil:
                SetCharacter(CharacterType.Devil, Skin);
                break;
            case KeyCharacter.Bunny:
                SetCharacter(CharacterType.Bunny, Skin);
                break;
            case KeyCharacter.Cat:
                SetCharacter(CharacterType.Cat, Skin);
                break;
            case KeyCharacter.Frog:
                SetCharacter(CharacterType.Frog, Skin);
                break;
            case KeyCharacter.Mow:
                SetCharacter(CharacterType.Mow, Skin);
                break;
            //
            case KeyCharacter.Alphaca:
                SetCharacter(CharacterType.Alphaca, Skin);
                break;
            case KeyCharacter.Bug:
                SetCharacter(CharacterType.Bug, Skin);
                break;
            case KeyCharacter.Fish:
                SetCharacter(CharacterType.Fish, Skin);
                break;
            case KeyCharacter.Mole:
                SetCharacter(CharacterType.Mole, Skin);
                break;
            case KeyCharacter.Pig:
                SetCharacter(CharacterType.Pig, Skin);
                break;
            case KeyCharacter.Wolf:
                SetCharacter(CharacterType.Wolf, Skin);
                break;
        }
    }

    public void SetCharacterSkin(int Skin = 0)
    {
        if (Skin > m_configCharacter.Skin.Count - 1)
        {
            m_animator.runtimeAnimatorController = m_configCharacter.Skin.Last().Animator;
            m_characterSkin = m_configCharacter.Skin.Count - 1;
        }
        else
        {
            m_animator.runtimeAnimatorController = m_configCharacter.Skin[Skin].Animator;
            m_characterSkin = Skin;
        }
    }

    #endregion

    #region Animation

    public void SetAnimationMove(IsometricBlock From, IsometricBlock To)
    {
        m_animator.SetLayerWeight(INDEX_ACTION, 0);
        //
        if (From == null || To == null)
            //Move from or to NONE BLOCK!!
            SetAnimation(TRIGGER_JUMP);
        //
        else
        if (From.GetTag(KeyTag.Water))
        {
            //Move from BLOCK WATER!!
            if (To.GetTag(KeyTag.Water))
                //Move from BLOCK WATER to BLOCK WATER!!
                SetAnimation(TRIGGER_SWIM);
            else
                //Move from BLOCK WATER to BLOCK NOT WATER!!
                SetAnimation(TRIGGER_JUMP);
        }
        else
        if (From.GetTag(KeyTag.Slow))
            //Move from BLOCK SLOW!!
            SetAnimation(TRIGGER_JUMP);
        else
        if (From.GetTag(KeyTag.Slip))
            //Move from BLOCK SLIP!!
            SetAnimation(TRIGGER_JUMP);
        else
        {
            //Move from BLOCK NORMAL!!
            //
            if (m_character == CharacterType.Cat)
                //Character Cat!!
                SetAnimation(TRIGGER_JUMP);
            else
            if (To.GetTag(KeyTag.Water))
                //Move from BLOCK NORMAL to BLOCK WATER!!
                SetAnimation(TRIGGER_JUMP);
            else
            if (To.GetTag(KeyTag.Slow))
                //Move from BLOCK NORMAL to BLOCK SLOW!!
                SetAnimation(TRIGGER_JUMP);
            else
            if (To.GetTag(KeyTag.Slip))
                //Move from BLOCK NORMAL to BLOCK SLIP!!
                SetAnimation(TRIGGER_JUMP);
            else
            if (To.GetTag(KeyTag.Bullet))
                //Move from BLOCK NORMAL to OBJECT BULLET!!
                SetAnimation(TRIGGER_JUMP);
            else
                //Move from BLOCK NORMAL to BLOCK NORMAL!!
                SetAnimation(TRIGGER_MOVE);
        }
    }

    public void SetAnimationStand(IsometricBlock On)
    {
        m_animator.SetLayerWeight(INDEX_ACTION, 0);
        //
        if (On == null)
            //Stand on NONE BLOCK!!
            SetAnimation(TRIGGER_JUMP);
        //
        else
        if (On.GetTag(KeyTag.Water))
            //Stand on WATER BLOCK!!
            SetAnimation(TRIGGER_SWIM);
        else
            //Stand on ANY BLOCK!!
            SetAnimation(TRIGGER_IDLE);
    }

    //

    public void SetAnimationAction(CharacterActionType Action)
    {
        m_animator.SetLayerWeight(INDEX_ACTION, 1);
        //
        switch (Action)
        {
            case CharacterActionType.Idle:
                SetAnimation(TRIGGER_IDLE);
                break;
            case CharacterActionType.Sit:
                SetAnimation(TRIGGER_SIT);
                break;
            case CharacterActionType.Hurt:
                SetAnimation(TRIGGER_HURT);
                break;
            case CharacterActionType.Happy:
                SetAnimation(TRIGGER_HAPPY);
                break;
        }
    }

    //

    private void SetAnimation(string Name)
    {
        if (Name == m_animatorName)
            return;
        //
        m_animatorName = Name;
        m_animator.SetTrigger(Name);
    }

    private void SetAnimation(string From, string To, float Duration = 0)
    {
        StartCoroutine(ISetAnimationDelay(From, To, Duration));
    }

    private IEnumerator ISetAnimationDelay(string From, string To, float Duration = 0)
    {
        SetAnimation(From);
        //
        if (Duration <= 0)
            yield return null;
        else
            yield return new WaitForSeconds(Duration);
        //
        SetAnimation(To);
    }

    #endregion

    #region IBodyPhysic

    public bool IControl() { return false; }

    public bool IControl(IsometricVector Dir) { return false; }

    public void IMove(bool State, IsometricVector Dir)
    {
        if (State && Dir != IsometricVector.None && Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
            SetAnimationMove(m_block.GetBlock(IsometricVector.Bot)[0], m_block.GetBlock(IsometricVector.Bot, Dir)[0]);
        else
            SetAnimationStand(m_block.GetBlock(IsometricVector.Bot)[0]);
    }

    public void IForce(bool State, IsometricVector Dir, IsometricVector From)
    {
        if (State && Dir != IsometricVector.None && Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
            SetAnimationMove(m_block.GetBlock(IsometricVector.Bot)[0], m_block.GetBlock(IsometricVector.Bot, Dir)[0]);
        else
            SetAnimationStand(m_block.GetBlock(IsometricVector.Bot)[0]);
    }

    public void IGravity(bool State)
    {
        if (!State)
            SetAnimationStand(m_block.GetBlock(IsometricVector.Bot)[0]);
    }

    public void IPush(bool State, IsometricVector Dir, IsometricVector From)
    {
        if (State && Dir != IsometricVector.None && Dir != IsometricVector.Top && Dir != IsometricVector.Bot && From != IsometricVector.Bot)
            SetAnimationMove(m_block.GetBlock(IsometricVector.Bot)[0], m_block.GetBlock(IsometricVector.Bot, Dir)[0]);
        else
            SetAnimationStand(m_block.GetBlock(IsometricVector.Bot)[0]);
    }

    #endregion
}

public enum CharacterActionType
{
    Idle,
    Sit,
    Hurt,
    Happy,
}