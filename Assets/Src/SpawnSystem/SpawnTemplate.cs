using Assets.Src.ResourceSystem;
using Assets.Src.Shared;
using Assets.Src.Tools;
using SharedCode.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    [ExecuteInEditMode]
    public class SpawnTemplate : MonoBehaviour, ISpatialHashed
    {
        [Tooltip("What kind of templates to check around when trying to activate this")]
        public JdbMetadata ExclusionGroup;
        public float ExclusionRadius = 100;

        public struct TemplateBounds
        {
            public float Radius;
            public UnityEngine.Vector3 Center;
        }

        public TemplateBounds CalcBounds()
        {
            var points = GetComponentsInChildren<SpawnPoint>();
            if (points.Length == 0)
                return new TemplateBounds() { Center = transform.position, Radius = 1 };

            var centerX = points.Average(p => p.transform.position.x);
            var centerY = points.Average(p => p.transform.position.y);
            var centerZ = points.Average(p => p.transform.position.z);
            var cVec = new UnityEngine.Vector3(centerX, centerY, centerZ);
            var maxDistance = Mathf.Sqrt(points.Max(p => (cVec - p.transform.position).sqrMagnitude));
            return new TemplateBounds() { Center = cVec, Radius = maxDistance };
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
                return;

            gameObject.layer = PhysicsLayers.Default;
            var col = GetComponent<SphereCollider>();
            if (col == null)
                col = gameObject.AddComponent<SphereCollider>();

            var bounds = CalcBounds();
            col.radius = bounds.Radius;
            col.center = transform.InverseTransformPoint(bounds.Center);
        }

        public void GetHash(ISpatialHash spHash, ICollection<SharedCode.Utils.Vector3Int> resultHash)
        {
            resultHash.Add(spHash.PosHash(gameObject.transform.position.ToSharedVec3()));
        }
    }
}
