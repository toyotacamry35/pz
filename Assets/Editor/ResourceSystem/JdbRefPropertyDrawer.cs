using Core.Reflection;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.ResourceSystem.Editor
{
    [CustomPropertyDrawer(typeof(JdbRefBase), true)]
    class JdbRefPropertyDrawer : PropertyDrawer
    {
        private static Type GetFieldViaPath(Type rootType, string path)
        {
            string[] slices = path.Split('.');
            Type type = rootType;
            for (int i = 0; type != null && i < slices.Length; i++)
            {
                if (slices[i] == "Array")
                {
                    i++; //skips "data[x]"
                    if (type.IsArray)
                        type = type.GetElementType(); //gets info on array elements
                    else
                        type = type.GetGenericArguments()[0];
                }
                else
                {
                    type = GetFieldType(type, slices[i]);
                }
            }
            return type;
        }

        private static Type GetFieldType(Type type, string name)
        {
            if (type == null)
                return null;
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (field == null)
                return GetFieldType(type.BaseType, name);
            return field.FieldType;
        }
        
        private static Type GetPropType(SerializedProperty prop)
        {
            var type = prop.serializedObject.targetObject.GetType();
            return GetFieldViaPath(type, prop.propertyPath);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            var prop = property.FindPropertyRelative("_metadata");
            var type = GetPropType(property);
            var targetType = type?.BaseType?.GetGenericArguments()[0];
            if (targetType != null)
            {
                var oldMeta = prop.objectReferenceValue as JdbMetadata;
                var newMeta = EditorGUI.ObjectField(position, label, prop.objectReferenceValue, typeof(JdbMetadata), false) as JdbMetadata;
                if (newMeta != oldMeta )
                {
                    var newType = newMeta != null && !string.IsNullOrEmpty(newMeta.Type)
                        ? AppDomain.CurrentDomain.GetAssembliesSafe().Select(x => x.GetTypeSafe(newMeta.Type)).Single(v => v != null)
                        : null;
                    if (newMeta == null || newType != null && targetType.IsAssignableFrom(newType))
                    {
                        prop.objectReferenceValue = newMeta;
                    }
                }
            }
            else
                EditorGUI.HelpBox(position, "Bad Reference Type", MessageType.Error);

            EditorGUI.EndProperty();
        }
    }
}
