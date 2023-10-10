using System.Collections;
using System.Linq;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    private const int INDEX_MOVE = 0;
    private const int INDEX_ACTION = 1;
    //
    private const string TRIGGER_LAND = "Land";
    private const string TRIGGER_JUMP = "Jump";
    private const string TRIGGER_MOVE = "Move";
    private const string TRIGGER_SWIM = "Swim";

    private const string TRIGGER_IDLE = "Idle";

    private const string TRIGGER_SIT = "Sit";
    private const string TRIGGER_HURT = "Hurt";
    private const string TRIGGER_HAPPY = "Happy";
    //
    private string m_animatorName = TRIGGER_IDLE;
    //
    [Space]
    [SerializeField] private CharacterType m_character = CharacterType.Angel;
    [SerializeField] private int m_skin = 0;

    public CharacterType Character => m_character;

    public int Skin => m_skin;
    //
    private IsometricBlock m_block;
    private BaseBody m_body;
    private Animator m_animator;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_body = GetComponent<BaseBody>();
        m_animator = GetComponent<Animator>();
        //
        SetCharacter(m_character, m_skin);
    }

    private void Start()
    {
        m_body.onMove += SetMove;
        m_body.onMoveForce += SetMove;
        m_body.onGravity += SetGravity;
        m_body.onPush += SetPush;
        m_body.onForce += SetForce;
    }

    private void OnDestroy()
    {
        m_body.onMove -= SetMove;
        m_body.onMoveForce -= SetMove;
        m_body.onGravity -= SetGravity;
        m_body.onPush -= SetPush;
        m_body.onForce -= SetForce;
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (m_character != CharacterType.Angel)
            return;
        //
        if (Input.GetKeyDown(KeyCode.Alpha1))
            m_animator.runtimeAnimatorController = GameManager.CharacterConfig.Angel.Skin[0].Animator;
        //
        if (Input.GetKeyDown(KeyCode.Alpha2))
            m_animator.runtimeAnimatorController = GameManager.CharacterConfig.Angel.Skin[1].Animator;
        //
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetAnimationAction(CharacterActionType.Happy);
        //
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetAnimationAction(CharacterActionType.Hurt);
    }

#endif

    //Animator

    public void SetCharacter(CharacterType Character, int Skin = 0)
    {
        m_character = Character;
        //
        SetSkin(Skin);
    }

    public void SetSkin(int Skin = 0)
    {
        ConfigCharacter Config = GameManager.CharacterConfig.GetConfig(m_character);
        //
        if (Skin > Config.Skin.Count - 1)
        {
            m_animator.runtimeAnimatorController = Config.Skin.Last().Animator;
            m_skin = Config.Skin.Count - 1;
        }
        else
        {
            m_animator.runtimeAnimatorController = Config.Skin[Skin].Animator;
            m_skin = Skin;
        }
    }

    //Animation

    public void SetAnimationMove(IsometricBlock From, IsometricBlock To)
    {
        m_animator.SetLayerWeight(INDEX_ACTION, 0);
        //
        if (From == null || To == null)
            //Move from or to NONE BLOCK!!
            SetAnimation(TRIGGER_JUMP);
        //
        else
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Water))
        {
            //Move from BLOCK WATER!!
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Water))
                //Move from BLOCK WATER to BLOCK WATER!!
                SetAnimation(TRIGGER_SWIM);
            else
                //Move from BLOCK WATER to BLOCK NOT WATER!!
                SetAnimation(TRIGGER_JUMP);
        }
        else
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Slow))
            //Move from BLOCK SLOW!!
            SetAnimation(TRIGGER_JUMP);
        else
        if (From.Tag.Contains(GameManager.GameConfig.Tag.Slip))
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
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Water))
                //Move from BLOCK NORMAL to BLOCK WATER!!
                SetAnimation(TRIGGER_JUMP);
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Slow))
                //Move from BLOCK NORMAL to BLOCK SLOW!!
                SetAnimation(TRIGGER_JUMP);
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Slip))
                //Move from BLOCK NORMAL to BLOCK SLIP!!
                SetAnimation(TRIGGER_JUMP);
            else
            if (To.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
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
        if (On.Tag.Contains(GameManager.GameConfig.Tag.Water))
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

    //

    private void SetMove(bool State, IsometricVector Dir)
    {
        if (State && Dir != IsometricVector.None && Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
            SetAnimationMove(m_body.GetCheckDir(IsometricVector.Bot), m_body.GetCheckDir(IsometricVector.Bot, Dir));
        else
            SetAnimationStand(m_body.GetCheckDir(IsometricVector.Bot));
    }

    private void SetGravity(bool State)
    {
        if (!State)
            SetAnimationStand(m_body.GetCheckDir(IsometricVector.Bot));
    }

    private void SetPush(bool State, IsometricVector Dir)
    {
        if (State && Dir != IsometricVector.None && Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
            SetAnimationMove(m_body.GetCheckDir(IsometricVector.Bot), m_body.GetCheckDir(IsometricVector.Bot, Dir));
        else
            SetAnimationStand(m_body.GetCheckDir(IsometricVector.Bot));
    }

    public void SetForce(bool State, IsometricVector Dir)
    {
        if (State && Dir != IsometricVector.None && Dir != IsometricVector.Top && Dir != IsometricVector.Bot)
            SetAnimationMove(m_body.GetCheckDir(IsometricVector.Bot), m_body.GetCheckDir(IsometricVector.Bot, Dir));
        else
            SetAnimationStand(m_body.GetCheckDir(IsometricVector.Bot));
    }
}

public enum CharacterActionType
{
    Idle,
    Sit,
    Hurt,
    Happy,
}