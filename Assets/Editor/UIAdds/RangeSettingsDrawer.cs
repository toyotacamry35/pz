using UnityEditor;
using UnityEngine;
using WeldAdds;

[CustomPropertyDrawer(typeof(RangeSettings))]
public class RangeSettingsDrawer : PropertyDrawer
{
    private const float HorizontalSpacing = 2;

    private static readonly GUIContent MinMaxLabel = new GUIContent("[min...max)");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        var rect = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));
        rect = EditorGUI.PrefixLabel(rect, label);

        int indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var settingsWdt = (rect.width - HorizontalSpacing * 2) * 0.4f;
        var minMaxPartWdt = rect.width - settingsWdt - HorizontalSpacing;
        var minMaxLabelWdt = minMaxPartWdt / 2;
        var minMaxField = (minMaxPartWdt - minMaxLabelWdt - HorizontalSpacing) / 2;

        rect.width = settingsWdt; //Сначала Settings без label
        var settingsProp = property.FindPropertyRelative(nameof(RangeSettings.Settings));
        EditorGUI.PropertyField(rect, settingsProp, GUIContent.none); //Отдаем GUIContent.none, чтобы свойство отрисовалось компактно, см. SettingsDrawer

        rect.x += rect.width + HorizontalSpacing * 2;
        rect.width = minMaxLabelWdt;
        EditorGUI.LabelField(rect, MinMaxLabel);

        rect.x += rect.width;
        rect.width = minMaxField;
        var minProp = property.FindPropertyRelative(nameof(RangeSettings.Min));
        EditorGUI.PropertyField(rect, minProp, GUIContent.none);

        rect.x += rect.width + HorizontalSpacing;
        var maxProp = property.FindPropertyRelative(nameof(RangeSettings.Max));
        EditorGUI.PropertyField(rect, maxProp, GUIContent.none);

        EditorGUI.indentLevel = indentLevel;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}