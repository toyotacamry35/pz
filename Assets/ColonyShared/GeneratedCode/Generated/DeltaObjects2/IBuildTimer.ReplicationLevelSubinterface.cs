// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 694571721, typeof(SharedCode.DeltaObjects.Building.IBuildTimer))]
    public interface IBuildTimerAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        long BuildTimestamp
        {
            get;
        }

        float BuildTime
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 514626108, typeof(SharedCode.DeltaObjects.Building.IBuildTimer))]
    public interface IBuildTimerClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        long BuildTimestamp
        {
            get;
        }

        float BuildTime
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 200746123, typeof(SharedCode.DeltaObjects.Building.IBuildTimer))]
    public interface IBuildTimerClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 2097281067, typeof(SharedCode.DeltaObjects.Building.IBuildTimer))]
    public interface IBuildTimerClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        long BuildTimestamp
        {
            get;
        }

        float BuildTime
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1899589513, typeof(SharedCode.DeltaObjects.Building.IBuildTimer))]
    public interface IBuildTimerServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1836623302, typeof(SharedCode.DeltaObjects.Building.IBuildTimer))]
    public interface IBuildTimerServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.ChainCalls.ChainCancellationToken BuildToken
        {
            get;
        }

        long BuildTimestamp
        {
            get;
        }

        float BuildTime
        {
            get;
        }
    }
}