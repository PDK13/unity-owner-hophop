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
    }

    //

    private void SetGUIBodyMoveStatic()
    {
        if (BlockFocus == null || BlockCurson == null)
            return;
        //
        if (BlockFocus == BlockCurson)
        {
            BodyMoveStatic BlockFocusMoveStatic = BlockFocus.GetComponent<BodyMoveStatic>();
            if (BlockFocusMoveStatic == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("MOVE-ST: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool MoveStaticFollow = BlockFocusMoveStatic.GetEditorFollowIdentity();
            if (MoveStaticFollow)
            {
                if (QUnityEditor.SetButton("REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
                {
                    BlockFocusMoveStatic.SetEditorFollowIdentityRemove();
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
            BodyMoveStatic BlockFocusMoveStatic = BlockFocus.GetComponent<BodyMoveStatic>();
            BodyMoveStatic BlockCursonMoveStatic = BlockCurson.GetComponent<BodyMoveStatic>();
            if (BlockFocusMoveStatic == null || BlockCursonMoveStatic == null)
                return;
            //
            QUnityEditor.SetHorizontalBegin();
            QUnityEditor.SetBackground(Color.white);
            QUnityEditor.SetLabel("MOVE-ST: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
            bool CursonCheckFollow = BlockCursonMoveStatic.GetEditorFollowIdentity();
            if (QUnityEditor.SetButton(!CursonCheckFollow ? "FOLLOW" : "REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                if (!CursonCheckFollow)
                {
                    BlockFocusMoveStatic.SetEditorFollowIdentity();
                    BlockCursonMoveStatic.SetEditorFollowIdentityCheck(BlockFocus);
                }
                else
                    BlockCursonMoveStatic.SetEditorFollowIdentityRemove();
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
        BodyMovePhysic BlockCursonMovePhysic = BlockCurson.GetComponent<BodyMovePhysic>();
        if (BlockCursonMovePhysic == null)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("MOVE-PH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        bool CursonCheckAhead = BlockCursonMovePhysic.GetEditorMoveCheckAhead();
        if (QUnityEditor.SetButton("AHEAD", QUnityEditor.GetGUIStyleButton(CursonCheckAhead ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonCheckAhead)
                BlockCursonMovePhysic.SetEditorMoveCheckAheadRemove();
            else
                BlockCursonMovePhysic.SetEditorMoveCheckAhead();
        }
        bool CursonCheckAheadBot = BlockCursonMovePhysic.GetEditorMoveCheckAheadBot();
        if (QUnityEditor.SetButton("AHEAD BOT", QUnityEditor.GetGUIStyleButton(CursonCheckAheadBot ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonCheckAhead)
                BlockCursonMovePhysic.SetEditorMoveCheckAheadBotRemove();
            else
                BlockCursonMovePhysic.SetEditorMoveCheckAheadBot();
        }
        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
    }

    private void SetGUIBodyPhysic()
    {
        if (BlockCurson == null)
            return;
        //
        BodyPhysic BlockCursonBodyStatic = BlockCurson.GetComponent<BodyPhysic>();
        if (BlockCursonBodyStatic == null)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("BODY-PH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        bool CursonCheckStatic = BlockCursonBodyStatic.GetEditorBodyStatic();
        if (QUnityEditor.SetButton("STATIC", QUnityEditor.GetGUIStyleButton(CursonCheckStatic ? FontStyle.Bold : FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (CursonCheckStatic)
                BlockCursonBodyStatic.SetEditorBodyStaticRemove();
            else
                BlockCursonBodyStatic.SetEditorBodyStatic();
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
        BodySwitch BlockFocusSwitch = BlockFocus.GetComponent<BodySwitch>();
        BodySwitch BlockCursonSwitch = BlockCurson.GetComponent<BodySwitch>();
        if (BlockFocusSwitch == null || BlockCursonSwitch == null)
            return;
        //
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetBackground(Color.white);
        QUnityEditor.SetLabel("SWITCH: ", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        bool CursonCheckFollow = BlockCursonSwitch.GetEditorSwitchCheck(BlockFocus.Pos);
        if (QUnityEditor.SetButton(!CursonCheckFollow ? "FOLLOW" : "REMOVE", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
        {
            if (!CursonCheckFollow)
            {
                BlockFocusSwitch.SetEditorSwitchIdentity();
                BlockCursonSwitch.SetEditorSwitchIdentityCheck(BlockFocus);
            }
            else
                BlockCursonSwitch.SetEditorSwitchCheckRemove(BlockFocus.Pos);
        }
        bool CursonCheckFollowExist = BlockCursonSwitch.GetEditorSwitchCheck();
        if (CursonCheckFollowExist)
        {
            if (QUnityEditor.SetButton("CLEAR", QUnityEditor.GetGUIStyleButton(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f)))
            {
                BlockCursonSwitch.SetEditorSwitchCheckRemove();
            }
        }
        QUnityEditor.SetLabel("", QUnityEditor.GetGUIStyleLabel(FontStyle.Normal, TextAnchor.MiddleCenter), QUnityEditorWindow.GetGUILayoutWidth(this, 0.25f));
        QUnityEditor.SetHorizontalEnd();
    }
}