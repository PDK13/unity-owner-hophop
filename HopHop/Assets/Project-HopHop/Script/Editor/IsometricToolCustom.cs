using UnityEditor;
using UnityEngine;

public class IsometricToolCustom : IsometricTool
{
    private IsometricBlock m_blockBaseMove = null;

    [MenuItem("Tools/IsometricToolCustom")]
    public static void SetInitCustom()
    {
        GetWindow<IsometricToolCustom>("IsometricTool");
    }

    protected override void SetCursonControlCustom()
    {

    }

    protected override void SetEditGUIGroupCustom()
    {
        QUnityEditor.SetSpace();
        //
        SetGUIBodyMoveStatic();
        SetGUIBodyMovePhysic();
        SetGUIBodySwitch();
        SetGUIBodyEvent();
    }

    //

    private void SetGUIBodyMoveStatic()
    {
        if (BlockFocus == null || BlockCurson == null)
            return;
        //
        if (BlockFocus == BlockCurson)
        {
            BodyMoveStatic FocusMoveStatic = BlockFocus.GetComponent<BodyMoveStatic>();
            if (FocusMoveStatic == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("MOVE ST: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool CursonMoveStaticBase = FocusMoveStatic.GetEditorMoveIdentityBase();
            if (CursonMoveStaticBase)
            {
                if (QUnityEditor.SetButton("OFF", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    FocusMoveStatic.SetEditorMoveIdentityBaseRemove();
                }
            }
            else
            {
                if (QUnityEditor.SetButton("ON", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    FocusMoveStatic.SetEditorMoveIdentityBase();
                }
            }
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetHorizontalEnd();
        }
        else
        {
            BodyMoveStatic FocusMoveStatic = BlockFocus.GetComponent<BodyMoveStatic>();
            BodyMoveStatic CursonMoveStatic = BlockCurson.GetComponent<BodyMoveStatic>();
            if (FocusMoveStatic == null || CursonMoveStatic == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("MOVE ST: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool CursonFollowCheck = CursonMoveStatic.GetEditorMoveIdentityCheck(BlockFocus);
            if (QUnityEditor.SetButton(!CursonFollowCheck ? "ON LINK" : "OFF LINK", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                if (!CursonFollowCheck)
                {
                    FocusMoveStatic.SetEditorMoveIdentityBase();
                    CursonMoveStatic.SetEditorMoveIdentityCheck(BlockFocus);
                }
                else
                    CursonMoveStatic.SetEditorMoveIdentityCheckRemove(BlockFocus);
            }
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetHorizontalEnd();
        }
    }

    private void SetGUIBodyMovePhysic()
    {
        if (BlockCurson == null)
            return;
        //
        BodyMovePhysic CursonBodyMovePhysic = BlockCurson.GetComponent<BodyMovePhysic>();
        if (CursonBodyMovePhysic == null)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("MOVE-PH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        bool CursonMoveCheckAheadSide = CursonBodyMovePhysic.GetEditorMoveCheckAheadSide();
        if (QUnityEditor.SetButton("[C] AHEAD SIDE", QUnityEditor.GetGUIStyleButton(CursonMoveCheckAheadSide ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter, 8), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonMoveCheckAheadSide)
                CursonBodyMovePhysic.SetEditorMoveCheckAheadSideRemove();
            else
                CursonBodyMovePhysic.SetEditorMoveCheckAheadSide();
        }
        bool CursonMoveCheckAheadBot = CursonBodyMovePhysic.GetEditorMoveCheckAheadBot();
        if (QUnityEditor.SetButton("[C] AHEAD BOT", QUnityEditor.GetGUIStyleButton(CursonMoveCheckAheadBot ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter, 8), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonMoveCheckAheadSide)
                CursonBodyMovePhysic.SetEditorMoveCheckAheadBotRemove();
            else
                CursonBodyMovePhysic.SetEditorMoveCheckAheadBot();
        }
        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIBodySwitch()
    {
        if (BlockFocus == null || BlockCurson == null)
            return;
        //
        if (BlockFocus == BlockCurson)
        {
            BodyInteractiveSwitch FocusSwitch = BlockFocus.GetComponent<BodyInteractiveSwitch>();
            if (FocusSwitch == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("SWITCH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool CursonSwitchBase = FocusSwitch.GetEditorSwitchIdentityBase();
            if (CursonSwitchBase)
            {
                if (QUnityEditor.SetButton("OFF", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    FocusSwitch.SetEditorSwitchIdentityBaseRemove();
                }
            }
            else
            {
                if (QUnityEditor.SetButton("ON", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    FocusSwitch.SetEditorSwitchIdentityBase();
                }
            }
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetHorizontalEnd();
        }
        else
        {
            BodyInteractiveSwitch FocusSwitch = BlockFocus.GetComponent<BodyInteractiveSwitch>();
            BodyInteractiveSwitch CursonSwitch = BlockCurson.GetComponent<BodyInteractiveSwitch>();
            if (FocusSwitch == null || CursonSwitch == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("SWITCH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool CursonSwitchCheck = CursonSwitch.GetEditorSwitchIdentityCheck(BlockFocus);
            if (QUnityEditor.SetButton(!CursonSwitchCheck ? "ON LINK" : "OFF LINK", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                if (!CursonSwitchCheck)
                {
                    FocusSwitch.SetEditorSwitchIdentityBase();
                    CursonSwitch.SetEditorSwitchIdentityCheck(BlockFocus);
                }
                else
                    CursonSwitch.SetEditorSwitchIdentityCheckRemove(BlockFocus);
            }
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            QUnityEditor.SetHorizontalEnd();
        }
    }

    private void SetGUIBodyEvent()
    {
        if (BlockCurson == null)
            return;

        BodyInteractiveEvent CursonBodyEvent = BlockCurson.GetComponent<BodyInteractiveEvent>();
        if (CursonBodyEvent == null)
            return;

        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("BODY-EV: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));

        QUnityEditor.SetVerticalBegin();
        if (CursonBodyEvent.GetEditorEventIdentityBase())
        {
            if (string.IsNullOrEmpty(CursonBodyEvent.EditorEventIdentityBase))
                CursonBodyEvent.SetEditorEventIdentityBaseFind();
            QUnityEditor.SetLabel(CursonBodyEvent.EditorEventIdentityBase, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));
        }
        else
            QUnityEditor.SetLabel("(No init config found)", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));
        CursonBodyEvent.EditorEventIdentityData = QUnityEditor.SetFieldScriptableObject<EventConfigSingle>(CursonBodyEvent.EditorEventIdentityData, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));

        QUnityEditor.SetHorizontalBegin();
        if (QUnityEditor.SetButton("[C] FIND", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventDataFind();
        if (QUnityEditor.SetButton("[C] UPDATE", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventIdentityBase();
        if (QUnityEditor.SetButton("[C] DEL", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventIdentityBaseRemove();
        QUnityEditor.SetHorizontalEnd();

        QUnityEditor.SetVerticalEnd();

        QUnityEditor.SetHorizontalEnd();
    }
}