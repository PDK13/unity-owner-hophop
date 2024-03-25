using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BodyShoot : MonoBehaviour, ITurnManager
{
    [SerializeField] private BodyBullet m_bullet;

    private bool m_turnControl = false;

    private IsometricDataAction m_dataAction;

    private List<string> m_turnCommand;

    private IsometricBlock m_block;

#if UNITY_EDITOR

    [Space]
    [SerializeField] private IsoDir m_editorSpawm = IsoDir.None;
    [SerializeField] private IsoDir m_editorMove = IsoDir.None;
    [SerializeField] private int m_editorSpeed = 0;
    [SerializeField] private string m_editorShoot;

#endif

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        m_dataAction = GetComponent<IsometricDataAction>();

        if (m_dataAction != null)
        {
            if (m_dataAction.Data.Count > 0)
            {
                TurnManager.SetInit(TurnType.Shoot, this);
            }
        }
    }

    private void OnDestroy()
    {
        if (m_dataAction != null)
        {
            if (m_dataAction.Data.Count > 0)
            {
                TurnManager.SetRemove(TurnType.Shoot, this);
            }
        }
    }

    #region Turn

    public bool TurnActive
    {
        get => m_turnControl;
        set => m_turnControl = value;
    }

    public void ITurn(int Turn)
    {
        //Reset!!
        m_turnControl = true;
    }

    public void IStepStart(string Name)
    {
        if (m_turnControl)
        {
            if (Name == TurnType.Shoot.ToString())
            {
                SetControlAction();
            }
        }
    }

    public void IStepEnd(string Name) { }

    #endregion

    #region Move

    private void SetControlAction()
    {
        m_turnCommand = this.m_dataAction.ActionCurrent;
        foreach (string CommandCheck in m_turnCommand)
        {
            List<string> Command = QEncypt.GetDencyptString('-', CommandCheck);
            //
            switch (Command[0])
            {
                case GameConfigAction.Shoot:
                    //shoot-[1]-[2]-[3]
                    IsometricVector DirSpawm = IsometricVector.GetDirDeEncypt(Command[1]);
                    IsometricVector DirMove = IsometricVector.GetDirDeEncypt(Command[2]);
                    int Speed = int.Parse(Command[3]);
                    SetShoot(DirSpawm, DirMove, Speed);
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
        yield return new WaitForSeconds(GameManager.TimeMove * 1);
        //
        m_turnControl = false;
        TurnManager.SetEndTurn(TurnType.Shoot, this);
    }

    private void SetShoot(IsometricVector DirSpawm, IsometricVector DirMove, int Speed)
    {
        IsometricBlock Block = m_block.WorldManager.World.Current.GetBlockCurrent(m_block.Pos + DirSpawm);
        if (Block != null)
        {
            if (Block.Tag.Contains(GameConfigTag.Player))
            {
                Debug.Log("[Debug] Bullet hit Player!!");
            }
            //
            //Surely can't spawm bullet here!!
            return;
        }
        //
        IsometricBlock Bullet = m_block.WorldManager.World.Current.SetBlockCreate(m_block.Pos + DirSpawm, m_bullet.gameObject, false);
        Bullet.GetComponent<BodyBullet>().SetInit(DirMove, Speed);
    } //Shoot Bullet!!

    #endregion

#if UNITY_EDITOR

    public void SetEditorShoot()
    {
        m_editorShoot = string.Format("{0}-{1}-{2}-{3}", GameConfigAction.Shoot, IsometricVector.GetDirEncypt(m_editorSpawm), IsometricVector.GetDirEncypt(m_editorMove), m_editorSpeed);
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

    private SerializedProperty m_editorSpawm;
    private SerializedProperty m_editorMove;
    private SerializedProperty m_editorSpeed;
    private SerializedProperty m_editorShoot;

    private void OnEnable()
    {
        m_target = target as BodyShoot;

        m_bullet = QUnityEditorCustom.GetField(this, "m_bullet");

        m_editorSpawm = QUnityEditorCustom.GetField(this, "m_editorSpawm");
        m_editorMove = QUnityEditorCustom.GetField(this, "m_editorMove");
        m_editorSpeed = QUnityEditorCustom.GetField(this, "m_editorSpeed");
        m_editorShoot = QUnityEditorCustom.GetField(this, "m_editorShoot");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(m_bullet);
        //
        QUnityEditorCustom.SetField(m_editorSpawm);
        QUnityEditorCustom.SetField(m_editorMove);
        QUnityEditorCustom.SetField(m_editorSpeed);
        QUnityEditorCustom.SetField(m_editorShoot);
        //
        if (QUnityEditor.SetButton("Editor Generate"))
            m_target.SetEditorShoot();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif