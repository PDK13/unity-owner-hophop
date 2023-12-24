using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseShoot : MonoBehaviour
{
    [SerializeField] private BaseBullet m_bullet;

    private bool m_turnControl = false;

    private IsometricDataAction m_dataAction;

    private string m_turnCommand;
    private int m_turnTime = 0;
    private int m_turnTimeCurrent = 0;

    private bool TurnEnd => m_turnTimeCurrent == m_turnTime && m_turnTime != 0;

    private BaseBody m_body;
    private IsometricBlock m_block;

    //**Editor**
    [Space]
    [SerializeField] private IsoDir m_editorSpawm = IsoDir.None;
    [SerializeField] private IsoDir m_editorMove = IsoDir.None;
    [SerializeField] private int m_editorSpeed = 0;
    //**Editor**

    private void Awake()
    {
        if (!GameManager.GameStart)
            return;
        //
        m_body = GetComponent<BaseBody>();
        m_block = GetComponent<IsometricBlock>();
    }

    private void Start()
    {
        if (!GameManager.GameStart)
            return;
        //
        m_dataAction = m_block.Data.Action;

        if (m_dataAction != null)
        {
            if (m_dataAction.DataExist)
            {
                TurnManager.SetInit(TurnType.Object, gameObject);
                TurnManager.Instance.onTurn += SetControlTurn;
                TurnManager.Instance.onStepStart += SetControlStep;
            }
        }
    }

    private void OnDestroy()
    {
        if (!GameManager.GameStart)
            return;
        //
        StopAllCoroutines();
        //
        if (m_dataAction != null)
        {
            if (m_dataAction.DataExist)
            {
                TurnManager.SetRemove(TurnType.Object, gameObject);
                TurnManager.Instance.onTurn -= SetControlTurn;
                TurnManager.Instance.onStepStart -= SetControlStep;
            }
        }
    }

    private void SetControlTurn(int Turn)
    {
        //Reset!!
        m_turnTime = 0;
        m_turnTimeCurrent = 0;
        //
        m_turnControl = true;
    }

    private void SetControlStep(string Name)
    {
        if (m_turnControl)
        {
            if (Name == TurnType.Object.ToString())
            {
                SetControlAction();
            }
        }
    }

    private void SetControlAction()
    {
        if (m_turnTime == 0)
        {
            m_turnCommand = m_dataAction.Action[m_dataAction.Index];
            m_turnTime = m_dataAction.Duration[m_dataAction.Index];
            m_turnTimeCurrent = 0;
        }
        //
        m_turnTimeCurrent++;
        //
        List<string> Command = QEncypt.GetDencyptString('-', m_turnCommand);
        //
        if (Command[0] == GameConfigAction.Wait)
        {
            //"wait"
        }
        else
        if (Command[0] == GameConfigAction.Shoot)
        {
            //"shoot-[1]-[2]-[3]"
            IsometricVector DirSpawm = IsometricVector.GetDirDeEncypt(Command[1]);
            IsometricVector DirMove = IsometricVector.GetDirDeEncypt(Command[2]);
            int Speed = int.Parse(Command[3]);
            SetShoot(DirSpawm, DirMove, Speed);
        }
        //
        StartCoroutine(ISetDelay());
        //
        if (TurnEnd)
        {
            m_dataAction.Index += m_dataAction.Quantity;
            if (m_dataAction.Type == DataBlockType.Forward && m_dataAction.Index > m_dataAction.DataCount - 1)
            {
                //End Here!!
            }
            else
            if (m_dataAction.Type == DataBlockType.Loop && m_dataAction.Index > m_dataAction.DataCount - 1)
            {
                m_dataAction.Index = 0;
            }
            else
            if (m_dataAction.Type == DataBlockType.Revert && (m_dataAction.Index < 0 || m_dataAction.Index > m_dataAction.DataCount - 1))
            {
                m_dataAction.Quantity *= -1;
                m_dataAction.Index += m_dataAction.Quantity;
            }
        }
    }

    private IEnumerator ISetDelay()
    {
        yield return new WaitForSeconds(GameManager.TimeMove * 1);
        //
        m_turnControl = false;
        TurnManager.SetEndTurn(TurnType.Object, gameObject); //Follow Object (!)
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
        IsometricBlock Bullet = m_block.WorldManager.World.Current.SetBlockCreate(m_block.Pos + DirSpawm, m_bullet.gameObject);
        Bullet.GetComponent<BaseBullet>().SetInit(DirMove, Speed);
    } //Shoot Bullet!!

    //**Editor**

    public void SetEditorShoot()
    {
        IsometricBlock Block = GetComponent<IsometricBlock>();
        string Data = string.Format("{0}-{1}-{2}-{3}", GameConfigAction.Shoot, IsometricVector.GetDirEncypt(m_editorSpawm), IsometricVector.GetDirEncypt(m_editorMove), m_editorSpeed);
        Block.Data.Action.SetDataAdd(Data);
    }

    public void SetEditorWait()
    {
        IsometricBlock Block = GetComponent<IsometricBlock>();
        string Data = string.Format("{0}", GameConfigAction.Wait);
        Block.Data.Action.SetDataAdd(new IsometricDataBlockActionSingle(Data, 1));
    }

    //**Editor**
}

#if UNITY_EDITOR

[CustomEditor(typeof(BaseShoot))]
[CanEditMultipleObjects]
public class BaseShootEditor : Editor
{
    private BaseShoot m_target;

    private SerializedProperty m_bullet;

    private SerializedProperty m_editorSpawm;
    private SerializedProperty m_editorMove;
    private SerializedProperty m_editorSpeed;

    private void OnEnable()
    {
        m_target = target as BaseShoot;

        m_bullet = QUnityEditorCustom.GetField(this, "m_bullet");

        m_editorSpawm = QUnityEditorCustom.GetField(this, "m_editorSpawm");
        m_editorMove = QUnityEditorCustom.GetField(this, "m_editorMove");
        m_editorSpeed = QUnityEditorCustom.GetField(this, "m_editorSpeed");
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
        //
        if (QUnityEditor.SetButton("Shoot"))
            m_target.SetEditorShoot();
        if (QUnityEditor.SetButton("Wait"))
            m_target.SetEditorWait();
        //
        QUnityEditorCustom.SetApply(this);
    }
}

#endif