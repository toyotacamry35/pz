using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using SharedCode.Entities.Cloud;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public class ReplicateRefsContainer
    {
        private static readonly ThreadLocal<Dictionary<long, int>> _subscribeIdsBuffer =
            new ThreadLocal<Dictionary<long, int>>(() => new Dictionary<long, int>());   
        
        [ProtoIgnore]
        private long _replicationMask;

        [ProtoMember(1)]
        public int SendedVersion { get; set; }

        [ProtoMember(2, OverwriteList = true)]
        public ConcurrentDictionary<long, int> _subscribeIds = new ConcurrentDictionary<long, int>();

        [ProtoMember(3)]
        public bool MigratedContainer { get; set; }

        /// <summary>
        /// Reference Counter {ReplicationLevel:Count}
        /// </summary>
        public IDictionary<long, int> SubscribeIds => _subscribeIds;

        public void MaskAsMigrated()
        {
            MigratedContainer = true;
        }

        public ReplicateRefsContainer()
        {
        }

        [ProtoAfterDeserialization]
        public void afterDeserialization()
        {
            lock (_subscribeIds)
                recalculateReplicationMask();
        }
        
        public bool Change(List<(bool subscribed, ReplicationLevel level)> subscriptions)
        {
            lock (_subscribeIds)
            {
                var changed = Change(_subscribeIds, subscriptions);
                recalculateReplicationMask();
                return changed;
            }
        }

        public long GetReplicationMask()
        {
            return _replicationMask;
        }

        public bool IsEmpty()
        {
            return _subscribeIds.Count == 0;
        }

        void recalculateReplicationMask()
        {
            _replicationMask = CalculateReplicationMask(_subscribeIds);
        }
        
        public static long CalculateReplicationMask(IDictionary<long, int> subscribers)
        {
            var replicationMask = 0L;
            foreach (var repLevel in subscribers.Keys)
                replicationMask = replicationMask | repLevel;
            return replicationMask;
        }

        public long GetChangedRecalculateReplicationMask(bool subscribe, ReplicationLevel replicationLevel)
        {
            lock (_subscribeIds)
            {
                long replicationMask = 0;
                foreach (var alreadySubscribed in _subscribeIds)
                {
                    if (!subscribe && (ReplicationLevel)alreadySubscribed.Key == replicationLevel &&
                        alreadySubscribed.Value == 1)
                        continue;
                    replicationMask = replicationMask | (long) alreadySubscribed.Key;
                }
                if (subscribe)
                    replicationMask = replicationMask | (long) replicationLevel;
                return replicationMask;
            }
        }

        public long GetChangedReplicationMask(List<(bool subscribe, ReplicationLevel replicationLevel)> changes)
        {
            lock (_subscribeIds)
            {
                _subscribeIdsBuffer.Value.Clear();
                foreach (var subscribeId in _subscribeIds)
                {
                    _subscribeIdsBuffer.Value.Add(subscribeId.Key, subscribeId.Value);
                }
                Change(_subscribeIdsBuffer.Value, changes);
                return CalculateReplicationMask(_subscribeIdsBuffer.Value);
            }
        }

        public static bool Change(IDictionary<long, int> subscribeIds, List<(bool subscribe, ReplicationLevel replicationLevel)> changes)
        {
            bool changed = false;
            foreach (var pair in changes)
            {
                if (pair.subscribe) //add
                {
                    if (!subscribeIds.ContainsKey((long) pair.replicationLevel))
                        subscribeIds[(long) pair.replicationLevel] = 1;
                    else
                        subscribeIds[(long) pair.replicationLevel]++;
                    changed = true;
                }
                else //remove
                {
                    if (subscribeIds.ContainsKey((long) pair.replicationLevel))
                    {
                        subscribeIds[(long) pair.replicationLevel]--;
                        if (subscribeIds[(long) pair.replicationLevel] <= 0)
                        {
                            subscribeIds.Remove((long) pair.replicationLevel);
                        }
                        
                        changed = true;
                    }
                }
            }
            
            return changed;
        }

        public void SetSendedVersion(int version)
        {
            SendedVersion = version;
        }
        
        public List<ReplicationLevel> GetFlatSubscribers()
        {
            lock (_subscribeIds)
            {
                List<ReplicationLevel> parentSubscribers = null;
                if (_subscribeIds.Count > 0)
                {
                    parentSubscribers = new List<ReplicationLevel>(_subscribeIds.Count);
                    foreach (var subscriber in _subscribeIds)
                    {
                        for (int i = 0; i < subscriber.Value; i++)
                        {
                            parentSubscribers.Add((ReplicationLevel) subscriber.Key);
                        }
                    }
                }
                
                return parentSubscribers;
            }
        }
    }
}
