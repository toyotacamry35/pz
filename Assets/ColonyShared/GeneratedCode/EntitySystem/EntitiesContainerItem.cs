using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Refs;

namespace SharedCode.EntitySystem
{
    public class EntitiesContainerItem
    {
        public Guid EntityId { get; }

        public int EntityTypeId { get; }

        public int EntityInterfaceTypeId { get; }

        public ReadWriteEntityOperationType OperationType { get; }

        public bool UpFromReadToExclusive { get; }

        public EntitiesContainerItem(Guid entityId, int entityTypeId, int entityInterfaceTypeId, ReadWriteEntityOperationType operationType, bool upFromReadToExclusive)
        {
            EntityId = entityId;
            EntityTypeId = entityTypeId;
            EntityInterfaceTypeId = entityInterfaceTypeId;
            OperationType = operationType;
            UpFromReadToExclusive = upFromReadToExclusive;
        }
    }
}
