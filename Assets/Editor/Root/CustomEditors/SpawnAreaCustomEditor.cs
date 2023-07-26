using Assets.Src.Aspects.Impl;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnArea))]
public class SpawnAreaCustomEditor : Editor
{

    private void OnSceneGUI()
    {
        var area = (SpawnArea)target;
        var serObject = new SerializedObject(target);
        var positionsField = serObject.FindProperty(nameof(SpawnArea.PositionsToSpawnAround));
        for (int i = 0; i < positionsField.arraySize; i++)
        {
            var vecProp = positionsField.GetArrayElementAtIndex(i);
            var vec = vecProp.FindPropertyRelative("Pos").vector3Value;
            var prevMatrix = Handles.matrix;
            Handles.matrix = area.transform.localToWorldMatrix;
            Handles.DrawSolidDisc(area.transform.localToWorldMatrix * vec, Vector3.up, 0.3f);
            var vec4 = area.transform.worldToLocalMatrix * Handles.PositionHandle(area.transform.localToWorldMatrix * vec, Quaternion.identity);
            if ((new Vector3(vec4.x, vec4.y, vec4.z) - vec).sqrMagnitude > Mathf.Epsilon * Mathf.Epsilon)
                vecProp.FindPropertyRelative("Pos").vector3Value = new Vector3(vec4.x, vec4.y, vec4.z);
            Handles.matrix = prevMatrix;
        }
        serObject.ApplyModifiedProperties();
    }
}
