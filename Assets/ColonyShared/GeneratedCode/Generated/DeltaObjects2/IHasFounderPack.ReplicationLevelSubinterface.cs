// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 269579676, typeof(GeneratedCode.DeltaObjects.IHasFounderPack))]
    public interface IHasFounderPackAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -2035494195, typeof(GeneratedCode.DeltaObjects.IHasFounderPack))]
    public interface IHasFounderPackClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1909666090, typeof(GeneratedCode.DeltaObjects.IHasFounderPack))]
    public interface IHasFounderPackClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1421308312, typeof(GeneratedCode.DeltaObjects.IHasFounderPack))]
    public interface IHasFounderPackClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IFounderPackClientFull FounderPack
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1018527950, typeof(GeneratedCode.DeltaObjects.IHasFounderPack))]
    public interface IHasFounderPackServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1410378754, typeof(GeneratedCode.DeltaObjects.IHasFounderPack))]
    public interface IHasFounderPackServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IFounderPackServer FounderPack
        {
            get;
        }
    }
}