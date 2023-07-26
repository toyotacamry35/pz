using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using AwesomeTechnologies.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    [System.Serializable]
    public class VegetationCellBaked
    {
        public Vector3 CellCorner;
        public Vector3 CellSize;
        public Bounds CellBounds;
        public bool EdgeCell;
        public bool SeaCell;

        public int RandomIndexOffset;

        public Rect Rectangle;
    }

    [System.Serializable]
    public class VegetationCellsStorage : ScriptableObject
    {
        [SerializeField]
        public List<VegetationCellBaked> vegetationCellBaked;

    }

   
}
