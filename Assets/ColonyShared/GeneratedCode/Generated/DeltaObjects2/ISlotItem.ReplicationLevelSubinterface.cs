// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -628438388, typeof(SharedCode.DeltaObjects.ISlotItem))]
    public interface ISlotItemAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 21921612, typeof(SharedCode.DeltaObjects.ISlotItem))]
    public interface ISlotItemClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int Stack
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.IItemClientBroadcast Item
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 404681708, typeof(SharedCode.DeltaObjects.ISlotItem))]
    public interface ISlotItemClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1716438178, typeof(SharedCode.DeltaObjects.ISlotItem))]
    public interface ISlotItemClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int Stack
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.IItemClientFull Item
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 985202732, typeof(SharedCode.DeltaObjects.ISlotItem))]
    public interface ISlotItemServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -834434320, typeof(SharedCode.DeltaObjects.ISlotItem))]
    public interface ISlotItemServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int Stack
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.IItemServer Item
        {
            get;
        }
    }
}