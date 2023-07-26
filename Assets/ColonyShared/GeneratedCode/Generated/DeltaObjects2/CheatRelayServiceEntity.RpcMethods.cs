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
    public partial class CheatRelayServiceEntity
    {
        public Task<Core.Cheats.ExecuteCheatResult> ExecuteCheatWithCheck(System.Guid target, string command, string[] args)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ExecuteCheatWithCheckRunMaster(target, command, args).AsTask();
                else
                    return ExecuteCheatWithCheckRun(target, command, args).AsTask();
            else
                return SendFuncs.ExecuteCheatWithCheck(target, command, args, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<Core.Cheats.ExecuteCheatResult>> ExecuteCheatWithCheckCreateDeferredDelegate(System.Guid target, string command, string[] args) => () =>
        {
            return ExecuteCheatWithCheck(target, command, args);
        }

        ;
        public async ValueTask<Core.Cheats.ExecuteCheatResult> ExecuteCheatWithCheckRunMaster(System.Guid target, string command, string[] args)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ExecuteCheatWithCheckCreateDeferredDelegate(target, command, args), nameof(ExecuteCheatWithCheck));
            }

            return await ExecuteCheatWithCheckRun(target, command, args);
        }

        public async ValueTask<Core.Cheats.ExecuteCheatResult> ExecuteCheatWithCheckRun(System.Guid target, string command, string[] args)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ICheatRelayServiceEntity), 0);
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
                var returnTask = ExecuteCheatWithCheckImpl(target, command, args);
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

        public Task<Core.Cheats.ExecuteCheatResult> ExecuteCheat(System.Guid target, string command, string[] args)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ExecuteCheatRunMaster(target, command, args).AsTask();
                else
                    return ExecuteCheatRun(target, command, args).AsTask();
            else
                return SendFuncs.ExecuteCheat(target, command, args, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<Core.Cheats.ExecuteCheatResult>> ExecuteCheatCreateDeferredDelegate(System.Guid target, string command, string[] args) => () =>
        {
            return ExecuteCheat(target, command, args);
        }

        ;
        public async ValueTask<Core.Cheats.ExecuteCheatResult> ExecuteCheatRunMaster(System.Guid target, string command, string[] args)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ExecuteCheatCreateDeferredDelegate(target, command, args), nameof(ExecuteCheat));
            }

            return await ExecuteCheatRun(target, command, args);
        }

        public async ValueTask<Core.Cheats.ExecuteCheatResult> ExecuteCheatRun(System.Guid target, string command, string[] args)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ICheatRelayServiceEntity), 1);
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
                var returnTask = ExecuteCheatImpl(target, command, args);
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