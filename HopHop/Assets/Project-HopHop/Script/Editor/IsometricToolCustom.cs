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
        QUnityEditor.SetSpace(5f);
        //
        SetGUIBodyMoveStatic();
        SetGUIBodyMovePhysic();
        SetGUIBodyPhysic();
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
            BodyMoveStatic FocusBodyMoveStatic = BlockFocus.GetComponent<BodyMoveStatic>();
            if (FocusBodyMoveStatic == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("MOVE-ST: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool FocusFollowIdentityBase = FocusBodyMoveStatic.GetEditorFollowIdentityBase();
            if (FocusFollowIdentityBase)
            {
                if (QUnityEditor.SetButton("REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    FocusBodyMoveStatic.SetEditorFollowIdentityBaseRemove();
                }
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            }
            else
            {
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
                QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            }
            QUnityEditor.SetHorizontalEnd();
        }
        else
        {
            BodyMoveStatic FocusBodyMoveStatic = BlockFocus.GetComponent<BodyMoveStatic>();
            BodyMoveStatic CursonBodyMoveStatic = BlockCurson.GetComponent<BodyMoveStatic>();
            if (FocusBodyMoveStatic == null || CursonBodyMoveStatic == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("MOVE-ST: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool CursonFollowIdentityBase = CursonBodyMoveStatic.GetEditorFollowIdentityBase();
            if (QUnityEditor.SetButton(!CursonFollowIdentityBase ? "FOLLOW" : "REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                if (!CursonFollowIdentityBase)
                {
                    FocusBodyMoveStatic.SetEditorFollowIdentityBase();
                    CursonBodyMoveStatic.SetEditorFollowIdentityBaseCheck(BlockFocus);
                }
                else
                    CursonBodyMoveStatic.SetEditorFollowIdentityBaseRemove();
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
        if (QUnityEditor.SetButton("AHEAD SIDE", QUnityEditor.GetGUIStyleButton(CursonMoveCheckAheadSide ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonMoveCheckAheadSide)
                CursonBodyMovePhysic.SetEditorMoveCheckAheadSideRemove();
            else
                CursonBodyMovePhysic.SetEditorMoveCheckAheadSide();
        }
        bool CursonMoveCheckAheadBot = CursonBodyMovePhysic.GetEditorMoveCheckAheadBot();
        if (QUnityEditor.SetButton("AHEAD BOT", QUnityEditor.GetGUIStyleButton(CursonMoveCheckAheadBot ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonMoveCheckAheadSide)
                CursonBodyMovePhysic.SetEditorMoveCheckAheadBotRemove();
            else
                CursonBodyMovePhysic.SetEditorMoveCheckAheadBot();
        }
        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIBodyPhysic()
    {
        if (BlockCurson == null)
            return;
        //
        BodyPhysic CursonBodyPhysic = BlockCurson.GetComponent<BodyPhysic>();
        if (CursonBodyPhysic == null)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("BODY-PH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        bool CursonBodyStatic = CursonBodyPhysic.GetEditorBodyStatic();
        if (QUnityEditor.SetButton("STATIC", QUnityEditor.GetGUIStyleButton(CursonBodyStatic ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonBodyStatic)
                CursonBodyPhysic.SetEditorBodyStaticRemove();
            else
                CursonBodyPhysic.SetEditorBodyStatic();
        }
        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIBodySwitch()
    {
        if (BlockFocus == null || BlockCurson == null)
            return;
        //
        if (BlockFocus == BlockCurson)
            return;
        //
        BodySwitch FocusSwitch = BlockFocus.GetComponent<BodySwitch>();
        BodySwitch CursonSwitch = BlockCurson.GetComponent<BodySwitch>();
        if (FocusSwitch == null || CursonSwitch == null)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("SWITCH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        bool CursonSwitchCheck = CursonSwitch.GetEditorSwitchCheck(BlockFocus.Pos);
        if (QUnityEditor.SetButton(!CursonSwitchCheck ? "FOLLOW" : "REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (!CursonSwitchCheck)
            {
                FocusSwitch.SetEditorSwitchIdentityBase();
                CursonSwitch.SetEditorSwitchIdentityCheck(BlockFocus);
            }
            else
                CursonSwitch.SetEditorSwitchCheckRemove(BlockFocus.Pos);
        }
        bool CursonSwitchBase = CursonSwitch.GetEditorSwitchBase();
        if (CursonSwitchBase)
        {
            if (QUnityEditor.SetButton("CLEAR", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                CursonSwitch.SetEditorSwitchCheckBaseRemove();
            }
        }
        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIBodyEvent()
    {
        if (BlockCurson == null)
            return;

        BodyEvent CursonBodyEvent = BlockCurson.GetComponent<BodyEvent>();
        if (CursonBodyEvent == null)
            return;

        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("BODY-EV: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));

        QUnityEditor.SetVerticalBegin();
        if (CursonBodyEvent.GetEditorEventBase())
        {
            if (string.IsNullOrEmpty(CursonBodyEvent.EditorEventIdentityBase))
                CursonBodyEvent.SetEditorEventBaseFind();
            QUnityEditor.SetLabel(CursonBodyEvent.EditorEventIdentityBase, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));
        }
        else
            QUnityEditor.SetLabel("(No config name found)", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));
        CursonBodyEvent.EditorEventIdentityData = QUnityEditor.SetField<EventConfigSingle>(CursonBodyEvent.EditorEventIdentityData, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));

        QUnityEditor.SetHorizontalBegin();
        if (QUnityEditor.SetButton("FIND", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventDataFind();
        if (QUnityEditor.SetButton("UPDATE", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventIdentityBase();
        QUnityEditor.SetHorizontalEnd();

        QUnityEditor.SetVerticalEnd();

        QUnityEditor.SetHorizontalEnd();
    }
}