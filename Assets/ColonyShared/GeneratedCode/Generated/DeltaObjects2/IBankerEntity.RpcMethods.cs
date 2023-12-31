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
    public partial class BankerEntity
    {
        public Task<ResourceSystem.Utils.OuterRef> GetBankCell(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, ResourceSystem.Utils.OuterRef bankCell)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return GetBankCellRunMaster(bankDef, bankCell).AsTask();
                else
                    return GetBankCellRun(bankDef, bankCell).AsTask();
            else
                return SendFuncs.GetBankCell(bankDef, bankCell, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<ResourceSystem.Utils.OuterRef>> GetBankCellCreateDeferredDelegate(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, ResourceSystem.Utils.OuterRef bankCell) => () =>
        {
            return GetBankCell(bankDef, bankCell);
        }

        ;
        public async ValueTask<ResourceSystem.Utils.OuterRef> GetBankCellRunMaster(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, ResourceSystem.Utils.OuterRef bankCell)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetBankCellCreateDeferredDelegate(bankDef, bankCell), nameof(GetBankCell));
            }

            return await GetBankCellRun(bankDef, bankCell);
        }

        public async ValueTask<ResourceSystem.Utils.OuterRef> GetBankCellRun(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, ResourceSystem.Utils.OuterRef bankCell)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBankerEntity), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(GetBankCell)} GetExclusive wrapper is null");
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
                    var returnTask = GetBankCellImpl(bankDef, bankCell);
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

        public Task DestroyBankCells(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, SharedCode.EntitySystem.PropertyAddress corpseInventoryAddress)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return DestroyBankCellsRunMaster(bankDef, corpseInventoryAddress).AsTask();
                else
                    return DestroyBankCellsRun(bankDef, corpseInventoryAddress).AsTask();
            else
                return SendFuncs.DestroyBankCells(bankDef, corpseInventoryAddress, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> DestroyBankCellsCreateDeferredDelegate(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, SharedCode.EntitySystem.PropertyAddress corpseInventoryAddress) => () =>
        {
            return DestroyBankCells(bankDef, corpseInventoryAddress);
        }

        ;
        public async ValueTask DestroyBankCellsRunMaster(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, SharedCode.EntitySystem.PropertyAddress corpseInventoryAddress)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(DestroyBankCellsCreateDeferredDelegate(bankDef, corpseInventoryAddress), nameof(DestroyBankCells));
            }

            await DestroyBankCellsRun(bankDef, corpseInventoryAddress);
        }

        public async ValueTask DestroyBankCellsRun(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, SharedCode.EntitySystem.PropertyAddress corpseInventoryAddress)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBankerEntity), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(DestroyBankCells)} GetExclusive wrapper is null");
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
                    var returnTask = DestroyBankCellsImpl(bankDef, corpseInventoryAddress);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
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