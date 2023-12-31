// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1477097853, typeof(SharedCode.Entities.IOwnerInformation))]
    public interface IOwnerInformationAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> Owner
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1999128277, typeof(SharedCode.Entities.IOwnerInformation))]
    public interface IOwnerInformationClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> Owner
        {
            get;
        }

        ResourceSystem.Aspects.AccessRights.AccessPredicateDef AccessPredicate
        {
            get;
        }

        ResourceSystem.Aspects.AccessRights.AccessPredicateDef LockPredicate
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1492491093, typeof(SharedCode.Entities.IOwnerInformation))]
    public interface IOwnerInformationClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 740309278, typeof(SharedCode.Entities.IOwnerInformation))]
    public interface IOwnerInformationClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> Owner
        {
            get;
        }

        ResourceSystem.Aspects.AccessRights.AccessPredicateDef AccessPredicate
        {
            get;
        }

        ResourceSystem.Aspects.AccessRights.AccessPredicateDef LockPredicate
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 189547947, typeof(SharedCode.Entities.IOwnerInformation))]
    public interface IOwnerInformationServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1198886858, typeof(SharedCode.Entities.IOwnerInformation))]
    public interface IOwnerInformationServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> Owner
        {
            get;
        }

        ResourceSystem.Aspects.AccessRights.AccessPredicateDef AccessPredicate
        {
            get;
        }

        ResourceSystem.Aspects.AccessRights.AccessPredicateDef LockPredicate
        {
            get;
        }

        System.Threading.Tasks.Task SetOwner(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> owner);
        System.Threading.Tasks.Task SetLockPredicate(ResourceSystem.Aspects.AccessRights.AccessPredicateDef accessPredicate);
    }
}