using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "event-config", menuName = "HopHop/Event Config", order = 0)]
public class EventConfig : ScriptableObject
{
    public string Code = "";
    public List<EventConfigSingle> Data = new List<EventConfigSingle>();
}

[Serializable]
public class EventConfigSingle
{
    public DialogueConfigSingle Dialogue;
    public List<string> Command = new List<string>();
}

#if UNITY_EDITOR

[CustomEditor(typeof(EventConfig))]
public class EventConfigEditor: Editor
{
    private EventConfig Target;

    private SerializedProperty Code;
    private SerializedProperty Data;

    private void OnEnable()
    {
        Target = target as EventConfig;
        //
        Code = QUnityEditorCustom.GetField(this, "Code");
        Data = QUnityEditorCustom.GetField(this, "Data");
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(Code);
        QUnityEditorCustom.SetField(Data);
        //
        QUnityEditor.SetButton("+", null, QUnityEditor.GetGUISize(20,20));
        //
        //QUnityEditorCustom.SetApply(this);
    }
}

#endif