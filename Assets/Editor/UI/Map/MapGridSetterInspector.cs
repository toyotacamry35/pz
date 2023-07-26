using Uins;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGridSetter))]
public class MapGridSetterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        if (GUILayout.Button("Set grid positions", GUILayout.MaxWidth(150)))
        {
            ((MapGridSetter) target).SetGridPositions();
        }
        EditorGUILayout.Separator();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
    }
}