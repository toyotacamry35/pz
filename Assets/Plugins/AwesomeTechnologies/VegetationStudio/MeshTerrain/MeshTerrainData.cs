using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.MeshTerrains
{
    [PreferBinarySerialization]
    [System.Serializable]
    public class MeshTerrainData : ScriptableObject
    {
        public Bounds Bounds;
        public int TriangleCount;

        
    }
}
