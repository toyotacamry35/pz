// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -493690417, typeof(SharedCode.Entities.ISessionResult))]
    public interface ISessionResultAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1566073377, typeof(SharedCode.Entities.ISessionResult))]
    public interface ISessionResultClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -455812295, typeof(SharedCode.Entities.ISessionResult))]
    public interface ISessionResultClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -767061083, typeof(SharedCode.Entities.ISessionResult))]
    public interface ISessionResultClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaList<ResourceSystem.Aspects.Rewards.RewardDef> Achievements
        {
            get;
        }

        System.Threading.Tasks.ValueTask<bool> Clear();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1166209743, typeof(SharedCode.Entities.ISessionResult))]
    public interface ISessionResultServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1581641462, typeof(SharedCode.Entities.ISessionResult))]
    public interface ISessionResultServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaList<ResourceSystem.Aspects.Rewards.RewardDef> Achievements
        {
            get;
        }

        System.Threading.Tasks.ValueTask<bool> Clear();
    }
}