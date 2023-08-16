#if UNITY_EDITOR

using QuickMethode;
using UnityEditor;
using UnityEngine;

public class IsometricTool : EditorWindow
{
    #region Enum

    private enum MainType { World, Block, }

    #endregion

    #region Varible: Manager

    private string m_pathOpen = "";
    private string m_pathSave = "";

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

    private IsometricManager m_manager;

    private int m_indexTag = 0;
    private int m_indexName = 0;
    private int m_countNameHorizontal = 4;

    #endregion

    #region Varible: Block Manager

    //...

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
        if (m_manager != null)
        {
            m_manager.SetInit();
            m_manager.List.SetList(m_manager.Config);
            m_manager.World.SetWorldRead(m_manager.transform);
        }
    }

    private void OnGUI()
    {
        if (!GetManager())
            return;

        QEditor.SetLabel("TOOL MANAGER", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this));

        SetGUIGroupManager();

        QEditor.SetSpace(5f);
        QEditor.SetLabel("CURSON MANAGER", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this));

        SetCursonControl();

        SetGUIGroupCurson();

        if (m_manager.List.BlockList.Count == 0)
            return;

        QEditor.SetSpace(5f);
        QEditor.SetLabel("MAIN MANAGER", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this));

        if (QEditor.SetButton((m_main == MainType.World ? "WORLD" : "BLOCK"), null, QEditorWindow.GetGUILayoutWidth(this)))
            m_main = m_main == MainType.World ? MainType.Block : MainType.World;

        switch (m_main)
        {
            case MainType.World:
                QEditor.SetSpace(5f);
                SetGUIGroupTag();
                QEditor.SetSpace(5f);
                SetGUIGroupBlock();
                break;
            case MainType.Block:
                QEditor.SetSpace(5f);
                //...
                break;
        }
    }

    #region Manager

    private bool GetManager()
    {
        if (m_manager == null)
        {
            m_manager = GameObject.FindFirstObjectByType<IsometricManager>();
            m_manager.SetInit();
            m_manager.List.SetList(m_manager.Config);
            m_manager.World.SetWorldRead(m_manager.transform);
        }

        return m_manager != null;
    }

    #endregion

    #region Curson

    //GUI Curson

    private void SetGUIGroupCurson()
    {
        if (m_curson == null) 
            return;

        QEditor.SetHorizontalBegin();
        {
            QEditor.SetBackground(Color.white);
            QEditor.SetLabel("RENDERER: ", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
            m_manager.Game.Scene.Renderer = (IsometricRendererType)QEditor.SetPopup<IsometricRendererType>((int)m_manager.Game.Scene.Renderer, QEditorWindow.GetGUILayoutWidth(this, 0.75f, 2.5f));
        }
        QEditor.SetHorizontalEnd();

        QEditor.SetHorizontalBegin();
        {
            QEditor.SetBackground(Color.white);
            QEditor.SetLabel("ROTATE: ", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QEditor.SetChanceCheckBegin();
            m_manager.Game.Scene.Rotate = (IsometricRotateType)QEditor.SetPopup<IsometricRotateType>((int)m_manager.Game.Scene.Rotate, QEditorWindow.GetGUILayoutWidth(this, 0.75f, 2.5f));
            if (QEditor.SetChanceCheckEnd())
            {
                m_manager.Game.Scene.Centre = m_curson.Pos;
                m_manager.Game.Scene.Centre.H = 0;
            }
        }
        QEditor.SetHorizontalEnd();

        QEditor.SetHorizontalBegin();
        {
            QEditor.SetBackground(Color.white);
            QEditor.SetLabel("CURSON: ", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QEditor.SetLabel(m_curson.Pos.XInt.ToString(), QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QEditor.SetLabel(m_curson.Pos.YInt.ToString(), QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QEditor.SetLabel(m_curson.Pos.HInt.ToString(), QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
        }
        QEditor.SetHorizontalEnd();

        if (m_focus != null)
        {
            QEditor.SetHorizontalBegin();
            {
                QEditor.SetBackground(Color.white);
                QEditor.SetLabel("FOCUS: ", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
                QEditor.SetLabel(m_focus.Pos.XInt.ToString(), QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
                QEditor.SetLabel(m_focus.Pos.YInt.ToString(), QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
                QEditor.SetLabel(m_focus.Pos.HInt.ToString(), QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 0.25f));
            }
            QEditor.SetHorizontalEnd();
        }

        if (m_manager.List.BlockList.Count > 0)
        {
            QEditor.SetHorizontalBegin();
            SetGUIButtonFocus(0.25f);
            SetGUIButtonBack(0.25f);
            SetGUIButtonCheck(0.25f);
            SetGUIButtonCamera(0.25f);
            QEditor.SetHorizontalEnd();
            QEditor.SetHorizontalBegin();
            SetGUIButtonMaskXY(0.5f);
            SetGUIButtonMaskH(0.5f);
            QEditor.SetHorizontalEnd();
        }
    } 

    private void SetGUIButtonFocus(float WidthPercent)
    {
        if (QEditor.SetButton("FOCUS", QEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            IsometricBlock BlockFocus = m_manager.World.GetBlockPrimary(m_curson.Pos);
            if (BlockFocus != null)
            {
                QGameObject.SetFocus(BlockFocus.gameObject);
                m_focus = BlockFocus;
            }
        }
    }

    private void SetGUIButtonBack(float WidthPercent)
    {
        if (QEditor.SetButton("BACK", QEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            if (m_focus != null)
                m_curson.Pos = m_focus.Pos;
        }
    }

    private void SetGUIButtonCheck(float WidthPercent)
    {
        if (QEditor.SetButton("CHECK", QEditor.GetGUIButton(m_check ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_check = !m_check;
            SetCursonCheck();
        }
    }

    private void SetGUIButtonCamera(float WidthPercent)
    {
        if (QEditor.SetButton("CAMERA", QEditor.GetGUIButton(m_camera ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_camera = !m_camera;
        }
    }

    private void SetGUIButtonMaskXY(float WidthPercent)
    {
        if (QEditor.SetButton("XY", QEditor.GetGUIButton(m_maskXY ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_maskXY = !m_maskXY;
            SetCursonMaskXY();
            SetCursonHiddenH();
        }
    }

    private void SetGUIButtonMaskH(float WidthPercent)
    {
        if (QEditor.SetButton("H", QEditor.GetGUIButton(m_hiddenH ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, WidthPercent)))
        {
            m_hiddenH = !m_hiddenH;
            SetCursonMaskXY();
            SetCursonHiddenH();
        }
    }

    //GUI Curson Control

    private void SetCursonControl()
    {
        if (m_curson == null)
        {
            Transform Curson = m_manager.transform.Find(IsometricDataWorld.CURSON_NAME);

            if (Curson == null)
            {
                GameObject CursonClone = QGameObject.SetCreate(IsometricDataWorld.CURSON_NAME, m_manager.transform);
                m_curson = CursonClone.AddComponent<IsometricBlock>();
            }
            else
            if (Curson.GetComponent<IsometricBlock>() == null)
                m_curson = Curson.gameObject.AddComponent<IsometricBlock>();
            else
                m_curson = Curson.GetComponent<IsometricBlock>();

            if (m_curson.GetComponent<SpriteRenderer>() == null)
                m_curson.gameObject.AddComponent<SpriteRenderer>();

            m_curson.WorldManager = m_manager;
        }

        if (m_curson != null)
        {
            if (m_manager.List.BlockList.Count > 0)
            {
                m_curson.GetComponent<SpriteRenderer>().sprite = m_manager.List.BlockList[m_indexTag].Block[m_indexName].GetComponent<SpriteRenderer>().sprite;
                m_curson.GetComponent<SpriteRenderer>().color = m_manager.List.BlockList[m_indexTag].Block[m_indexName].GetComponent<SpriteRenderer>().color;
            }
        }

        m_event = Event.current;

        switch (m_event.type)
        {
            case EventType.KeyDown:
                {
                    switch (Event.current.keyCode)
                    {
                        //Move Curson!!
                        case KeyCode.UpArrow:
                            m_curson.Pos += IsometricVector.GetDir(IsoDir.Up, m_manager.Game.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.DownArrow:
                            m_curson.Pos += IsometricVector.GetDir(IsoDir.Down, m_manager.Game.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.LeftArrow:
                            m_curson.Pos += IsometricVector.GetDir(IsoDir.Left, m_manager.Game.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.RightArrow:
                            m_curson.Pos += IsometricVector.GetDir(IsoDir.Right, m_manager.Game.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.PageUp:
                            m_curson.Pos += IsometricVector.GetDir(IsoDir.Top, m_manager.Game.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        case KeyCode.PageDown:
                            m_curson.Pos += IsometricVector.GetDir(IsoDir.Bot, m_manager.Game.Scene.Rotate);
                            SetCursonMaskXY();
                            SetCursonHiddenH();
                            SetCursonCheck();
                            SetCameraFollow();
                            m_event.Use();
                            break;
                        //Block Curson!!
                        case KeyCode.Home:
                            m_manager.World.SetBlockCreate(m_curson.Pos, m_manager.List.BlockList[m_indexTag].Block[m_indexName].gameObject);
                            m_event.Use();
                            break;
                        case KeyCode.End:
                            m_manager.World.SetBlockRemovePrimary(m_curson.Pos);
                            m_event.Use();
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
            IsometricBlock BlockFocus = m_manager.World.GetBlockPrimary(m_curson.Pos);
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
        if (m_maskXY)
        {
            bool CentreFound = m_manager.World.SetEditorMask(m_curson.Pos, Color.red, Color.white, Color.yellow);
            m_curson.GetComponent<SpriteRenderer>().enabled = !CentreFound;
        }
        else
        {
            bool CentreFound = m_manager.World.SetEditorMask(m_curson.Pos, Color.white, Color.white, Color.white);
            m_curson.GetComponent<SpriteRenderer>().enabled = !CentreFound;
        }
    }

    private void SetCursonHiddenH()
    {
        if (m_hiddenH)
            m_manager.World.SetEditorHidden(m_curson.Pos.HInt, 0.01f);
        else
            m_manager.World.SetEditorHidden(m_curson.Pos.HInt, 1f);
    }

    #endregion

    #region Manager

    private void SetGUIGroupManager()
    {
        QEditor.SetHorizontalBegin();
        if (QEditor.SetButton("Refresh", QEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f / 2)))
        {
            m_maskXY = false;
            m_hiddenH = false;
            m_indexTag = 0;
            m_indexName = 0;
            //
            m_manager.SetInit();
            m_manager.List.SetList(m_manager.Config);
            m_manager.World.SetWorldRead(m_manager.transform);
            //
            SetCursonMaskXY();
            SetCursonHiddenH();
            //
            QAssetsDatabase.SetRefresh();
        }
        if (QEditor.SetButton("Clear", QEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f / 2)))
        {
            m_manager.World.SetWorldRemove();
        }
        QEditor.SetHorizontalEnd();
        QEditor.SetHorizontalBegin();
        if (QEditor.SetButton("Save", QEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f / 2)))
        {
            var Path = QPath.GetPathFileSavePanel("Save", "txt", m_pathSave == "" ? QPath.GetPath(QPath.PathType.Assets) : m_pathOpen);
            if (Path.Result)
            {
                m_pathSave = Path.Path;
                IsometricDataFile.SetFileWrite(m_manager, QPath.GetPath(QPath.PathType.None, Path.Path));
                QAssetsDatabase.SetRefresh();
            }
        }
        if (QEditor.SetButton("Open", QEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f / 2)))
        {
            QAssetsDatabase.SetRefresh();
            //
            var Path = QPath.GetPathFileOpenPanel("Open", "txt", m_pathOpen == "" ? QPath.GetPath(QPath.PathType.Assets) : m_pathOpen);
            if (Path.Result)
            {
                m_maskXY = false;
                m_hiddenH = false;
                m_indexTag = 0;
                m_indexName = 0;
                //
                m_manager.List.SetList(m_manager.Config);
                //
                m_pathOpen = Path.Path;
                IsometricDataFile.SetFileRead(m_manager, QPath.GetPath(QPath.PathType.None, Path.Path));
            }
        }
        QEditor.SetHorizontalEnd();
    }

    #endregion

    #region World Manager

    //Group Tag

    private void SetGUIGroupTag()
    {
        QEditor.SetLabel("TAG", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this));
        m_scrollTag = QEditor.SetScrollViewBegin(m_scrollTag);
        for (int i = 0; i < m_manager.List.BlockList.Count; i++)
        {
            string Tag = m_manager.List.BlockList[i].Tag != "" ? m_manager.List.BlockList[i].Tag : "[...]";
            if (m_indexTag == i)
            {
                if (QEditor.SetButton(Tag, QEditor.GetGUIButton(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f, 0f)))
                {
                    m_indexTag = i;
                    m_indexName = 0;
                }
            }
            else
            {
                if (QEditor.SetButton(Tag, QEditor.GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this, 1f, 0f)))
                {
                    m_indexTag = i;
                    m_indexName = 0;
                }
            }
        }
        QEditor.SetScrollViewEnd();
    }

    //Group Block
    
    private void SetGUIGroupBlock()
    {
        QEditor.SetLabel("BLOCK", QEditor.GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter), QEditorWindow.GetGUILayoutWidth(this));
        {
            QEditor.SetHorizontalBegin();
            if (QEditor.SetButton(" - ", null, QEditor.GetGUIWidth(20f)))
            {
                if (m_countNameHorizontal > 1)
                    m_countNameHorizontal--;
            }
            QEditor.SetLabel(m_countNameHorizontal.ToString(), QEditor.GetGUILabel(FontStyle.Normal, TextAnchor.MiddleCenter), QEditor.GetGUIWidth(20));
            if (QEditor.SetButton(" + ", null, QEditor.GetGUIWidth(20f)))
            {
                m_countNameHorizontal++;
            }
            QEditor.SetHorizontalEnd();
        }
        m_scrollBlock = QEditor.SetScrollViewBegin(m_scrollBlock);
        int BlockIndex = 0;
        while (BlockIndex <= m_manager.List.BlockList[m_indexTag].Block.Count - 1)
        {
            QEditor.SetHorizontalBegin();
            for (int i = 0; i < m_countNameHorizontal; i++)
            {
                if (BlockIndex > m_manager.List.BlockList[m_indexTag].Block.Count - 1)
                    continue;
                
                QEditor.SetBackground(m_indexName == BlockIndex ? Color.white : Color.clear);
                if (GetGUIGroupBlockButton(BlockIndex))
                {
                    m_indexName = BlockIndex;
                }
                BlockIndex++;

            }
            QEditor.SetHorizontalEnd();
        }
        QEditor.SetScrollViewEnd();
    }

    private bool GetGUIGroupBlockButton(int Index)
    {
        return QEditor.SetButton(
            m_manager.List.BlockList[m_indexTag].Block[Index].GetComponent<SpriteRenderer>().sprite,
            QEditorWindow.GetGUILayoutWidth(this, 1f / m_countNameHorizontal),
            QEditorWindow.GetGUILayoutHeightBaseWidth(this, 1f / m_countNameHorizontal));
    }

    #endregion

    #region Block Manager

    //...

    #endregion
}

#endif