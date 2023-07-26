using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;

namespace GeneratedCode.Transactions
{
    public class ItemRemoveBatchManagementTransaction : BaseTransaction<ContainerItemOperation>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private List<RemoveItemBatchElement> _items;
        private bool _manual;

        public ItemRemoveBatchManagementTransaction(List<RemoveItemBatchElement> items, bool manual, IEntitiesRepository repository) : base(repository)
        {
            _items = items;
            _manual = manual;
        }

        public ItemRemoveBatchManagementTransaction(List<ItemMoveManagementTransaction> moveTransactions, IEntitiesRepository repository) : base(repository)
        {
            _items = moveTransactions.Select(v => (RemoveItemBatchElement)v).ToList();
            _manual = false;
        }

        public override async Task<ContainerItemOperation> ExecuteTransaction()
        {
            if ((_items == null) || (_items.Count == 0))
            {
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.Success };
            }

            var batches = _items.GroupBy(x => new KeyValuePair<int, Guid>(x.Source.EntityTypeId, x.Source.EntityId)).Select(x => new KeyValuePair<KeyValuePair<int, Guid>, List<RemoveItemBatchElement>>(x.Key, x.ToList()));

            var result = new ContainerItemOperation() { Result = ContainerItemOperationResult.Success };

            foreach (var groupInfo in batches)
            {
                var entityTypeId = groupInfo.Key.Key;
                var entityId = groupInfo.Key.Value;
                var batch = EntityBatch.Create().Add(entityTypeId, entityId);
                using (var container = await Repository.Get(batch))
                {
                    var srcEntity = container.Get<IHasContainerApiServer>(entityTypeId, entityId, ReplicationLevel.Server);
                    if (srcEntity == null)
                    {
                        Logger.IfError()?.Message("Source entity not found typeId {0}, entityId {1}", entityTypeId, entityId).Write();
                        return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstNotFound };
                    }
                    var opResult = await srcEntity.ContainerApi.ContainerOperationRemoveBatchItem(groupInfo.Value, _manual);
                    if (!opResult.IsSuccess)
                    {
                        Logger.IfError()?.Message("Source entity {0}, entityId {1} remove items batch result {2}", entityTypeId, entityId, opResult.Result).Write();
                        result.Result = opResult.Result;
                    }
                    else
                        result.ItemsCount += opResult.ItemsCount;
                }
            }

            return result;
        }
    }
}
