using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControllerPlayer : MonoBehaviour
{
    private bool m_playerControl = false;

    private ControllerBody m_body;
    private AnimationCharacter m_animation;
    private IsometricBlock m_block;

    private void Awake()
    {
        m_body = GetComponent<ControllerBody>();
        m_animation = GetComponent<AnimationCharacter>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_body.onGravity += SetGravity;
        m_body.onPush += SetPush;
        m_body.onForce += SetForce;
        //
        GameTurn.SetInit(TypeTurn.Phase, this.gameObject);
        GameTurn.SetInit(TypeTurn.Player, this.gameObject);
        GameTurn.onTurn += SetControlTurn;
    }

    private void OnDestroy()
    {
        m_body.onGravity -= SetGravity;
        m_body.onPush -= SetPush;
        m_body.onForce -= SetForce;
        //
        GameTurn.SetRemove(TypeTurn.Player, this.gameObject);
        GameTurn.onTurn -= SetControlTurn;
    }

    private void Update()
    {
        if (!m_playerControl)
            return;

        if (Input.GetKey(KeyCode.UpArrow))
            SetControlMove(IsoVector.Up);

        if (Input.GetKey(KeyCode.DownArrow))
            SetControlMove(IsoVector.Down);

        if (Input.GetKey(KeyCode.LeftArrow))
            SetControlMove(IsoVector.Left);

        if (Input.GetKey(KeyCode.RightArrow))
            SetControlMove(IsoVector.Right);

        if (Input.GetKeyDown(KeyCode.Space))
            SetControlMove(IsoVector.None);
    }

    #region Move

    private void SetControlTurn(string Turn)
    {
        if (Turn == TypeTurn.Phase.ToString())
        {
            //Reset!!
            //
            GameTurn.SetEndTurn(TypeTurn.Phase, this.gameObject);
        }
        else
        if (Turn == TypeTurn.Player.ToString())
        {
            if (m_body.MoveForceXY.HasValue)
            {
                SetControlMove(m_body.MoveForceXY.Value);
                m_body.MoveForceXY = null;
                return;
            }
            //
            m_playerControl = true;
        }
    }

    private void SetControlMove(IsoVector Dir)
    {
        m_body.MoveLastXY = Dir;
        //
        if (Dir == IsoVector.None)
        {
            m_playerControl = false;
            GameTurn.SetEndTurn(TypeTurn.Player, this.gameObject); //Follow Player (!)
            return;
        }
        //
        int Length = 1; //Follow Character (!)
        //
        //Check if there is a Block ahead?!
        IsometricBlock Block = m_block.WorldManager.GetWorldBlockCurrent(m_block.Pos + Dir * Length);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameManager.GameConfig.Tag.Bullet))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
                //
                Block.GetComponent<ControllerBullet>().SetHit();
            }
            else
            {
                ControllerBody BlockBody = Block.GetComponent<ControllerBody>();
                //
                if (BlockBody == null)
                    //Surely can't continue move to this Pos, because this Block can't be push!!
                    return;
                //
                if (!BlockBody.GetCheckDir(Dir, Dir))
                    //Surely can't continue move to this Pos, because this Block can't be push to the Pos ahead!!
                    return;
                //
                //Fine to continue push this Block ahead!!
            }
        }
        //Fine to continue move to pos ahead!!
        //
        m_playerControl = false;
        //
        m_body.SetCheckGravity(Dir);
        m_animation.SetMove(m_body.GetCheckDir(IsoVector.Bot), m_body.GetCheckDir(IsoVector.Bot, Dir));
        //
        Vector3 MoveDir = IsoVector.GetVector(Dir);
        Vector3 MoveStart = IsoVector.GetVector(m_block.Pos);
        Vector3 MoveEnd = IsoVector.GetVector(m_block.Pos) + MoveDir * 1;
        DOTween.To(() => MoveStart, x => MoveEnd = x, MoveEnd, GameManager.TimeMove * 1)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                //...
            })
            .OnUpdate(() =>
            {
                m_block.Pos = new IsoVector(MoveEnd);
            })
            .OnComplete(() =>
            {
                m_body.SetStandOn();
                m_animation.SetStand(m_body.GetCheckDir(IsoVector.Bot));
                //
                GameTurn.SetEndTurn(TypeTurn.Player, this.gameObject); //Follow Player (!)
            });
        //
    }

    #endregion

    #region Body

    private void SetGravity(bool State)
    {
        if (!State)
        {
            m_body.SetStandOn();
            m_animation.SetStand(m_body.GetCheckDir(IsoVector.Bot));
        }
    }

    private void SetPush(bool State, IsoVector Dir)
    {
        if (State)
        {
            m_animation.SetMove(m_body.GetCheckDir(IsoVector.Bot), m_body.GetCheckDir(IsoVector.Bot, Dir));
        }
        else
        {
            m_body.SetStandOn();
            m_animation.SetStand(m_body.GetCheckDir(IsoVector.Bot));
        }
    }

    public void SetForce(bool State)
    {
        if (!State)
        {
            m_body.SetStandOn();
            m_animation.SetStand(m_body.GetCheckDir(IsoVector.Bot));
        }
    }

    #endregion
}