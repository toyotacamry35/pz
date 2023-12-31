// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1419537705, typeof(SharedCode.MapSystem.IMapEntity))]
    public interface IMapEntityAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Guid RealmId
        {
            get;
        }

        SharedCode.Aspects.Sessions.RealmRulesDef RealmRules
        {
            get;
        }

        GeneratedCode.Custom.Config.MapDef Map
        {
            get;
        }

        SharedCode.MapSystem.MapEntityState State
        {
            get;
        }

        bool Dead
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool> SavedScenes
        {
            get;
        }

        System.Threading.Tasks.Task<bool> TryAquireSpawnRightsForPointsSet(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef);
        System.Threading.Tasks.Task<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPoint(SharedCode.Utils.Vector3 point);
        System.Threading.Tasks.Task<bool> NotifyAllCharactersViaChat(string text);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 301630317, typeof(SharedCode.MapSystem.IMapEntity))]
    public interface IMapEntityClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Guid RealmId
        {
            get;
        }

        SharedCode.Aspects.Sessions.RealmRulesDef RealmRules
        {
            get;
        }

        GeneratedCode.Custom.Config.MapDef Map
        {
            get;
        }

        SharedCode.MapSystem.MapEntityState State
        {
            get;
        }

        bool Dead
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool> SavedScenes
        {
            get;
        }

        System.Threading.Tasks.Task SpawnNewBots(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath);
        System.Threading.Tasks.Task<bool> TryAquireSpawnRightsForPointsSet(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef);
        System.Threading.Tasks.Task<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPoint(SharedCode.Utils.Vector3 point);
        System.Threading.Tasks.Task<bool> NotifyAllCharactersViaChat(string text);
        event System.Func<string, string, System.Threading.Tasks.Task> NewChatMessageEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1667232136, typeof(SharedCode.MapSystem.IMapEntity))]
    public interface IMapEntityClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -893578069, typeof(SharedCode.MapSystem.IMapEntity))]
    public interface IMapEntityClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Guid RealmId
        {
            get;
        }

        SharedCode.Aspects.Sessions.RealmRulesDef RealmRules
        {
            get;
        }

        GeneratedCode.Custom.Config.MapDef Map
        {
            get;
        }

        SharedCode.MapSystem.MapEntityState State
        {
            get;
        }

        bool Dead
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool> SavedScenes
        {
            get;
        }

        System.Threading.Tasks.Task SpawnNewBots(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath);
        System.Threading.Tasks.Task<bool> TryAquireSpawnRightsForPointsSet(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef);
        System.Threading.Tasks.Task<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPoint(SharedCode.Utils.Vector3 point);
        System.Threading.Tasks.Task<bool> NotifyAllCharactersViaChat(string text);
        event System.Func<string, string, System.Threading.Tasks.Task> NewChatMessageEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1510182993, typeof(SharedCode.MapSystem.IMapEntity))]
    public interface IMapEntityServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1040046380, typeof(SharedCode.MapSystem.IMapEntity))]
    public interface IMapEntityServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Guid RealmId
        {
            get;
        }

        SharedCode.Aspects.Sessions.RealmRulesDef RealmRules
        {
            get;
        }

        GeneratedCode.Custom.Config.MapDef Map
        {
            get;
        }

        SharedCode.MapSystem.MapEntityState State
        {
            get;
        }

        bool Dead
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaListWrapper<GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpaceDescriptionServer> WorldSpaces
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool> SavedScenes
        {
            get;
        }

        System.Threading.Tasks.Task<bool> SetMapEntityState(SharedCode.MapSystem.MapEntityState state);
        System.Threading.Tasks.Task<bool> ChangeChunkDescription(System.Guid descriptionId, SharedCode.MapSystem.MapChunkState newState, System.Guid unityRepositoryId);
        System.Threading.Tasks.Task<bool> OnLastUserLeft();
        System.Threading.Tasks.Task SpawnNewBots(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath);
        System.Threading.Tasks.Task<bool> TryAquireSpawnRightsForPointsSet(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef);
        System.Threading.Tasks.Task<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPoint(SharedCode.Utils.Vector3 point);
        System.Threading.Tasks.Task<bool> NotifyAllCharactersViaChat(string text);
        event System.Func<string, string, System.Threading.Tasks.Task> NewChatMessageEvent;
    }
}