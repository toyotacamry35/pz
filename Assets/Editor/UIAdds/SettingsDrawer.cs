using UnityEditor;
using UnityEngine;
using WeldAdds;


[CustomPropertyDrawer(typeof(Settings))]
public class SettingsDrawer : PropertyDrawer
{
    private const float HorizontalSpacing = 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var rect = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));
        if (!string.IsNullOrEmpty(label.text))
        {
            //Обычное отображение свойства: Название - значение
            rect = EditorGUI.PrefixLabel(rect, label);
            //Если просто оставить везде PrefixLabel(), то при отображении в ряду других свойств он запорет rect, съев  всю площадь.
            //Метод ведет себя так, будто ему выделили пространство от самого левого края инспектора :(
            //Короче, в режиме "одной строкой с другими свойствами" не используем его, ну или можно вместо него использовать LabelField()
        }

        int indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var floatWdt = (rect.width - HorizontalSpacing) * 0.3f;
        var colorWdt = rect.width - floatWdt - HorizontalSpacing;
        rect.width = floatWdt;
        var floatProp = property.FindPropertyRelative(nameof(Settings.Float));
        EditorGUI.PropertyField(rect, floatProp, GUIContent.none);

        rect.x += rect.width + HorizontalSpacing; //передвигаем rect
        rect.width = colorWdt;
        var colorProp = property.FindPropertyRelative(nameof(Settings.Color));
        EditorGUI.PropertyField(rect, colorProp, GUIContent.none);

        EditorGUI.indentLevel = indentLevel;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}