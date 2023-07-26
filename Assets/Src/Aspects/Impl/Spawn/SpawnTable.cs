using System;
using System.Collections.Generic;
using Assets.Src.Shared;
using Core.Environment.Logging.Extension;
using UnityEngine;
using NLog;
using NLog.Fluent;

namespace Assets.Src.Aspects.Impl
{
    public class SpawnTable : ColonyBehaviour
    {

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        [Serializable]
        public class SpawnerData
        {
            public GameObject SpawnedObject;
            public int Weight;

            public GameObject LimitingObject;
            public int LimitMin;

            public GameObject SpawnNear;
            [Range(0, 1f)]
            public float ChanceOfSpawningNear = 1;
        }
        public HashSet<GameObject> SpawnedObjects = new HashSet<GameObject>();
        public List<SpawnerData> Spawnables = new List<SpawnerData>();
        public List<SpawnerData> SpawnablesObjects { get; set; } = new List<SpawnerData>();
        bool _init { get; set; } = false;
        public void Init()
        {
            if (_init)
                return;
            _init = true;
            foreach (var spawnable in Spawnables)
                if (spawnable.SpawnedObject != null)
                {
                    SpawnablesObjects.Add(spawnable);
                    SpawnedObjects.Add(spawnable.SpawnedObject);
                }
                else
                    Logger.If(LogLevel.Error)
                        ?.Message($"Missing spawned object in table {this.gameObject.name}").UnityObj(this).Write();
        }
    }
}
