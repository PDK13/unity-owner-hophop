using QuickMethode;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(IsoVector))]
public class IsoVectorEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return QObjectEditor.GetContainer(
            property,
            nameof(IsoVector.X),
            nameof(IsoVector.Y),
            nameof(IsoVector.H));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        QObjectEditor.SetPropertyBegin(position, property, label);
        //
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        //
        QEditor.IndentLevel = 0;
        //
        float SpaceBetween = 5f;

        float WidthLabel = 15f;
        float WidthField = (position.width - WidthLabel * 3) / 3f - 5f;

        float PosXLabel = position.x;
        float SpaceXLabel = WidthLabel + WidthField;

        float PosXField = PosXLabel + WidthLabel;
        float SpaceXField = WidthField + WidthLabel;
        //
        var RecLabelX = new Rect(PosXLabel + SpaceXLabel * 0 + SpaceBetween * 0, position.y, WidthLabel, position.height);
        var RecLabelY = new Rect(PosXLabel + SpaceXLabel * 1 + SpaceBetween * 1, position.y, WidthLabel, position.height);
        var RecLabelH = new Rect(PosXLabel + SpaceXLabel * 2 + SpaceBetween * 2, position.y, WidthLabel, position.height);

        var RecFieldX = new Rect(PosXField + SpaceXField * 0 + SpaceBetween * 0, position.y, WidthField, position.height);
        var RecFieldY = new Rect(PosXField + SpaceXField * 1 + SpaceBetween * 1, position.y, WidthField, position.height);
        var RecFieldH = new Rect(PosXField + SpaceXField * 2 + SpaceBetween * 2, position.y, WidthField, position.height);
        //
        QEditor.SetLabel("X", RecLabelX);
        QObjectEditor.SetField(property, nameof(IsoVector.X), RecFieldX, false);

        QEditor.SetLabel("Y", RecLabelY);
        QObjectEditor.SetField(property, nameof(IsoVector.Y), RecFieldY, false);

        QEditor.SetLabel("H", RecLabelH);
        QObjectEditor.SetField(property, nameof(IsoVector.H), RecFieldH, false);
        //
        QObjectEditor.SetPropertyEnd();
    }
}

#endif