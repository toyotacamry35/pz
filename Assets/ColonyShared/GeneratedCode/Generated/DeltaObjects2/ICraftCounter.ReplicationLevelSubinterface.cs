// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -555054592, typeof(GeneratedCode.DeltaObjects.ICraftCounter))]
    public interface ICraftCounterAlways : SharedCode.EntitySystem.IDeltaObject, IQuestCounterAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 798116027, typeof(GeneratedCode.DeltaObjects.ICraftCounter))]
    public interface ICraftCounterClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 490534285, typeof(GeneratedCode.DeltaObjects.ICraftCounter))]
    public interface ICraftCounterClientFullApi : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1621904909, typeof(GeneratedCode.DeltaObjects.ICraftCounter))]
    public interface ICraftCounterClientFull : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 514125420, typeof(GeneratedCode.DeltaObjects.ICraftCounter))]
    public interface ICraftCounterServerApi : SharedCode.EntitySystem.IDeltaObject, IQuestCounterServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 316185053, typeof(GeneratedCode.DeltaObjects.ICraftCounter))]
    public interface ICraftCounterServer : SharedCode.EntitySystem.IDeltaObject, IQuestCounterServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }
}