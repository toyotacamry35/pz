// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1098362926, typeof(Src.Aspects.Impl.Stats.ITimeStat))]
    public interface ITimeStatAlways : SharedCode.EntitySystem.IDeltaObject, IStatAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -2142175535, typeof(Src.Aspects.Impl.Stats.ITimeStat))]
    public interface ITimeStatClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IStatClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Src.Aspects.Impl.Stats.Proxy.TimeStatState State
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1973880896, typeof(Src.Aspects.Impl.Stats.ITimeStat))]
    public interface ITimeStatClientFullApi : SharedCode.EntitySystem.IDeltaObject, IStatClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -128524881, typeof(Src.Aspects.Impl.Stats.ITimeStat))]
    public interface ITimeStatClientFull : SharedCode.EntitySystem.IDeltaObject, IStatClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Src.Aspects.Impl.Stats.Proxy.TimeStatState State
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1868361124, typeof(Src.Aspects.Impl.Stats.ITimeStat))]
    public interface ITimeStatServerApi : SharedCode.EntitySystem.IDeltaObject, IStatServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1967319136, typeof(Src.Aspects.Impl.Stats.ITimeStat))]
    public interface ITimeStatServer : SharedCode.EntitySystem.IDeltaObject, IStatServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Src.Aspects.Impl.Stats.Proxy.TimeStatState State
        {
            get;
        }

        System.Threading.Tasks.ValueTask<bool> ChangeValue(float delta);
    }
}