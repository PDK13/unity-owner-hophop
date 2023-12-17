#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IsometricTool : EditorWindow
{
    #region Enum

    private enum MainType { World, Block, }

    #endregion

    #region Varible: Manager

    private IsometricManager m_manager;
    private bool m_managerUpdate = false;

    private string m_pathOpen = "";
    private string m_pathSave = "";

    private List<string> m_listMapScene = new List<string>();
    private int m_indexMapScene;

    private List<string> m_listMapFile = new List<string>();
    private int m_indexMapFile;

    #endregion

    #region Varible: Curson

    private IsometricBlock m_curson;
    private IsometricBlock m_focus;
    private Event m_event;

    private bool m_check = false; //When turn ON, always focus on Curson!
    private bool m_camera = false; //When turn ON, main Camera always follow Curson!

    private bool m_maskXY = false;
    private bool m_hiddenH = false;

    #endregion

    #region Varible: Main

    private MainType m_main;

    #endregion

    #region Varible: World Manager

    private int m_indexTag = 0;
    private int m_indexName = 0;
    private int m_countNameHorizontal = 4;
    private int m_countNameHorizontalCurrent = 4;

    private List<string> m_listTag = new List<string>();
    private int m_indexTagLast = 0;

    #endregion

    #region Varible: Scroll

    private Vector2 m_scrollTag;
    private Vector2 m_scrollBlock;

    #endregion

    [MenuItem("Tools/IsometricTool")]
    public static void Init()
    {
        GetWindow<IsometricTool>("IsometricTool");
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
            return;
        //
        if (SetManagerFind())
            SetManagerRefresh();
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            QUnityEditor.SetLabel("(Not avaible when playing)", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
            return;
        }
        //
        QUnityEditor.SetLabel("ISOMETRIC MANAGER", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
        //
        if (m_manager == null)
        {
            if (QUnityEditor.SetButton("Refresh"))
                SetManagerFind();
            QUnityEditor.SetLabel("(Not found isometric manager)", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
            return;
        }
        //
        if (QUnityEditor.SetButton("Refresh"))
            SetManagerRefresh();
        //
        QUnityEditor.SetSpace(5f);
        QUnityEditor.SetLabel("TOOL MANAGER", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
        //
        SetGUIGroupManager();
        //
        QUnityEditor.SetSpace(5f);
        QUnityEditor.SetLabel("CURSON MANAGER", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
        //
        SetCursonControl();
        //
        SetGUIGroupCurson();
        //
        if (m_manager.List.BlockList.Count == 0)
            return;
        //
        QUnityEditor.SetSpace(5f);
        QUnityEditor.SetLabel("MAIN MANAGER", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this));
        //
        //if (QUnityEditor.SetButton((m_main == MainType.World ? "WORLD" : "BLOCK"), null, QUnityEditorWindow.GetGUILayoutWidth(this)))
        //    m_main = m_main == MainType.World ? MainType.Block : MainType.World;
        //
        switch (m_main)
        {
            case MainType.World:
                SetGUIGroupWorld();
                break;
            case MainType.Block:
                QUnityEditor.SetSpace(5f);
                //...
                break;
        }
    }

    #region Manager

    private bool SetManagerFind()
    {
        m_manager = GameObject.FindFirstObjectByType<IsometricManager>();
        //
        if (m_manager == null)
            return false;
        //
        return true;
    }

    private void SetManagerRefresh()
    {
        m_manager.World.SetRefresh();
        m_manager.Config.Reset();
        //
        m_listMapFile = m_manager.Config.Map.ListName;
        //
        m_manager.List.SetList(m_manager.Config, false);
        m_manager.World.SetInit();
        m_listMapScene = m_manager.World.RoomName;
        //
        m_listTag = new List<string>();
        for (int i = 0; i < m_manager.List.BlockList.Count; i++)
            m_listTag.Add(m_manager.List.BlockList[i].Tag);
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

    #endregion

    #region Curson

    //GUI Curson

    private void SetGUIGroupCurson()
    {
        if (m_curson == null)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("RENDERER: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        m_manager.Scene.Renderer = (IsometricRendererType)QUnityEditor.SetPopup<IsometricRendererType>((int)m_manager.Scene.Renderer, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f, 2.5f));
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("ROTATE: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetChanceCheckBegin();
        m_manager.Scene.Rotate = (IsometricRotateType)QUnityEditor.SetPopup<IsometricRotateType>((int)m_manager.Scene.Rotate, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f, 2.5f));
        if (QUnityEditor.SetChanceCheckEnd())
        {
            m_manager.Scene.Centre = m_curson.Pos;
            m_manager.Scene.Centre.H = 0;
        }
        QUnityEditor.SetHorizontalEnd();
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("CURSON: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetLabel(m_curson.Pos.XInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetLabel(m_curson.Pos.YInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetLabel(m_curson.Pos.HInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
        //
        if (m_focus != null)
        {
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("FOCUS: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel(m_focus.Pos.XInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel(m_focus.Pos.YInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel(m_focus.Pos.HInt.ToString(), QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetHorizontalEnd();
        }
        //
        if (m_manager.List.BlockList.Count > 0)
        {
            QUnityEditor.SetHorizontalBegin();
            SetGUIButtonFocus(0.25f);
            SetGUIButtonBack(0.25f);
            SetGUIButtonCheck(0.25f);
            SetGUIButtonCamera(0.25f);
            QUnityEditor.SetHorizontalEnd();
            QUnityEditor.SetHorizontalBegin();
            SetGUIButtonMaskXY(0.5f);
            SetGUIButtonMaskH(0.5f);
            QUnityEditor.SetHorizontalEnd();
        }
    }

    private void SetGUIButtonFocus(float WidthPercent)
    {
        if (QUnityEditor.SetButton("FOCUS", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            IsometricBlock BlockFocus = m_manager.World.Current.GetBlockPrimary(m_curson.Pos);
            if (BlockFocus != null)
            {
                QGameObject.SetFocus(BlockFocus.gameObject);
                m_focus = BlockFocus;
            }
        }
    }

    private void SetGUIButtonBack(float WidthPercent)
    {
        if (QUnityEditor.SetButton("BACK", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            if (m_focus != null)
                m_curson.Pos = m_focus.Pos;
        }
    }

    private void SetGUIButtonCheck(float WidthPercent)
    {
        if (QUnityEditor.SetButton("CHECK", QUnityEditor.GetGUIButton(m_check ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_check = !m_check;
            SetCursonCheck();
        }
    }

    private void SetGUIButtonCamera(float WidthPercent)
    {
        if (QUnityEditor.SetButton("CAMERA", QUnityEditor.GetGUIButton(m_camera ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
            m_camera = !m_camera;
    }

    private void SetGUIButtonMaskXY(float WidthPercent)
    {
        if (QUnityEditor.SetButton("XY", QUnityEditor.GetGUIButton(m_maskXY ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_maskXY = !m_maskXY;
            SetCursonMaskXY();
            SetCursonHiddenH();
        }
    }

    private void SetGUIButtonMaskH(float WidthPercent)
    {
        if (QUnityEditor.SetButton("H", QUnityEditor.GetGUIButton(m_hiddenH ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_hiddenH = !m_hiddenH;
            SetCursonMaskXY();
            SetCursonHiddenH();
        }
    }

    //GUI Curson Control

    private void SetCursonControl()
    {
        if (!m_manager.World.CurrentAvaible)
        {
            m_manager.World.SetInit();
            return;
        }
        //
        if (m_curson == null)
        {
            Transform Curson = m_manager.World.Current.Root.transform.Find(IsometricManagerRoom.NAME_CURSON);
            //
            if (Curson == null)
            {
                GameObject CursonClone = QGameObject.SetCreate(IsometricManagerRoom.NAME_CURSON, m_manager.World.Current.Root.transform);
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
                            m_manager.World.Current.SetBlockCreate(m_curson.Pos, m_manager.List.BlockList[m_indexTag].Block[m_indexName].gameObject);
                            m_event.Use();
                            QUnityEditor.SetDirty(m_manager.gameObject);
                            break;
                        case KeyCode.End:
                            m_manager.World.Current.SetBlockRemovePrimary(m_curson.Pos);
                            m_event.Use();
                            QUnityEditor.SetDirty(m_manager.gameObject);
                            break;
                    }
                }
                break;
        }
        //Event Keyboard when Tool on focus!!
    }

    private void SetCursonCheck()
    {
        if (m_check)
        {
            IsometricBlock BlockFocus = m_manager.World.Current.GetBlockPrimary(m_curson.Pos);
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
        if (!m_manager.World.CurrentAvaible)
            return;
        //
        if (m_maskXY)
        {
            bool CentreFound = m_manager.World.Current.SetEditorMask(m_curson.Pos, Color.red, Color.white, Color.yellow);
            m_curson.GetComponent<SpriteRenderer>().enabled = !CentreFound;
        }
        else
        {
            bool CentreFound = m_manager.World.Current.SetEditorMask(m_curson.Pos, Color.white, Color.white, Color.white);
            m_curson.GetComponent<SpriteRenderer>().enabled = !CentreFound;
        }
    }

    private void SetCursonHiddenH()
    {
        if (!m_manager.World.CurrentAvaible)
            return;
        //
        if (m_hiddenH)
            m_manager.World.Current.SetEditorHidden(m_curson.Pos.HInt, 0.01f);
        else
            m_manager.World.Current.SetEditorHidden(m_curson.Pos.HInt, 1f);
    }

    #endregion

    #region Manager

    private void SetGUIGroupManager()
    {
        QUnityEditor.SetHorizontalBegin();
        {
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("NAME: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            m_manager.World.Current.Name = QUnityEditor.SetField(m_manager.World.Current.Name, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.5f, 2.5f));
            if (m_manager.World.Current.Emty)
            {
                if (QUnityEditor.SetButton("Destroy", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    m_manager.World.SetRemove(m_manager.World.Current);
                    m_listMapScene = m_manager.World.RoomName;
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
        if (m_listMapScene.Count > 0)
        {
            QUnityEditor.SetHorizontalBegin();
            {
                QUnityEditor.SetBackground(Color.white);
                QUnityEditor.SetLabel("SCENE: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
                m_indexMapScene = QUnityEditor.SetPopup(m_indexMapScene, m_listMapScene, QUnityEditorWindow.GetGUILayoutWidth(this, 0.5f, 2.5f));
                if (QUnityEditor.SetButton("Active", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    m_manager.World.SetActive(m_listMapScene[m_indexMapScene]);
                    QUnityEditor.SetDirty(m_manager.gameObject);
                }
            }
            QUnityEditor.SetHorizontalEnd();
        }
        //
        if (m_manager.Config.Map.ListAssets.Count > 0)
        {
            QUnityEditor.SetHorizontalBegin();
            {
                QUnityEditor.SetBackground(Color.white);
                QUnityEditor.SetLabel("CONFIG: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
                m_indexMapFile = QUnityEditor.SetPopup(m_indexMapFile, m_listMapFile, QUnityEditorWindow.GetGUILayoutWidth(this, 0.5f, 2.5f));
                if (QUnityEditor.SetButton("Open", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    IsometricDataFile.SetFileRead(m_manager, m_manager.Config.Map.ListAssets[m_indexMapFile]);
                    m_listMapScene = m_manager.World.RoomName;
                    m_listMapFile = m_manager.Config.Map.ListName;
                }
            }
            QUnityEditor.SetHorizontalEnd();
        }
        //
        QUnityEditor.SetSpace(5f);
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("FILE: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
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
        if (QUnityEditor.SetButton("Open", QUnityEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            QUnityAssets.SetRefresh();
            //
            (bool Result, string Path, string Name) Path = QPath.GetPathFileOpenPanel("Open", "txt", m_pathOpen == "" ? QPath.GetPath(QPath.PathType.Assets) : m_pathOpen);
            if (Path.Result)
            {
                m_maskXY = false;
                m_hiddenH = false;
                m_indexTag = 0;
                m_indexName = 0;
                //
                m_manager.List.SetList(m_manager.Config, false);
                //
                m_pathOpen = Path.Path;
                IsometricDataFile.SetFileRead(m_manager, QPath.GetPath(QPath.PathType.None, Path.Path));
                m_listMapScene = m_manager.World.RoomName;
                m_listMapFile = m_manager.Config.Map.ListName;
                //
                QUnityEditor.SetDirty(m_manager.gameObject);
            }
        }
        QUnityEditor.SetHorizontalEnd();
    }

    #endregion

    #region World Manager

    private void SetGUIGroupWorld()
    {
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("TAG: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
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
        QUnityEditor.SetLabel("SIZE: ", QUnityEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
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
                if (GetGUIGroupBlockButton(BlockIndex))
                    m_indexName = BlockIndex;
                //
                BlockIndex++;
            }
            QUnityEditor.SetHorizontalEnd();
        }
        QUnityEditor.SetScrollViewEnd();
    }

    private bool GetGUIGroupBlockButton(int Index)
    {
        return QUnityEditor.SetButton(
            m_manager.List.BlockList[m_indexTag].Block[Index].GetComponent<SpriteRenderer>().sprite,
            QUnityEditorWindow.GetGUILayoutWidth(this, 1f / m_countNameHorizontalCurrent),
            QUnityEditorWindow.GetGUILayoutHeightBaseWidth(this, 1f / m_countNameHorizontalCurrent));
    }

    #endregion

    #region Block Manager

    //...

    #endregion
}

#endif