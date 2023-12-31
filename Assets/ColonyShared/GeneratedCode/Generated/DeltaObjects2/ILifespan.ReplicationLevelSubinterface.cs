// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1545012228, typeof(Assets.ColonyShared.SharedCode.Entities.ILifespan))]
    public interface ILifespanAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 2083544130, typeof(Assets.ColonyShared.SharedCode.Entities.ILifespan))]
    public interface ILifespanClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        float LifespanSec
        {
            get;
        }

        bool IsLifespanExpired
        {
            get;
        }

        int LifespanCycleNumber
        {
            get;
        }

        long BirthTime
        {
            get;
        }

        System.Threading.Tasks.Task<float> GetExpiredLifespanPercent();
        System.Threading.Tasks.Task<bool> IsExpiredLifespanPercentInRange(float fromIncluding, float tillExcluding);
        event System.Func<System.Guid, int, Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired, System.Threading.Tasks.Task> LifespanExpiredEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -793675756, typeof(Assets.ColonyShared.SharedCode.Entities.ILifespan))]
    public interface ILifespanClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1696722562, typeof(Assets.ColonyShared.SharedCode.Entities.ILifespan))]
    public interface ILifespanClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        float LifespanSec
        {
            get;
        }

        bool IsLifespanExpired
        {
            get;
        }

        int LifespanCycleNumber
        {
            get;
        }

        long BirthTime
        {
            get;
        }

        System.Threading.Tasks.Task<float> GetExpiredLifespanPercent();
        System.Threading.Tasks.Task<bool> IsExpiredLifespanPercentInRange(float fromIncluding, float tillExcluding);
        event System.Func<System.Guid, int, Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired, System.Threading.Tasks.Task> LifespanExpiredEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1995431200, typeof(Assets.ColonyShared.SharedCode.Entities.ILifespan))]
    public interface ILifespanServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 616885219, typeof(Assets.ColonyShared.SharedCode.Entities.ILifespan))]
    public interface ILifespanServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired DoOnExpired
        {
            get;
        }

        float LifespanSec
        {
            get;
        }

        bool IsLifespanExpired
        {
            get;
        }

        int LifespanCycleNumber
        {
            get;
        }

        long BirthTime
        {
            get;
        }

        System.Threading.Tasks.Task<bool> InvokeLifespanExpiredEvent(Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired whatToDo);
        System.Threading.Tasks.Task<bool> CancelLifespanCountdown();
        System.Threading.Tasks.Task StartLifespanCountdown();
        System.Threading.Tasks.Task<float> GetExpiredLifespanPercent();
        System.Threading.Tasks.Task<bool> IsExpiredLifespanPercentInRange(float fromIncluding, float tillExcluding);
        event System.Func<System.Guid, int, Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired, System.Threading.Tasks.Task> LifespanExpiredEvent;
    }
}