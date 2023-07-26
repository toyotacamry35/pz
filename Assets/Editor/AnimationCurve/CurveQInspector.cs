using Assets.ColonyShared.SharedCode.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
    [CustomEditor(typeof(CurveQ))]
    public class CurveQInspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var curveXProperty = serializedObject.FindProperty(nameof(CurveQ.curveX));
            var curveYProperty = serializedObject.FindProperty(nameof(CurveQ.curveY));
            var curveZProperty = serializedObject.FindProperty(nameof(CurveQ.curveZ));
            var curveWProperty = serializedObject.FindProperty(nameof(CurveQ.curveW));
            EditorGUILayout.PropertyField(curveXProperty, GUIContent.none, GUILayout.Height(100));
            EditorGUILayout.PropertyField(curveYProperty, GUIContent.none, GUILayout.Height(100));
            EditorGUILayout.PropertyField(curveZProperty, GUIContent.none, GUILayout.Height(100));
            EditorGUILayout.PropertyField(curveWProperty, GUIContent.none, GUILayout.Height(100));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
