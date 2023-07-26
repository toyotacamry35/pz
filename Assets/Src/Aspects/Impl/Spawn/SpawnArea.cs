using System;
using System.Linq;
using UnityEngine;

namespace Assets.Src.Aspects.Impl
{
    public class SpawnArea : MonoBehaviour
    {
        public GameObject MarkerPrefab;
        public bool NearObjectsCanProduceNewClusters = false;
        public bool DropOnTerrain = true;
        public SpawnPoint[] PositionsToSpawnAround;
        [Serializable]
        public struct SpawnPoint
        {
            public Vector3 Pos;
        }

        private void Awake()
        {
            if(PositionsToSpawnAround.Length == 0)
            {
                PositionsToSpawnAround = GetComponentsInChildren<SpawnAreaMarker>().Select(x => new SpawnPoint() { Pos = x.transform.localPosition }).ToArray();
            }
        }
    }
}
