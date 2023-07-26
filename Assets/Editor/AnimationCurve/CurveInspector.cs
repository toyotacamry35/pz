using Assets.ColonyShared.SharedCode.Utils;
using Src.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
    [CustomEditor(typeof(Curve))]
    public class CurveInspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var curveProperty = serializedObject.FindProperty(nameof(Curve.curve));
            EditorGUILayout.PropertyField(curveProperty, GUIContent.none, GUILayout.Height(100));
            serializedObject.ApplyModifiedProperties();
        }
    }
}