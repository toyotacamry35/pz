// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using System.Threading;
using System.Threading.Tasks;
using GeneratedCode.EntitySystem;
using SharedCode.Logging;
using System.Linq;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace GeneratedCode.DeltaObjects
{
    public partial class ContainerServiceEntity
    {
        public Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return MoveItemRunMaster(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId).AsTask();
                else
                    return MoveItemRun(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId).AsTask();
            else
                return SendFuncs.MoveItem(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation>> MoveItemCreateDeferredDelegate(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId) => () =>
        {
            return MoveItem(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
        }

        ;
        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveItemRunMaster(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(MoveItemCreateDeferredDelegate(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId), nameof(MoveItem));
            }

            return await MoveItemRun(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
        }

        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveItemRun(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IContainerServiceEntity), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(MoveItem)} GetExclusive wrapper is null");
                GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
                Guid __oldMigrationId__ = default;
                var __needSetMigrationgId__ = MigratingId != Guid.Empty;
                if (__needSetMigrationgId__)
                {
                    __oldMigrationId__ = GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                    GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
                }

                try
                {
                    var returnTask = MoveItemImpl(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }

        public Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> RemoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RemoveItemRunMaster(source, sourceSlotId, count, clientEntityId).AsTask();
                else
                    return RemoveItemRun(source, sourceSlotId, count, clientEntityId).AsTask();
            else
                return SendFuncs.RemoveItem(source, sourceSlotId, count, clientEntityId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation>> RemoveItemCreateDeferredDelegate(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId) => () =>
        {
            return RemoveItem(source, sourceSlotId, count, clientEntityId);
        }

        ;
        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> RemoveItemRunMaster(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RemoveItemCreateDeferredDelegate(source, sourceSlotId, count, clientEntityId), nameof(RemoveItem));
            }

            return await RemoveItemRun(source, sourceSlotId, count, clientEntityId);
        }

        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> RemoveItemRun(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IContainerServiceEntity), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RemoveItem)} GetExclusive wrapper is null");
                GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
                Guid __oldMigrationId__ = default;
                var __needSetMigrationgId__ = MigratingId != Guid.Empty;
                if (__needSetMigrationgId__)
                {
                    __oldMigrationId__ = GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                    GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
                }

                try
                {
                    var returnTask = RemoveItemImpl(source, sourceSlotId, count, clientEntityId);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }
    }
}