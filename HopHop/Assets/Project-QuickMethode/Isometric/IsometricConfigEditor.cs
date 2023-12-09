using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricConfig))]
public class IsometricConfigEditor : Editor
{
    private IsometricConfig m_target;

    private SerializedProperty m_blockList;

    private void OnEnable()
    {
        m_target = (target as IsometricConfig);

        m_blockList = QUnityEditorCustom.GetField(this, "m_blockList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        QUnityEditorCustom.SetField(m_blockList);

        if (QUnityEditor.SetButton("Refresh"))
        {
            m_target.SetRefresh();
        }

        QUnityEditorCustom.SetApply(this);
    }
}

#endif