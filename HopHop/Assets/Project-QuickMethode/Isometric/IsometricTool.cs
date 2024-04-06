#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IsometricTool : EditorWindow
{
    //Enum

    private enum MainType { World, Custom, }

    //Color

    private readonly Color COLOR_NORMAL = Color.white;
    private readonly Color COLOR_MASK = Color.gray;
    private readonly Color COLOR_UNMASK = Color.white;
    private readonly Color COLOR_CENTRE = Color.cyan;
    private readonly Color COLOR_FOCUS = Color.red;

    //Varible: Manager

    private IsometricManager m_manager;

    public IsometricManager Manager => m_manager;

    //

    private bool m_showWorld = false;
    private bool m_showMain = false;

    //

    private string m_mapName = "";

    private List<string> m_listMapScene = new List<string>();
    private int m_indexMapScene;

    private List<string> m_listMapFile = new List<string>();
    private int m_indexMapFile;

    private string m_pathOpen = "";
    private string m_pathSave = "";

    //Varible: Curson

    private IsometricBlock m_curson;

    public IsometricBlock Curson => m_curson;

    //

    private IsometricBlock m_blockFocus;
    private Event m_event;

    public IsometricBlock BlockCurson => m_manager.World.Current.GetBlockPrimary(m_curson.Pos);

    public IsometricBlock BlockFocus => m_blockFocus;

    public Event Event => m_event;

    //

    private bool m_check = false; //When turn ON, always focus on Curson!
    private bool m_camera = false; //When turn ON, main Camera always follow Curson!

    private bool m_maskXY = false;
    private bool m_hiddenH = false;

    //Varible: Main

    private MainType m_main;

    //Varible: World Manager

    private int m_indexTag = 0;
    private int m_indexName = 0;
    private int m_countNameHorizontal = 4;
    private int m_countNameHorizontalCurrent = 4;

    private List<string> m_listTag = new List<string>();
    private int m_indexTagLast = 0;

    //Varible: Scroll

    private Vector2 m_scrollBlock;

    [MenuItem("Tools/IsometricTool")]
    public static void SetInit()
    {
        GetWindow<IsometricTool>("IsometricTool");
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
            return;
        //
        SetInitFind();
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            QUnityEditor.SetLabel("[Not avaible when playing]", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
            return;
        }
        //
        if (!SetInitFind())
        {
            QUnityEditor.SetLabel("[Not found isometric manager]", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
            return;
        }
        //
        //START TOOL EDITOR:
        //
        QUnityEditor.SetLabel("ISOMETRIC TOOL", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
        //
        if (QUnityEditor.SetButton("Refresh"))
            SetInitRefresh();
        //
        SetGUIGroupWorld();
        //
        if (m_manager.World.Current == null)
        {
            QUnityEditor.SetLabel("[Not found current map]", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
            return;
        }
        //
        SetCursonControl();
        //
        SetGUIGroupCurson();
        //
        SetGUIGroupEdit();
    }

    //Control: Init

    private bool SetInitFind()
    {
        if (m_manager == null)
            m_manager = GameObject.FindFirstObjectByType<IsometricManager>();
        //
        return m_manager != null;
    }

    private void SetInitRefresh()
    {
        m_manager.SetEditorConfigFind();
        m_manager.SetEditorDataRefresh();
        //
        m_manager.World.SetRefresh();
        m_manager.Config.SetRefresh();
        //
        m_manager.List.SetList(m_manager.Config, false);
        //
        m_listMapScene = m_manager.World.ListMapName;
        m_listMapFile = m_manager.Config.Map.ListName;
        m_listTag = m_manager.List.ListTag;
        //
        m_maskXY = false;
        m_hiddenH = false;
        m_indexTag = 0;
        m_indexName = 0;
        //
        m_indexMapScene = 0;
        m_indexMapFile = 0;
        //
        SetCursonMaskXY();
        SetCursonHiddenH();
        //
        QUnityAssets.SetRefresh();
        //
        QUnityEditor.SetDirty(m_manager.gameObject);
    }

    //Control: Curson

    private void SetCursonControl()
    {
        if (m_curson == null)
        {
            Transform Curson = m_manager.World.Current.Root.transform.Find(IsometricManagerMap.NAME_CURSON);
            //
            if (Curson == null)
            {
                GameObject CursonClone = QGameObject.SetCreate(IsometricManagerMap.NAME_CURSON, m_manager.World.Current.Root.transform);
                m_curson = CursonClone.AddComponent<IsometricBlock>();
            }
            else
            if (Curson.GetComponent<IsometricBlock>() == null)
                m_curson = Curson.gameObject.AddComponent<IsometricBlock>();
            else
                m_curson = Curson.GetComponent<IsometricBlock>();
            //
            if (m_curson.GetComponent<SpriteRenderer>() == null)
                m_curson.gameObject.AddComponent<SpriteRenderer>();
            //
            m_curson.WorldManager = m_manager;
        }
        //
        if (m_curson != null)
        {
            if (!m_curson.transform.parent.Equals(m_manager.World.Current.Root))
                m_curson.transform.SetParent(m_manager.World.Current.Root, true);
            //
            if (m_manager.List.BlockList.Count > 0)
            {
                m_curson.GetComponent<SpriteRenderer>().sprite = m_manager.List.BlockList[m_indexTag].Block[m_indexName].GetComponent<SpriteRenderer>().sprite;
                m_curson.GetComponent<SpriteRenderer>().color = m_manager.List.BlockList[m_indexTag].Block[m_indexName].GetComponent<SpriteRenderer>().color;
            }
        }
        //
        m_event = Event.current;
        //
        switch (m_event.type)
        {
            case EventType.KeyDown:
                {
                    switch (Event.current.keyCode)
                    {
                        //Move Curson!!
                        case KeyCode.UpArrow:
                            m_curson.Pos += IsometricVector.GetRotateDir(IsometricVector.GetDir(IsoDir.Up), m_manager.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.DownArrow:
                            m_curson.Pos += IsometricVector.GetRotateDir(IsometricVector.GetDir(IsoDir.Down), m_manager.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.LeftArrow:
                            m_curson.Pos += IsometricVector.GetRotateDir(IsometricVector.GetDir(IsoDir.Left), m_manager.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.RightArrow:
                            m_curson.Pos += IsometricVector.GetRotateDir(IsometricVector.GetDir(IsoDir.Right), m_manager.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.PageUp:
                            m_curson.Pos += IsometricVector.GetRotateDir(IsometricVector.GetDir(IsoDir.Top), m_manager.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.PageDown:
                            m_curson.Pos += IsometricVector.GetRotateDir(IsometricVector.GetDir(IsoDir.Bot), m_manager.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        //Block Curson!!
                        case KeyCode.Home:
                            m_manager.World.Current.SetBlockCreate(m_curson.Pos, m_manager.List.BlockList[m_indexTag].Block[m_indexName].gameObject, true);
                            m_event.Use();
                            QUnityEditor.SetDirty(m_manager.gameObject);
                            break;
                        case KeyCode.End:
                            m_manager.World.Current.SetBlockRemovePrimary(m_curson.Pos);
                            m_event.Use();
                            QUnityEditor.SetDirty(m_manager.gameObject);
                            break;
                        default:
                            //Debug.Log("[IsometricTool] Key Pressed: " + QText.GetKeyboardFormat(Event.current.keyCode));
                            break;
                    }
                }
                break;
        }
        //Event Keyboard when Tool on focus!!
        //
        SetCursonControlCustom();
    }

    protected virtual void SetCursonControlCustom() { } //Custom!

    private void SetCursonCheck()
    {
        if (m_check)
        {
            IsometricBlock BlockFocus = BlockCurson;
            if (BlockFocus != null)
                QGameObject.SetFocus(BlockFocus.gameObject);
        }
    }

    private void SetCameraFollow()
    {
        if (m_camera)
            Camera.main.transform.position = m_curson.transform.position + Vector3.back * 100f;
    }

    private void SetCursonMaskXY()
    {
        if (m_manager.World.Current == null || m_curson == null)
            return;
        //
        if (m_maskXY)
        {
            bool CentreFound = m_manager.World.Current.SetEditorMask(
                m_curson.Pos,
                (m_blockFocus != null ? m_blockFocus.Pos : null),
                COLOR_MASK,
                COLOR_UNMASK,
                COLOR_CENTRE,
                COLOR_FOCUS);
            m_curson.GetComponent<SpriteRenderer>().enabled = !CentreFound;
        }
        else
        {
            bool CentreFound = m_manager.World.Current.SetEditorMask(
                m_curson.Pos,
                (m_blockFocus != null ? m_blockFocus.Pos : null),
                COLOR_NORMAL,
                COLOR_NORMAL,
                COLOR_NORMAL,
                COLOR_FOCUS);
            m_curson.GetComponent<SpriteRenderer>().enabled = !CentreFound;
        }
    }

    private void SetCursonHiddenH()
    {
        if (m_manager.World.Current == null || m_curson == null)
            return;
        //
        if (m_hiddenH)
            m_manager.World.Current.SetEditorHidden(m_curson.Pos.HInt, 0.01f);
        else
            m_manager.World.Current.SetEditorHidden(m_curson.Pos.HInt, 1f);
    }

    //GUI: World

    private void SetGUIGroupWorld()
    {
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("WORLD: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        if (QUnityEditor.SetButton(m_showWorld ? "Show" : "Hide", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f)))
            m_showWorld = !m_showWorld;
        QUnityEditor.SetHorizontalEnd();
        //
        if (m_showWorld)
        {
            SetGUIWorldName();
            SetGUIWorldScene();
            SetGUIWorldConfig();
            SetGUIWorldFile();
            //
            if (m_curson != null)
            {
                SetGUIWorldRenderer();
                SetGUIWorldRotate();
            }
            //
            QUnityEditor.SetLabel("~~~", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter));
        }
    }

    //

    private void SetGUIWorldName()
    {
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("NAME: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        m_mapName = QUnityEditor.SetField(m_mapName, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.5f, 2.5f));
        if (m_manager.World.Current != null)
        {
            if (m_manager.World.Current.Emty)
            {
                if (QUnityEditor.SetButton("Destroy", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    m_manager.World.SetRemove(m_manager.World.Current);
                    m_manager.World.SetRefresh();
                    m_listMapScene = m_manager.World.ListMapName;
                    m_indexMapScene = 0;
                    QUnityEditor.SetDirty(m_manager.gameObject);
                }
            }
            else
            {
                if (QUnityEditor.SetButton("Clear", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    m_manager.World.Current.SetWorldRemove();
                    QUnityEditor.SetDirty(m_manager.gameObject);
                }
            }
        }
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        if (QUnityEditor.SetButton("New", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            SetInitRefresh();
            //
            m_manager.World.SetGenerate(m_mapName);
            m_manager.World.SetActive(m_mapName);
            //
            m_manager.World.SetRefresh();
            m_listMapScene = m_manager.World.ListMapName;
            //
            QUnityEditor.SetDirty(m_manager.gameObject);
        }
        if (m_manager.World.Current != null)
        {
            if (QUnityEditor.SetButton("Rename", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                m_manager.World.Current.Name = m_mapName;
                //
                m_manager.World.SetRefresh();
                m_listMapScene = m_manager.World.ListMapName;
                //
                QUnityEditor.SetDirty(m_manager.gameObject);
            }
        }
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIWorldScene()
    {
        if (m_listMapScene.Count <= 0)
            return;
        //
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("SCENE: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        m_indexMapScene = QUnityEditor.SetPopup(m_indexMapScene, m_listMapScene, QUnityEditorWindow.GetGUILayoutWidth(this, 0.5f, 2.5f));
        if (QUnityEditor.SetButton("Active", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            m_manager.World.SetRefresh();
            m_manager.World.SetActive(m_listMapScene[m_indexMapScene]);
            //
            m_mapName = m_manager.World.Current.Name;
            //
            QUnityEditor.SetDirty(m_manager.gameObject);
        }
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIWorldConfig()
    {
        if (m_manager.Config.Map.ListAssets.Count <= 0)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("CONFIG: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        m_indexMapFile = QUnityEditor.SetPopup(m_indexMapFile, m_listMapFile, QUnityEditorWindow.GetGUILayoutWidth(this, 0.5f, 2.5f));
        if (QUnityEditor.SetButton("Open", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            IsometricDataFile.SetFileRead(m_manager, m_manager.Config.Map.ListAssets[m_indexMapFile]);
            m_listMapScene = m_manager.World.ListMapName;
            m_listMapFile = m_manager.Config.Map.ListName;
            //
            m_mapName = m_manager.World.Current.Name;
            //
            QUnityEditor.SetDirty(m_manager.gameObject);
        }
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIWorldFile()
    {
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("FILE: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        if (QUnityEditor.SetButton("Open", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            QUnityAssets.SetRefresh();
            //
            (bool Result, string Path, string Name) Path = QPath.GetPathFileOpenPanel("Open", "txt", m_pathOpen == "" ? QPath.GetPath(QPath.PathType.Assets) : m_pathOpen);
            if (Path.Result)
            {
                m_pathOpen = Path.Path;
                IsometricDataFile.SetFileRead(m_manager, QPath.GetPath(QPath.PathType.None, Path.Path));
                m_listMapScene = m_manager.World.ListMapName;
                m_listMapFile = m_manager.Config.Map.ListName;
                //
                m_mapName = m_manager.World.Current.Name;
                //
                QUnityEditor.SetDirty(m_manager.gameObject);
            }
        }
        //
        if (m_manager.World.Current != null)
        {
            if (QUnityEditor.SetButton("Save", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                (bool Result, string Path, string Name) Path = QPath.GetPathFileSavePanel("Save", "txt", m_pathSave == "" ? QPath.GetPath(QPath.PathType.Assets) : m_pathSave);
                if (Path.Result)
                {
                    m_pathSave = Path.Path;
                    IsometricDataFile.SetFileWrite(m_manager, QPath.GetPath(QPath.PathType.None, Path.Path));
                    QUnityAssets.SetRefresh();
                }
            }
        }
        //
        if (m_manager.World.Current != null)
        {
            if (QUnityEditor.SetButton("Q.Save", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                IsometricDataFile.SetFileWrite(m_manager, QPath.GetPath(QPath.PathType.None, m_pathSave));
                QUnityAssets.SetRefresh();
            }
        }
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIWorldRenderer()
    {
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("RENDERER: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        m_manager.Scene.Renderer = (IsometricRendererType)QUnityEditor.SetPopup<IsometricRendererType>((int)m_manager.Scene.Renderer, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f, 2.5f));
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIWorldRotate()
    {
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("ROTATE: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetChanceCheckBegin();
        m_manager.Scene.Rotate = (IsometricRotateType)QUnityEditor.SetPopup<IsometricRotateType>((int)m_manager.Scene.Rotate, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f, 2.5f));
        if (QUnityEditor.SetChanceCheckEnd())
        {
            m_manager.Scene.Centre = m_curson.Pos;
            m_manager.Scene.Centre.H = 0;
        }
        QUnityEditor.SetHorizontalEnd();
    }

    //GUI: Curson

    private void SetGUIGroupCurson()
    {
        if (m_curson == null)
            return;
        //
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("MAIN: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        if (QUnityEditor.SetButton(m_showMain ? "Show" : "Hide", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f)))
            m_showMain = !m_showMain;
        QUnityEditor.SetHorizontalEnd();
        //
        if (m_showMain)
        {
            SetGUIMainPosCurrent();
            SetGUIMainPosFocus();
            SetGUIMainCursonOption();
            //
            QUnityEditor.SetLabel("~~~", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter));
        }
    }

    //

    private void SetGUIMainPosCurrent()
    {
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("CURSON: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetLabel(m_curson.Pos.XInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetLabel(m_curson.Pos.YInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetLabel(m_curson.Pos.HInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIMainPosFocus()
    {
        if (m_blockFocus != null)
        {
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("FOCUS: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel(m_blockFocus.Pos.XInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel(m_blockFocus.Pos.YInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel(m_blockFocus.Pos.HInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetHorizontalEnd();
        }
    }

    private void SetGUIMainCursonOption()
    {
        if (m_manager.List.BlockList.Count > 0)
        {
            QUnityEditor.SetHorizontalBegin();
            SetGUIMainButtonFocus(0.25f);
            SetGUIMainButtonBack(0.25f);
            SetGUIMainButtonCheck(0.25f);
            SetGUIMainButtonCamera(0.25f);
            QUnityEditor.SetHorizontalEnd();
            QUnityEditor.SetHorizontalBegin();
            SetGUIMainButtonMaskXY(0.5f);
            SetGUIMainButtonMaskH(0.5f);
            QUnityEditor.SetHorizontalEnd();
        }
    }

    //

    private void SetGUIMainButtonFocus(float WidthPercent)
    {
        if (QUnityEditor.SetButton("FOCUS", QUnityEditor.GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            IsometricBlock BlockFocus = BlockCurson;
            if (BlockFocus != null)
            {
                QGameObject.SetFocus(BlockFocus.gameObject);
                m_blockFocus = BlockFocus;
                SetCursonMaskXY();
                SetCursonHiddenH();
            }
        }
    }

    private void SetGUIMainButtonBack(float WidthPercent)
    {
        if (QUnityEditor.SetButton("BACK", QUnityEditor.GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            if (m_blockFocus != null)
            {
                m_curson.Pos = m_blockFocus.Pos;
                m_curson.GetComponent<SpriteRenderer>().enabled = false;
                SetCursonMaskXY();
                SetCursonHiddenH();
            }
        }
    }

    private void SetGUIMainButtonCheck(float WidthPercent)
    {
        if (QUnityEditor.SetButton("CHECK", QUnityEditor.GetGUIButton(m_check ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_check = !m_check;
            SetCursonCheck();
        }
    }

    private void SetGUIMainButtonCamera(float WidthPercent)
    {
        if (QUnityEditor.SetButton("CAMERA", QUnityEditor.GetGUIButton(m_camera ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
            m_camera = !m_camera;
    }

    private void SetGUIMainButtonMaskXY(float WidthPercent)
    {
        if (QUnityEditor.SetButton("XY", QUnityEditor.GetGUIButton(m_maskXY ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_maskXY = !m_maskXY;
            SetCursonMaskXY();
            SetCursonHiddenH();
        }
    }

    private void SetGUIMainButtonMaskH(float WidthPercent)
    {
        if (QUnityEditor.SetButton("H", QUnityEditor.GetGUIButton(m_hiddenH ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_hiddenH = !m_hiddenH;
            SetCursonMaskXY();
            SetCursonHiddenH();
        }
    }

    //GUI: Edit

    private void SetGUIGroupEdit()
    {
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("EDIT: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        if (QUnityEditor.SetButton(m_main == MainType.World ? "World" : "Custom", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f)))
            m_main = m_main == MainType.World ? MainType.Custom : MainType.World;
        QUnityEditor.SetHorizontalEnd();
        //
        switch (m_main)
        {
            case MainType.World:
                SetGUIEditGroupWorld();
                break;
            case MainType.Custom:
                SetEditGUIGroupCustom();
                break;
        }
    }

    private void SetGUIEditGroupWorld()
    {
        if (m_listTag.Count == 0)
        {
            QUnityEditor.SetLabel("[Not found tag(s) list]", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
            return;
        }
        //
        if (m_manager.List.BlockList.Count == 0)
        {
            QUnityEditor.SetLabel("[Not found block(s) list]", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
            return;
        }
        //
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("TAG: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        m_indexTag = QUnityEditor.SetPopup(m_indexTag, m_listTag, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f, 2.5f));
        if (m_indexTag != m_indexTagLast)
        {
            m_indexTagLast = m_indexTag;
            m_indexName = 0;
        }
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("SIZE: ", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        m_countNameHorizontal = QUnityEditor.SetField(m_countNameHorizontal, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.5f));
        if (QUnityEditor.SetButton("Apply", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            m_countNameHorizontalCurrent = Mathf.Clamp(m_countNameHorizontal, 0, m_countNameHorizontal);
        QUnityEditor.SetHorizontalEnd();
        //
        m_scrollBlock = QUnityEditor.SetScrollViewBegin(m_scrollBlock);
        int BlockIndex = 0;
        while (BlockIndex <= m_manager.List.BlockList[m_indexTag].Block.Count - 1)
        {
            QUnityEditor.SetHorizontalBegin();
            for (int i = 0; i < m_countNameHorizontalCurrent; i++)
            {
                if (BlockIndex > m_manager.List.BlockList[m_indexTag].Block.Count - 1)
                    continue;
                //
                QUnityEditor.SetBackground(m_indexName == BlockIndex ? Color.white : Color.clear);
                if (GetEditGUIGroupBlockButton(BlockIndex))
                    m_indexName = BlockIndex;
                //
                BlockIndex++;
            }
            QUnityEditor.SetHorizontalEnd();
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private bool GetEditGUIGroupBlockButton(int Index)
    {
        return QUnityEditor.SetButton(
            m_manager.List.BlockList[m_indexTag].Block[Index].GetComponent<SpriteRenderer>().sprite,
            QUnityEditorWindow.GetGUILayoutWidth(this, 1f / m_countNameHorizontalCurrent),
            QUnityEditorWindow.GetGUILayoutHeightBaseWidth(this, 1f / m_countNameHorizontalCurrent));
    }

    //

    protected virtual void SetEditGUIGroupCustom()
    {
        QUnityEditor.SetLabel("[Custom script not found]", QUnityEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
    } //Custom!
}

#endif