// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 795405287, typeof(GeneratedCode.MapSystem.IEventPoint))]
    public interface IEventPointAlways : SharedCode.EntitySystem.IEntity, IEntityObjectAlways, IHasMappedAlways, IScenicEntityAlways, IHasWorldSpacedAlways, IHasSimpleMovementSyncAlways, IHasLinksEngineAlways, IDatabasedMapedEntityAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> RunningEvent
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Entities.GameObjectEntities.IEntityObjectDef> LoadEvent();
        System.Threading.Tasks.Task<bool> AssignEvent(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> newEvent, SharedCode.Entities.GameObjectEntities.EventInstanceDef eventDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> RemoveEvent();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -818642948, typeof(GeneratedCode.MapSystem.IEventPoint))]
    public interface IEventPointClientBroadcast : SharedCode.EntitySystem.IEntity, IEntityObjectClientBroadcast, IHasMappedClientBroadcast, IScenicEntityClientBroadcast, IHasWorldSpacedClientBroadcast, IHasSimpleMovementSyncClientBroadcast, IHasLinksEngineClientBroadcast, IDatabasedMapedEntityClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> RunningEvent
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Entities.GameObjectEntities.IEntityObjectDef> LoadEvent();
        System.Threading.Tasks.Task<bool> AssignEvent(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> newEvent, SharedCode.Entities.GameObjectEntities.EventInstanceDef eventDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> RemoveEvent();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 625305233, typeof(GeneratedCode.MapSystem.IEventPoint))]
    public interface IEventPointClientFullApi : SharedCode.EntitySystem.IEntity, IEntityObjectClientFullApi, IHasMappedClientFullApi, IScenicEntityClientFullApi, IHasWorldSpacedClientFullApi, IHasSimpleMovementSyncClientFullApi, IHasLinksEngineClientFullApi, IDatabasedMapedEntityClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 310851842, typeof(GeneratedCode.MapSystem.IEventPoint))]
    public interface IEventPointClientFull : SharedCode.EntitySystem.IEntity, IEntityObjectClientFull, IHasMappedClientFull, IScenicEntityClientFull, IHasWorldSpacedClientFull, IHasSimpleMovementSyncClientFull, IHasLinksEngineClientFull, IDatabasedMapedEntityClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> RunningEvent
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Entities.GameObjectEntities.IEntityObjectDef> LoadEvent();
        System.Threading.Tasks.Task<bool> AssignEvent(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> newEvent, SharedCode.Entities.GameObjectEntities.EventInstanceDef eventDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> RemoveEvent();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 69486789, typeof(GeneratedCode.MapSystem.IEventPoint))]
    public interface IEventPointServerApi : SharedCode.EntitySystem.IEntity, IEntityObjectServerApi, IHasMappedServerApi, IScenicEntityServerApi, IHasWorldSpacedServerApi, IHasSimpleMovementSyncServerApi, IHasLinksEngineServerApi, IDatabasedMapedEntityServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 930824636, typeof(GeneratedCode.MapSystem.IEventPoint))]
    public interface IEventPointServer : SharedCode.EntitySystem.IEntity, IEntityObjectServer, IHasMappedServer, IScenicEntityServer, IHasWorldSpacedServer, IHasSimpleMovementSyncServer, IHasLinksEngineServer, IDatabasedMapedEntityServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> RunningEvent
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Entities.GameObjectEntities.IEntityObjectDef> LoadEvent();
        System.Threading.Tasks.Task<bool> AssignEvent(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> newEvent, SharedCode.Entities.GameObjectEntities.EventInstanceDef eventDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> RemoveEvent();
    }
}