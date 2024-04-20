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
            bool FocusFollowIdentityBase = FocusBodyMoveStatic.GetEditorFollowIdentity();
            if (FocusFollowIdentityBase)
            {
                if (QUnityEditor.SetButton("REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    FocusBodyMoveStatic.SetEditorFollowIdentityRemove();
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
            bool CursonFollowIdentityBase = CursonBodyMoveStatic.GetEditorFollowIdentity();
            if (QUnityEditor.SetButton(!CursonFollowIdentityBase ? "FOLLOW" : "REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                if (!CursonFollowIdentityBase)
                {
                    FocusBodyMoveStatic.SetEditorFollowIdentityBase();
                    CursonBodyMoveStatic.SetEditorFollowIdentityCheck(BlockFocus);
                }
                else
                    CursonBodyMoveStatic.SetEditorFollowIdentityRemove();
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
        bool CursonSwitchCheck = CursonSwitch.GetEditorSwitchIdentityCheck(BlockFocus.Pos);
        if (QUnityEditor.SetButton(!CursonSwitchCheck ? "FOLLOW" : "REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (!CursonSwitchCheck)
            {
                FocusSwitch.SetEditorSwitchIdentityBase();
                CursonSwitch.SetEditorSwitchIdentityCheck(BlockFocus);
            }
            else
                CursonSwitch.SetEditorSwitchIdentityCheckRemove(BlockFocus.Pos);
        }
        bool CursonSwitchBase = CursonSwitch.GetEditorSwitchIdentityBase();
        if (CursonSwitchBase)
        {
            if (QUnityEditor.SetButton("CLEAR", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                CursonSwitch.SetEditorSwitchIdentityBaseRemove();
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
        if (CursonBodyEvent.GetEditorEventIdentityBase())
        {
            if (string.IsNullOrEmpty(CursonBodyEvent.EditorEventIdentityBase))
                CursonBodyEvent.SetEditorEventIdentityBaseFind();
            QUnityEditor.SetLabel(CursonBodyEvent.EditorEventIdentityBase, null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));
        }
        else
            QUnityEditor.SetLabel("(No init config found)", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));
        CursonBodyEvent.EditorEventIdentityData = QUnityEditor.SetField<EventConfigSingle>(CursonBodyEvent.EditorEventIdentityData, QUnityEditorWindow.GetGUILayoutWidth(this, 0.75f));

        QUnityEditor.SetHorizontalBegin();
        if (QUnityEditor.SetButton("FIND", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventDataFind();
        if (QUnityEditor.SetButton("UPDATE", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventIdentityBase();
        if (QUnityEditor.SetButton("REMOVE", null, QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            CursonBodyEvent.SetEditorEventIdentityBaseRemove();
        QUnityEditor.SetHorizontalEnd();

        QUnityEditor.SetVerticalEnd();

        QUnityEditor.SetHorizontalEnd();
    }
}