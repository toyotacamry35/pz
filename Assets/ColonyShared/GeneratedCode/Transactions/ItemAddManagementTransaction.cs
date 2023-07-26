using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;

namespace GeneratedCode.Transactions
{
    public class ItemAddManagementTransaction : BaseTransaction<ContainerItemOperation>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private IItem _itemToAdd;
        private int _count;
        private PropertyAddress _destination;
        private int _destinationSlotId;
        private bool _manual;

        public ItemAddManagementTransaction(IItem itemToAdd, int count, PropertyAddress destination, int destinationSlotId, bool manual, IEntitiesRepository repository) : base(repository)
        {
            _itemToAdd = itemToAdd;
            _count = count;
            _destination = destination;
            _destinationSlotId = destinationSlotId;
            _manual = manual;
        }

        public override async Task<ContainerItemOperation> ExecuteTransaction()
        {
            var batch = EntityBatch.Create()
                .Add(_destination.EntityTypeId, _destination.EntityId);

            using (var container = await Repository.Get(batch))
            {
                var destEntity = container.Get<IHasContainerApiServer>(_destination.EntityTypeId, _destination.EntityId, ReplicationLevel.Server);
                if (destEntity == null)
                {
                    Logger.IfError()?.Message("Destination entity not found {0}", _destination).Write();
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstNotFound };
                }
                var result = await destEntity.ContainerApi.ContainerOperationAddNewItem(new IItemWrapper { Item = _itemToAdd, Count = _count }, _destination, _destinationSlotId, _manual);
                return result;
            }
        }
    }
}
