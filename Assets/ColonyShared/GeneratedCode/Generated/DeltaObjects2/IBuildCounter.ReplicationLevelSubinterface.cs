// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 2093196526, typeof(GeneratedCode.DeltaObjects.IBuildCounter))]
    public interface IBuildCounterAlways : SharedCode.EntitySystem.IDeltaObject, IQuestCounterAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1415845373, typeof(GeneratedCode.DeltaObjects.IBuildCounter))]
    public interface IBuildCounterClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 135981653, typeof(GeneratedCode.DeltaObjects.IBuildCounter))]
    public interface IBuildCounterClientFullApi : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -973203331, typeof(GeneratedCode.DeltaObjects.IBuildCounter))]
    public interface IBuildCounterClientFull : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -544842152, typeof(GeneratedCode.DeltaObjects.IBuildCounter))]
    public interface IBuildCounterServerApi : SharedCode.EntitySystem.IDeltaObject, IQuestCounterServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 441895899, typeof(GeneratedCode.DeltaObjects.IBuildCounter))]
    public interface IBuildCounterServer : SharedCode.EntitySystem.IDeltaObject, IQuestCounterServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }
}