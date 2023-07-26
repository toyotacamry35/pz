using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Repositories;

namespace GeneratedCode.EntitySystem
{
    public struct BatchItem
    {
        public BatchItem(ReadWriteEntityOperationType requestOperationType, Guid entityId, int entityMasterTypeId, int entityRequestedTypeId, string callerTag)
        {
            RequestOperationType = requestOperationType;
            EntityId = entityId;
            EntityMasterTypeId = entityMasterTypeId;
            EntityRequestedTypeId = entityRequestedTypeId;
            CallerTag = callerTag;
            UpFromReadToExclusive = false;
        }

        public ReadWriteEntityOperationType RequestOperationType { get; set; }

        public Guid EntityId { get; }

        public int EntityMasterTypeId { get; }

        public int EntityRequestedTypeId { get; }

        public bool UpFromReadToExclusive { get; set; }

        public string CallerTag { get; }

        public override string ToString()
        {
            return $"{typeof(BatchItem).Name} {ReplicaTypeRegistry.GetTypeById(EntityMasterTypeId)}:{EntityId}]";
        }
    }
}
