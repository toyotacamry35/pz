// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 222478138, typeof(SharedCode.Entities.Building.IFencePlace))]
    public interface IFencePlaceAlways : SharedCode.EntitySystem.IEntity, IWorldObjectAlways, IHasAutoAddToWorldSpaceAlways, IEntityObjectAlways, IHasMappedAlways, IScenicEntityAlways, IHasWorldSpacedAlways, IHasBuildPlaceAlways, IBuildCollectionAlways, IHasSimpleMovementSyncAlways, IDatabasedMapedEntityAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedFenceElementAlways> Elements
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedAttachmentAlways> Attachments
        {
            get;
        }

        SharedCode.DeltaObjects.Building.BuildState State
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1172766608, typeof(SharedCode.Entities.Building.IFencePlace))]
    public interface IFencePlaceClientBroadcast : SharedCode.EntitySystem.IEntity, IWorldObjectClientBroadcast, IHasAutoAddToWorldSpaceClientBroadcast, IEntityObjectClientBroadcast, IHasMappedClientBroadcast, IScenicEntityClientBroadcast, IHasWorldSpacedClientBroadcast, IHasBuildPlaceClientBroadcast, IBuildCollectionClientBroadcast, IHasSimpleMovementSyncClientBroadcast, IDatabasedMapedEntityClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedFenceElementClientBroadcast> Elements
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedAttachmentClientBroadcast> Attachments
        {
            get;
        }

        SharedCode.DeltaObjects.Building.BuildState State
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 474635579, typeof(SharedCode.Entities.Building.IFencePlace))]
    public interface IFencePlaceClientFullApi : SharedCode.EntitySystem.IEntity, IWorldObjectClientFullApi, IHasAutoAddToWorldSpaceClientFullApi, IEntityObjectClientFullApi, IHasMappedClientFullApi, IScenicEntityClientFullApi, IHasWorldSpacedClientFullApi, IHasBuildPlaceClientFullApi, IBuildCollectionClientFullApi, IHasSimpleMovementSyncClientFullApi, IDatabasedMapedEntityClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1929766400, typeof(SharedCode.Entities.Building.IFencePlace))]
    public interface IFencePlaceClientFull : SharedCode.EntitySystem.IEntity, IWorldObjectClientFull, IHasAutoAddToWorldSpaceClientFull, IEntityObjectClientFull, IHasMappedClientFull, IScenicEntityClientFull, IHasWorldSpacedClientFull, IHasBuildPlaceClientFull, IBuildCollectionClientFull, IHasSimpleMovementSyncClientFull, IDatabasedMapedEntityClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedFenceElementClientFull> Elements
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedAttachmentClientFull> Attachments
        {
            get;
        }

        SharedCode.DeltaObjects.Building.BuildState State
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -843547753, typeof(SharedCode.Entities.Building.IFencePlace))]
    public interface IFencePlaceServerApi : SharedCode.EntitySystem.IEntity, IWorldObjectServerApi, IHasAutoAddToWorldSpaceServerApi, IEntityObjectServerApi, IHasMappedServerApi, IScenicEntityServerApi, IHasWorldSpacedServerApi, IHasBuildPlaceServerApi, IBuildCollectionServerApi, IHasSimpleMovementSyncServerApi, IDatabasedMapedEntityServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1830653290, typeof(SharedCode.Entities.Building.IFencePlace))]
    public interface IFencePlaceServer : SharedCode.EntitySystem.IEntity, IWorldObjectServer, IHasAutoAddToWorldSpaceServer, IEntityObjectServer, IHasMappedServer, IScenicEntityServer, IHasWorldSpacedServer, IHasBuildPlaceServer, IBuildCollectionServer, IHasSimpleMovementSyncServer, IDatabasedMapedEntityServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedFenceElementServer> Elements
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<System.Guid, GeneratedCode.DeltaObjects.ReplicationInterfaces.IPositionedAttachmentServer> Attachments
        {
            get;
        }

        SharedCode.DeltaObjects.Building.BuildState State
        {
            get;
        }

        System.Threading.Tasks.Task<bool> StateSet(SharedCode.DeltaObjects.Building.BuildState value);
    }
}