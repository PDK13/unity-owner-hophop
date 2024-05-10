using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config-trade", menuName = "HopHop/Event Config Trade", order = 0)]
public class EventConfigTrade : EventConfigOptional
{
    public List<string> Data = new List<string>();

    //

    public override OptionalType Type => OptionalType.Trade;
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfigTrade))]
public class EventConfigTradeEditor : Editor
{
    private const float LABEL_WIDTH = 65f;

    private EventConfigTrade m_target;

    private void OnEnable()
    {
        m_target = target as EventConfigTrade;
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