// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1910613010, typeof(GeneratedCode.EntityModel.Bots.IBotCoordinator))]
    public interface IBotCoordinatorAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef BotSpawnPointTypeDef
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 348192593, typeof(GeneratedCode.EntityModel.Bots.IBotCoordinator))]
    public interface IBotCoordinatorClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef BotSpawnPointTypeDef
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBotsHolderClientBroadcast> BotsByAccount
        {
            get;
        }

        System.Threading.Tasks.Task Register();
        System.Threading.Tasks.Task Initialize(GeneratedCode.Custom.Config.MapDef mapDef, System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> botsRefs, SharedCode.AI.LegionaryEntityDef botConfig);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -359749367, typeof(GeneratedCode.EntityModel.Bots.IBotCoordinator))]
    public interface IBotCoordinatorClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 139260939, typeof(GeneratedCode.EntityModel.Bots.IBotCoordinator))]
    public interface IBotCoordinatorClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef BotSpawnPointTypeDef
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBotsHolderClientFull> BotsByAccount
        {
            get;
        }

        System.Threading.Tasks.Task Register();
        System.Threading.Tasks.Task Initialize(GeneratedCode.Custom.Config.MapDef mapDef, System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> botsRefs, SharedCode.AI.LegionaryEntityDef botConfig);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1084965361, typeof(GeneratedCode.EntityModel.Bots.IBotCoordinator))]
    public interface IBotCoordinatorServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1411603023, typeof(GeneratedCode.EntityModel.Bots.IBotCoordinator))]
    public interface IBotCoordinatorServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef BotSpawnPointTypeDef
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBotsHolderServer> BotsByAccount
        {
            get;
        }

        System.Threading.Tasks.Task Register();
        System.Threading.Tasks.Task Initialize(GeneratedCode.Custom.Config.MapDef mapDef, System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> botsRefs, SharedCode.AI.LegionaryEntityDef botConfig);
        System.Threading.Tasks.Task ActivateBots(System.Guid account, System.Collections.Generic.List<System.Guid> botsIds);
        System.Threading.Tasks.Task DeactivateBots(System.Guid account);
    }
}