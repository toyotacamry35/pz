using Assets.ColonyShared.SharedCode.Entities.Service;
using Assets.Src.Lib.Extensions;
using Assets.Src.ResourceSystem;
using Assets.Src.Server.Impl;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using System;
using UnityEngine;

namespace Assets.Src.Aspects
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public string Name { get; private set; }
        public float SpawnRadius = 1f;
        public JdbMetadata PointTypeMetaData;
        public SpawnPointTypeDef SpawnPointType;
        [NonSerialized]
        bool _awaken = false;
        
        void OnDrawGizmos()
        {
            if(!_awaken)
            {
                SpawnPointType = PointTypeMetaData?.Get<SpawnPointTypeDef>();
                Name = SpawnPointType?.____GetDebugShortName() ?? gameObject.name;

                var textMesh = transform.root.GetComponent<TextMesh>();
                if (textMesh)
                    textMesh.text = Name;
                _awaken = false;
            }
            Gizmos.DrawIcon(transform.position, Name, true);
        }

        // --- Privates: --------------------------------
    }
}
