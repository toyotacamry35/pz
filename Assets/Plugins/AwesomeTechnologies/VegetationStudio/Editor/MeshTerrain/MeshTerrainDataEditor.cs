using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.MeshTerrains
{
    [CustomEditor(typeof(MeshTerrainData))]
    public class MeshTerrainDataEditor : VegetationStudioBaseEditor
    {
        [MenuItem("Assets/Create/Awesome Technologies/Mesh Terrain/MeshTerrainData")]
        public static void CreateMeshTerrainDataScriptableObject()
        {
            ScriptableObjectUtility2.CreateAndReturnAsset<MeshTerrainData>();

            //MeshTerrainData meshTerrainData = 
        }
    }
}
