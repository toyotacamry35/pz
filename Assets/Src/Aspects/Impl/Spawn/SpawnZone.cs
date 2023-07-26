using Assets.Src.Shared;
using Assets.Src.Tools;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using UnityEngine;
using UnityEngine.AI;
using NLog;
using NLog.Fluent;
using Src.Locomotion.Unity;

namespace Assets.Src.Aspects.Impl
{
    public class SpawnZone : ColonyBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        Dictionary<GameObject, List<SpawnArea>> _spawnAreasStatus = new Dictionary<GameObject, List<SpawnArea>>();
        public int CalculatedMaxObjects;
        public int CurrentObjectsCount;

        public float Radius = 5;
        [MinValue(0.1f)]
        public float MinRadius = 0.1f;
        private int _seedsRange;
        [MinValue(0f)]
        public float WeightMod = 1;
        [Header("Do not set this for non-spawned SpawnZones")]
        public int PrecalculatedMaxObjects;

        public SpawnTable LocalTable { get; set; }
        private void Awake()
        {
            LocalTable = GetComponent<SpawnTable>();
            var localAreas = GetComponentsInChildren<SpawnArea>();
            foreach (var area in localAreas)
                _spawnAreasStatus.GetOrCreate(area.MarkerPrefab).Add(area);
        }

        bool _loggedNavMeshError = false;
        public NavMeshHit FindPlace()
        {
            NavMeshHit hit = new NavMeshHit();
            int maxIteractions = 3;
            bool foundHit;
            do
            {
                if (maxIteractions-- <= 0)
                    break;
                var localPoint = Random.insideUnitCircle * (Radius - MinRadius) + Random.insideUnitCircle.normalized * MinRadius;
                var pos = transform.position + new Vector3(localPoint.x, 0, localPoint.y);
                foundHit = LocomotionNavMeshAgent.TryGetSurface(this.gameObject, pos, out hit); 
            } while (!foundHit);
            return hit;
        }
        public bool PlaceObject(GameObject spawnedObject)
        {
            var hit = FindPlace();
            if (!hit.hit && !_loggedNavMeshError)
            {
                _loggedNavMeshError = true;
                Logger.If(LogLevel.Error)
                    ?.Message($"NavMesh is possible missing under {this.gameObject.name} SpawnZone")
                    .UnityObj(this.gameObject).Write();
            }
            
            spawnedObject.name += Random.value;

            if (hit.hit)
                spawnedObject.transform.position = hit.position;
            else
                spawnedObject.transform.position = transform.position;

            var agent = spawnedObject.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.nextPosition = spawnedObject.transform.position;
                agent.enabled = true;
                //if (DbgLog.Enabled) DbgLog.Log($"_agentS.enabled == TRUE (SpZo) [{agent.isOnNavMesh} :: {agent.navMeshOwner}]({spawnedObject.GetComponent<EntityGameObjectComponent>()?.EntityId.ToString() ?? "noEGOC"})");
            }

            return hit.hit;
        }


        void AddSpawnArea(SpawnArea area, GameObject original)
        {
            _spawnAreasStatus.GetOrCreate(original).Add(area);
            if (area.MarkerPrefab != null)
                _spawnAreasStatus.GetOrCreate(area.MarkerPrefab).Add(area);
        }

        void RemoveSpawnArea(SpawnArea area, GameObject original)
        {
            _spawnAreasStatus.GetOrCreate(original).Remove(area);
            if (area.MarkerPrefab != null)
                _spawnAreasStatus.GetOrCreate(area.MarkerPrefab).Remove(area);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, MinRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        internal void OnObjectDestroyed(GameObject obj, GameObject original)
        {
            CurrentObjectsCount--;
            var area = obj.GetComponent<SpawnArea>();
            if (area != null)
            {
                RemoveSpawnArea(area, original);
            }
        }
    }
}
