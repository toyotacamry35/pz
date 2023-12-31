// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -989199048, typeof(SharedCode.Entities.IBakenCharacterEntity))]
    public interface IBakenCharacterEntityAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -631410912, typeof(SharedCode.Entities.IBakenCharacterEntity))]
    public interface IBakenCharacterEntityClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1043675101, typeof(SharedCode.Entities.IBakenCharacterEntity))]
    public interface IBakenCharacterEntityClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 491003801, typeof(SharedCode.Entities.IBakenCharacterEntity))]
    public interface IBakenCharacterEntityClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool> RegisteredBakens
        {
            get;
        }

        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ActiveBaken
        {
            get;
        }

        System.Threading.Tasks.ValueTask<bool> BakenCanBeActivated(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -756161820, typeof(SharedCode.Entities.IBakenCharacterEntity))]
    public interface IBakenCharacterEntityServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -138394534, typeof(SharedCode.Entities.IBakenCharacterEntity))]
    public interface IBakenCharacterEntityServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Guid CharacterId
        {
            get;
        }

        bool CharacterLoaded
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool> RegisteredBakens
        {
            get;
        }

        SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ActiveBaken
        {
            get;
        }

        System.Threading.Tasks.ValueTask<bool> BakenCanBeActivated(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef);
        System.Threading.Tasks.ValueTask<bool> ActivateBaken(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef);
        System.Threading.Tasks.ValueTask RegisterBaken(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef, bool loaded);
        System.Threading.Tasks.ValueTask BakenIsDestroyed(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef);
        System.Threading.Tasks.ValueTask SetCharacterLoaded(bool loaded);
        System.Threading.Tasks.ValueTask SetLogin(bool logined);
        System.Threading.Tasks.ValueTask<bool> CanBeUnloaded();
    }
}