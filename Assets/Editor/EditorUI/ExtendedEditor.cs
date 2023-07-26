using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Src.Utils;
using UnityEditor;

namespace Assets.Editor.EditorUI
{
    public class ExtendedEditor : UnityEditor.Editor
    {
        private readonly Dictionary<string, bool> _foldouts =  new Dictionary<string, bool>();
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        public bool DrawDefaultInspector()
        {
            return DoDrawDefaultInspector(serializedObject, _foldouts);
        }

        private static bool DoDrawDefaultInspector(SerializedObject obj, Dictionary<string, bool> foldouts)
        {
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            SerializedProperty iterator = obj.GetIterator();
            bool skip = false;
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if ("m_Script" == iterator.propertyPath)
                    continue;
                
                var attrs = GetCustomAttributes(iterator);
                using (new EditorGUI.DisabledScope(Array.Exists(attrs, x => x is DisabledInInspectorAttribute)))
                {
                    if (attrs.FirstOrDefault(x => x is FoldoutAttribute) is FoldoutAttribute fld)
                    {
                        if (fld.Text != null)
                            skip = !(foldouts[iterator.propertyPath] =
                                EditorGUILayout.Foldout(foldouts.TryGetValue(iterator.propertyPath, out var b) ? b : fld.Default,
                                    fld.Text));
                        else
                            skip = false;
                    }
                    if (!skip)
                        EditorGUILayout.PropertyField(iterator, true);
                }
            }
            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        public static Attribute[] GetCustomAttributes(SerializedProperty property)
        {
            if (property == null)
                return Array.Empty<Attribute>();
            var type = property.serializedObject.targetObject.GetType();
            FieldInfo field = null;
            string[] path = property.propertyPath.Split('.');
            for (int i = 0; i < path.Length; i++)
            {
                field = type.GetField(path[i], BindingFlags.Public
                                               | BindingFlags.NonPublic
                                               | BindingFlags.FlattenHierarchy
                                               | BindingFlags.Instance);
                if (field == null)
                    return Array.Empty<Attribute>();
                type = field.FieldType;
                int next = i + 1;
                if (next < path.Length && path[next] == "Array")
                {
                    i += 2;
                    type = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
                }
            }
            return field != null ? field.GetCustomAttributes(true).Cast<Attribute>().ToArray() : Array.Empty<Attribute>();
        }
    }
}