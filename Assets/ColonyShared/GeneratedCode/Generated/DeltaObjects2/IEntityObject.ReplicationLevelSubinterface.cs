// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 537541264, typeof(SharedCode.Entities.GameObjectEntities.IEntityObject))]
    public interface IEntityObjectAlways : SharedCode.EntitySystem.IDeltaObject, IHasMappedAlways, IScenicEntityAlways, IHasWorldSpacedAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1220023505, typeof(SharedCode.Entities.GameObjectEntities.IEntityObject))]
    public interface IEntityObjectClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IHasMappedClientBroadcast, IScenicEntityClientBroadcast, IHasWorldSpacedClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1183890870, typeof(SharedCode.Entities.GameObjectEntities.IEntityObject))]
    public interface IEntityObjectClientFullApi : SharedCode.EntitySystem.IDeltaObject, IHasMappedClientFullApi, IScenicEntityClientFullApi, IHasWorldSpacedClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1371165712, typeof(SharedCode.Entities.GameObjectEntities.IEntityObject))]
    public interface IEntityObjectClientFull : SharedCode.EntitySystem.IDeltaObject, IHasMappedClientFull, IScenicEntityClientFull, IHasWorldSpacedClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1999965164, typeof(SharedCode.Entities.GameObjectEntities.IEntityObject))]
    public interface IEntityObjectServerApi : SharedCode.EntitySystem.IDeltaObject, IHasMappedServerApi, IScenicEntityServerApi, IHasWorldSpacedServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 2103838073, typeof(SharedCode.Entities.GameObjectEntities.IEntityObject))]
    public interface IEntityObjectServer : SharedCode.EntitySystem.IDeltaObject, IHasMappedServer, IScenicEntityServer, IHasWorldSpacedServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def
        {
            get;
        }
    }
}