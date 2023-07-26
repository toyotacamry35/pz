using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;
using SharedCode.EntitySystem;

namespace SharedCode.Refs.Operations
{
    public class DowngradeOperation : BaseEntityRefOperation, IOperationWithVersion
    {
        private long _mask;

        private int _version;

        private int _previousVersion;

        public int Version => _version;

        public int PreviousVersion => _previousVersion;

        public int GetVersion()
        {
            return _version;
        }

        public DowngradeOperation(long mask, int version, int previousVersion)
        {
            _mask = mask;
            _version = version;
            _previousVersion = previousVersion;
        }

        public override bool Do(IEntityRef entityRef, out EntityRefOperationResult? result)
        {
            result = null;
            
            var oldReplicationMask = entityRef.CurrentReplicationMask;
            var entityRefExt = (IEntityRefExt)entityRef;
            var entity = entityRefExt.GetEntity();
            var oldVersion = entityRefExt.Version;
            if (oldVersion != _previousVersion)
            {
                Log.Logger.IfWarn()?.Message("DowngradeOperation Version incorrect old {0} received {1}. TypeId {2} entityId {3} repoId {4}", oldVersion, _version, entity.TypeId, entity.Id, entity.EntitiesRepository.Id).Write();
                return false;
            }
            
            ((IDeltaObjectExt)entity).Downgrade(_mask);
            entityRef.RemoveReplicationMask(_mask);
            entityRefExt.SetVersion(_version);
            var newReplicationMask = entityRef.CurrentReplicationMask;
            if (oldReplicationMask != newReplicationMask)
            {
                if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                {
                    Logger.IfInfo()?.Message(
                        "Replication level dowgraded entityTypeId={entityTypeId} entityId={entityId} oldReplicationMask={oldReplicationMask} newReplicationMask={newReplicationMask}",
                        entity.TypeId, entity.Id, oldReplicationMask, newReplicationMask)
                        .Write();
                }
                
                ((IEntityExt) entity).FireOnReplicationLevelChanged(oldReplicationMask, newReplicationMask);
            }
            
            if (oldReplicationMask != newReplicationMask)
            {
                result = new EntityRefOperationResult(null, oldReplicationMask != newReplicationMask);
            }

            return true;
        }
    }
}
