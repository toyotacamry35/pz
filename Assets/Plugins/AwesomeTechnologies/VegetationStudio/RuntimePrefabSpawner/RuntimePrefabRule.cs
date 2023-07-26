using UnityEngine;

namespace AwesomeTechnologies.RuntimePrefabSpawner
{
    [System.Serializable]
    public class RuntimePrefabRule
    {
        public string VegetationItemID = "";
        public GameObject RuntimePrefab;
        public float SpawnFrequency = 1f;
        public int Seed;
        public Vector3 PrefabOffset = new Vector3(0, 0, 0);
        public Vector3 PrefabRotation = new Vector3(0, 0, 0);
        public Vector3 PrefabScale = new Vector3(1, 1, 1);
        public LayerMask PrefabLayer = 0;
        public bool UseVegetationItemScale;

        public void SetSeed()
        {
            Seed = Random.Range(0, 99);
        }
    }
}
