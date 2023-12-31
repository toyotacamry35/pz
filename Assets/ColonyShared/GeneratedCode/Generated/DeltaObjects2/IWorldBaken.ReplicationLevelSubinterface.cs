// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 332021020, typeof(SharedCode.Entities.IWorldBaken))]
    public interface IWorldBakenAlways : SharedCode.EntitySystem.IEntity, IMountableAlways, IEntityObjectAlways, IHasMappedAlways, IWorldObjectAlways, IHasAutoAddToWorldSpaceAlways, IHasWorldSpacedAlways, IHasSimpleMovementSyncAlways, IHasSpecificStatsAlways, IHasOwnerAlways, IDatabasedMapedEntityAlways, IHasStatsEngineAlways, IHasHealthAlways, IScenicEntityAlways, IHasPersistentIncomingDamageMultiplierAlways, IHasIncomingDamageMultiplierAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1687455612, typeof(SharedCode.Entities.IWorldBaken))]
    public interface IWorldBakenClientBroadcast : SharedCode.EntitySystem.IEntity, IMountableClientBroadcast, IEntityObjectClientBroadcast, IHasMappedClientBroadcast, IWorldObjectClientBroadcast, IHasAutoAddToWorldSpaceClientBroadcast, IHasWorldSpacedClientBroadcast, IHasSimpleMovementSyncClientBroadcast, IHasSpecificStatsClientBroadcast, IHasOwnerClientBroadcast, IDatabasedMapedEntityClientBroadcast, IHasStatsEngineClientBroadcast, IHasHealthClientBroadcast, IScenicEntityClientBroadcast, IHasPersistentIncomingDamageMultiplierClientBroadcast, IHasIncomingDamageMultiplierClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        long ReadyTimeUTC0InMilliseconds
        {
            get;
        }

        System.Threading.Tasks.Task SetCooldown();
        System.Threading.Tasks.Task<float> GetVerticalSpawnPointDistance();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 456031215, typeof(SharedCode.Entities.IWorldBaken))]
    public interface IWorldBakenClientFullApi : SharedCode.EntitySystem.IEntity, IMountableClientFullApi, IEntityObjectClientFullApi, IHasMappedClientFullApi, IWorldObjectClientFullApi, IHasAutoAddToWorldSpaceClientFullApi, IHasWorldSpacedClientFullApi, IHasSimpleMovementSyncClientFullApi, IHasSpecificStatsClientFullApi, IHasOwnerClientFullApi, IDatabasedMapedEntityClientFullApi, IHasStatsEngineClientFullApi, IHasHealthClientFullApi, IScenicEntityClientFullApi, IHasPersistentIncomingDamageMultiplierClientFullApi, IHasIncomingDamageMultiplierClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1563335252, typeof(SharedCode.Entities.IWorldBaken))]
    public interface IWorldBakenClientFull : SharedCode.EntitySystem.IEntity, IMountableClientFull, IEntityObjectClientFull, IHasMappedClientFull, IWorldObjectClientFull, IHasAutoAddToWorldSpaceClientFull, IHasWorldSpacedClientFull, IHasSimpleMovementSyncClientFull, IHasSpecificStatsClientFull, IHasOwnerClientFull, IDatabasedMapedEntityClientFull, IHasStatsEngineClientFull, IHasHealthClientFull, IScenicEntityClientFull, IHasPersistentIncomingDamageMultiplierClientFull, IHasIncomingDamageMultiplierClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        long ReadyTimeUTC0InMilliseconds
        {
            get;
        }

        System.Threading.Tasks.Task SetCooldown();
        System.Threading.Tasks.Task<float> GetVerticalSpawnPointDistance();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -640886143, typeof(SharedCode.Entities.IWorldBaken))]
    public interface IWorldBakenServerApi : SharedCode.EntitySystem.IEntity, IMountableServerApi, IEntityObjectServerApi, IHasMappedServerApi, IWorldObjectServerApi, IHasAutoAddToWorldSpaceServerApi, IHasWorldSpacedServerApi, IHasSimpleMovementSyncServerApi, IHasSpecificStatsServerApi, IHasOwnerServerApi, IDatabasedMapedEntityServerApi, IHasStatsEngineServerApi, IHasHealthServerApi, IScenicEntityServerApi, IHasPersistentIncomingDamageMultiplierServerApi, IHasIncomingDamageMultiplierServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1942784853, typeof(SharedCode.Entities.IWorldBaken))]
    public interface IWorldBakenServer : SharedCode.EntitySystem.IEntity, IMountableServer, IEntityObjectServer, IHasMappedServer, IWorldObjectServer, IHasAutoAddToWorldSpaceServer, IHasWorldSpacedServer, IHasSimpleMovementSyncServer, IHasSpecificStatsServer, IHasOwnerServer, IDatabasedMapedEntityServer, IHasStatsEngineServer, IHasHealthServer, IScenicEntityServer, IHasPersistentIncomingDamageMultiplierServer, IHasIncomingDamageMultiplierServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        long ReadyTimeUTC0InMilliseconds
        {
            get;
        }

        System.Threading.Tasks.Task SetCooldown();
        System.Threading.Tasks.Task<float> GetVerticalSpawnPointDistance();
    }
}