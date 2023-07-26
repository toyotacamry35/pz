using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.Src.Shared;

public class DropToTerrain : MonoBehaviour
{
    [MenuItem("Level Design/Drop to Terrain %t")]
    static void Align()
    {
        Transform[] transforms = Selection.transforms;
        foreach (Transform myTransform in transforms)
        {
            RaycastHit hit;
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, 3000, PhysicsLayers.TerrainMask))
            {
                Vector3 targetPosition = hit.point;
                myTransform.position = targetPosition;
            }
        }
    }
}