// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1695896057, typeof(SharedCode.Entities.Service.IWorldSpaceServiceEntity))]
    public interface IWorldSpaceServiceEntityAlways : SharedCode.EntitySystem.IEntity, IHasWorldObjectsInformationSetsMapEngineAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.MapSystem.IMapEntity> OwnMap
        {
            get;
        }

        System.Collections.Concurrent.ConcurrentDictionary<System.Guid, System.Collections.Concurrent.ConcurrentDictionary<(int, System.Guid), bool>> AllUsersAndTheirCharacters
        {
            get;
        }

        System.Threading.Tasks.Task<bool> Teleport(System.Guid oldRepositoryGuid);
        System.Threading.Tasks.Task<bool> PrepareStaticsFor(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> sceneEntity);
        System.Threading.Tasks.Task<bool> SpawnEntity(System.Guid staticIdFromExport, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, GeneratedCode.MapSystem.MapOwner mapOwner, System.Guid spawner, SharedCode.Entities.GameObjectEntities.IEntityObjectDef def, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, SharedCode.Entities.GameObjectEntities.ScenicEntityDef scenicEntityDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> DespawnEntity(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1874962953, typeof(SharedCode.Entities.Service.IWorldSpaceServiceEntity))]
    public interface IWorldSpaceServiceEntityClientBroadcast : SharedCode.EntitySystem.IEntity, IHasWorldObjectsInformationSetsMapEngineClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.MapSystem.IMapEntity> OwnMap
        {
            get;
        }

        System.Collections.Concurrent.ConcurrentDictionary<System.Guid, System.Collections.Concurrent.ConcurrentDictionary<(int, System.Guid), bool>> AllUsersAndTheirCharacters
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Entities.Service.AddClientResult> Login(Assets.Src.Aspects.Doings.BotActionDef botDef, string spawnPointPath, System.Guid userRepository, GeneratedCode.MapSystem.MapOwner mapOwner);
        System.Threading.Tasks.Task<bool> Teleport(System.Guid oldRepositoryGuid);
        System.Threading.Tasks.Task<bool> PrepareStaticsFor(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> sceneEntity);
        System.Threading.Tasks.Task<bool> SpawnEntity(System.Guid staticIdFromExport, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, GeneratedCode.MapSystem.MapOwner mapOwner, System.Guid spawner, SharedCode.Entities.GameObjectEntities.IEntityObjectDef def, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, SharedCode.Entities.GameObjectEntities.ScenicEntityDef scenicEntityDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> DespawnEntity(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1035192496, typeof(SharedCode.Entities.Service.IWorldSpaceServiceEntity))]
    public interface IWorldSpaceServiceEntityClientFullApi : SharedCode.EntitySystem.IEntity, IHasWorldObjectsInformationSetsMapEngineClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1361417140, typeof(SharedCode.Entities.Service.IWorldSpaceServiceEntity))]
    public interface IWorldSpaceServiceEntityClientFull : SharedCode.EntitySystem.IEntity, IHasWorldObjectsInformationSetsMapEngineClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.MapSystem.IMapEntity> OwnMap
        {
            get;
        }

        System.Collections.Concurrent.ConcurrentDictionary<System.Guid, System.Collections.Concurrent.ConcurrentDictionary<(int, System.Guid), bool>> AllUsersAndTheirCharacters
        {
            get;
        }

        System.Threading.Tasks.Task<bool> Respawn(System.Guid charId, bool checkBakens, bool anyCommonBaken, System.Guid commonBakenId);
        System.Threading.Tasks.Task<SharedCode.Entities.Service.AddClientResult> Login(Assets.Src.Aspects.Doings.BotActionDef botDef, string spawnPointPath, System.Guid userRepository, GeneratedCode.MapSystem.MapOwner mapOwner);
        System.Threading.Tasks.Task<bool> Teleport(System.Guid oldRepositoryGuid);
        System.Threading.Tasks.Task<bool> PrepareStaticsFor(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> sceneEntity);
        System.Threading.Tasks.Task<bool> SpawnEntity(System.Guid staticIdFromExport, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, GeneratedCode.MapSystem.MapOwner mapOwner, System.Guid spawner, SharedCode.Entities.GameObjectEntities.IEntityObjectDef def, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, SharedCode.Entities.GameObjectEntities.ScenicEntityDef scenicEntityDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> DespawnEntity(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -691793637, typeof(SharedCode.Entities.Service.IWorldSpaceServiceEntity))]
    public interface IWorldSpaceServiceEntityServerApi : SharedCode.EntitySystem.IEntity, IHasWorldObjectsInformationSetsMapEngineServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1770586940, typeof(SharedCode.Entities.Service.IWorldSpaceServiceEntity))]
    public interface IWorldSpaceServiceEntityServer : SharedCode.EntitySystem.IEntity, IHasWorldObjectsInformationSetsMapEngineServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.MapSystem.IMapEntity> OwnMap
        {
            get;
        }

        System.Collections.Concurrent.ConcurrentDictionary<System.Guid, System.Collections.Concurrent.ConcurrentDictionary<(int, System.Guid), bool>> AllUsersAndTheirCharacters
        {
            get;
        }

        System.Threading.Tasks.Task<System.Guid?> GetWorldNodeId(ResourceSystem.Utils.OuterRef entityRef);
        System.Threading.Tasks.Task<bool> AddWorldObject(int typeId, System.Guid entityId);
        System.Threading.Tasks.Task<bool> RemoveWorldObject(int typeId, System.Guid entityId);
        System.Threading.Tasks.Task<SharedCode.Entities.Service.AddClientResult> AddClient(System.Guid characterId, System.Guid repositoryId);
        System.Threading.Tasks.Task<System.Guid> GetWorldBoxIdToDrop(SharedCode.Utils.Vector3 position, System.Guid characterOwnerId);
        System.Threading.Tasks.Task<System.Guid> GetFencePlaceId(SharedCode.Utils.Vector3 position, bool onlyExisted);
        System.Threading.Tasks.Task OnVisibilityChanged(int subjectTypeId, System.Guid subjectEntityId, System.Collections.Generic.List<(int, System.Guid)> addedObjects, System.Collections.Generic.List<(int, System.Guid)> removedObjects);
        System.Threading.Tasks.Task EnableReplications(int subjectTypeId, System.Guid subjectEntityId, bool enable);
        System.Threading.Tasks.Task<int> GetCCU();
        System.Threading.Tasks.Task SpawnNewBot(string spawnPointTypePath, System.Collections.Generic.List<System.Guid> botIds, System.Guid userId);
        System.Threading.Tasks.Task<bool> Respawn(System.Guid charId, bool checkBakens, bool anyCommonBaken, System.Guid commonBakenId);
        System.Threading.Tasks.Task<SharedCode.Entities.Service.AddClientResult> Login(Assets.Src.Aspects.Doings.BotActionDef botDef, string spawnPointPath, System.Guid userRepository, GeneratedCode.MapSystem.MapOwner mapOwner);
        System.Threading.Tasks.Task<bool> Logout(System.Guid userId, bool terminal);
        System.Threading.Tasks.Task<bool> LogoutAll();
        System.Threading.Tasks.Task<bool> ConnectStreamingRepo(System.Guid repo);
        System.Threading.Tasks.Task<bool> DisconnectStreamingRepo(System.Guid repo);
        System.Threading.Tasks.Task<bool> Teleport(System.Guid oldRepositoryGuid);
        System.Threading.Tasks.Task<bool> PrepareStaticsFor(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> sceneEntity);
        System.Threading.Tasks.Task<bool> SpawnEntity(System.Guid staticIdFromExport, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, GeneratedCode.MapSystem.MapOwner mapOwner, System.Guid spawner, SharedCode.Entities.GameObjectEntities.IEntityObjectDef def, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, SharedCode.Entities.GameObjectEntities.ScenicEntityDef scenicEntityDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> DespawnEntity(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent);
        System.Threading.Tasks.Task<SharedCode.Entities.GameObjectEntities.PositionRotation> GetPositionToSpawnAt(System.Guid charId, bool checkBakens, bool anyCommonBaken, System.Guid commonBakenId, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef overrideAllowedPointType);
    }
}