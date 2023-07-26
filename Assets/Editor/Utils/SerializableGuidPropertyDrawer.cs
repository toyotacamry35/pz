using Src.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Utils
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _property;
        private string _guid;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_property != property)
            {
                SerializedProperty prop_A = property.FindPropertyRelative("_a");
                SerializedProperty prop_B = property.FindPropertyRelative("_b");
                var guid = new SerializableGuid((ulong) prop_A.longValue, (ulong) prop_B.longValue).Guid;
                _guid = guid.ToString();
                _property = property;
            }
            EditorGUI.TextField(position, label, _guid);
        }
    }
}