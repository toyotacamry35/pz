using Assets.ColonyShared.SharedCode.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
    [CustomEditor(typeof(Curve3))]
    public class Curve3Inspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var curveXProperty = serializedObject.FindProperty(nameof(Curve3.curveX));
            var curveYProperty = serializedObject.FindProperty(nameof(Curve3.curveY));
            var curveZProperty = serializedObject.FindProperty(nameof(Curve3.curveZ));
            EditorGUILayout.PropertyField(curveXProperty, GUIContent.none, GUILayout.Height(100));
            EditorGUILayout.PropertyField(curveYProperty, GUIContent.none, GUILayout.Height(100));
            EditorGUILayout.PropertyField(curveZProperty, GUIContent.none, GUILayout.Height(100));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
