using System;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;

using TimeUnits = System.Int64;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasLifespan
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ILifespan Lifespan { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        Task LifespanExpired();
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ILifespan : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)] ChainCancellationToken CountdownCancellationToken { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]  OnLifespanExpired DoOnExpired { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  float LifespanSec       { get; set; }
        // Makes sense only if DoOnExpired == Destroy
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  bool IsLifespanExpired  { get; set; }
        // Makes sense only if DoOnExpired == Reset. Increments every reset. 1st == 0
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  int LifespanCycleNumber { get; set; }
        // Time of the last rebirth (when the last cycle began)
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  TimeUnits BirthTime     { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  event Func<Guid/*entityId*/, int/*typeId*/, OnLifespanExpired /*whatToDo*/, Task> LifespanExpiredEvent;

        // --- Proxies to call methods from implementation class (are needed 'cos of cluster restrictions): ----------------------------------------------------------

        // It's proxy to call `On..Event` not only from inside entity (but from implementation class or by Interface ref, f.e.) (Boris: This restriction is for security reasons)
        // Implement it by:  "await OnLifespanExpiredEvent(Id, TypeId, whatToDo);"
        [ReplicationLevel(ReplicationLevel.Server)]  Task<bool> InvokeLifespanExpiredEvent(OnLifespanExpired whatToDo);
        [ReplicationLevel(ReplicationLevel.Server)]  Task<bool> CancelLifespanCountdown();
        // Call it to restart lifespan countdown after it has been canceled:
        [ReplicationLevel(ReplicationLevel.Server)]  Task StartLifespanCountdown();
        [ReplicationLevel(ReplicationLevel.Master)]  Task LifespanExpired();

        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  Task<float> GetExpiredLifespanPercent();
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  Task<bool> IsExpiredLifespanPercentInRange(float fromIncluding, float tillExcluding);
    }
}
