using Assets.Src.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Lib.Wrappers.Editor
{

    [CustomPropertyDrawer(typeof(PathString))]
    public class PathStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                var path = property.FindPropertyRelative("Path");
                string val = path.stringValue;
                if (!val.IsNullOrWhitespace())
                {
                    path.stringValue = val.Replace('\\', '/');
                    if (val[0] != '/')
                        path.stringValue = "/" + val;
                }

                // Draw value
                EditorGUI.PropertyField(position, path, GUIContent.none);
            }
            EditorGUI.EndProperty();
        }
    }
}