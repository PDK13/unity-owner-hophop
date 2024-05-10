using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config-mode", menuName = "HopHop/Event Config Mode", order = 0)]
public class EventConfigMode : EventConfigOptional
{
    public string Data = "";

    //

    public override OptionalType Type => OptionalType.Mode;
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfigMode))]
public class EventConfigModeEditor : Editor
{
    private const float LABEL_WIDTH = 65f;

    private EventConfigMode m_target;

    private void OnEnable()
    {
        m_target = target as EventConfigMode;
    }

    public override void OnInspectorGUI()
    {
        SetGUIGroupOptional();

        QUnityEditor.SetDirty(m_target);
    }

    //

    public void SetGUIGroupOptional()
    {
        QUnityEditor.SetLabel("OPTIONAL", QUnityEditor.GetGUIStyleLabel(FontStyle.Bold));

        #region Optional - Name
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Name", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.OptionName = QUnityEditor.SetField(m_target.OptionName);
        QUnityEditor.SetHorizontalEnd();
        #endregion

        #region Optional - Tip
        QUnityEditor.SetHorizontalBegin();
        QUnityEditor.SetLabel("Tip", null, QUnityEditor.GetGUILayoutWidth(LABEL_WIDTH));
        m_target.OptionalTip = QUnityEditor.SetFieldScriptableObject(m_target.OptionalTip);
        QUnityEditor.SetHorizontalEnd();
        #endregion
    }
}

#endif