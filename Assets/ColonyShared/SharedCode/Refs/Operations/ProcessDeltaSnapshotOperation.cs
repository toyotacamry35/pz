using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace SharedCode.Refs.Operations
{
    public interface IOperationWithVersion
    {
        int GetVersion();
    }

    public class ProcessDeltaSnapshotOperation: BaseEntityRefOperation, IOperationWithVersion
    {
        private Dictionary<ulong, byte[]> _snapshot;
        private readonly List<DeferredEntityModel> _deferredEntities;
        private readonly int _byteCount;

        private long _replicationMask;

        private int _version;

        private int _previousVersion;

        public long ReplicationMask => _replicationMask;

        public int Version => _version;

        public int PreviousVersion => _previousVersion;

        public int DataLength => _byteCount;

        public int GetVersion()
        {
            return _version;
        }

        public ProcessDeltaSnapshotOperation(
            Dictionary<ulong, byte[]> snapshot,
            List<DeferredEntityModel> deferredEntities, 
            int byteCount,
            long replicationMask,
            int version, 
            int previousVersion)
        {
            _snapshot = snapshot;
            _deferredEntities = deferredEntities;
            _byteCount = byteCount;
            _replicationMask = replicationMask;
            _version = version;
            _previousVersion = previousVersion;
        }
        
        public override bool Do(IEntityRef entityRef, out EntityRefOperationResult? result)
        {
            var oldReplicationMask = entityRef.CurrentReplicationMask;
            var entityRefExt = (IEntityRefExt) entityRef;
            var entity = entityRefExt.GetEntity();
            var oldVersion = entityRefExt.Version;
            if (oldVersion != _previousVersion)
            {
                var opCount = ((IEntityRefExt)entityRef).GetOperationCount();
                if (opCount >= 10)
                {
                    if (Logger.IsWarnEnabled)
                        Logger.Warn(
                            "ProcessDeltaSnapshotOperation Version incorrect old {old} received {received}, need old {need_old}. TypeId {type_id} entityId {entity_id} repoId {repo_id} operations {op_count}",
                            oldVersion, _version, _previousVersion, entityRef.TypeName, entityRef.Id, entity.EntitiesRepository.Id, opCount);
                }
                
                result = null;
                return false;
            }

            var repository = entity.EntitiesRepository;
            var entityAsExt = (IEntityExt) entity;

            if (_snapshot?.Count > 0)
            {
                ((IEntitiesRepositoryExtension) repository).EntitySerializer.DeserializeDeltaObjects(entity, _snapshot);
            }

            entityRef.AddReplicationMask(_replicationMask);
            entityRefExt.SetVersion(_version);
            entityAsExt.CheckUpdateOwnerNode();

            var newReplicationMask = entityRef.CurrentReplicationMask;
            if (oldReplicationMask != newReplicationMask)
            {
                if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                {
                    Logger.IfInfo()?.Message(
                        "Replication level upgraded entityTypeId={entityTypeId} entityId={entityId} oldReplicationMask={oldReplicationMask} newReplicationMask={newReplicationMask}",
                        entity.TypeId, entity.Id, oldReplicationMask, newReplicationMask)
                        .Write();
                }
                
                entityAsExt.FireOnReplicationLevelChanged(oldReplicationMask, newReplicationMask);
            }
            
            if (_deferredEntities != null && _deferredEntities.Count != 0
                || oldReplicationMask != newReplicationMask)
            {
                result = new EntityRefOperationResult(_deferredEntities, oldReplicationMask != newReplicationMask);
            }
            else
            {
                result = null;
            }

            var eventsActions = ProcessEventsAndDelta(_snapshot, entityAsExt);
            if (eventsActions?.Count > 0)
            {
                entityRefExt.AddEvents(eventsActions);
            }

            return true;
        }

        public static List<Func<Task>> ProcessEventsAndDelta(Dictionary<ulong, byte[]> delta, IEntityExt entity)
        {
            if (delta?.Count > 0)
            {
                var eventsActions = new List<Func<Task>>();
                foreach (var snapshotObject in delta)
                {
                    var deltaObject = entity.ResolveDeltaObject(snapshotObject.Key);
                    if (deltaObject != null)
                    {
                        if (deltaObject.NeedFireEvents())
                        {
                            deltaObject.ProcessEvents(eventsActions);
                        }
                        
                        deltaObject.ClearDelta();
                    }
                }

                return eventsActions;
            }
            
            return null;
        }
    }
}
