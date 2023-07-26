// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1479610872, typeof(SharedCode.Entities.IRealmsCollectionEntity))]
    public interface IRealmsCollectionEntityAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, SharedCode.Aspects.Sessions.RealmRulesDef> Realms
        {
            get;
        }

        System.Threading.Tasks.Task<bool> AddRealm(System.Guid mapId, SharedCode.Aspects.Sessions.RealmRulesDef realmDef);
        System.Threading.Tasks.Task<bool> RemoveRealm(System.Guid mapId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -623877766, typeof(SharedCode.Entities.IRealmsCollectionEntity))]
    public interface IRealmsCollectionEntityClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, SharedCode.Aspects.Sessions.RealmRulesDef> Realms
        {
            get;
        }

        System.Threading.Tasks.Task<bool> AddRealm(System.Guid mapId, SharedCode.Aspects.Sessions.RealmRulesDef realmDef);
        System.Threading.Tasks.Task<bool> RemoveRealm(System.Guid mapId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1766654300, typeof(SharedCode.Entities.IRealmsCollectionEntity))]
    public interface IRealmsCollectionEntityClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1293204450, typeof(SharedCode.Entities.IRealmsCollectionEntity))]
    public interface IRealmsCollectionEntityClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, SharedCode.Aspects.Sessions.RealmRulesDef> Realms
        {
            get;
        }

        System.Threading.Tasks.Task<bool> AddRealm(System.Guid mapId, SharedCode.Aspects.Sessions.RealmRulesDef realmDef);
        System.Threading.Tasks.Task<bool> RemoveRealm(System.Guid mapId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 433943747, typeof(SharedCode.Entities.IRealmsCollectionEntity))]
    public interface IRealmsCollectionEntityServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 2130766256, typeof(SharedCode.Entities.IRealmsCollectionEntity))]
    public interface IRealmsCollectionEntityServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, SharedCode.Aspects.Sessions.RealmRulesDef> Realms
        {
            get;
        }

        System.Threading.Tasks.Task<bool> AddRealm(System.Guid mapId, SharedCode.Aspects.Sessions.RealmRulesDef realmDef);
        System.Threading.Tasks.Task<bool> RemoveRealm(System.Guid mapId);
    }
}