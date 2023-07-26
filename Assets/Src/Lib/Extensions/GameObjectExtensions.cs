using UnityEngine;

public static partial class Extensions
{
    public static void SetLayerRecursively(this GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}