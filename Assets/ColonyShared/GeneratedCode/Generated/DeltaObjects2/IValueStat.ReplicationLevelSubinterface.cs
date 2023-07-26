// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -401160201, typeof(Src.Aspects.Impl.Stats.IValueStat))]
    public interface IValueStatAlways : SharedCode.EntitySystem.IDeltaObject, IStatAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1055545685, typeof(Src.Aspects.Impl.Stats.IValueStat))]
    public interface IValueStatClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IStatClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 2056921449, typeof(Src.Aspects.Impl.Stats.IValueStat))]
    public interface IValueStatClientFullApi : SharedCode.EntitySystem.IDeltaObject, IStatClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1778475118, typeof(Src.Aspects.Impl.Stats.IValueStat))]
    public interface IValueStatClientFull : SharedCode.EntitySystem.IDeltaObject, IStatClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        float Value
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1314513396, typeof(Src.Aspects.Impl.Stats.IValueStat))]
    public interface IValueStatServerApi : SharedCode.EntitySystem.IDeltaObject, IStatServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1385058581, typeof(Src.Aspects.Impl.Stats.IValueStat))]
    public interface IValueStatServer : SharedCode.EntitySystem.IDeltaObject, IStatServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        float Value
        {
            get;
        }

        System.Threading.Tasks.ValueTask<bool> ChangeValue(float delta);
    }
}