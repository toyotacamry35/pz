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
    public class ItemAddBatchManagementTransaction : BaseTransaction<ContainerItemOperation>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private List<ItemResourcePack> _itemResourcesToAdd;
        private PropertyAddress _destination;
        private bool _manual;

        public ItemAddBatchManagementTransaction(List<ItemResourcePack> itemResourcesToAdd, PropertyAddress destination, bool manual, IEntitiesRepository repository) : base(repository)
        {
            _itemResourcesToAdd = itemResourcesToAdd;
            _destination = destination;
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
                var result = await destEntity.ContainerApi.ContainerOperationAddItems(_itemResourcesToAdd, _destination, _manual);
                return result;
            }
        }
    }
}
