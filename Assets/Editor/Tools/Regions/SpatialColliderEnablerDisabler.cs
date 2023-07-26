using Assets.Src.Aspects;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpatialColliderEnablerDisabler : ScriptableObject
{
    [MenuItem("Level Design/Regions/Disable selected SpatialTrigger colliders")]
    private static void DisableColliders()
    {
        SetColliders(false);
    }

    [MenuItem("Level Design/Regions/Enable selected SpatialTrigger colliders")]
    private static void EnableColliders()
    {
        SetColliders(true);
    }

    private static void SetColliders(bool enabled)
    {
        var GOsWithColliders = Selection.gameObjects.Where(x => x.GetComponent<SpatialTrigger>() != null).Select(x => x.GetComponent<Collider>());
        foreach (var collider in GOsWithColliders)
        {
            if (collider != null)
                collider.enabled = enabled;
        }
    }
}