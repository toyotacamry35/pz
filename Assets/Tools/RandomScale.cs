using UnityEngine;
using System.Collections;

public class RandomScale : MonoBehaviour
{
    public float Max;
    public float Min;
    public void Scale()
    {
        var val = Mathf.Lerp(Min, Max, Random.value);
        transform.localScale = new Vector3(val, val, val);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Level Design/Randomize Scale &I")]
    public static void RandomizeScale()
    {
        var targetObjects = UnityEditor.Selection.gameObjects;

        UnityEditor.Undo.SetCurrentGroupName("scaling");
        var undoID = UnityEditor.Undo.GetCurrentGroup();
        foreach (var go in targetObjects)
        {
            var scale = go.GetComponent<RandomScale>();
            if (scale == null)
            {
                scale = UnityEditor.Undo.AddComponent<RandomScale>(go);
                scale.Min = 0.8f;
                scale.Max = 1.2f;
            }
            UnityEditor.Undo.RecordObject(go.transform,"scaling");
            scale.Scale();

        }
        UnityEditor.Undo.CollapseUndoOperations(undoID);
    }
#endif
}
