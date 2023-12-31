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
    public partial class KillingRewardMechanics
    {
        public ValueTask<ResourceSystem.Utils.OuterRef> ReplaceKillingDamageDealer(ResourceSystem.Utils.OuterRef entity)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ReplaceKillingDamageDealerRunMaster(entity);
                else
                    return ReplaceKillingDamageDealerRun(entity);
            else
                return SendFuncs.ReplaceKillingDamageDealer(entity, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<ResourceSystem.Utils.OuterRef>> ReplaceKillingDamageDealerCreateDeferredDelegate(ResourceSystem.Utils.OuterRef entity) => () =>
        {
            return ReplaceKillingDamageDealer(entity).AsTask();
        }

        ;
        public async ValueTask<ResourceSystem.Utils.OuterRef> ReplaceKillingDamageDealerRunMaster(ResourceSystem.Utils.OuterRef entity)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ReplaceKillingDamageDealerCreateDeferredDelegate(entity), nameof(ReplaceKillingDamageDealer));
            }

            return await ReplaceKillingDamageDealerRun(entity);
        }

        public async ValueTask<ResourceSystem.Utils.OuterRef> ReplaceKillingDamageDealerRun(ResourceSystem.Utils.OuterRef entity)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IKillingRewardMechanics), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(ReplaceKillingDamageDealer)} GetExclusive wrapper is null");
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
                    var returnTask = ReplaceKillingDamageDealerImpl(entity);
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