using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratedCode.Custom.Containers;
using GeneratedCode.Transactions;
using SharedCode.Cloud;
using SharedCode.CustomData;
using SharedCode.Entities;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace GeneratedCode.DeltaObjects
{
    public partial class ContainerServiceEntity
    {
        public async Task<ContainerItemOperation> MoveItemImpl(PropertyAddress source, int sourceSlotId, PropertyAddress destination, int destinationSlotId, int count,
            Guid clientSrcEntityId)
        {
            var itemTransaction = new ItemMoveManagementTransaction(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId, true, EntitiesRepository);
            var result = await itemTransaction.ExecuteTransaction();
            return result;
        }

        public async Task<ContainerItemOperation> RemoveItemImpl(PropertyAddress source, int sourceSlotId, int count, Guid clientEntityId)
        {            
            var itemTransaction = new ItemRemoveBatchManagementTransaction(new List<RemoveItemBatchElement>() { new RemoveItemBatchElement(source, sourceSlotId, count, clientEntityId) }, true, EntitiesRepository);
            var result = await itemTransaction.ExecuteTransaction();
            return result;
        }
    }
}
