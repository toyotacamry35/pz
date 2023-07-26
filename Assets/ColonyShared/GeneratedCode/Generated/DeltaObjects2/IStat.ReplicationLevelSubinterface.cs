// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -517391280, typeof(Src.Aspects.Impl.Stats.IStat))]
    public interface IStatAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -2068951906, typeof(Src.Aspects.Impl.Stats.IStat))]
    public interface IStatClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        float LimitMinCache
        {
            get;
        }

        float LimitMaxCache
        {
            get;
        }

        Assets.Src.Aspects.Impl.Stats.StatType StatType
        {
            get;
        }

        System.Threading.Tasks.ValueTask<float> GetValue();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1566194125, typeof(Src.Aspects.Impl.Stats.IStat))]
    public interface IStatClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -567739172, typeof(Src.Aspects.Impl.Stats.IStat))]
    public interface IStatClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        float LimitMinCache
        {
            get;
        }

        float LimitMaxCache
        {
            get;
        }

        Assets.Src.Aspects.Impl.Stats.StatType StatType
        {
            get;
        }

        System.Threading.Tasks.ValueTask<float> GetValue();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1933204079, typeof(Src.Aspects.Impl.Stats.IStat))]
    public interface IStatServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1089461252, typeof(Src.Aspects.Impl.Stats.IStat))]
    public interface IStatServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        float LimitMinCache
        {
            get;
        }

        float LimitMaxCache
        {
            get;
        }

        Assets.Src.Aspects.Impl.Stats.StatType StatType
        {
            get;
        }

        System.Threading.Tasks.ValueTask<float> GetValue();
    }
}