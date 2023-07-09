using UnityEditor;
using QuickMethode;

#if UNITY_EDITOR

[CustomEditor(typeof(IsometricConfig))]
public class IsometricConfigEditor : Editor
{
    private IsometricConfig m_target;

    private SerializedProperty m_blockList;

    private void OnEnable()
    {
        m_target = (target as IsometricConfig);

        m_blockList = QCustomEditor.GetField(this, "m_blockList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        QCustomEditor.SetField(m_blockList);

        if (QEditor.SetButton("Refresh"))
            m_target.SetRefresh();

        QCustomEditor.SetApply(this);
    }
}

#endif