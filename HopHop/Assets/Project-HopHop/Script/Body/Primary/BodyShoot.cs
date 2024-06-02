using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyShoot : MonoBehaviour, ITurnManager, IBodyShoot
{
    #region Shoot

    [SerializeField] private GameObject m_bullet;

    private IsometricDataAction m_dataAction;

    private List<string> m_turnCommand;

#if UNITY_EDITOR

    [Space]
    [SerializeField] private IsoDir m_eSpawm = IsoDir.None;
    [SerializeField] private IsoDir m_eMove = IsoDir.None;
    [SerializeField] private int m_eSpeed = 0;
    [SerializeField] private string m_eShoot;

#endif

    #endregion

    #region Get

    public bool State => m_switch != null ? m_switch.State : true;

    public StepType Step => StepType.Shoot;

    #endregion

    #region Component

    private IsometricBlock m_block;
    private BodyInteractiveSwitch m_switch;

    #endregion

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
        m_switch = GetComponent<BodyInteractiveSwitch>();
    }

    private void Start()
    {
        m_dataAction = GetComponent<IsometricDataAction>();

        if (m_dataAction != null)
        {
            if (m_dataAction.Data.Count > 0)
            {
                TurnManager.Instance.SetInit(Step, this);
                TurnManager.Instance.onTurn += ISetTurn;
                TurnManager.Instance.onStepStart += ISetStepStart;
                TurnManager.Instance.onStepEnd += ISetStepEnd;
            }
        }
    }

    private void OnDestroy()
    {
        if (m_dataAction != null)
        {
            if (m_dataAction.Data.Count > 0)
            {
                TurnManager.Instance.SetRemove(Step, this);
                TurnManager.Instance.onTurn -= ISetTurn;
                TurnManager.Instance.onStepStart -= ISetStepStart;
                TurnManager.Instance.onStepEnd -= ISetStepEnd;
            }
        }
    }

    #region ITurnManager

    public void ISetTurn(int Turn)
    {
        //Reset!!
    }

    public void ISetStepStart(string Step)
    {
        if (Step != this.Step.ToString())
            return;
        //
        if (!State)
        {
            TurnManager.Instance.SetEndStep(this.Step, this);
            return;
        }
        //
        SetControlAction();
    }

    public void ISetStepEnd(string Step) { }

    #endregion

    #region IBodyShoot

    public void IShoot(IsometricVector DirSpawm, IsometricVector DirMove, int Speed)
    {
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + DirSpawm);
        if (Block != null)
        {
            if (Block.GetTag(KeyTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //Surely can't spawm bullet here!!
            return;
        }

        IsometricBlock Bullet = m_block.WorldManager.World.Current.SetBlockCreate(m_block.Pos + DirSpawm, m_bullet, false);
        Bullet.GetComponent<IBodyBullet>().IInit(DirMove, Speed);
    }

    #endregion

    #region Shoot

    private void SetControlAction()
    {
        m_turnCommand = this.m_dataAction.ActionCurrent;
        foreach (string CommandCheck in m_turnCommand)
        {
            List<string> Command = QEncypt.GetDencyptString('-', CommandCheck);
            //
            switch (Command[0])
            {
                case KeyInit.Shoot:
                    //shoot-[1]-[2]-[3]
                    IsometricVector DirSpawm = IsometricVector.GetDirDeEncypt(Command[1]);
                    IsometricVector DirMove = IsometricVector.GetDirDeEncypt(Command[2]);
                    int Speed = int.Parse(Command[3]);
                    IShoot(DirSpawm, DirMove, Speed);
                    break;
            }
        }
        //
        StartCoroutine(ISetDelay());
        //
        m_dataAction.SetDirNext();
    }

    private IEnumerator ISetDelay()
    {
        yield return new WaitForSeconds(GameManager.Instance.TimeMove * 1);

        TurnManager.Instance.SetEndStep(Step, this);
    }

    #endregion

#if UNITY_EDITOR

    public void SetEditorShoot()
    {
        m_eShoot = string.Format("{0}-{1}-{2}-{3}", KeyInit.Shoot, IsometricVector.GetDirEncypt(m_eSpawm), IsometricVector.GetDirEncypt(m_eMove), m_eSpeed);
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(BodyShoot))]
[CanEditMultipleObjects]
public class BodyShootEditor : Editor
{
    private BodyShoot m_target;

    private SerializedProperty m_bullet;

    private SerializedProperty m_eSpawm;
    private SerializedProperty m_eMove;
    private SerializedProperty m_eSpeed;
    private SerializedProperty m_eShoot;

    private void OnEnable()
    {
        m_target = target as BodyShoot;

        m_bullet = QUnityEditorCustom.GetField(this, "m_bullet");

        m_eSpawm = QUnityEditorCustom.GetField(this, "m_eSpawm");
        m_eMove = QUnityEditorCustom.GetField(this, "m_eMove");
        m_eSpeed = QUnityEditorCustom.GetField(this, "m_eSpeed");
        m_eShoot = QUnityEditorCustom.GetField(this, "m_eShoot");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_bullet);
        //
        QUnityEditorCustom.SetField(m_eSpawm);
        QUnityEditorCustom.SetField(m_eMove);
        QUnityEditorCustom.SetField(m_eSpeed);
        QUnityEditorCustom.SetField(m_eShoot);
        //
        if (QUnityEditor.SetButton("Editor Generate"))
            m_target.SetEditorShoot();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif