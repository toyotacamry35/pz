using UnityEditor;
using UnityEngine;

internal static partial class EditorMenus
{
    [MenuItem("Tools/Group %g", false)]
    private static void Group()
    {
        if (Selection.transforms.Length > 0)
        {
            var group = new GameObject("Group");

            // set pivot to average of selection
            var pivotPosition = Vector3.zero;
            foreach (var g in Selection.transforms) pivotPosition += g.transform.position;
            pivotPosition /= Selection.transforms.Length;
            group.transform.position = pivotPosition;

            // register undo as we parent objects into the group
            Undo.RegisterCreatedObjectUndo(group, "Group");
            foreach (var s in Selection.gameObjects) Undo.SetTransformParent(s.transform, group.transform, "Group");

            Selection.activeGameObject = group;
        }
        else
        {
            Debug.LogWarning("You must select one or more objects.");
        }
    }
}