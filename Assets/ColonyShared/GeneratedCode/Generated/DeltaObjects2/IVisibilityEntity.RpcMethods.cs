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
    public partial class VisibilityEntity
    {
        public Task<bool> Update()
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return UpdateRunMaster().AsTask();
                else
                    return UpdateRun().AsTask();
            else
                return SendFuncs.Update(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> UpdateCreateDeferredDelegate() => () =>
        {
            return Update();
        }

        ;
        public async ValueTask<bool> UpdateRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(UpdateCreateDeferredDelegate(), nameof(Update));
            }

            return await UpdateRun();
        }

        public async ValueTask<bool> UpdateRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IVisibilityEntity), 0);
            global::GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
            Guid __oldMigrationId__ = default;
            var __needSetMigrationgId__ = MigratingId != Guid.Empty;
            if (__needSetMigrationgId__)
            {
                __oldMigrationId__ = global::GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
            }

            try
            {
                var returnTask = UpdateImpl();
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
                    global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
            }
        }

        public Task<bool> ForceUnsubscribeAll(System.Guid user)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ForceUnsubscribeAllRunMaster(user).AsTask();
                else
                    return ForceUnsubscribeAllRun(user).AsTask();
            else
                return SendFuncs.ForceUnsubscribeAll(user, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> ForceUnsubscribeAllCreateDeferredDelegate(System.Guid user) => () =>
        {
            return ForceUnsubscribeAll(user);
        }

        ;
        public async ValueTask<bool> ForceUnsubscribeAllRunMaster(System.Guid user)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ForceUnsubscribeAllCreateDeferredDelegate(user), nameof(ForceUnsubscribeAll));
            }

            return await ForceUnsubscribeAllRun(user);
        }

        public async ValueTask<bool> ForceUnsubscribeAllRun(System.Guid user)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IVisibilityEntity), 1);
            global::GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
            Guid __oldMigrationId__ = default;
            var __needSetMigrationgId__ = MigratingId != Guid.Empty;
            if (__needSetMigrationgId__)
            {
                __oldMigrationId__ = global::GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
            }

            try
            {
                var returnTask = ForceUnsubscribeAllImpl(user);
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
                    global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
            }
        }
    }
}