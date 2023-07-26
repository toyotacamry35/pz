using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GeneratedCode.EntitySystem.Migrating;
using SharedCode.EntitySystem;

namespace GeneratedCode.Manual.Repositories
{
    public class RepositoryEntityTypeStates
    {
        public ConcurrentDictionary<Guid, EntityUsageRefsCount> LockedEntities { get; }  = new ConcurrentDictionary<Guid, EntityUsageRefsCount>(GuidComparer.Default);

        public ConcurrentDictionary<Guid, EntityQueue> WaitQueues { get; } = new ConcurrentDictionary<Guid, EntityQueue>(GuidComparer.Default);

        public ConcurrentDictionary<Guid, WaitUpdateInfo> WaitUpdates { get; } = new ConcurrentDictionary<Guid, WaitUpdateInfo>(GuidComparer.Default);

        public ConcurrentDictionary<Guid, MigratingEntityRpcQueue> MigratingQueues { get; } = new ConcurrentDictionary<Guid, MigratingEntityRpcQueue>();

        public ConcurrentDictionary<Guid, DeferredEntityState> DeferredEntities { get; } = new ConcurrentDictionary<Guid, DeferredEntityState>();

        public bool IsEntityDeferred(Guid entityId, ReplicationLevel requestedLevel)
        {
            return DeferredEntities.TryGetValue(entityId, out var deferredEntity) 
                   && deferredEntity.ExistedLevel < requestedLevel && requestedLevel <= deferredEntity.DeferredLevel;
        }

        
        public bool TryRemoveDeferredEntity(Guid entityId, ReplicationLevel updatedLevel)
        {
            if (DeferredEntities.TryGetValue(entityId, out var deferredEntity))
            {
                if (deferredEntity.DeferredLevel == updatedLevel)
                {
                    DeferredEntities.TryRemove(entityId, out _);
                }
                else
                {
                    DeferredEntities[entityId] = new DeferredEntityState(deferredEntity.DeferredLevel, updatedLevel);
                }

                return true;
            }

            return false;
        }
    }
    
    public readonly struct WaitUpdateInfo : IEquatable<WaitUpdateInfo>
    {
        public WaitUpdateInfo(ConcurrentQueue<UpdateBatch> waitUpdateQueue, DateTime firstWaitDate, bool logged)
        {
            WaitUpdateQueue = waitUpdateQueue;
            FirstWaitDate = firstWaitDate;
            Logged = logged;
        }

        public ConcurrentQueue<UpdateBatch> WaitUpdateQueue { get; }
        
        public DateTime FirstWaitDate { get; }
        
        public bool Logged { get; }

        public bool Equals(WaitUpdateInfo other)
        {
            return Equals(WaitUpdateQueue, other.WaitUpdateQueue) && FirstWaitDate.Equals(other.FirstWaitDate) && Logged == other.Logged;
        }

        public override bool Equals(object obj)
        {
            return obj is WaitUpdateInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (WaitUpdateQueue != null ? WaitUpdateQueue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FirstWaitDate.GetHashCode();
                hashCode = (hashCode * 397) ^ Logged.GetHashCode();
                return hashCode;
            }
        }
    }

        

    public class GuidComparer : IEqualityComparer<Guid>
    {
        public static GuidComparer Default = new GuidComparer();
        public bool Equals(Guid x, Guid y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Guid obj)
        {
            return obj.GetHashCode();
        }
    }
}
