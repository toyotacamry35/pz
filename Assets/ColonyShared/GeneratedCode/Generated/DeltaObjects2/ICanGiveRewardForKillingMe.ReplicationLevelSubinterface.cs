// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1093269747, typeof(SharedCode.Entities.ICanGiveRewardForKillingMe))]
    public interface ICanGiveRewardForKillingMeAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1643864667, typeof(SharedCode.Entities.ICanGiveRewardForKillingMe))]
    public interface ICanGiveRewardForKillingMeClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -824420802, typeof(SharedCode.Entities.ICanGiveRewardForKillingMe))]
    public interface ICanGiveRewardForKillingMeClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 586611990, typeof(SharedCode.Entities.ICanGiveRewardForKillingMe))]
    public interface ICanGiveRewardForKillingMeClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1868010038, typeof(SharedCode.Entities.ICanGiveRewardForKillingMe))]
    public interface ICanGiveRewardForKillingMeServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -2108721910, typeof(SharedCode.Entities.ICanGiveRewardForKillingMe))]
    public interface ICanGiveRewardForKillingMeServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IKillingRewardMechanicsServer KillingRewardMechanics
        {
            get;
        }
    }
}