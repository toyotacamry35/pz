using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorAnnotations;
using GeneratedCode.MapSystem;
using Assets.ColonyShared.SharedCode.Entities;
using SharedCode.MovementSync;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedCode.Entities.GameObjectEntities
{
    [GenerateDeltaObjectCode]
    public interface ISpawnDaemon : IEntity, IScenicEntity, IEntityObject, IHasLinksEngine, IHasSimpleMovementSync
    {
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> UpdateSpawnDaemon(bool outOfOrder);
        
        [ReplicationLevel(ReplicationLevel.Master)] string Name { get; set; }
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.Master)] IDeltaDictionary<IEntityObjectDef, int> SpawnedObjectsAmounts { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> NotifyOfObjectDestruction(Guid id, int typeId);
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> ResetDaemon();
        [ReplicationLevel(ReplicationLevel.Master)]
        [BsonIgnore]
        IDeltaList<SpawnTemplatesMapDef> Maps { get; set; }
        Task<bool> TryPlaceObjectNear(SpawnPointTypeDef pointType, IEntityObjectDef objDef, Vector3 pos, bool ignoreGeometry = false);
        Task<bool> ActivateTemplatePointsBatch(List<SpawnTemplateDef> def);

        Task NotifyOfEntityDissipation(Vector3 pos, Quaternion rot, SpawnPointTypeDef point, Guid guid, int typeId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> AwaitUnityThread();
        [BsonIgnore]
        SpawnDaemonSceneDef SceneDef { get; set; }
    }

    public struct PointData
    {
        public Vector3 Pos;
        public Quaternion Rot;
    }

    public interface IHasSpawnedObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [LockFreeReadonlyProperty]
        ISpawnedObject SpawnedObject{ get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface ISpawnedObject : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [LockFreeReadonlyProperty]
        OuterRef<ISpawnDaemon> Spawner { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)]
        SpawnPointTypeDef PointType { get; set; }
    }
}