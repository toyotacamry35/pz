using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Aspects.Doings;
using GeneratorAnnotations;
using SharedCode.AI;
using SharedCode.Entities.Building;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.MapSystem;
using SharedCode.Utils;
using MongoDB.Bson.Serialization.Attributes;
using GeneratedCode.Custom.Config;
using SharedCode.Entities.GameObjectEntities;
using GeneratedCode.MapSystem;
using ResourceSystem.Utils;
using Scripting;

namespace SharedCode.Entities.Service
{
    public interface IHasWorldSpaced
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        IWorldSpaced WorldSpaced { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface IWorldSpaced : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> AssignToWorldSpace(OuterRef<IWorldSpaceServiceEntity> ownWorldSpace);

        [ReplicationLevel(ReplicationLevel.Always)]
        [BsonIgnore]
        OuterRef<IWorldSpaceServiceEntity> OwnWorldSpace { get; set; }
        [ReplicationLevel(ReplicationLevel.Always)]
        bool Destroyed { get; set; }
    }

    public static class ReplicatedObjectsWhitelist
    {
        public static bool ShouldReplicateToAnyoneEnMasse(IEntity ent) => typeof(ILegionaryEntity).IsAssignableFrom(ent.GetType()) || typeof(IBuildingPlace).IsAssignableFrom(ent.GetType()) || typeof(IFencePlace).IsAssignableFrom(ent.GetType()) || typeof(IWorldCharacter).IsAssignableFrom(ent.GetType());
    }

    // THIS IS NOT A SERVICE ENTITY
    [GenerateDeltaObjectCode]
    public interface IWorldSpaceServiceEntity : IEntity, IHasWorldObjectsInformationSetsMapEngine
    {
        //bool Internal { get; set; }
        //string Host { get; }
        //int Port { get; }
        OuterRef<IMapEntity> OwnMap { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<Vector2Int, OuterRef<IFencePlace>> FencePlaces { get; set; }
        [RuntimeData]
        ConcurrentDictionary<Guid, ConcurrentDictionary<ValueTuple<int, Guid>, bool>> AllUsersAndTheirCharacters { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<Guid?> GetWorldNodeId(OuterRef entityRef);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> AddWorldObject(int typeId, Guid entityId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> RemoveWorldObject(int typeId, Guid entityId);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> UpdateTransform(int typeId, Guid entityId); 

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<AddClientResult> AddClient(Guid characterId, Guid repositoryId);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> RemoveClient(Guid repositoryId, bool immediate = false);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<Guid> GetWorldBoxIdToDrop(Vector3 position, Guid characterOwnerId);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> RegisterFencePlace(OuterRef<IFencePlace> fencePlace);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> UnregisterFencePlace(OuterRef<IFencePlace> fencePlace);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<Guid> CreateFencePlaceId(Vector3 position);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<Guid> GetFencePlaceId(Vector3 position, bool onlyExisted);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task OnVisibilityChanged(int subjectTypeId, Guid subjectEntityId, List<ValueTuple<int, Guid>> addedObjects, List<ValueTuple<int, Guid>> removedObjects);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task EnableReplications(int subjectTypeId, Guid subjectEntityId, bool enable);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<int> GetCCU();

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task SpawnNewBot(string spawnPointTypePath, List<Guid> botIds, Guid userId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<bool> Respawn(Guid charId, bool checkBakens, bool anyCommonBaken, Guid commonBakenId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<AddClientResult> Login(BotActionDef botDef, string spawnPointPath, Guid userRepository, MapOwner mapOwner);
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> Logout(Guid userId, bool terminal);
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> LogoutAll();

        //World space will stream all world objects to this repo
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> ConnectStreamingRepo(Guid repo);
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> DisconnectStreamingRepo(Guid repo);

        Task<bool> Teleport(Guid oldRepositoryGuid);
        Task<bool> PrepareStaticsFor(OuterRef<IEntity> sceneEntity);
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> SpawnEntity(Guid staticIdFromExport, OuterRef<IEntity> ent, Vector3 pos, Quaternion rot, MapOwner mapOwner, Guid spawner, IEntityObjectDef def, SpawnPointTypeDef point, ScenicEntityDef scenicEntityDef, ScriptingContext ctx = null);
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> DespawnEntity(OuterRef<IEntity> ent);
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<PositionRotation> GetPositionToSpawnAt(Guid charId, bool checkBakens, bool anyCommonBaken, Guid commonBakenId, SpawnPointTypeDef overrideAllowedPointType = null);
    }

    public enum AddClientResult
    {
        None,
        Added,
        AlreadyExist,
        Error
    }
}
